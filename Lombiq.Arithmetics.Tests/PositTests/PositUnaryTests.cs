using System;
using Xunit;

namespace Lombiq.Arithmetics.Tests;

public class PositUnaryTests
{
    internal static void Verify(int size, Func<uint, uint[]> evaluate)
    {
        foreach (var bits in PositUnaryReference.Inputs(size))
        {
            var expected = PositUnaryReference.Evaluate(bits, size);
            var actual = evaluate(bits);
            Assert.Equal(8, actual.Length);
            for (var operation = 0; operation < expected.Length; operation++)
            {
                Assert.True(
                    expected[operation] == actual[operation],
                    $"Unary operation {operation}, posit{size} 0x{bits:X8}: expected 0x{expected[operation]:X8}, got 0x{actual[operation]:X8}.");
            }
        }
    }

    [Fact]
    public void Posit32MatchesIndependentUnaryReference() =>
        Verify(32, bits =>
        {
            var input = new Posit32(bits, fromBitMask: true);
            Assert.Equal(Posit32.Negate(input), -input);
            return
            [
                Posit32.Negate(input).PositBits, Posit32.Abs(input).PositBits, Posit32.Sign(input).PositBits,
                Posit32.NearestInt(input).PositBits, Posit32.Ceiling(input).PositBits, Posit32.Floor(input).PositBits,
                Posit32.Next(input).PositBits, Posit32.Prior(input).PositBits,
            ];
        });

    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    public void ReferenceMatchesHandDerivedUnaryBoundaries(int size)
    {
        var shift = size - 8;
        foreach (var (source, nearest, ceiling, floor) in new (uint, uint, uint, uint)[]
        {
            (0x38, 0, 0x40, 0),          // 0.5 -> 0, 1, 0.
            (0x44, 0x48, 0x48, 0x40),  // 1.5 -> 2, 2, 1.
            (0x4A, 0x48, 0x4C, 0x48),  // 2.5 -> 2, 3, 2.
            (0x4E, 0x50, 0x50, 0x4C),  // 3.5 -> 4, 4, 3.
            (0xC8, 0, 0, 0xC0),       // -0.5 -> 0, 0, -1.
            (0xBC, 0xB8, 0xC0, 0xB8), // -1.5 -> -2, -1, -2.
            (0xB6, 0xB8, 0xB8, 0xB4), // -2.5 -> -2, -2, -3.
        })
        {
            var actual = PositUnaryReference.Evaluate(source << shift, size);
            Assert.Equal(nearest << shift, actual[3]);
            Assert.Equal(ceiling << shift, actual[4]);
            Assert.Equal(floor << shift, actual[5]);
        }

        var nar = 1U << (size - 1);
        var mask = size == 32 ? uint.MaxValue : (1U << size) - 1;
        Assert.Equal(nar, PositUnaryReference.Evaluate(nar - 1, size)[6]);
        Assert.Equal(nar + 1, PositUnaryReference.Evaluate(nar, size)[6]);
        Assert.Equal(nar - 1, PositUnaryReference.Evaluate(nar, size)[7]);
        Assert.Equal(0U, PositUnaryReference.Evaluate(mask, size)[6]);
        Assert.Equal(mask, PositUnaryReference.Evaluate(0, size)[7]);
    }
}
