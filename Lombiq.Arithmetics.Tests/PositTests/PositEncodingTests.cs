using System;
using System.Collections.Generic;
using Xunit;

namespace Lombiq.Arithmetics.Tests;

public class PositEncodingTests
{
    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    public void ReferenceMatchesHandDerivedEncodings(int size)
    {
        var sign = 1U << (size - 1);
        Assert.Equal(new PositEncodingReference(0, 0), PositEncodingReference.Decode(0, size));
        Assert.True(PositEncodingReference.Decode(sign, size).IsNaR);
        Assert.Equal(new PositEncodingReference(1, -4 * size + 8), PositEncodingReference.Decode(1, size));
        Assert.Equal(new PositEncodingReference(-1, -4 * size + 8), PositEncodingReference.Decode((sign * 2U) - 1, size));
        Assert.Equal(new PositEncodingReference(1, 4 * size - 8), PositEncodingReference.Decode(sign - 1, size));
        Assert.Equal(new PositEncodingReference(-1, 4 * size - 8), PositEncodingReference.Decode(sign + 1, size));
        Assert.Equal(new PositEncodingReference(1, 0), PositEncodingReference.Decode(sign >> 1, size));
        Assert.Equal(new PositEncodingReference(-1, 0), PositEncodingReference.Decode(sign | (sign >> 1), size));
        Assert.Equal(new PositEncodingReference(3, -1), PositEncodingReference.Decode(0x44U << (size - 8), size));
        Assert.Equal(new PositEncodingReference(-3, -1), PositEncodingReference.Decode(0xBCU << (size - 8), size));
        // One explicit exponent bit: 000...0011 and 011...1101.
        Assert.Equal(new PositEncodingReference(1, -4 * size + 14), PositEncodingReference.Decode(3, size));
        Assert.Equal(new PositEncodingReference(1, 4 * size - 14), PositEncodingReference.Decode(sign - 3, size));
        Assert.Equal(new PositEncodingReference(-1, -4 * size + 14), PositEncodingReference.Decode((sign * 2U) - 3, size));
        Assert.Equal(new PositEncodingReference(-1, 4 * size - 14), PositEncodingReference.Decode(sign + 3, size));
    }

    [Fact]
    public void Posit32EncodingAndComparisonsMatchStandard() =>
        VerifySurface(
            32,
            bits => new Posit32(bits, fromBitMask: true),
            posit => posit.PositBits,
            posit => posit.IsNaR(),
            posit => posit.IsNaN(),
            posit => posit.IsNegative(),
            (left, right) => ComparisonFlags(left == right, left != right, left > right, left >= right, left < right, left <= right));

    internal static int ComparisonFlags(bool equal, bool notEqual, bool greater, bool greaterEqual, bool less, bool lessEqual) =>
        (equal ? 1 : 0) | (notEqual ? 2 : 0) | (greater ? 4 : 0) | (greaterEqual ? 8 : 0) |
        (less ? 16 : 0) | (lessEqual ? 32 : 0);

    internal static void VerifySurface<T>(
        int size,
        Func<uint, T> create,
        Func<T, uint> raw,
        Func<T, bool> isNaR,
        Func<T, bool> isNaN,
        Func<T, bool> isNegative,
        Func<T, T, int> flags)
        where T : struct, IComparable, IComparable<T>, IEquatable<T>
    {
        var vectors = new List<uint>();
        var sign = 1U << (size - 1);
        if (size <= 16)
        {
            for (uint bits = 0; bits < (1U << size); bits++) vectors.Add(bits);
        }
        else
        {
            // Every regime transition, both signs, and either side of each boundary.
            for (var bit = 0; bit < size - 1; bit++)
            {
                var boundary = 1U << bit;
                vectors.Add(boundary - 1);
                vectors.Add(boundary);
                vectors.Add(boundary + 1);
                vectors.Add(unchecked(0U - boundary - 1));
                vectors.Add(unchecked(0U - boundary));
                vectors.Add(unchecked(0U - boundary + 1));
            }

            vectors.Add(sign);
            vectors.Add(sign + 1);
            vectors.Add(sign - 1);
            var random = new Random(20220203);
            for (var i = 0; i < 4096; i++) vectors.Add((uint)random.NextInt64(1L << 32));
        }

        foreach (var bits in vectors)
        {
            var reference = PositEncodingReference.Decode(bits, size);
            var posit = create(bits);
            Assert.Equal(bits, raw(posit));
            Assert.Equal(reference.IsNaR, isNaR(posit));
            Assert.Equal(reference.IsNaR, isNaN(posit));
            Assert.Equal(!reference.IsNaR && reference.Coefficient.Sign < 0, isNegative(posit));
            CheckPair(bits, bits);
            CheckPair(bits, 0);
            CheckPair(bits, sign);
            if (bits > 0) CheckPair(bits, bits - 1);
        }

        if (size == 8)
        {
            foreach (var left in vectors)
            {
                foreach (var right in vectors) CheckPair(left, right);
            }
        }
        else
        {
            var random = new Random(20220503);
            for (var i = 0; i < 8192; i++) CheckPair(vectors[random.Next(vectors.Count)], vectors[random.Next(vectors.Count)]);
        }

        void CheckPair(uint leftBits, uint rightBits)
        {
            var expected = PositEncodingReference.Decode(leftBits, size).CompareTo(PositEncodingReference.Decode(rightBits, size));
            var signedLeft = leftBits >= sign ? (long)leftBits - (1L << size) : leftBits;
            var signedRight = rightBits >= sign ? (long)rightBits - (1L << size) : rightBits;
            Assert.Equal(signedLeft.CompareTo(signedRight), expected);
            var left = create(leftBits);
            var right = create(rightBits);
            Assert.Equal(ComparisonFlags(expected == 0, expected != 0, expected > 0, expected >= 0, expected < 0, expected <= 0), flags(left, right));
            Assert.Equal(expected, left.CompareTo(right));
            Assert.Equal(expected, left.CompareTo((object)right));
            Assert.Equal(expected == 0, left.Equals(right));
            Assert.Equal(expected == 0, left.Equals((object)right));
            if (expected == 0) Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }
    }
}
