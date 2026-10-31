using System;
using System.Numerics;
using Xunit;

namespace Lombiq.Arithmetics.Tests;

public class PositRoundingReferenceTests
{
    [Fact]
    public void FromDyadicRepresentsAnyExactSignedDyadic()
    {
        Assert.Equal(new PositEncodingReference(0, 0), PositEncodingReference.FromDyadic(0, 17));
        Assert.Equal(new PositEncodingReference(3, 2), PositEncodingReference.FromDyadic(12, 0));
        Assert.Equal(new PositEncodingReference(-5, -7), PositEncodingReference.FromDyadic(-40, -10));
        Assert.Equal(PositEncodingReference.Decode(0x44, 8), PositEncodingReference.FromDyadic(6, -2));
        Assert.False(PositEncodingReference.FromDyadic(0, 0).IsNaR);
        Assert.True(PositEncodingReference.NaR.IsNaR);

        // 1 + 2^-20 lies strictly between the posit8 values 1 and 1.125, and 3^100 × 2^5000 exceeds every posit.
        var between = PositEncodingReference.FromDyadic((BigInteger.One << 20) + 1, -20);
        Assert.Equal(1, between.CompareTo(PositEncodingReference.Decode(0x40, 8)));
        Assert.Equal(-1, between.CompareTo(PositEncodingReference.Decode(0x41, 8)));
        var huge = PositEncodingReference.FromDyadic(BigInteger.Pow(3, 100), 5000);
        Assert.Equal(new PositEncodingReference(BigInteger.Pow(3, 100), 5000), huge);
    }

