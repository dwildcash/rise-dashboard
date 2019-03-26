namespace rise.Helpers
{
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Defines the <see cref="RandomGenerator" />
    /// </summary>
    internal static class RandomGenerator
    {
        public static long NextLong(long min, long max)
        {
            if (min > max) throw new ArgumentOutOfRangeException(nameof(min));
            if (min == max) return min;

            using (var rng = new RNGCryptoServiceProvider())
            {
                var data = new byte[16];
                rng.GetBytes(data);

                Int64 generatedValue = Math.Abs(BitConverter.ToInt64(data, startIndex: 0));

                Int64 diff = max - min;
                Int64 mod = generatedValue % diff;
                Int64 normalizedNumber = min + mod;

                return normalizedNumber;
            }
        }
    }
}