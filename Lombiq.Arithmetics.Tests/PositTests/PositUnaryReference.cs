using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Lombiq.Arithmetics.Tests;

internal static class PositUnaryReference
{
    public static uint[] Evaluate(uint bits, int size)
    {
        var value = PositEncodingReference.Decode(bits, size);
        var mask = size == 32 ? uint.MaxValue : (1U << size) - 1;
        var nar = 1U << (size - 1);
        if (value.IsNaR)
        {
            return [nar, nar, nar, nar, nar, nar, (bits + 1) & mask, (bits - 1) & mask];
        }

        var numerator = value.Coefficient;
        var denominator = BigInteger.One;
        if (value.Exponent >= 0) numerator <<= value.Exponent;
        else denominator <<= -value.Exponent;

        var truncated = BigInteger.DivRem(numerator, denominator, out var remainder);
        var floor = remainder.Sign < 0 ? truncated - 1 : truncated;
        var ceiling = remainder.Sign > 0 ? truncated + 1 : truncated;
        var halfComparison = (2 * BigInteger.Abs(remainder)).CompareTo(denominator);
        var nearest = halfComparison > 0 || (halfComparison == 0 && !truncated.IsEven)
            ? truncated + numerator.Sign
            : truncated;

        return
        [
            EncodeExact(new(-value.Coefficient, value.Exponent), size),
            EncodeExact(new(BigInteger.Abs(value.Coefficient), value.Exponent), size),
            EncodeExact(new(numerator.Sign, 0), size),
            EncodeExact(new(nearest, 0), size),
            EncodeExact(new(ceiling, 0), size),
            EncodeExact(new(floor, 0), size),
            unchecked(bits + 1) & mask,
            unchecked(bits - 1) & mask,
        ];
    }

    // Search signed numeric order using only the independently decoded standard equation.
    // Unary integer results are exactly representable: this is not an arithmetic rounding oracle.
    public static uint EncodeExact(PositEncodingReference value, int size)
    {
        var sign = 1L << (size - 1);
        var mask = (1UL << size) - 1;
        var lower = -sign + 1;
        var upper = sign - 1;
        while (lower <= upper)
        {
            var middle = lower + ((upper - lower) / 2);
            var bits = (uint)((ulong)middle & mask);
            var comparison = PositEncodingReference.Decode(bits, size).CompareTo(value);
            if (comparison == 0) return bits;
            if (comparison < 0) lower = middle + 1;
            else upper = middle - 1;
        }

        throw new InvalidOperationException(string.Create(
            CultureInfo.InvariantCulture,
            $"The exact unary result {value} is not representable at precision {size}."));
    }

    [SuppressMessage(
        "Security",
        "CA5394:Do not use insecure randomness",
        Justification = "Deterministic numerical test vectors, not security-sensitive randomness.")]
    public static IEnumerable<uint> Inputs(int size)
    {
        if (size <= 16)
        {
            for (uint bits = 0; bits < (1U << size); bits++) yield return bits;
            yield break;
        }

        yield return 0;
        yield return 0x80000000;
        yield return uint.MaxValue;
        yield return 1;
        yield return 0x7FFFFFFF;
        yield return 0x80000001;

        // Both sides of every power of two, including regime transitions and the last nonintegral region.
        for (var exponent = -120; exponent <= 120; exponent++)
        {
            if ((exponent is < -112 or > 112) && exponent % 4 != 0) continue;

            var center = EncodeExact(new(BigInteger.One, exponent), size);
            for (var offset = -2; offset <= 2; offset++)
            {
                var bits = unchecked(center + (uint)offset);
                yield return bits;
                yield return unchecked(0U - bits);
            }
        }

        for (var twiceInteger = 1; twiceInteger <= 8193; twiceInteger++)
        {
            var center = EncodeExact(new(twiceInteger, -1), size);
            for (var offset = -1; offset <= 1; offset++)
            {
                var bits = unchecked(center + (uint)offset);
                yield return bits;
                yield return unchecked(0U - bits);
            }
        }

        var random = new Random(20_220_504);
        for (var i = 0; i < 8192; i++) yield return (uint)random.NextInt64(1L << size);
    }
}
