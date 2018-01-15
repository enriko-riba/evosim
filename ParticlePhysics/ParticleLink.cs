using System;
using System.Collections.Generic;

namespace ParticlePhysics
{
    /// <summary>
    /// Represents a link or a constraint between two particles.
    /// </summary>
    public class ParticleLink
    {
        /// <summary>
        /// Is particle link simulated by a simulation?
        /// </summary>
        public bool IsSimulated { get; internal set; }

        public Particle Particle1 { get; private set; }

        public Particle Particle2 { get; private set; }

		/// <summary>
		/// Resting length of the link.
		/// </summary>
        public float Length { get; set; }

        public float ContractionCoeff { get; set; }

        public float Frequency { get; set; }

		public float Phase { get; internal set; }

		//public float CurrentAngle { get; internal set; }

		public float TargetAngle { get; internal set; }

		public float TargetOscillationScale { get; internal set; }

        public ParticleLink(Particle particle1, Particle particle2, float length = 1f, float frequency = 1f, float contractionCoeff = 0.2f)
        {
            Particle1 = particle1 ?? throw new ArgumentNullException(nameof(particle1));
            Particle2 = particle2 ?? throw new ArgumentNullException(nameof(particle2));

            if (particle1 == particle2)
            {
                Particle1 = Particle2 = null;

                throw new ArgumentException("Cannot link a particle with itself.");
            }

            Length = length;
            Frequency = frequency;
            ContractionCoeff = contractionCoeff;
        }
    }
}
