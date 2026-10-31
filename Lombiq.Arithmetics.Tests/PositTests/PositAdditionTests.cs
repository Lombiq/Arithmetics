using System;
using System.Collections.Generic;
using System.Numerics;
using Xunit;

namespace Lombiq.Arithmetics.Tests;

public class PositAdditionTests
{
    private const int ExpressionCount = 3;

    internal static void Verify(int size, Func<uint, uint, uint> add, Func<uint, uint, uint> subtract)
    {
        var count = 0;
        foreach (var (left, right) in PositAdditionReference.Pairs(size))
        {
            Check(size, "+", left, right, PositAdditionReference.Add(left, right, size), add(left, right));
            Check(size, "-", left, right, PositAdditionReference.Subtract(left, right, size), subtract(left, right));
            count++;
        }

        if (size == 8) Assert.Equal(65_536, count);
    }

    /// <summary>
    /// Verifies <c>(a + b) - c</c>, <c>a + (b - c)</c> and <c>(a - c) + b</c>, written as source expressions by
    /// <paramref name="evaluate"/>, round after every operator in source order (Standard (2022), sections 4.2-4.3,
    /// p. 8). The distinguishing triples have a different single-rounding (fused) result, which
    /// <paramref name="fusedSum"/> (explicit fusion of <c>a + b - c</c>) must not leak into the operators.
    /// </summary>
    internal static void VerifyExpressions(
        int size,
        Func<uint, uint, uint, uint[]> evaluate,
        Func<uint, uint, uint, uint> fusedSum)
    {
        foreach (var (a, b, c) in DistinguishingTriples(size))
        {
            var actual = VerifyExpression(size, evaluate, a, b, c);
            var fused = RoundExactExpression(a, b, c, size);
            Assert.NotEqual(fused, actual[0]);
            Assert.Equal(fused, fusedSum(a, b, c));
        }

        foreach (var (a, b, c) in RandomTriples(size)) VerifyExpression(size, evaluate, a, b, c);
    }

    [Fact]
    public void Posit32MatchesExactReferenceAndPosit32E2BitForBit() =>
        Verify(
            32,
            (left, right) =>
            {
                var result = (new Posit32(left, fromBitMask: true) + new Posit32(right, fromBitMask: true)).PositBits;
                Assert.Equal(
                    (new Posit32E2(left, fromBitMask: true) + new Posit32E2(right, fromBitMask: true)).PositBits,
                    result);
                return result;
            },
            (left, right) =>
            {
                var result = (new Posit32(left, fromBitMask: true) - new Posit32(right, fromBitMask: true)).PositBits;
                Assert.Equal(
                    (new Posit32E2(left, fromBitMask: true) - new Posit32E2(right, fromBitMask: true)).PositBits,
                    result);
                return result;
            });

    [Fact]
    public void Posit32ExpressionsRoundInSourceOrder() =>
        VerifyExpressions(
            32,
            (aBits, bBits, cBits) =>
            {
                var a = new Posit32(aBits, fromBitMask: true);
                var b = new Posit32(bBits, fromBitMask: true);
                var c = new Posit32(cBits, fromBitMask: true);
                return [((a + b) - c).PositBits, (a + (b - c)).PositBits, ((a - c) + b).PositBits];
            },
            (a, b, c) => Posit32.FusedSum(
            [
                new Posit32(a, fromBitMask: true),
                new Posit32(b, fromBitMask: true),
                -new Posit32(c, fromBitMask: true),
            ]).PositBits);

