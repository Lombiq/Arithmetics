using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Lombiq.Arithmetics.Tests;

// Posit Standard (2022) section 5.4 division as the exact rational quotient of the independently decoded operands,
// rounded once by the section 4.1 reference. Production arithmetic is never consulted.
internal static class PositDivisionReference
{
    public static uint Divide(uint leftBits, uint rightBits, int size)
    {
        var left = PositEncodingReference.Decode(leftBits, size);
        var right = PositEncodingReference.Decode(rightBits, size);

        // Section 5.1: NaR operands and x / 0, including 0 / 0, have no unique real result.
        if (left.IsNaR || right.IsNaR || right.Coefficient.IsZero) return 1U << (size - 1);

        var shift = left.Exponent - right.Exponent;
        return PositRoundingReference.Round(
            shift > 0 ? left.Coefficient << shift : left.Coefficient,
            shift < 0 ? right.Coefficient << -shift : right.Coefficient,
            size);
    }

    [SuppressMessage(
        "Security",
        "CA5394:Do not use insecure randomness",
        Justification = "Deterministic numerical test vectors, not security-sensitive randomness.")]
    public static IEnumerable<(uint Left, uint Right)> Pairs(int size)
    {
        if (size == 8)
        {
            return Enumerable.Range(0, 1 << 16).Select(pair => ((uint)pair >> 8, (uint)pair & 0xFF));
        }

        var random = new Random(20_220_507);
        var operands = BoundaryOperands(size).ToList();
        var randomOperands = Enumerable.Range(0, 8192).Select(_ => (uint)random.NextInt64(1L << size)).ToList();

        return operands.SelectMany(left => operands.Select(right => (left, right)))
            .Concat(PowerOfTwoDivisorPairs(size, randomOperands.Take(48).Concat(operands.Take(16))))
            .Concat(NearTargetPairs(size, random))
            .Concat(randomOperands.Zip(randomOperands.Skip(1), (left, right) => (left, right)));
    }

    // Zero, NaR, the extremes and both neighbors of every regime transition 2^(4k), with both signs.
    private static IEnumerable<uint> BoundaryOperands(int size)
    {
        var nar = 1U << (size - 1);
        var mask = size == 32 ? uint.MaxValue : (1U << size) - 1;
        var magnitudes = new List<uint> { 1, 2, nar - 2, nar - 1 };
        for (var scale = -4 * (size - 2); scale <= 4 * (size - 2); scale += 4)
        {
            var center = PowerOfTwo(scale, size);
            magnitudes.AddRange([center - 1, center, center + 1]);
        }

        var positive = magnitudes.Where(magnitude => magnitude is not 0 && magnitude < nar).Distinct().ToList();
        return new[] { 0U, nar }.Concat(positive).Concat(positive.Select(magnitude => (0U - magnitude) & mask));
    }

    // Exact power-of-two divisors shift every dividend through all regime lengths, including exponent-truncating regions
    // where exact quotients tie.
    private static IEnumerable<(uint Left, uint Right)> PowerOfTwoDivisorPairs(int size, IEnumerable<uint> dividends)
    {
        var mask = size == 32 ? uint.MaxValue : (1U << size) - 1;
        var divisors = Enumerable.Range(-4 * (size - 2), (8 * (size - 2)) + 1)
            .Select(scale => PowerOfTwo(scale, size))
            .SelectMany(divisor => new[] { divisor, (0U - divisor) & mask })
            .ToList();

        return dividends.SelectMany(left => divisors.Select(right => (left, right)));
    }

    // A dividend rounded from target × divisor puts the quotient on the target when that product is representable, and
    // otherwise within one dividend rounding error of it, where the remainder alone can decide. Odd (Size + 1)-bit codes
    // are the section 4.1 U1 rounding boundaries; even codes are the Size-bit posits themselves.
    [SuppressMessage(
        "Security",
        "CA5394:Do not use insecure randomness",
        Justification = "Deterministic numerical test vectors, not security-sensitive randomness.")]
    private static IEnumerable<(uint Left, uint Right)> NearTargetPairs(int size, Random random)
    {
        var mask = size == 32 ? uint.MaxValue : (1U << size) - 1;
        for (var i = 0; i < 8192; i++)
        {
            var target = PositEncodingReference.Decode((ulong)random.NextInt64(1L << (size + 1)), size + 1);
            var rightBits = i % 2 == 0
                ? (uint)random.NextInt64(1L << size)
                : PowerOfTwo((int)random.NextInt64(-4 * (size - 2), (4 * (size - 2)) + 1), size);
            if (random.NextInt64(2) == 0) rightBits = (0U - rightBits) & mask;

            var right = PositEncodingReference.Decode(rightBits, size);
            if (target.IsNaR || right.IsNaR || right.Coefficient.IsZero) continue;

            var product = PositEncodingReference.FromDyadic(target.Coefficient * right.Coefficient, target.Exponent + right.Exponent);
            yield return (PositRoundingReference.Round(product, size), rightBits);
        }
    }

    private static uint PowerOfTwo(int exponent, int size) =>
        PositRoundingReference.Round(PositEncodingReference.FromDyadic(BigInteger.One, exponent), size);
}
