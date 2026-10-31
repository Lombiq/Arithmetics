using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Lombiq.Arithmetics.Tests;

/// <summary>
/// Expected Standard (2022), section 5.4 (p. 9) addition and subtraction of n-bit E2 posits: the exact signed sum of
/// the operands decoded by <see cref="PositEncodingReference"/>, rounded once by the shared section 4.1 oracle
/// <see cref="PositRoundingReference.Round"/>. It uses no production arithmetic, arithmetic midpoints or IEEE
/// conversions.
/// </summary>
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

    /// <summary>
    /// Returns every pair for posit8, otherwise deterministic hard and seeded pairs: NaR and zero combinations, exact and
    /// near cancellation, signed maxPos/minPos clamping, every scale difference up to the maximum span, exact section 4.1
    /// ties with their neighbors in both signs, and uniformly random pairs.
    /// </summary>
    [SuppressMessage(
        "Security",
        "CA5394:Do not use insecure randomness",
        Justification = "Deterministic numerical test vectors, not security-sensitive randomness.")]
    public static IEnumerable<(uint Left, uint Right)> Pairs(int size)
    {
        if (size == 8)
        {
            for (uint left = 0; left < 256; left++)
            {
                for (uint right = 0; right < 256; right++) yield return (left, right);
            }

            yield break;
        }

        var mask = size == 32 ? uint.MaxValue : (1U << size) - 1;
        var nar = 1U << (size - 1);
        var maxPos = nar - 1;
        var maxScale = 4 * (size - 2);
        var one = PowerOfTwo(0, size);
        uint NegateBits(uint bits) => unchecked(0U - bits) & mask;
        uint Offset(uint bits, int offset) => unchecked(bits + (uint)offset) & mask;

        var special = new List<uint> { 0, nar, 1, 2, 3, maxPos - 2, maxPos - 1, maxPos };
        foreach (var scale in new[] { -maxScale + 1, -maxScale / 2, -5, -4, -3, -1, 0, 1, 3, 4, 5, maxScale / 2, maxScale - 1 })
        {
            var power = PowerOfTwo(scale, size);
            special.AddRange([Offset(power, -1), power, Offset(power, 1)]);
        }

        var signedSpecial = new List<uint>(special);
        signedSpecial.AddRange(special.ConvertAll(NegateBits));
        foreach (var left in signedSpecial)
        {
            foreach (var right in signedSpecial) yield return (left, right);
        }

        // Both sides of every regime boundary against fixed partners.
        for (var regime = -(size - 2); regime <= size - 2; regime++)
        {
            var power = PowerOfTwo(4 * regime, size);
            foreach (var boundary in new[] { power, Offset(power, -1) })
            {
                foreach (var partner in new[] { 1U, one, maxPos, boundary, Offset(boundary, 1), Offset(boundary, -1) })
                {
                    yield return (boundary, partner);
                    yield return (boundary, NegateBits(partner));
                    yield return (NegateBits(boundary), partner);
                }
            }
        }

        // Every scale difference from 0 to the maximum span, across the exact and sticky alignment paths.
        foreach (var leftScale in new[] { maxScale, maxScale - 1, (maxScale / 2) + 1, 0, -1, -maxScale / 2, -maxScale + 4 })
        {
            var leftPower = PowerOfTwo(leftScale, size);
            var lefts = new[] { leftPower, Offset(leftPower, leftPower == maxPos ? -2 : 1), Offset(leftPower, -1) };
            for (var rightScale = leftScale; rightScale >= -maxScale; rightScale--)
            {
                var rightPower = PowerOfTwo(rightScale, size);
                var rights = new[] { rightPower, Offset(rightPower, 1), Offset(rightPower, rightPower == 1 ? 2 : -1) };
                foreach (var left in lefts)
                {
                    foreach (var right in rights)
                    {
                        yield return (left, right);
                        yield return (left, NegateBits(right));
                    }
                }

                yield return (rightPower, leftPower);
                yield return (NegateBits(rightPower), leftPower);
            }
        }

        var random = new Random(20_220_505);

        // Exact section 4.1 ties x = v(U1) and their neighbors, for both LSB parities of u and both signs.
        for (var i = 0; i < 384; i++)
        {
            var u = i < 2 ? one + (uint)i : (uint)random.NextInt64(2, maxPos - 1);
            var lower = PositEncodingReference.Decode(u, size);
            var midpoint = PositEncodingReference.Decode(((ulong)u << 1) | 1, size + 1);
            var halfGap = Sum(midpoint, Negate(lower));
            var halfGapBits = PositRoundingReference.Round(halfGap, size);
            if (PositEncodingReference.Decode(halfGapBits, size).CompareTo(halfGap) != 0) continue;

            foreach (var right in new[] { halfGapBits, Offset(halfGapBits, 1), Offset(halfGapBits, -1) })
            {
                yield return (u, right);
                yield return (right, u);
                yield return (NegateBits(u), NegateBits(right));
            }
        }

        // Exact and near cancellation.
        for (var i = 0; i < 512; i++)
        {
            var left = (uint)random.NextInt64(1L << size);
            for (var offset = -2; offset <= 2; offset++)
            {
                yield return (left, Offset(left, offset));
                yield return (left, NegateBits(Offset(left, offset)));
            }

            var far = Offset(left, random.Next(-4096, 4097));
            yield return (left, NegateBits(far));
        }

        for (var i = 0; i < 4096; i++)
        {
            yield return ((uint)random.NextInt64(1L << size), (uint)random.NextInt64(1L << size));
        }
    }

    private static uint Evaluate(uint left, uint right, int size, bool subtract)
    {
        var rightValue = PositEncodingReference.Decode(right, size);
        return PositRoundingReference.Round(
            Sum(PositEncodingReference.Decode(left, size), subtract ? Negate(rightValue) : rightValue),
            size);
    }
}
