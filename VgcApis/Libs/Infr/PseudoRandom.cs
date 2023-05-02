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
        private static RNGCryptoServiceProvider global;

        // Thread-local pseudo-random generator
        [ThreadStatic]
        private static Random local;

        // Gets or initializes a thread-local Random instance.
        private static Random Local
        {
            get
            {
                Random random = local;
                if (random == null)
                {
                    byte[] buffer = new byte[4];
                    global.GetBytes(buffer);
                    int seed = BitConverter.ToInt32(buffer, 0);
                    random = local = new Random(seed);
                }
                return random;
            }
        }

        static PseudoRandom()
        {
            global = new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        public static int Next()
        {
            return Local.Next();
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        public static int Next(int maxValue)
        {
            return Local.Next(maxValue);
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        public static int Next(int minValue, int maxValue)
        {
            return Local.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a random double between 0.0 and 1.0.
        /// </summary>
        public static double NextDouble()
        {
            return Local.NextDouble();
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
            return Local.NextDouble() * (maxValue - minValue) + minValue;
        }
    }

}
