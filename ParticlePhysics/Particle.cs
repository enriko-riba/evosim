using System;
using System.Collections.Generic;
using System.Numerics;

namespace ParticlePhysics
{
    /// <summary>
    /// Represents a particle for simulation.
    /// </summary>
    public class Particle
    {
        /// <summary>
        /// Current position of the particle.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Position of the particle from the previously performed simulation step.
        /// </summary>
        public Vector2 OldPosition { get; set; }

        /// <summary>
        /// Radius of the particle for collision detection. Should not be negative.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Mass of the particle. Should not be zero or negative.
        /// </summary>
        public float Mass { get; set; } = 1f;

		/// <summary>
		/// Is particle on the ground? This is re-evalueated in every simulation step, it does not necessarily reflect the real current state of the particle in relation to the ground (i.e. if the particle has been manually moved).  
		/// </summary>
		public bool IsTouchingGround { get; internal set; }

		/// <summary>
		/// Friction coeff. of the particle. Should not be negative.
		/// </summary>
		public float Friction { get; set; }

        /// <summary>
        /// Is particle simulated by a simulation?
        /// </summary>
        public bool IsSimulated { get; internal set; }

		public Particle() { }

		public Particle(Vector2 position)
		{
			SetPosition(position);
		}

		/// <summary>
		/// Sets the current and previous position of the particle to a specified value - particle won't suddenly gain energy on next simulation step and will in fact possibly lose energy.
		/// </summary>
		/// <param name="value">Value to set.</param>
		public void SetPosition(Vector2 value)
		{
			Position = value;
			OldPosition = value;
		}

		/// <summary>
		/// Sets the position of the particle to a specified value, but conserves the velocity of the particle, i.e it will not introduce or remove energy from the particle.
		/// </summary>
		/// <param name="value">Value to set.</param>
		public void SetPositionConservative(Vector2 value)
		{
			var dp = Position - OldPosition;

			Position = value;
			OldPosition = value - dp;
		}
    }
}
