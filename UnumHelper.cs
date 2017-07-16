namespace Lombiq.Unum
{
    public static class UnumHelper
    {
        /// <summary>
        /// Calculates the maximum number of bits to describe the size of the given segment,
        /// e.g. "eSizeSize" for "eSize", which is essentially the binary logarithm of the segment size.
        /// </summary>
        /// <param name="segmentSize">The size of the Unum segment (fraction or exponent).</param>
        /// <returns>The maximum size of the Unum segment size.</returns>
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
        /// which is essentialy 2 to the power of "segmentSizeSize".
        /// </summary>
        /// <param name="segmentSizeSize">The size of the segment size.</param>
        /// <returns>The maximum number of bits of a segment.</returns>
        public static ushort SegmentSizeSizeToSegmentSize(byte segmentSizeSize) =>
            (ushort)(1 << segmentSizeSize);

        /// <summary>
        /// Calculates whether a Unum with the given configuration of exponent and fraction size can fit
        /// into the given number of bits.
        /// </summary>
        /// <param name="eSize">The maximum size of the exponent.</param>
        /// <param name="fSize">The maximum size of the fraction.</param>
        /// <param name="maximumSize">The maximum size allowed for the Unum.</param>
        /// <returns>Whether the number of bits required to store the Unum
        /// with the given configuration fits the desired maximum size.</returns>
        public static bool ConfigurationFitsSize(byte eSize, ushort fSize, ushort maximumSize) =>
            ConfigurationRequiredMaximumBits(eSize, fSize) <= maximumSize;

        /// <summary>
        /// Calculates the maximum necessary number of bits that a Unum with the given configuration requires.
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
        /// Calculates the maximum number of bits required for the given Unum environment.
        /// </summary>
        /// <param name="eSizeSize">The size of the maximum size of the exponent.</param>
        /// <param name="fSizeSize">The size of the maximum size of the fraction.</param>
        /// <param name="maximumSize">The maximum size allowed for the Unum.</param>
        /// <returns>Whether the Unum of the given environment fits the desired number of bits.</returns>
        public static bool EnvironmentFitsSize(byte eSizeSize, byte fSizeSize, ushort maximumSize) =>
            EnvironmentRequiredMaximumBits(eSizeSize, fSizeSize) <= maximumSize;

        /// <summary>
        /// Calculates the maximum necessary number of bits that a Unum with the given environment requires.
        /// </summary>
        /// <param name="eSizeSize">The size of the maximum size of the exponent.</param>
        /// <param name="fSizeSize">The size of the maximum size of the fraction.</param>
        /// <returns>The maximum number of bits for the given environment.</returns>
        public static ushort EnvironmentRequiredMaximumBits(byte eSizeSize, byte fSizeSize) =>
            // Sign bit + exponent size + fraction size +
            // uncertainty bit + exponent size size + fraction size size.
            (ushort)(1 + SegmentSizeSizeToSegmentSize(eSizeSize) + SegmentSizeSizeToSegmentSize(fSizeSize) +
                1 + eSizeSize + fSizeSize);
    }
}
