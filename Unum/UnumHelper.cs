namespace Lombiq.Arithmetics
{
    public static class UnumHelper
    {
        /// <summary>
        /// Calculates the maximum number of bits to describe the size of the given segment,
        /// e.g. "eSizeSize" for "eSize", which is essentially the binary logarithm of the segment size.
        /// </summary>
        /// <param name="segmentSize">The size of the unum segment (fraction or exponent).</param>
        /// <returns>The maximum size of the unum segment size.</returns>
        public static byte SegmentSizeToSegmentSizeSize(ushort segmentSize)
        {
            if (segmentSize == 0 || segmentSize == 1) return 0;

            segmentSize--;

            byte position = 15; // Position of the most significant 1-bit.
            while ((segmentSize >> position) == 0) { position--; }

            return ++position;
        }

        /// <summary>
        /// Calculates the maximum number of bits of a segment given its "segment size size",
        /// which is essentially 2 to the power of "segmentSizeSize".
        /// </summary>
        /// <param name="segmentSizeSize">The size of the segment size.</param>
        /// <returns>The maximum number of bits of a segment.</returns>
        public static ushort SegmentSizeSizeToSegmentSize(byte segmentSizeSize) =>
            (ushort)(1 << segmentSizeSize);

        /// <summary>
        /// Calculates whether a unum with the given configuration of exponent and fraction size can fit
        /// into the given number of bits.
        /// </summary>
        /// <param name="eSize">The maximum size of the exponent.</param>
        /// <param name="fSize">The maximum size of the fraction.</param>
        /// <param name="maximumSize">The maximum size allowed for the unum.</param>
        /// <returns>Whether the number of bits required to store the unum
        /// with the given configuration fits the desired maximum size.</returns>
        public static bool ConfigurationFitsSize(byte eSize, ushort fSize, ushort maximumSize) =>
            ConfigurationRequiredMaximumBits(eSize, fSize) <= maximumSize;

        /// <summary>
        /// Calculates the maximum necessary number of bits that a unum with the given configuration requires.
        /// </summary>
        /// <param name="eSize">The maximum size of the exponent.</param>
        /// <param name="fSize">The maximum size of the fraction.</param>
        /// <returns>The maximum number of bits for the given configuration.</returns>
        public static ushort ConfigurationRequiredMaximumBits(byte eSize, ushort fSize) =>
            // Sign bit + exponent size + fraction size +
            // uncertainty bit + exponent size size + fraction size size.
            (ushort)(1 + eSize + fSize +
                1 + SegmentSizeToSegmentSizeSize(eSize) + SegmentSizeToSegmentSizeSize(fSize));

        /// <summary>
        /// Calculates the maximum number of bits required for the given unum environment.
        /// </summary>
        /// <param name="eSizeSize">The size of the maximum size of the exponent.</param>
        /// <param name="fSizeSize">The size of the maximum size of the fraction.</param>
        /// <param name="maximumSize">The maximum size allowed for the unum.</param>
        /// <returns>Whether the unum of the given environment fits the desired number of bits.</returns>
        public static bool EnvironmentFitsSize(byte eSizeSize, byte fSizeSize, ushort maximumSize) =>
            EnvironmentRequiredMaximumBits(eSizeSize, fSizeSize) <= maximumSize;

        /// <summary>
        /// Calculates the maximum necessary number of bits that a unum with the given environment requires.
        /// </summary>
        /// <param name="eSizeSize">The size of the maximum size of the exponent.</param>
        /// <param name="fSizeSize">The size of the maximum size of the fraction.</param>
        /// <returns>The maximum number of bits for the given environment.</returns>
        public static ushort EnvironmentRequiredMaximumBits(byte eSizeSize, byte fSizeSize) =>
            // Sign bit + exponent size + fraction size +
            // uncertainty bit + exponent size size + fraction size size.
            (ushort)(1 + SegmentSizeSizeToSegmentSize(eSizeSize) + SegmentSizeSizeToSegmentSize(fSizeSize) +
                1 + eSizeSize + fSizeSize);

        public static int BitsRequiredByLargestExpressablePositiveInteger(UnumEnvironment environment) =>
            (1 << (environment.ExponentSizeMax - 1)) + 1;


        /// <summary>
        /// Calculates the biggest expressible integer in the given environment in an integer-like notation.
        /// Returns an empty BitMask if the calculated number would be too big to fit in a BitMask of
        /// the size of the environment.
        /// </summary>
        /// <param name="environment">The environment thats Largest Expressible Integer needs to be calculated.</param>
        /// <returns>
        /// The biggest expressible integer in the given environment if it fits in a BitMask the size of the
        /// environment, an empty BitMask otherwise.
        /// </returns>
        public static BitMask LargestExpressablePositiveInteger(UnumEnvironment environment)
        {
            if (BitsRequiredByLargestExpressablePositiveInteger(environment) >
                environment.EmptyBitMask.SegmentCount * 32) return environment.EmptyBitMask;

            return environment.EmptyBitMask.SetOne((ushort)(environment.FractionSizeMax)) - 1 <<
                     (1 << (environment.ExponentSizeMax - 1)) - environment.FractionSizeMax + 1;
        }
    }
}
