using System;
using System.Collections.Generic;
using System.Numerics;
using Xunit;

namespace Lombiq.Arithmetics.Tests;

public class PositPrecisionConversionTests
{
    private static readonly Conversion[] Conversions =
    [
        new(8, 16, bits => ((Posit16E2)new Posit8E2((byte)bits, fromBitMask: true)).PositBits),
        new(8, 32, bits => ((Posit32E2)new Posit8E2((byte)bits, fromBitMask: true)).PositBits),
        new(16, 8, bits => ((Posit8E2)new Posit16E2((ushort)bits, fromBitMask: true)).PositBits),
        new(16, 32, bits => ((Posit32E2)new Posit16E2((ushort)bits, fromBitMask: true)).PositBits),
        new(32, 8, bits => ((Posit8E2)new Posit32E2(bits, fromBitMask: true)).PositBits),
        new(32, 16, bits => ((Posit16E2)new Posit32E2(bits, fromBitMask: true)).PositBits),
    ];

    private static readonly Conversion[] Posit32CompatibilityConversions =
    [
        new(8, 32, bits => ((Posit32)new Posit8E2((byte)bits, fromBitMask: true)).PositBits),
        new(16, 32, bits => ((Posit32)new Posit16E2((ushort)bits, fromBitMask: true)).PositBits),
        new(32, 8, bits => ((Posit8E2)new Posit32(bits, fromBitMask: true)).PositBits),
        new(32, 16, bits => ((Posit16E2)new Posit32(bits, fromBitMask: true)).PositBits),
        new(32, 32, bits => ((Posit32E2)new Posit32(bits, fromBitMask: true)).PositBits),
        new(32, 32, bits => ((Posit32)new Posit32E2(bits, fromBitMask: true)).PositBits),
    ];

    internal static void VerifyConversionsFrom(int sourceSize)
    {
        foreach (var conversion in Conversions)
        {
            if (conversion.SourceSize != sourceSize) continue;

            foreach (var bits in SourceVectors(conversion.SourceSize, conversion.DestinationSize))
            {
                var expected = PositPrecisionConversionReference.Convert(bits, conversion.SourceSize, conversion.DestinationSize);
                var actual = conversion.Convert(bits);
                Assert.True(
                    expected == actual,
                    $"Converting {conversion.SourceSize}-bit 0x{bits:X8} to {conversion.DestinationSize}-bit posit expected 0x{expected:X8}, got 0x{actual:X8}.");
            }
        }
    }

    [Fact]
    public void Posit32CompatibilityConversionsMatchTheExactRationalReference()
    {
        foreach (var conversion in Posit32CompatibilityConversions)
        {
            foreach (var bits in SourceVectors(conversion.SourceSize, conversion.DestinationSize))
            {
                var expected = PositPrecisionConversionReference.Convert(bits, conversion.SourceSize, conversion.DestinationSize);
                var actual = conversion.Convert(bits);
                Assert.True(
                    expected == actual,
                    $"Converting through Posit32 from {conversion.SourceSize}-bit 0x{bits:X8} to {conversion.DestinationSize}-bit posit expected 0x{expected:X8}, got 0x{actual:X8}.");
            }
        }
    }

    [Fact]
    public void NarrowingUsesTheExtendedRepresentationTieAtRegimeBoundaries()
    {
        foreach (var lowerBits in new uint[] { 0x1F, 0x20, 0x3F, 0x40, 0x5F, 0x60 })
        {
            var boundaryBits = ((lowerBits << 1) | 1) << 23;
            var tieResult = (lowerBits & 1) == 0 ? lowerBits : lowerBits + 1;
            AssertConversion(boundaryBits - 1, lowerBits);
            AssertConversion(boundaryBits, tieResult);
            AssertConversion(boundaryBits + 1, lowerBits + 1);

            var negativeBoundaryBits = unchecked(0U - boundaryBits);
            AssertConversion(negativeBoundaryBits - 1, unchecked(0U - (lowerBits + 1)) & 0xFF);
            AssertConversion(negativeBoundaryBits, unchecked(0U - tieResult) & 0xFF);
            AssertConversion(negativeBoundaryBits + 1, unchecked(0U - lowerBits) & 0xFF);
        }

        static void AssertConversion(uint sourceBits, uint expectedBits)
        {
            var result = (Posit8E2)new Posit32E2(sourceBits, fromBitMask: true);
            Assert.Equal(expectedBits, result.PositBits);
        }
    }