    [Fact]
    public void DecodeAcceptsEveryWidthFromTwoTo64Bits()
    {
        Assert.Equal(new PositEncodingReference(1, 0), PositEncodingReference.Decode(1, 2));
        Assert.Equal(new PositEncodingReference(17, -4), PositEncodingReference.Decode(0x81, 9));
        Assert.Equal(new PositEncodingReference(-17, -4), PositEncodingReference.Decode(0x17F, 9));
        Assert.Equal(new PositEncodingReference(1, 22), PositEncodingReference.Decode(0xFD, 9));
        Assert.Equal(
            PositEncodingReference.FromDyadic((BigInteger.One << 28) + 1, -28),
            PositEncodingReference.Decode((0x4000_0000UL << 1) | 1, 33));
        Assert.Equal(new PositEncodingReference(1, (4 * 33) - 8), PositEncodingReference.Decode(uint.MaxValue, 33));
        Assert.Equal(new PositEncodingReference(1, (4 * 64) - 8), PositEncodingReference.Decode(long.MaxValue, 64));
        Assert.Equal(new PositEncodingReference(-1, (-4 * 64) + 8), PositEncodingReference.Decode(ulong.MaxValue, 64));
        Assert.True(PositEncodingReference.Decode(1UL << 63, 64).IsNaR);
        Assert.Throws<ArgumentOutOfRangeException>(() => PositEncodingReference.Decode(0x100, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => PositEncodingReference.Decode(1UL << 33, 33));
        Assert.Throws<ArgumentOutOfRangeException>(() => PositEncodingReference.Decode(0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => PositEncodingReference.Decode(0, 65));
    }

    [Fact]
    public void RoundMatchesHandDerivedPosit8Boundaries()
    {
        Assert.Equal(0U, Round8(0, 9));
        Assert.Equal(0x80U, PositRoundingReference.Round(PositEncodingReference.NaR, 8));

        // (1, 1.125): v = 1.0625 = 17 × 2^-4 and U = 0x40 is even.
        Assert.Equal(0x40U, Round8(17, -4));
        Assert.Equal(0x40U, Round8((17 << 6) - 1, -10));
        Assert.Equal(0x41U, Round8((17 << 6) + 1, -10));

        // (1.125, 1.25): v = 1.1875 and U = 0x41 is odd.
        Assert.Equal(0x42U, Round8(19, -4));
        Assert.Equal(0x41U, Round8((19 << 6) - 1, -10));

        // Negative mirrors: (-1.125, -1): U = 0xBF is odd; (-1.25, -1.125): U = 0xBE is even.
        Assert.Equal(0xC0U, Round8(-17, -4));
        Assert.Equal(0xBFU, Round8(-((17 << 6) + 1), -10));
        Assert.Equal(0xBEU, Round8(-19, -4));

        // (2^20, 2^24): v = 2^22, not the arithmetic midpoint 8.5 × 2^20, so 2^23 rounds up.
        Assert.Equal(0x7EU, Round8(1, 22));
        Assert.Equal(0x7EU, Round8((1 << 22) - 1, 0));
        Assert.Equal(0x7FU, Round8((1 << 22) + 1, 0));
        Assert.Equal(0x7FU, Round8(1, 23));
        Assert.Equal(0x81U, Round8(-1, 23));

        // (minPos, 2^-20): v = 2^-22 and U = 0x01 is odd.
        Assert.Equal(0x02U, Round8(1, -22));
        Assert.Equal(0x01U, Round8((1 << 10) - 1, -32));
        Assert.Equal(0x01U, Round8(1, -23));
        Assert.Equal(0xFEU, Round8(-1, -22));

        // Clamping: |x| > maxPos = 2^24 and 0 < |x| < minPos = 2^-24.
        Assert.Equal(0x7FU, Round8(1, 24));
        Assert.Equal(0x7FU, Round8((1 << 24) + 1, 0));
        Assert.Equal(0x7FU, Round8(1, 1000));
        Assert.Equal(0x81U, Round8(-((1 << 24) + 1), 0));
        Assert.Equal(0x01U, Round8(1, -24));
        Assert.Equal(0x01U, Round8(3, -26));
        Assert.Equal(0x01U, Round8(1, -1000));
        Assert.Equal(0xFFU, Round8(-1, -25));
    }

    [Fact]
    public void RoundMatchesHandDerivedPosit32Boundaries()
    {
        // (1, 1 + 2^-27): v = 1 + 2^-28 and U = 0x40000000 is even.
        Assert.Equal(0x4000_0000U, Round32((1L << 28) + 1, -28));
        Assert.Equal(0x4000_0001U, Round32((((1L << 28) + 1) << 32) + 1, -60));
        Assert.Equal(0x4000_0002U, Round32((1L << 28) + 3, -28));
        Assert.Equal(0xC000_0000U, Round32(-((1L << 28) + 1), -28));

        // (2^116, 2^120): v = 2^118; (minPos, 2^-116): v = 2^-118 and U = 1 is odd.
        Assert.Equal(0x7FFF_FFFEU, Round32(1, 118));
        Assert.Equal(0x7FFF_FFFFU, Round32(1, 119));
        Assert.Equal(0x0000_0002U, Round32(1, -118));
        Assert.Equal(0x0000_0001U, Round32(1, -119));

        Assert.Equal(0x7FFF_FFFFU, Round32(1, 121));
        Assert.Equal(0x8000_0001U, Round32(-3, 120));
        Assert.Equal(0x0000_0001U, Round32(1, -121));
        Assert.Equal(0xFFFF_FFFFU, Round32(-1, -500));
    }

    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    public void RoundHonorsEveryBoundary(int size)
    {
        var sign = 1U << (size - 1);
        var mask = sign | (sign - 1);
        for (uint bits = 0; bits <= mask; bits++)
        {
            Assert.Equal(bits, PositRoundingReference.Round(PositEncodingReference.Decode(bits, size), size));
        }

        var ties = new int[2];
        long maxPos = sign - 1;
        for (var signedU = -maxPos; signedU < maxPos; signedU++)
        {
            // (-minPos, 0) and (0, minPos) clamp before the U1 rule; see VerifyClamping.
            if (signedU is -1 or 0) continue;
            var uBits = (uint)((ulong)signedU & mask);
            var wBits = (uint)((ulong)(signedU + 1) & mask);
            VerifyInterval(uBits, wBits, size);
            ties[uBits & 1]++;
        }

        Assert.Equal((int)maxPos - 1, ties[0]);
        Assert.Equal((int)maxPos - 1, ties[1]);
        VerifyClamping(size);
    }

    [Fact]
    public void RoundHonorsSeededPosit32Boundaries()
    {
        VerifyClamping(32);
        var random = new Random(20220504);
        for (var i = 0; i < 512; i++)
        {
            var signedU = random.NextInt64(1, int.MaxValue) * (random.Next(2) == 0 ? 1 : -1);

            // (-minPos, 0) clamps before the U1 rule.
            if (signedU == -1) continue;
            VerifyInterval((uint)signedU, (uint)(signedU + 1), 32);
        }

        foreach (var uBits in new[] { 0x0000_0001U, 0x3FFF_FFFFU, 0x4000_0000U, 0x7FFF_FFFDU, 0x7FFF_FFFEU, 0x8000_0001U, 0xBFFF_FFFFU, 0xFFFF_FFFEU })
        {
            VerifyInterval(uBits, uBits + 1, 32);
        }
    }

    [Fact]
    public void RationalRoundNormalizesDenominatorAndRejectsZero()
    {
        // 3/4 = 2^-1 × 1.5 is exactly posit8 0 01 11 100 (k = -1, e = 3, f = .1).
        foreach (var (numerator, denominator) in new[] { (3, 4), (-3, -4), (6, 8), (-9, -12) })
        {
            Assert.Equal(0x3CU, PositRoundingReference.Round(numerator, denominator, 8));
            Assert.Equal(0xC4U, PositRoundingReference.Round(-numerator, denominator, 8));
        }

        Assert.Equal(0U, PositRoundingReference.Round(0, -5, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => PositRoundingReference.Round(1, 0, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => PositRoundingReference.Round(0, 0, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => PositRoundingReference.Round(1, 1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => PositRoundingReference.Round(1, 1, 33));
        Assert.Throws<ArgumentOutOfRangeException>(() => PositRoundingReference.Round(PositEncodingReference.NaR, 33));
    }

    [Fact]
    public void RationalRoundBreaksExactU1TiesByLsbForBothSigns()
    {
        // The hand-derived boundaries of RoundMatchesHandDerivedPosit8Boundaries: U = 0x40 and 0xBE are even, U = 0x41
        // and 0xBF are odd; (2^20, 2^24) has v = 2^22 and even U = 0x7E, (-2^24, -2^20) odd U = 0x81; (minPos, 2^-20)
        // has v = 2^-22 and odd U = 0x01, (-2^-20, -minPos) even U = 0xFE.
        AssertTie(17, 16, 0x40U, 8);
        AssertTie(19, 16, 0x42U, 8);
        AssertTie(-17, 16, 0xC0U, 8);
        AssertTie(-19, 16, 0xBEU, 8);
        AssertTie(1 << 22, 1, 0x7EU, 8);
        AssertTie(-(1 << 22), 1, 0x82U, 8);
        AssertTie(1, 1 << 22, 0x02U, 8);
        AssertTie(-1, 1 << 22, 0xFEU, 8);

        // (1, 1 + 2^-27) has v = 1 + 2^-28 and even U = 0x40000000; (1 + 2^-27, 1 + 2^-26) odd U = 0x40000001; their
        // negations have odd U = 0xBFFFFFFF and even U = 0xBFFFFFFE.
        AssertTie((1L << 28) + 1, 1L << 28, 0x4000_0000U, 32);
        AssertTie((1L << 28) + 3, 1L << 28, 0x4000_0002U, 32);
        AssertTie(-((1L << 28) + 1), 1L << 28, 0xC000_0000U, 32);
        AssertTie(-((1L << 28) + 3), 1L << 28, 0xBFFF_FFFEU, 32);
    }

    [Fact]
    public void RationalRoundSeparatesNonDyadicNeighborsOfU1Thresholds()
    {
        // The same thresholds as RationalRoundBreaksExactU1TiesByLsbForBothSigns; u is below v and w above.
        AssertAround(17, 16, 0x40U, 0x41U, 8);
        AssertAround(19, 16, 0x41U, 0x42U, 8);
        AssertAround(-17, 16, 0xBFU, 0xC0U, 8);
        AssertAround(-19, 16, 0xBEU, 0xBFU, 8);
        AssertAround(1 << 22, 1, 0x7EU, 0x7FU, 8);
        AssertAround(-(1 << 22), 1, 0x81U, 0x82U, 8);
        AssertAround(1, 1 << 22, 0x01U, 0x02U, 8);
        AssertAround(-1, 1 << 22, 0xFEU, 0xFFU, 8);
        AssertAround((1L << 28) + 1, 1L << 28, 0x4000_0000U, 0x4000_0001U, 32);
        AssertAround((1L << 28) + 3, 1L << 28, 0x4000_0001U, 0x4000_0002U, 32);
        AssertAround(-((1L << 28) + 1), 1L << 28, 0xBFFF_FFFFU, 0xC000_0000U, 32);
        AssertAround(BigInteger.One << 118, 1, 0x7FFF_FFFEU, 0x7FFF_FFFFU, 32);
        AssertAround(1, BigInteger.One << 118, 0x0000_0001U, 0x0000_0002U, 32);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    public void RationalRoundHonorsEveryNonDyadicBoundary(int size)
    {
        var sign = 1U << (size - 1);
        var mask = sign | (sign - 1);
        long maxPos = sign - 1;
        for (var signedU = -maxPos; signedU < maxPos; signedU++)
        {
            if (signedU is -1 or 0) continue;
            var uBits = (uint)((ulong)signedU & mask);
            var wBits = (uint)((ulong)(signedU + 1) & mask);
            var u = PositEncodingReference.Decode(uBits, size);
            var w = PositEncodingReference.Decode(wBits, size);
            var v = PositEncodingReference.Decode(((ulong)uBits << 1) | 1, size + 1);
            var exponent = Math.Min(Math.Min(u.Exponent, v.Exponent), w.Exponent) - 1;
            Assert.Equal(uBits, RoundNonDyadic(v, exponent, -1, size));
            Assert.Equal(wBits, RoundNonDyadic(v, exponent, 1, size));
            Assert.Equal(uBits, RoundNonDyadic(u, exponent, 1, size));
            Assert.Equal(wBits, RoundNonDyadic(w, exponent, -1, size));
        }
    }

    // Every scaled form of the exact tie v = numerator / denominator, including negative denominators, rounds alike.
    private static void AssertTie(BigInteger numerator, BigInteger denominator, uint expected, int size)
    {
        Assert.Equal(expected, PositRoundingReference.Round(numerator, denominator, size));
        Assert.Equal(expected, PositRoundingReference.Round(-numerator, -denominator, size));
        Assert.Equal(expected, PositRoundingReference.Round(3 * numerator, 3 * denominator, size));
        Assert.Equal(expected, PositRoundingReference.Round(-5 * numerator, -5 * denominator, size));
    }

    // v ∓ 1 / (3^20 × denominator) is not dyadic, since 3 divides neither 3^20 × numerator ∓ 1 nor the power-of-two
    // denominator, so it cannot tie and must round to u below v or to w above it.
    private static void AssertAround(BigInteger numerator, BigInteger denominator, uint u, uint w, int size)
    {
        var scale = BigInteger.Pow(3, 20);
        Assert.Equal(u, PositRoundingReference.Round((numerator * scale) - 1, denominator * scale, size));
        Assert.Equal(u, PositRoundingReference.Round(1 - (numerator * scale), -denominator * scale, size));
        Assert.Equal(w, PositRoundingReference.Round((numerator * scale) + 1, denominator * scale, size));
        Assert.Equal(w, PositRoundingReference.Round(-1 - (numerator * scale), -denominator * scale, size));
    }

    // Rounds the non-dyadic value + direction × 2^exponent / 3, for exponent <= value.Exponent.
    private static uint RoundNonDyadic(PositEncodingReference value, int exponent, int direction, int size)
    {
        var numerator = ((3 * value.Coefficient) << (value.Exponent - exponent)) + direction;
        return exponent >= 0
            ? PositRoundingReference.Round(numerator << exponent, 3, size)
            : PositRoundingReference.Round(numerator, 3 * (BigInteger.One << -exponent), size);
    }

    private static void VerifyInterval(uint uBits, uint wBits, int size)
    {
        var u = PositEncodingReference.Decode(uBits, size);
        var w = PositEncodingReference.Decode(wBits, size);
        var v = PositEncodingReference.Decode(((ulong)uBits << 1) | 1, size + 1);
        Assert.True(u.CompareTo(v) < 0 && v.CompareTo(w) < 0);

        var epsilon = PositEncodingReference.FromDyadic(1, Math.Min(Math.Min(u.Exponent, v.Exponent), w.Exponent) - 1);
        Assert.Equal((uBits & 1) == 0 ? uBits : wBits, PositRoundingReference.Round(v, size));
        Assert.Equal(uBits, PositRoundingReference.Round(Add(v, epsilon, negate: true), size));
        Assert.Equal(wBits, PositRoundingReference.Round(Add(v, epsilon, negate: false), size));
        Assert.Equal(uBits, PositRoundingReference.Round(Add(u, epsilon, negate: false), size));
        Assert.Equal(wBits, PositRoundingReference.Round(Add(w, epsilon, negate: true), size));
    }

    private static void VerifyClamping(int size)
    {
        var sign = 1U << (size - 1);
        var mask = sign | (sign - 1);
        var maxPos = PositEncodingReference.Decode(sign - 1, size);
        var minPos = PositEncodingReference.Decode(1, size);
        var finer = PositEncodingReference.FromDyadic(1, minPos.Exponent - 1);
        foreach (var overflow in new[]
        {
            Add(maxPos, finer, negate: false),
            PositEncodingReference.FromDyadic(1, maxPos.Exponent + 1),
            PositEncodingReference.FromDyadic(BigInteger.Pow(3, 50), maxPos.Exponent + 200),
        })
        {
            Assert.Equal(sign - 1, PositRoundingReference.Round(overflow, size));
            Assert.Equal(sign + 1, PositRoundingReference.Round(Negate(overflow), size));
        }

        foreach (var underflow in new[]
        {
            Add(minPos, PositEncodingReference.FromDyadic(1, minPos.Exponent - 40), negate: true),
            finer,
            PositEncodingReference.FromDyadic(1, minPos.Exponent - 2),
            PositEncodingReference.FromDyadic(3, minPos.Exponent - 3),
            PositEncodingReference.FromDyadic(1, minPos.Exponent - 1000),
        })
        {
            Assert.Equal(1U, PositRoundingReference.Round(underflow, size));
            Assert.Equal(mask, PositRoundingReference.Round(Negate(underflow), size));
        }
    }

    private static uint Round8(long coefficient, int exponent) =>
        PositRoundingReference.Round(PositEncodingReference.FromDyadic(coefficient, exponent), 8);

    private static uint Round32(BigInteger coefficient, int exponent) =>
        PositRoundingReference.Round(PositEncodingReference.FromDyadic(coefficient, exponent), 32);

    private static PositEncodingReference Negate(PositEncodingReference value) =>
        PositEncodingReference.FromDyadic(-value.Coefficient, value.Exponent);

    private static PositEncodingReference Add(PositEncodingReference left, PositEncodingReference right, bool negate)
    {
        var exponent = Math.Min(left.Exponent, right.Exponent);
        var rightCoefficient = right.Coefficient << (right.Exponent - exponent);
        return PositEncodingReference.FromDyadic(
            (left.Coefficient << (left.Exponent - exponent)) + (negate ? -rightCoefficient : rightCoefficient),
            exponent);
    }
}
