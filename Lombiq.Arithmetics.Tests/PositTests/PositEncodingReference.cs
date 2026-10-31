using System;
using System.Numerics;

namespace Lombiq.Arithmetics.Tests;

/// <summary>
/// Exact E2 reference using the original signed fields in Standard (2022), section 3.3, p. 7.
/// Deliberately independent of production decoding, magnitude transforms and floating-point conversions.
/// A value is <see cref="Coefficient"/> × 2^<see cref="Exponent"/>, or NaR.
/// </summary>
internal readonly record struct PositEncodingReference(BigInteger Coefficient, int Exponent, bool IsNaR = false)
    : IComparable<PositEncodingReference>
{
    public static PositEncodingReference NaR { get; } = new(0, 0, IsNaR: true);

    /// <summary>
    /// Creates the exact signed dyadic value <paramref name="coefficient"/> × 2^<paramref name="exponent"/>, which need
    /// not be a posit value of any precision. It is normalized to an odd coefficient (or 0 × 2^0), so record equality
    /// is value equality, as for <see cref="Decode"/>.
    /// </summary>
    public static PositEncodingReference FromDyadic(BigInteger coefficient, int exponent)
    {
        if (coefficient.IsZero) return new(0, 0);
        while (coefficient.IsEven)
        {
            coefficient >>= 1;
            exponent++;
        }

        return new(coefficient, exponent);
    }

    /// <summary>
    /// Decodes an n-bit E2 posit for any 2 ≤ n ≤ 64, so (n+1)-bit section 4.1 boundaries such as a 33-bit U1 decode
    /// directly.
    /// </summary>
    public static PositEncodingReference Decode(ulong bits, int size)
    {
        if (size is < 2 or > 64) throw new ArgumentOutOfRangeException(nameof(size));
        var signMask = 1UL << (size - 1);
        if ((bits & ~(signMask | (signMask - 1))) != 0) throw new ArgumentOutOfRangeException(nameof(bits));
        if (bits == 0 || bits == signMask) return new(0, 0, bits == signMask);

        var sign = (bits & signMask) == 0 ? 0 : 1;
        var position = size - 2;
        var regimeBit = (bits >> position) & 1;
        var run = 0;
        while (position >= 0 && ((bits >> position) & 1) == regimeBit)
        {
            run++;
            position--;
        }

        var regime = regimeBit == 0 ? -run : run - 1;
        position--; // Consume the terminator, even if it is beyond the stored LSB.
        var exponent = 0;
        for (var i = 0; i < 2; i++, position--)
        {
            exponent <<= 1;
            if (position >= 0) exponent |= (int)((bits >> position) & 1);
        }

        var fractionLength = Math.Max(0, position + 1);
        var fraction = bits & ((1UL << fractionLength) - 1);
        var coefficient = ((BigInteger)(1 - (3 * sign)) << fractionLength) + fraction;
        var scale = ((1 - (2 * sign)) * ((4 * regime) + exponent + sign)) - fractionLength;
        return FromDyadic(coefficient, scale);
    }

    public double ToDouble() => IsNaR ? double.NaN : Math.ScaleB((double)Coefficient, Exponent);

    public int CompareTo(PositEncodingReference other)
    {
        if (IsNaR) return other.IsNaR ? 0 : -1;
        if (other.IsNaR) return 1;
        var commonExponent = Math.Min(Exponent, other.Exponent);
        return (Coefficient << (Exponent - commonExponent))
            .CompareTo(other.Coefficient << (other.Exponent - commonExponent));
    }
}
