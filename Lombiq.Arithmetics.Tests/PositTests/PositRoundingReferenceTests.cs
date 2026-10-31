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
