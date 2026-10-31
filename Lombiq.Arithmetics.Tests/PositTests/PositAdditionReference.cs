using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Lombiq.Arithmetics.Tests;

/// <summary>
/// Expected Standard (2022), section 5.4 (p. 9) addition and subtraction of n-bit E2 posits: the exact signed sum of
/// the operands decoded by <see cref="PositEncodingReference"/>, rounded once by the shared section 4.1 oracle
/// <see cref="PositRoundingReference.Round(PositEncodingReference, int)"/>. It uses no production arithmetic,
/// arithmetic midpoints or IEEE conversions.
/// </summary>
[SuppressMessage(
    "Security",
    "CA5394:Do not use insecure randomness",
    Justification = "Deterministic numerical test vectors, not security-sensitive randomness.")]
internal static class PositAdditionReference
{
    public static uint Add(uint left, uint right, int size) => Evaluate(left, right, size, subtract: false);

    public static uint Subtract(uint left, uint right, int size) => Evaluate(left, right, size, subtract: true);

    /// <summary>Returns the exact sum, or NaR if either addend is NaR (section 5.1).</summary>
    public static PositEncodingReference Sum(PositEncodingReference left, PositEncodingReference right)
    {
        if (left.IsNaR || right.IsNaR) return PositEncodingReference.NaR;

        var exponent = Math.Min(left.Exponent, right.Exponent);
        return PositEncodingReference.FromDyadic(
            (left.Coefficient << (left.Exponent - exponent)) + (right.Coefficient << (right.Exponent - exponent)),
            exponent);
    }

    public static PositEncodingReference Negate(PositEncodingReference value) =>
        value.IsNaR ? value : value with { Coefficient = -value.Coefficient };

    /// <summary>Returns the n-bit encoding of exactly 2^<paramref name="scale"/>, rounded if not representable.</summary>
    public static uint PowerOfTwo(int scale, int size) =>
        PositRoundingReference.Round(PositEncodingReference.FromDyadic(BigInteger.One, scale), size);

    /// <summary>Returns the n-bit two's complement of <paramref name="bits"/>: the encoding of the negated value.</summary>
    public static uint NegateBits(uint bits, int size) => unchecked(0U - bits) & Mask(size);

    /// <summary>Returns the mask of the <paramref name="size"/> low bits that hold an n-bit encoding.</summary>
    public static uint Mask(int size) => size == 32 ? uint.MaxValue : (1U << size) - 1;

    /// <summary>
    /// Returns every pair for posit8, otherwise deterministic hard and seeded pairs: NaR and zero combinations, exact and
    /// near cancellation, signed maxPos/minPos clamping, every scale difference up to the maximum span, exact section 4.1
    /// ties with their neighbors in both signs, and uniformly random pairs.
    /// </summary>
    public static IEnumerable<(uint Left, uint Right)> Pairs(int size)
    {
        if (size == 8) return AllPosit8Pairs();

        // Concat enumerates lazily in order, so the seeded generators share one deterministic sequence.
        var random = new Random(20_220_505);
        return SpecialPairs(size)
            .Concat(RegimeBoundaryPairs(size))
            .Concat(ScaleDifferencePairs(size))
            .Concat(TiePairs(size, random))
            .Concat(CancellationPairs(size, random))
            .Concat(RandomPairs(size, random));
    }

    /// <summary>
    /// Returns <c>(a + b) - c</c>, <c>a + (b - c)</c> and <c>(a - c) + b</c>, each operator rounded in source order
    /// (sections 4.2-4.3, p. 8).
    /// </summary>
    public static uint[] EvaluateExpressions(uint a, uint b, uint c, int size) =>
    [
        Subtract(Add(a, b, size), c, size),
        Add(a, Subtract(b, c, size), size),
        Add(Subtract(a, c, size), b, size),
    ];

    /// <summary>Returns <c>a + b - c</c> rounded once, as only explicit fusion may compute it (section 4.3, p. 8).</summary>
    public static uint FusedSum(uint a, uint b, uint c, int size) =>
        PositRoundingReference.Round(
            Sum(
                Sum(PositEncodingReference.Decode(a, size), PositEncodingReference.Decode(b, size)),
                Negate(PositEncodingReference.Decode(c, size))),
            size);

    /// <summary>Returns triples whose source-order <c>(a + b) - c</c> differs from <see cref="FusedSum"/>.</summary>
    public static IEnumerable<(uint A, uint B, uint C)> DistinguishingTriples(int size)
    {
        var maxPos = (1U << (size - 1)) - 1;
        var mask = size == 32 ? uint.MaxValue : (1U << size) - 1;
        var one = PowerOfTwo(0, size);
        var large = PowerOfTwo(4 * (size - 4), size);
        var small = PowerOfTwo(-4 * (size - 4), size);
        var halfUnitInTheLastPlace = PowerOfTwo(-(size - 4), size);

        yield return (large, small, large); // (a + b) - a = 0, while a + b - a = b.
        yield return (one, halfUnitInTheLastPlace, unchecked(0U - halfUnitInTheLastPlace) & mask); // 1, not 1 + ulp.
        yield return (maxPos, maxPos, maxPos); // Clamped to maxPos, then 0.
        yield return (1, 1, 1); // 2 minPos rounds to minPos, then 0.
    }

    public static IEnumerable<(uint A, uint B, uint C)> RandomTriples(int size)
    {
        var random = new Random(20_220_505 + size);
        for (var i = 0; i < 2048; i++)
        {
            yield return ((uint)random.NextInt64(1L << size), (uint)random.NextInt64(1L << size), (uint)random.NextInt64(1L << size));
        }
    }

