using System;
using System.Collections.Generic;
using System.Numerics;

namespace Lombiq.Arithmetics.Tests;

/// <summary>
/// Shared oracle for the literal Standard (2022), section 4.1 (p. 8) rounding of an exact signed dyadic or rational
/// value to an n-bit E2 posit. It only decodes encodings with <see cref="PositEncodingReference"/> and compares exact
/// values: no production arithmetic, arithmetic midpoints, finite approximations or IEEE conversions.
/// </summary>
internal static class PositRoundingReference
{
    private static readonly Dictionary<int, PositEncodingReference[]> OrderedValues = new()
    {
        [8] = DecodeOrderedValues(8),
        [16] = DecodeOrderedValues(16),
    };

    /// <summary>Returns the n-bit representation of <paramref name="exact"/> rounded per section 4.1.</summary>
    public static uint Round(PositEncodingReference exact, int size)
    {
        if (exact.IsNaR)
        {
            if (size is < 2 or > 32) throw new ArgumentOutOfRangeException(nameof(size));
            return 1U << (size - 1);
        }

        return exact.Exponent >= 0
            ? Round(exact.Coefficient << exact.Exponent, BigInteger.One, size)
            : Round(exact.Coefficient, BigInteger.One << -exact.Exponent, size);
    }

    /// <summary>
    /// Returns the n-bit representation of the exact real <paramref name="numerator"/> / <paramref
    /// name="denominator"/> rounded per section 4.1. Callers handle exceptional operations, such as division by zero,
    /// themselves; a zero <paramref name="denominator"/> is rejected.
    /// </summary>
    public static uint Round(BigInteger numerator, BigInteger denominator, int size)
    {
        if (size is < 2 or > 32) throw new ArgumentOutOfRangeException(nameof(size));
        if (denominator.IsZero) throw new ArgumentOutOfRangeException(nameof(denominator));
        if (numerator.IsZero) return 0;
        if (denominator.Sign < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        var sign = 1U << (size - 1);
        var mask = sign | (sign - 1);
        long maxPos = sign - 1;
        var negative = numerator.Sign < 0;
        var magnitude = BigInteger.Abs(numerator);
        if (Compare(magnitude, denominator, Value(maxPos, size)) > 0) return ToBits(negative ? -maxPos : maxPos, mask);
        if (Compare(magnitude, denominator, Value(1, size)) < 0) return ToBits(negative ? -1 : 1, mask);

        // Section 5.3: real posit values are ordered as their representations regarded as 2's complement integers, so
        // this finds the largest n-bit u <= x; -maxPos <= x holds after clamping.
        var lower = -maxPos;
        var upper = maxPos;
        while (lower < upper)
        {
            var middle = lower + ((upper - lower + 1) / 2);
            if (Compare(numerator, denominator, Value(middle, size)) >= 0) lower = middle;
            else upper = middle - 1;
        }

        var u = ToBits(lower, mask);
        if (Compare(numerator, denominator, Value(lower, size)) == 0) return u;

        // x is strictly inside (u, w): v is the (n+1)-bit posit value of the representation U1.
        var v = PositEncodingReference.Decode(((ulong)u << 1) | 1, size + 1);
        var comparison = Compare(numerator, denominator, v);
        return comparison < 0 || (comparison == 0 && (u & 1) == 0) ? u : ToBits(lower + 1, mask);
    }

    // The sign of numerator / denominator - value for a positive denominator, by exact cross-multiplication.
    private static int Compare(BigInteger numerator, BigInteger denominator, PositEncodingReference value) =>
        value.Exponent >= 0
            ? numerator.CompareTo((value.Coefficient * denominator) << value.Exponent)
            : (numerator << -value.Exponent).CompareTo(value.Coefficient * denominator);

    private static PositEncodingReference Value(long signedBits, int size) =>
        OrderedValues.TryGetValue(size, out var values)
            ? values[signedBits + ((1L << (size - 1)) - 1)]
            : PositEncodingReference.Decode(ToBits(signedBits, (uint)((1UL << size) - 1)), size);

    private static uint ToBits(long signedBits, uint mask) => (uint)((ulong)signedBits & mask);

    private static PositEncodingReference[] DecodeOrderedValues(int size)
    {
        var maxPos = (1L << (size - 1)) - 1;
        var mask = (uint)((1UL << size) - 1);
        var values = new PositEncodingReference[(2 * maxPos) + 1];
        for (var signedBits = -maxPos; signedBits <= maxPos; signedBits++)
        {
            values[signedBits + maxPos] = PositEncodingReference.Decode(ToBits(signedBits, mask), size);
        }

        return values;
    }
}
