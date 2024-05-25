using System;
using System.Security.Cryptography;

namespace VgcApis.Libs.Infr
{
    // https://gist.github.com/jaykang920/8234457

    /// <summary>
    /// A fast thread-safe wrapper for the default pseudo-random number generator.
    /// </summary>
    public static class PseudoRandom
    {
        // Global seed generator
        private static readonly RNGCryptoServiceProvider crnd;

        private static readonly Random rnd;

        static PseudoRandom()
        {
            crnd = new RNGCryptoServiceProvider();

            byte[] buffer = new byte[4];
            crnd.GetBytes(buffer);
            int seed = BitConverter.ToInt32(buffer, 0);
            rnd = new Random(seed);
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        public static int Next()
        {
            lock (rnd)
            {
                return rnd.Next();
            }
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        public static int Next(int maxValue)
        {
            lock (rnd)
            {
                return rnd.Next(maxValue);
            }
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        public static int Next(int minValue, int maxValue)
        {
            lock (rnd)
            {
                return rnd.Next(minValue, maxValue);
            }
        }

        /// <summary>
        /// Returns a random double between 0.0 and 1.0.
        /// </summary>
        public static double NextDouble()
        {
            lock (rnd)
            {
                return rnd.NextDouble();
            }
        }

        /// <summary>
        /// Returns a nonnegative random double less than the specified maximum.
        /// </summary>
        public static double NextDouble(double maxValue)
        {
            return NextDouble(0.0, maxValue);
        }

        /// <summary>
        /// REturns a random double within a specified range.
        /// </summary>
        public static double NextDouble(double minValue, double maxValue)
        {
            lock (rnd)
            {
                return rnd.NextDouble() * (maxValue - minValue) + minValue;
            }
        }
    }
}
