namespace Lombiq.Arithmetics
{
    public class UnumConfiguration
    {
        /// <summary>
        /// The number of bits in the exponent.
        /// </summary>
        public readonly byte ExponentSize;

        /// <summary>
        /// The number of bits in the fraction.
        /// </summary>
        public readonly byte FractionSize;

        public UnumConfiguration(byte exponentSize, byte fractionSize)
        {
            ExponentSize = exponentSize;
            FractionSize = fractionSize;
        }

        public static UnumConfiguration FromIeeeConfiguration(IeeeConfiguration configuration)
        {
            switch (configuration)
            {
                case IeeeConfiguration.HalfPrecision:
                    return new UnumConfiguration(5, 10);
                case IeeeConfiguration.SinglePrecision:
                    return new UnumConfiguration(8, 23);
                case IeeeConfiguration.DoublePrecision:
                    return new UnumConfiguration(11, 52);
                case IeeeConfiguration.ExtendedPrecision:
                    return new UnumConfiguration(15, 64);
                case IeeeConfiguration.QuadPrecision:
                    return new UnumConfiguration(15, 112);
                default:
                    return new UnumConfiguration(0, 0);
            }
        }
    }

    public enum IeeeConfiguration
    {
        HalfPrecision,     // 16-bit.
        SinglePrecision,   // 32-bit.
        DoublePrecision,   // 64-bit.
        ExtendedPrecision, // 80-bit (Intel x87).
        QuadPrecision      // 128-bit.
    }
}