    [Theory]
    [InlineData(16)]
    [InlineData(32)]
    public void HardPairsReachEveryRoundingCase(int size)
    {
        var nar = 1U << (size - 1);
        var mask = size == 32 ? uint.MaxValue : (1U << size) - 1;
        var maxScale = 4 * (size - 2);
        var maxPos = PositEncodingReference.Decode(nar - 1, size);
        var minPos = PositEncodingReference.Decode(1, size);
        var cases = new HashSet<string>(StringComparer.Ordinal);
        var scaleDifferences = new HashSet<int>();
        foreach (var (leftBits, rightBits) in PositAdditionReference.Pairs(size))
        {
            var left = PositEncodingReference.Decode(leftBits, size);
            var right = PositEncodingReference.Decode(rightBits, size);
            if (left.IsNaR || right.IsNaR)
            {
                if (left.Coefficient.IsZero || right.Coefficient.IsZero) cases.Add("NaR with zero");
                continue;
            }

            if (left.Coefficient.IsZero || right.Coefficient.IsZero) continue;

            var sum = PositAdditionReference.Sum(left, right);
            var sign = sum.Coefficient.Sign < 0 ? "negative" : "positive";
            var larger = Math.Max(Scale(left), Scale(right));
            var smallerExponent = Scale(left) < Scale(right) ? left.Exponent : right.Exponent;
            var difference = Math.Abs(Scale(left) - Scale(right));
            scaleDifferences.Add(difference);
            if (difference > 3 && smallerExponent < larger - (size - 2)) cases.Add("inexact alignment");
            if (sum.Coefficient.IsZero)
            {
                cases.Add("exact cancellation");
                continue;
            }

            if (Math.Abs(Scale(sum) - larger) > 4) cases.Add("near cancellation");

            // Posit values are multiples of minPos (section 3.2, p. 6), so a nonzero sum never needs the minPos clamp.
            var magnitude = sum with { Coefficient = BigInteger.Abs(sum.Coefficient) };
            Assert.True(magnitude.CompareTo(minPos) >= 0);
            if (magnitude.CompareTo(maxPos) > 0) cases.Add("maxPos clamp " + sign);

            var result = PositRoundingReference.Round(sum, size);
            if (PositEncodingReference.Decode(result, size).CompareTo(sum) == 0) continue;
            if (result == 1 || result == mask) cases.Add("rounded to minPos " + sign);

            // An inexact x is a tie when it equals v(U1) for its lower neighbor u, the result itself or its predecessor.
            var predecessor = unchecked(result - 1) & mask;
            if (IsMidpoint(result, sum, size)) cases.Add("tie to lower even u " + sign);
            if (predecessor != nar && IsMidpoint(predecessor, sum, size)) cases.Add("tie to upper even w " + sign);
        }

        foreach (var sign in new[] { "positive", "negative" })
        {
            Assert.Contains("maxPos clamp " + sign, cases);
            Assert.Contains("rounded to minPos " + sign, cases);
            Assert.Contains("tie to lower even u " + sign, cases);
            Assert.Contains("tie to upper even w " + sign, cases);
        }

        Assert.Contains("NaR with zero", cases);
        Assert.Contains("exact cancellation", cases);
        Assert.Contains("near cancellation", cases);
        Assert.Contains("inexact alignment", cases);

        // Near the extreme regimes truncated exponent bits leave some scales unrepresentable (section 3.3, p. 7).
        var scales = new List<int>();
        for (var scale = -maxScale; scale <= maxScale; scale++)
        {
            var power = PositEncodingReference.FromDyadic(BigInteger.One, scale);
            if (PositEncodingReference.Decode(PositAdditionReference.PowerOfTwo(scale, size), size) == power) scales.Add(scale);
        }

        foreach (var larger in scales)
        {
            foreach (var smaller in scales)
            {
                if (larger >= smaller) Assert.Contains(larger - smaller, scaleDifferences);
            }
        }
    }

    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    public void ReferenceMatchesHandDerivedSums(int size)
    {
        var shift = size - 8;
        foreach (var (left, right, sum, difference) in new (uint, uint, uint, uint)[]
        {
            (0x40, 0x40, 0x48, 0),     // 1 + 1 = 2, 1 - 1 = 0.
            (0x40, 0xC0, 0, 0x48),     // 1 + -1 = 0, 1 - -1 = 2.
            (0x48, 0x40, 0x4C, 0x40),  // 2 + 1 = 3, 2 - 1 = 1.
            (0x80, 0, 0x80, 0x80),     // NaR is checked before the zero shortcut.
            (0, 0x80, 0x80, 0x80),
        })
        {
            Assert.Equal(sum << shift, PositAdditionReference.Add(left << shift, right << shift, size));
            Assert.Equal(difference << shift, PositAdditionReference.Subtract(left << shift, right << shift, size));
        }

        var maxPos = (1U << (size - 1)) - 1;
        var mask = size == 32 ? uint.MaxValue : (1U << size) - 1;
        Assert.Equal(maxPos, PositAdditionReference.Add(maxPos, maxPos, size));
        Assert.Equal(maxPos + 2, PositAdditionReference.Subtract(maxPos + 2, maxPos, size)); // -maxPos - maxPos.
        Assert.Equal(0U, PositAdditionReference.Subtract(maxPos, maxPos, size));
        Assert.Equal(mask, PositAdditionReference.Add(mask, 0, size));

        // posit8 1 + 1/16 is the U1 midpoint 0x81 between 0x40 (even) and 0x41, so it ties down; 0x41 + 1/16 = 1 + 3/16
        // is the U1 midpoint 0x83 between 0x41 (odd) and 0x42, so it ties up. 2^-20 - 2^-24 = 3.75 * 2^-22 lies above
        // the U1 boundary 0x003 (2^-22) between minPos and 0x02 (2^-20).
        Assert.Equal(0x40U, PositAdditionReference.Add(0x40, 0x20, 8));
        Assert.Equal(0x42U, PositAdditionReference.Add(0x41, 0x20, 8));
        Assert.Equal(2U, PositAdditionReference.Subtract(2, 1, 8));
        Assert.Equal(1U, PositAdditionReference.Add(1, 1, 8));
    }

