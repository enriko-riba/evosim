using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace ParticlePhysics
{
    /// <summary>
    /// Simulates particles and links between them.
    /// </summary>
    public class Simulation
    {
        public const float GravitationalAcceleration = 9.80665f;

        private List<Particle> particles = new List<Particle>(10);

        /// <summary>
        /// Particles simulated by the simulation.
        /// </summary>
        public IReadOnlyList<Particle> Particles { get { return particles; } }

        private List<ParticleLink> particleLinks = new List<ParticleLink>(20);

        /// <summary>
        /// Particle links simulated by the simulation. 
        /// </summary>
        public IReadOnlyList<ParticleLink> ParticleLinks { get { return particleLinks; } }

        ///// <summary>
        ///// Number of iterations to do when solving particle links. 
        ///// </summary>
        //public int SolveIterations { get; set; } = 10;

        /// <summary>
        /// Acceleration of the particle system.
        /// </summary>
        public Vector2 GlobalAcceleration { get; set; } = new Vector2(0f, -GravitationalAcceleration);

        /// <summary>
        /// Y-coordinate of the ground level.
        /// </summary>
        public float GroundLevel { get; set; }

        /// <summary>
        /// How much a particle can penetrate into the ground without the need to correct the particle's position.
        /// </summary>
        public float CollisionSlop { get; set; }

        /// <summary>
        /// Value in the range [0, 1] that determines how fast interpenetrations are resolved. The higher the value, the faster the resolution of the collision. 
        /// </summary>
        public float CollisionBaumgarte { get; set; } = 1f;

        ///// <summary>
        ///// Friction coeff. between the ground and the particles. The lower the value, the more slippery surface of the ground is. Should never be negative number. 
        ///// </summary>
        //public float GroundFriction { get; set; }

        /// <summary>
        /// Adds a particle to the simulation.
        /// </summary>
        /// <param name="particle">The particle to add.</param>
        public void AddParticle(Particle particle)
        {
            if (particle == null)
            {
                throw new ArgumentNullException(nameof(particle));
            }

            if (particle.IsSimulated)
            {
                throw new ArgumentException($"{nameof(Particle)} is already added to the simulation.");
            }

            particles.Add(particle);
            particle.IsSimulated = true;
        }

        /// <summary>
        /// Adds a particle link to the simulation. Also adds linked particles to the simulation if they are not already in the simulation.
        /// </summary>
        /// <param name="particleLink">The particle link to add.</param>
        public void AddParticleLink(ParticleLink particleLink)
        {
            if (particleLink == null)
            {
                throw new ArgumentNullException(nameof(particleLink));
            }

            if (particleLink.IsSimulated)
            {
                throw new ArgumentException($"{nameof(ParticleLink)} is already added to the simulation.");
            }

            particleLinks.Add(particleLink);
            particleLink.IsSimulated = true;

            TryAddParticle(particleLink.Particle1);
            TryAddParticle(particleLink.Particle2);

			CalcParticleLinkAngleAndPhase(particleLink);
        }

        /// <summary>
        /// Removes a particle from the simulation. Also removes all links with the removed particle.
        /// </summary>
        /// <param name="particle">The particle to remove.</param>
        public bool RemoveParticle(Particle particle)
        {
            if (particles.Remove(particle))
            {
                particle.IsSimulated = false;

                for (var i = 0; i < particleLinks.Count; ++i)
                {
                    var link = particleLinks[i];

                    if (link.Particle1 == particle || link.Particle2 == particle)
                    {
                        particleLinks.RemoveAt(i);
                        link.IsSimulated = false;

                        --i;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a particle link from the simulation.
        /// </summary>
        /// <param name="particleLink">The particle link to add.</param>
        public bool RemoveParticleLink(ParticleLink particleLink)
        {
            if (particleLinks.Remove(particleLink))
            {
                particleLink.IsSimulated = false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs simulation step.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since the last simulation step.</param>
        /// <param name="solveIterations">Number of iterations to do when solving particle links and collisions. This should be positive number.</param>
        public void DoStep(TimeSpan elapsedTime, int solveIterations = 5)
        {
            var deltaTime = (float)elapsedTime.TotalSeconds;

            Integrate(deltaTime);
			UpdateLinkPhases(deltaTime);

            for (var i = 0; i < solveIterations; ++i)
            {
				SolveParticleLinks();
				DetectAndSolveCollisions();
            }

			ApplyFriction();
			UpdateLinkAngles();
        }

        private void Integrate(float deltaTime)
        {
            var deltaTimeSq = deltaTime * deltaTime;

            for (var i = 0; i < particles.Count; ++i)
            {
                var particle = particles[i];

                var p0 = particle.Position;
                Debug.Assert(!float.IsNaN(2f * p0.X));
                particle.Position = 2f * p0 - particle.OldPosition + GlobalAcceleration * deltaTimeSq;
                particle.OldPosition = p0;
				particle.IsTouchingGround = false;
            }
        }

        private void SolveParticleLinks()
        {
            for (var i = 0; i < particleLinks.Count; ++i)
            {
                var link = particleLinks[i];
                var particle1 = link.Particle1;
                var particle2 = link.Particle2;

				var diff = particle1.Position - particle2.Position;
				var curLength = diff.Length();
				var normal = diff / curLength;

				if (float.IsInfinity(normal.X) || float.IsNaN(normal.X)) // Are particles in the same position? (div by approx. zero occured in that case)
				{
					normal = Vector2.UnitX; // Pick arbitrary normal
				}

				var maxOscillation = link.ContractionCoeff * link.Length;
				var targetLength = link.Length + link.TargetOscillationScale * maxOscillation;
				var error = curLength - targetLength;

				var invM1 = 1f / particle1.Mass;
				var invM2 = 1f / particle2.Mass;
				var mSum = invM1 + invM2;
				var invM = 1f / mSum;

				particle1.Position -= (invM1 * invM * error) * normal;
				particle2.Position += (invM2 * invM * error) * normal;
			}
        }

		private void UpdateLinkPhases(float deltaTime)
		{
			for (var i = 0; i < particleLinks.Count; ++i)
			{
				var link = particleLinks[i];
				//var angle = link.TargetAngle;

				link.TargetAngle += link.Phase * link.Frequency * deltaTime * MathUtil.TwoPi;

				if (link.TargetAngle <= -MathUtil.PiOver2)
				{
					var da = -link.TargetAngle - MathUtil.PiOver2;

					link.TargetAngle = -MathUtil.PiOver2 + da;
					link.Phase = 1f;
				}
				else if (link.TargetAngle >= MathUtil.PiOver2)
				{
					var da = link.TargetAngle - MathUtil.PiOver2;

					link.TargetAngle = MathUtil.PiOver2 - da;
					link.Phase = -1f;
				}

				link.TargetOscillationScale = (float)Math.Sin(link.TargetAngle);
			}
		}

        private void DetectAndSolveCollisions()
        {
            for (var i = 0; i < particles.Count; ++i)
            {
                var particle = particles[i];
                var particlePos = particle.Position;

                var depth = GroundLevel - (particlePos.Y - particle.Radius);

                if (depth >= CollisionSlop) // Is particle penetrating into the ground?
                {
                    //var particlePos0 = particle.OldPosition;

                    // Move the particle out of ground...
                    particlePos.Y += CollisionBaumgarte * (depth - CollisionSlop);

                    // Friction (tangent vector is (1, 0))...
                    //var vt = particlePos.X - particlePos0.X; // Tangent Vel.

                    //particlePos0.X += vt * particle.Friction;

                    particle.Position = particlePos;
					particle.IsTouchingGround = true;
                    //particle.OldPosition = particlePos0;
                }
            }
        }

		private void ApplyFriction()
		{
			for (var i = 0; i < particles.Count; ++i)
			{
				var particle = particles[i];

				if (particle.IsTouchingGround)
				{
					var position = particle.Position;
					var position0 = particle.OldPosition;
					var vt = position.X - position0.X;
                    Debug.Assert(!float.IsNaN(vt));
                    position.X -= vt * particle.Friction;
                    Debug.Assert(!float.IsNaN(position.X));
                    particle.Position = position;
				}
			}
		}

		private void UpdateLinkAngles()
		{
			for (var i = 0; i < particleLinks.Count; ++i)
			{
				var link = particleLinks[i];
				var distance = Vector2.Distance(link.Particle1.Position, link.Particle2.Position);
				var diff = (distance - link.Length) / (link.Length * link.ContractionCoeff);

				if (!(float.IsInfinity(diff) || float.IsNaN(diff)))
				{
					diff = MathUtil.Clamp(diff, -1f, 1f);

					//link.TargetOscillationScale = diff;
					link.TargetAngle = (float)Math.Asin(diff);
				}
				else
				{
					link.TargetAngle = 0f;
				}
			}
		}

        private void TryAddParticle(Particle particle)
        {
            if (!particle.IsSimulated)
            {
                particles.Add(particle);
                particle.IsSimulated = true;
            }
        }

        public void Reset()
        {
            foreach (var p in Particles)
            {
                p.SetPosition(Vector2.Zero);
                p.IsTouchingGround = false;
            }
            foreach (var pl in ParticleLinks)
            {
                CalcParticleLinkAngleAndPhase(pl);
            }
        }

		private static void CalcParticleLinkAngleAndPhase(ParticleLink link)
		{
			var distance = Vector2.Distance(link.Particle1.Position, link.Particle2.Position);
			var diff = (distance - link.Length) / (link.Length * link.ContractionCoeff);

			if (!(float.IsInfinity(diff) || float.IsNaN(diff)))
			{
				diff = MathUtil.Clamp(diff, -1f, 1f);

				//link.TargetOscillationScale = diff;
				link.TargetAngle = (float)Math.Asin(diff);

				//if (link.TargetAngle < 0f)
				//{
				//	link.TargetAngle += MathUtil.TwoPi;
				//}
				if (link.TargetAngle <= 0f)
				{
					link.Phase = 1f;
				}
				else
				{
					link.Phase = -1f;
				}
			}
			else
			{
				link.TargetAngle = 0f;
			}
		}
    }
}