    private static IEnumerable<(uint Left, uint Right)> AllPosit8Pairs()
    {
        for (uint left = 0; left < 256; left++)
        {
            for (uint right = 0; right < 256; right++) yield return (left, right);
        }
    }

    private static IEnumerable<(uint Left, uint Right)> SpecialPairs(int size)
    {
        var maxPos = MaxPos(size);
        var maxScale = 4 * (size - 2);
        var special = new List<uint> { 0, maxPos + 1, 1, 2, 3, maxPos - 2, maxPos - 1, maxPos };
        foreach (var scale in new[] { -maxScale + 1, -maxScale / 2, -5, -4, -3, -1, 0, 1, 3, 4, 5, maxScale / 2, maxScale - 1 })
        {
            var power = PowerOfTwo(scale, size);
            special.AddRange([Offset(power, -1, size), power, Offset(power, 1, size)]);
        }

        var signedSpecial = new List<uint>(special);
        signedSpecial.AddRange(special.ConvertAll(bits => NegateBits(bits, size)));
        foreach (var left in signedSpecial)
        {
            foreach (var right in signedSpecial) yield return (left, right);
        }
    }

    // Both sides of every regime boundary against fixed partners.
    private static IEnumerable<(uint Left, uint Right)> RegimeBoundaryPairs(int size)
    {
        var one = PowerOfTwo(0, size);
        var maxPos = MaxPos(size);
        for (var regime = -(size - 2); regime <= size - 2; regime++)
        {
            var power = PowerOfTwo(4 * regime, size);
            foreach (var boundary in new[] { power, Offset(power, -1, size) })
            {
                foreach (var partner in new[] { 1U, one, maxPos, boundary, Offset(boundary, 1, size), Offset(boundary, -1, size) })
                {
                    yield return (boundary, partner);
                    yield return (boundary, NegateBits(partner, size));
                    yield return (NegateBits(boundary, size), partner);
                }
            }
        }
    }

    // Every scale difference from 0 to the maximum span, across the exact and sticky alignment paths.
    private static IEnumerable<(uint Left, uint Right)> ScaleDifferencePairs(int size)
    {
        var maxPos = MaxPos(size);
        var maxScale = 4 * (size - 2);
        foreach (var leftScale in new[] { maxScale, maxScale - 1, (maxScale / 2) + 1, 0, -1, -maxScale / 2, -maxScale + 4 })
        {
            var leftPower = PowerOfTwo(leftScale, size);
            var lefts = new[] { leftPower, Offset(leftPower, leftPower == maxPos ? -2 : 1, size), Offset(leftPower, -1, size) };
            for (var rightScale = leftScale; rightScale >= -maxScale; rightScale--)
            {
                var rightPower = PowerOfTwo(rightScale, size);
                var rights = new[] { rightPower, Offset(rightPower, 1, size), Offset(rightPower, rightPower == 1 ? 2 : -1, size) };
                foreach (var pair in CrossPairs(lefts, rights, size)) yield return pair;

                yield return (rightPower, leftPower);
                yield return (NegateBits(rightPower, size), leftPower);
            }
        }
    }

    private static IEnumerable<(uint Left, uint Right)> CrossPairs(uint[] lefts, uint[] rights, int size)
    {
        foreach (var left in lefts)
        {
            foreach (var right in rights)
            {
                yield return (left, right);
                yield return (left, NegateBits(right, size));
            }
        }
    }

    // Exact section 4.1 ties x = v(U1) and their neighbors, for both LSB parities of u and both signs.
    private static IEnumerable<(uint Left, uint Right)> TiePairs(int size, Random random)
    {
        var one = PowerOfTwo(0, size);
        var maxPos = MaxPos(size);
        for (var i = 0; i < 384; i++)
        {
            var u = i < 2 ? one + (uint)i : (uint)random.NextInt64(2, maxPos - 1);
            var lower = PositEncodingReference.Decode(u, size);
            var midpoint = PositEncodingReference.Decode(((ulong)u << 1) | 1, size + 1);
            var halfGap = Sum(midpoint, Negate(lower));
            var halfGapBits = PositRoundingReference.Round(halfGap, size);
            if (PositEncodingReference.Decode(halfGapBits, size).CompareTo(halfGap) != 0) continue;

            foreach (var right in new[] { halfGapBits, Offset(halfGapBits, 1, size), Offset(halfGapBits, -1, size) })
            {
                yield return (u, right);
                yield return (right, u);
                yield return (NegateBits(u, size), NegateBits(right, size));
            }
        }
    }

    // Exact and near cancellation.
    private static IEnumerable<(uint Left, uint Right)> CancellationPairs(int size, Random random)
    {
        for (var i = 0; i < 512; i++)
        {
            var left = (uint)random.NextInt64(1L << size);
            for (var delta = -2; delta <= 2; delta++)
            {
                yield return (left, Offset(left, delta, size));
                yield return (left, NegateBits(Offset(left, delta, size), size));
            }

            var far = Offset(left, (int)random.NextInt64(-4096, 4097), size);
            yield return (left, NegateBits(far, size));
        }
    }

    private static IEnumerable<(uint Left, uint Right)> RandomPairs(int size, Random random)
    {
        for (var i = 0; i < 4096; i++)
        {
            yield return ((uint)random.NextInt64(1L << size), (uint)random.NextInt64(1L << size));
        }
    }

    private static uint MaxPos(int size) => (1U << (size - 1)) - 1;

    private static uint Offset(uint bits, int delta, int size) => unchecked(bits + (uint)delta) & Mask(size);

    private static uint Evaluate(uint left, uint right, int size, bool subtract)
    {
        var rightValue = PositEncodingReference.Decode(right, size);
        return PositRoundingReference.Round(
            Sum(PositEncodingReference.Decode(left, size), subtract ? Negate(rightValue) : rightValue),
            size);
    }
}