    private static uint[] VerifyExpression(int size, Func<uint, uint, uint, uint[]> evaluate, uint a, uint b, uint c)
    {
        var expected = new[]
        {
            PositAdditionReference.Subtract(PositAdditionReference.Add(a, b, size), c, size),
            PositAdditionReference.Add(a, PositAdditionReference.Subtract(b, c, size), size),
            PositAdditionReference.Add(PositAdditionReference.Subtract(a, c, size), b, size),
        };
        var actual = evaluate(a, b, c);
        Assert.Equal(ExpressionCount, actual.Length);
        for (var expression = 0; expression < ExpressionCount; expression++)
        {
            Assert.True(
                expected[expression] == actual[expression],
                $"Expression {expression}, posit{size} 0x{a:X8}, 0x{b:X8}, 0x{c:X8}: expected " +
                $"0x{expected[expression]:X8}, got 0x{actual[expression]:X8}.");
        }

        return actual;
    }

    private static uint RoundExactExpression(uint a, uint b, uint c, int size) =>
        PositRoundingReference.Round(
            PositAdditionReference.Sum(
                PositAdditionReference.Sum(PositEncodingReference.Decode(a, size), PositEncodingReference.Decode(b, size)),
                PositAdditionReference.Negate(PositEncodingReference.Decode(c, size))),
            size);

    private static IEnumerable<(uint A, uint B, uint C)> DistinguishingTriples(int size)
    {
        var maxPos = (1U << (size - 1)) - 1;
        var one = PositAdditionReference.PowerOfTwo(0, size);
        var large = PositAdditionReference.PowerOfTwo(4 * (size - 4), size);
        var small = PositAdditionReference.PowerOfTwo(-4 * (size - 4), size);
        var halfUnitInTheLastPlace = PositAdditionReference.PowerOfTwo(-(size - 4), size);
        var mask = size == 32 ? uint.MaxValue : (1U << size) - 1;

        yield return (large, small, large); // (a + b) - a = 0, while a + b - a = b.
        yield return (one, halfUnitInTheLastPlace, unchecked(0U - halfUnitInTheLastPlace) & mask); // 1, not 1 + ulp.
        yield return (maxPos, maxPos, maxPos); // Clamped to maxPos, then 0.
        yield return (1, 1, 1); // 2 minPos rounds to minPos, then 0.
    }

    private static IEnumerable<(uint A, uint B, uint C)> RandomTriples(int size)
    {
        var random = new Random(20_220_505 + size);
        for (var i = 0; i < 2048; i++)
        {
#pragma warning disable CA5394 // Deterministic numerical test vectors, not security-sensitive randomness.
            yield return ((uint)random.NextInt64(1L << size), (uint)random.NextInt64(1L << size), (uint)random.NextInt64(1L << size));
#pragma warning restore CA5394
        }
    }

    private static bool IsMidpoint(uint lower, PositEncodingReference value, int size) =>
        PositEncodingReference.Decode(((ulong)lower << 1) | 1, size + 1).CompareTo(value) == 0;

    private static int Scale(PositEncodingReference value) =>
        (int)BigInteger.Abs(value.Coefficient).GetBitLength() - 1 + value.Exponent;

    private static void Check(int size, string operation, uint left, uint right, uint expected, uint actual) =>
        Assert.True(
            expected == actual,
            $"posit{size} 0x{left:X8} {operation} 0x{right:X8}: expected 0x{expected:X8}, got 0x{actual:X8}.");
}