    private static IEnumerable<uint> SourceVectors(int sourceSize, int destinationSize)
    {
        var sourceMask = sourceSize == 32 ? uint.MaxValue : (1U << sourceSize) - 1;
        if (sourceSize <= 16)
        {
            for (uint bits = 0; bits <= sourceMask; bits++) yield return bits;
            yield break;
        }

        var sourceSign = 1U << (sourceSize - 1);
        yield return 0;
        yield return 1;
        yield return 2;
        yield return 3;
        yield return sourceSign - 3;
        yield return sourceSign - 2;
        yield return sourceSign - 1;
        yield return sourceSign;
        yield return sourceSign + 1;
        yield return sourceSign + 2;
        yield return sourceSign + 3;
        yield return sourceMask - 2;
        yield return sourceMask - 1;
        yield return sourceMask;

        if (sourceSize == destinationSize)
        {
            var sameSizeRandom = new Random(20220503);
            for (var i = 0; i < 2048; i++) yield return (uint)sameSizeRandom.NextInt64(1L << sourceSize);
            yield break;
        }

        var destinationSign = 1U << (destinationSize - 1);
        var destinationMaximum = destinationSign - 1;
        var destinationMask = (destinationSign * 2) - 1;
        for (var bit = 0; bit < destinationSize - 1; bit++)
        {
            var power = 1U << bit;
            for (var offset = -1; offset <= 1; offset++)
            {
                var lowerBits = (uint)((int)power + offset);
                if (lowerBits == 0 || lowerBits >= destinationMaximum) continue;

                var boundary = ((lowerBits << 1) | 1) << (sourceSize - destinationSize - 1);
                for (var neighbor = -1; neighbor <= 1; neighbor++)
                {
                    var positive = unchecked(boundary + (uint)neighbor) & sourceMask;
                    yield return positive;
                    yield return unchecked(0U - positive) & sourceMask;
                }
            }
        }

        for (uint lowerBits = 0; lowerBits <= destinationMask; lowerBits++)
        {
            if (lowerBits == destinationSign || lowerBits == destinationMaximum) continue;

            var extendedEncoding = (lowerBits << 1) | 1;
            var boundary = extendedEncoding << (sourceSize - destinationSize - 1);
            for (var neighbor = -1; neighbor <= 1; neighbor++)
            {
                var sourceBits = unchecked(boundary + (uint)neighbor);
                yield return sourceBits;
            }
        }

        var random = new Random(20220503 + destinationSize);
        for (var i = 0; i < 2048; i++) yield return (uint)random.NextInt64(1L << sourceSize);
    }

    private sealed record Conversion(int SourceSize, int DestinationSize, Func<uint, uint> Convert);
}

internal static class PositPrecisionConversionReference
{
    private static readonly Dictionary<int, (uint Bits, PositEncodingReference Value)[]> TargetValues = new()
    {
        [8] = DecodeTargetValues(8),
        [16] = DecodeTargetValues(16),
    };

    public static uint Convert(uint sourceBits, int sourceSize, int destinationSize)
    {
        var source = PositEncodingReference.Decode(sourceBits, sourceSize);
        if (source.IsNaR) return 1U << (destinationSize - 1);
        if (sourceBits == 0) return 0;
        if (sourceSize < destinationSize) return sourceBits << (destinationSize - sourceSize);
        if (sourceSize == destinationSize) return sourceBits;

        var magnitude = new PositEncodingReference(BigInteger.Abs(source.Coefficient), source.Exponent);
        var destinationSign = 1U << (destinationSize - 1);
        var destinationMask = (destinationSign * 2) - 1;
        var maximumBits = destinationSign - 1;
        var maximum = PositEncodingReference.Decode(maximumBits, destinationSize);
        if (magnitude.CompareTo(maximum) > 0)
        {
            return source.Coefficient.Sign < 0
                ? unchecked(0U - maximumBits) & destinationMask
                : maximumBits;
        }

        var minimum = PositEncodingReference.Decode(1, destinationSize);
        if (magnitude.CompareTo(minimum) < 0)
        {
            return source.Coefficient.Sign < 0 ? destinationMask : 1;
        }

        var targets = TargetValues[destinationSize];
        var firstGreater = 0;
        var end = targets.Length;
        while (firstGreater < end)
        {
            var middle = firstGreater + ((end - firstGreater) / 2);
            if (targets[middle].Value.CompareTo(source) <= 0) firstGreater = middle + 1;
            else end = middle;
        }

        var lower = targets[firstGreater - 1];
        if (lower.Value.CompareTo(source) == 0) return lower.Bits;

        var upper = targets[firstGreater];
        var extendedEncoding = (lower.Bits << 1) | 1;
        var boundary = PositEncodingReference.Decode(extendedEncoding << (32 - destinationSize - 1), 32);
        var comparison = source.CompareTo(boundary);
        return comparison < 0 || (comparison == 0 && (lower.Bits & 1) == 0)
            ? lower.Bits
            : upper.Bits;
    }

    private static (uint Bits, PositEncodingReference Value)[] DecodeTargetValues(int size)
    {
        var values = new List<(uint Bits, PositEncodingReference Value)>();
        var nar = 1U << (size - 1);
        for (uint bits = 0; bits < (1U << size); bits++)
        {
            if (bits != nar) values.Add((bits, PositEncodingReference.Decode(bits, size)));
        }

        values.Sort((left, right) => left.Value.CompareTo(right.Value));
        return values.ToArray();
    }
}
