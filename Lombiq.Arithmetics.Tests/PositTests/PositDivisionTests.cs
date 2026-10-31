using System;
using Xunit;

namespace Lombiq.Arithmetics.Tests;

public class PositDivisionTests
{
    [Fact]
    public void Posit8E2MatchesExactReferenceForAllPairs() =>
        Verify(8, (left, right) => (new Posit8E2((byte)left, true) / new Posit8E2((byte)right, true)).PositBits);

    [Fact]
    public void Posit16E2MatchesExactReference() =>
        Verify(16, (left, right) => (new Posit16E2((ushort)left, true) / new Posit16E2((ushort)right, true)).PositBits);

    [Fact]
    public void Posit32E2AndPosit32MatchExactReference() =>
        Verify(32, (left, right) =>
        {
            var quotient = (new Posit32E2(left, true) / new Posit32E2(right, true)).PositBits;
            Assert.Equal(quotient, (new Posit32(left, fromBitMask: true) / new Posit32(right, fromBitMask: true)).PositBits);
            return quotient;
        });

    [Theory]
    [InlineData(8, 0x33U)]
    [InlineData(16, 0x32ABU)]
    [InlineData(32, 0x32AAAAABU)]
    public void ReferenceAndProductionMatchHandDerivedQuotients(int size, uint oneThird)
    {
        var shift = size - 8;
        var mask = size == 32 ? uint.MaxValue : (1U << size) - 1;
        var nar = 1U << (size - 1);
        var maxPos = nar - 1;
        var one = 0x40U << shift;
        var three = 0x4CU << shift;
        var oneAndHalf = 0x44U << shift;
        var largestExponentOnlyPower = ((1U << (size - 4)) - 1) << 3;
        uint Negate(uint bits) => (0U - bits) & mask;

        foreach (var (left, right, expected) in new (uint, uint, uint)[]
        {
            // 1/3 = 1.0101...b * 2^-2 never terminates: regime 01, exponent 10, Size - 5 fraction bits rounded up.
            (one, three, oneThird),
            (Negate(one), three, Negate(oneThird)),
            (one, Negate(three), Negate(oneThird)),
            (Negate(one), Negate(three), oneThird),

            // (3 + 2^-(Size-6)) / 3 = 1 + 2^-(Size-4) + 2^-(Size-4) / 3: the truncated quotient is exactly the U1 tie between
            // 1 and its successor, and only the nonzero remainder rounds it up.
            (three + 1, three, one + 1),
            (Negate(three + 1), three, Negate(one + 1)),

            // 1.5 / 2^-(4 Size - 20) and 1.5 / 2^-(4 Size - 19) are exact U1 ties where the n-bit regime leaves only the
            // exponent: 2^(4 Size - 20) has an even encoding and is kept, 2^(4 Size - 19) is odd and rounds up.
            (oneAndHalf, 0x08, largestExponentOnlyPower),
            (oneAndHalf, 0x07, largestExponentOnlyPower | 2),
            (Negate(oneAndHalf), 0x07, Negate(largestExponentOnlyPower | 2)),

            // Exact quotient 1.5 / 3 = 0.5: regime 01, exponent 11.
            (oneAndHalf, three, 0x38U << shift),

            // Signed range extremes never round to 0 or NaR.
            (maxPos, 1, maxPos),
            (1, maxPos, 1),
            (Negate(maxPos), 1, Negate(maxPos)),
            (1, Negate(maxPos), mask),
            (1, three, 1),
            (maxPos, maxPos, one),
            (Negate(maxPos), Negate(maxPos), one),

            // Section 5.1 exceptional cases.
            (0, 0, nar),
            (one, 0, nar),
            (nar, one, nar),
            (one, nar, nar),
            (0, nar, nar),
            (nar, 0, nar),
            (0, one, 0),
            (0, Negate(maxPos), 0),
        })
        {
            Assert.Equal(expected, PositDivisionReference.Divide(left, right, size));
            Assert.Equal(expected, ProductionDivide(left, right, size));
        }
    }

    private static void Verify(int size, Func<uint, uint, uint> divide)
    {
        foreach (var (left, right) in PositDivisionReference.Pairs(size))
        {
            var expected = PositDivisionReference.Divide(left, right, size);
            var actual = divide(left, right);
            Assert.True(
                expected == actual,
                $"posit{size} 0x{left:X8} / 0x{right:X8}: expected 0x{expected:X8}, got 0x{actual:X8}.");
        }
    }

    private static uint ProductionDivide(uint left, uint right, int size) =>
        size switch
        {
            8 => (new Posit8E2((byte)left, true) / new Posit8E2((byte)right, true)).PositBits,
            16 => (new Posit16E2((ushort)left, true) / new Posit16E2((ushort)right, true)).PositBits,
            _ => (new Posit32(left, fromBitMask: true) / new Posit32(right, fromBitMask: true)).PositBits,
        };
}
