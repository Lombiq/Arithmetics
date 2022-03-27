namespace Lombiq.Arithmetics;

public class UnumConfiguration
{
    /// <summary>
    /// Gets the number of bits in the exponent.
    /// </summary>
    public byte ExponentSize { get; }

    /// <summary>
    /// Gets the number of bits in the fraction.
    /// </summary>
    public byte FractionSize { get; }

    public UnumConfiguration(byte exponentSize, byte fractionSize)
    {
        ExponentSize = exponentSize;
        FractionSize = fractionSize;
    }

    public static UnumConfiguration FromIeeeConfiguration(IeeeConfiguration configuration) =>
        configuration switch
        {
            IeeeConfiguration.HalfPrecision => new UnumConfiguration(5, 10),
            IeeeConfiguration.SinglePrecision => new UnumConfiguration(8, 23),
            IeeeConfiguration.DoublePrecision => new UnumConfiguration(11, 52),
            IeeeConfiguration.ExtendedPrecision => new UnumConfiguration(15, 64),
            IeeeConfiguration.QuadPrecision => new UnumConfiguration(15, 112),
            _ => new UnumConfiguration(0, 0),
        };
}

public enum IeeeConfiguration
{
    HalfPrecision,     // 16-bit.
    SinglePrecision,   // 32-bit.
    DoublePrecision,   // 64-bit.
    ExtendedPrecision, // 80-bit (Intel x87).
    QuadPrecision,      // 128-bit.
}
