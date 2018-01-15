using System;

namespace ParticlePhysics
{
    /// <summary>
    /// Contains commonly used precalculated values.
    /// </summary>
    public static class MathUtil
    {
        /// <summary>Represents the value of pi.</summary>
        public const float Pi = (float)Math.PI;

        /// <summary>Represents the value of pi times two.</summary>
        public const float TwoPi = 6.28318548f;

        /// <summary>Represents the value of pi divided by two.</summary>
        public const float PiOver2 = 1.57079637f;

        /// <summary>Represents the value of pi divided by three.</summary>
        public const float PiOver3 = 1.04719755f;

        /// <summary>Represents the value of pi divided by four.</summary>
        public const float PiOver4 = 0.7853982f;

        /// <summary>
        /// Represents a constant that converts a value expressed in degrees to radians when multiplied with the value.
        /// </summary>
        public const float DegreesToRadiansScale = 0.0174532924f;

        /// <summary>
        /// Represents a constant that converts a value expressed in radians to degrees when multiplied with the value.
        /// </summary>
        public const float RadiansToDegreesScale = 57.2957764f;

        /// <summary>Converts degrees to radians.</summary>
        /// <param name="degrees">The angle in degrees.</param>
        public static float ToRadians(float degrees)
        {
            return degrees * DegreesToRadiansScale;
        }

        /// <summary>Converts radians to degrees.</summary>
        /// <param name="radians">The angle in radians.</param>
        public static float ToDegrees(float radians)
        {
            return radians * RadiansToDegreesScale;
		}

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
		/// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
		public static float Clamp(float value, float min, float max)
		{
			value = ((value > max) ? max : value);
			value = ((value < min) ? min : value);

			return value;
		}
	}
}
