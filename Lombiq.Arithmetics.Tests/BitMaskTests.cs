using System;
using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests;

public class BitMaskTests
{
    [Fact]
    public void BitMaskSegmentCountIsCorrectlyCalculatedFromSize()
    {
        var sizesAndSegmentCounts = new[]
        {
            Tuple.Create(new BitMask(0), 0U),
            Tuple.Create(new BitMask(31), 1U),
            Tuple.Create(new BitMask(32), 1U),
            Tuple.Create(new BitMask(33), 2U),
            Tuple.Create(new BitMask(1023), 32U),
            Tuple.Create(new BitMask(1024), 32U),
            Tuple.Create(new BitMask(1025), 33U),
        };

        foreach (var item in sizesAndSegmentCounts)
        {
            Assert.AreEqual(item.Item2, item.Item1.SegmentCount, $"Size: {item.Item1.Size}");
        }
    }

    [Fact]
    public void BitMaskSizeIsCorrectlySetWithSegments()
    {
        var sizesAndSegmentCounts = new[]
        {
            Tuple.Create(new BitMask([]), 0U),
            Tuple.Create(new BitMask([1]), 32U),
            Tuple.Create(new BitMask([2, 2]), 64U),
            Tuple.Create(new BitMask([3, 3, 3]), 96U),
            Tuple.Create(new BitMask([4, 4, 4, 4]), 128U),
            Tuple.Create(new BitMask([0], 222), 222U),
        };

        foreach (var item in sizesAndSegmentCounts)
        {
            Assert.AreEqual(item.Item2, item.Item1.Size, $"Mask: {item.Item1}");
        }
    }

    [Fact]
    public void BitMaskSetOneIsCorrect()
    {
        Assert.AreEqual(1, new BitMask([0]).SetOne(0).Segments[0]);
        Assert.AreEqual(1 << 5, new BitMask(32).SetOne(5).Segments[0]);
        Assert.AreEqual(0xFFFF, new BitMask([0xFFFF]).SetOne(5).Segments[0]);
        Assert.AreEqual(0xFFFF + (1 << 30), new BitMask([0xFFFF]).SetOne(30).Segments[0]);
        Assert.AreEqual((uint)(1 << 30) << 1, new BitMask([0]).SetOne(31).Segments[0]);
        Assert.AreEqual(uint.MaxValue, new BitMask([0xFFFFFFFE]).SetOne(0).Segments[0]);
        Assert.AreEqual(uint.MaxValue, new BitMask([0x7FFFFFFF]).SetOne(31).Segments[0]);
        Assert.AreEqual(new BitMask([0, 0, 0xFFFF]), new BitMask([0, 0, 0xFFFF]).SetOne(79));
        Assert.AreEqual(new BitMask([0, 0, 0x1FFFF]), new BitMask([0, 0, 0xFFFF]).SetOne(80));
    }

    [Fact]
    public void BitMaskSetZeroIsCorrect()
    {
        Assert.AreEqual(0, new BitMask([1]).SetZero(0).Segments[0]);
        Assert.AreEqual(0x7FFF, new BitMask([0xFFFF]).SetZero(15).Segments[0]);
    }

    [Fact]
    public void BitMaskConstructorCorrectlyCopiesBitMask()
    {
        var masks = new[]
        {
            new BitMask([0x42, 0x42]), new BitMask([0x88, 0x88, 0x88]),
        };

        foreach (var mask in masks)
        {
            Assert.AreEqual(mask, new BitMask(mask));
        }
    }

    [Fact]
    public void BitMaskIntegerAdditionIsCorrect()
    {
        Assert.AreEqual(1, (new BitMask([0]) + 1).Segments[0]);
        Assert.AreEqual(0x1FFFE, (new BitMask([0xFFFF]) + 0xFFFF).Segments[0]);
        Assert.AreEqual(0xFFFFFFFF, (new BitMask([0xFFFFFFFE]) + 1).Segments[0]);
        Assert.AreEqual(0xFFFFFFFF, (new BitMask([0xEFFFFFFF]) + 0x10000000).Segments[0]);
        Assert.AreEqual(0, (new BitMask([0xFFFFFFFF]) + 1).Segments[0]);
        Assert.AreEqual(1, (new BitMask([0xFFFFFFFF]) + 2).Segments[0]);
        Assert.AreEqual(new BitMask([0, 0, 1]), new BitMask([0xFFFFFFFF, 0xFFFFFFFF, 0]) + 1);
    }

    [Fact]
    public void BitMaskIntegerSubtractionIsCorrect()
    {
        Assert.AreEqual(0, (new BitMask([1]) - 1).Segments[0]);
        Assert.AreEqual(0, (new BitMask([0xFFFFFFFF]) - 0xFFFFFFFF).Segments[0]);
        Assert.AreEqual(1, (new BitMask([0xFFFFFFFF]) - 0xFFFFFFFE).Segments[0]);
        Assert.AreEqual(0xFFFFFFFF, (new BitMask([0]) - 1).Segments[0]);
        Assert.AreEqual(0xFFFFFFFE, (new BitMask([0]) - 2).Segments[0]);
        Assert.AreEqual(0xEFFFFFFF, (new BitMask([0xFFFFFFFF]) - 0x10000000).Segments[0]);
        Assert.AreEqual(0xFFFFFF, (new BitMask([0x017FFFFF]) - 0x800000).Segments[0]);
        Assert.AreEqual(new BitMask([0xFFFFFFFF, 0], 33), new BitMask([0x7FFFFFFF, 1], 33) - 0x80000000);
    }

    [Fact]
    public void BitMaskAdditionIsCorrect()
    {
        Assert.AreEqual(
            new BitMask([0xFFFFFFFF]),
            new BitMask([0x55555555]) + new BitMask([0xAAAAAAAA]));
        Assert.AreEqual(
            new BitMask([0xFFFFFFFE, 1]),
            new BitMask([0xFFFFFFFF, 0]) + new BitMask([0xFFFFFFFF, 0]));
        Assert.AreEqual(
            new BitMask([0xFFFFFFFE, 0xFFFFFFFF, 1]),
            new BitMask([0xFFFFFFFF, 0xFFFFFFFF, 0]) + new BitMask([0xFFFFFFFF, 0xFFFFFFFF, 0]));
    }

    [Fact]
    public void BitMaskSubtractionIsCorrect()
    {
        Assert.AreEqual(
            new BitMask([0xAAAAAAAA]),
            new BitMask([0xFFFFFFFF]) - new BitMask([0x55555555]));
        Assert.AreEqual(
            new BitMask([0xFFFFFFFF, 0]),
            new BitMask([0xFFFFFFFE, 1]) - new BitMask([0xFFFFFFFF, 0]));
        Assert.AreEqual(
            new BitMask([0xFFFFFFFF, 0xFFFFFFFF, 0]),
            new BitMask([0xFFFFFFFE, 0xFFFFFFFF, 1]) - new BitMask([0xFFFFFFFF, 0xFFFFFFFF, 0]));
    }

    [Fact]
    public void BitMaskBitShiftLeftIsCorrect()
    {
        Assert.AreEqual(
            new BitMask([0x80000000]),
            new BitMask([1]) << 31);
        Assert.AreEqual(
            new BitMask([0x00000003]),
            new BitMask([6]) << -1);
        Assert.AreEqual(
            new BitMask([0x00000000, 0x80000000]),
            new BitMask([1, 0]) << 63);
        Assert.AreEqual(
            new BitMask([0]),
            new BitMask([0x00000001]) << 32);
        Assert.AreEqual(
            new BitMask([0x80000000, 0x00000000]),
            new BitMask([0x00800000, 0x00000000]) << 8);
        Assert.AreEqual(
            new BitMask([1]),
            new BitMask([0x80000000]) << -31);
        Assert.AreEqual(
            new BitMask([1, 0]) << 63,
            new BitMask([0x00000000, 0x80000000]));
    }

    [Fact]
    public void BitMaskBitShiftRightIsCorrect()
    {
        Assert.AreEqual(
            new BitMask([0x00800000, 0x00000000]),
            new BitMask([0x80000000, 0x00000000]) >> 8);
        Assert.AreEqual(
            new BitMask([1]),
            new BitMask([0x80000000]) >> 31);
        Assert.AreEqual(
            new BitMask([1, 0]),
            new BitMask([0x00000000, 0x80000000]) >> 63);
        Assert.AreEqual(
            new BitMask([0x10000010, 0x00000000]),
            new BitMask([0x00000100, 0x00000001]) >> 4);
        Assert.AreEqual(
            new BitMask([0]),
            new BitMask([0x80000000]) >> 32);
        Assert.AreEqual(
            new BitMask([0x80000000]),
            new BitMask([1]) >> -31);
    }

    [Fact]
    public void FindMostSignificantOneIsCorrect()
    {
        Assert.AreEqual(0, new BitMask([0x00000000, 0x00000000]).FindMostSignificantOnePosition());
        Assert.AreEqual(1, new BitMask([0x00000001, 0x00000000]).FindMostSignificantOnePosition());
        Assert.AreEqual(2, new BitMask([0x00000002, 0x00000000]).FindMostSignificantOnePosition());
        Assert.AreEqual(33, new BitMask([0x00000002, 0x00000001]).FindMostSignificantOnePosition());
    }

    [Fact]
    public void FindLeastSignificantOneIsCorrect()
    {
        Assert.AreEqual(0, new BitMask([0x00000000, 0x00000000]).FindLeastSignificantOnePosition());
        Assert.AreEqual(1, new BitMask([0x00000001, 0x00000000]).FindLeastSignificantOnePosition());
        Assert.AreEqual(2, new BitMask([0x00000002, 0x00000000]).FindLeastSignificantOnePosition());
        Assert.AreEqual(2, new BitMask([0x00000002, 0x00000001]).FindLeastSignificantOnePosition());
        Assert.AreEqual(33, new BitMask([0x00000000, 0x00000001]).FindLeastSignificantOnePosition());
    }

    [Fact]
    public void ShiftToRightEndIsCorrect()
    {
        Assert.AreEqual(
            new BitMask([0x00000000, 0x00000000]),
            new BitMask([0x00000000, 0x00000000]).ShiftOutLeastSignificantZeros());
        Assert.AreEqual(
            new BitMask([0x00000001, 0x00000000]).ShiftOutLeastSignificantZeros(),
            new BitMask([0x00000001, 0x00000000]).ShiftOutLeastSignificantZeros());
        Assert.AreEqual(
            new BitMask([0x00000001, 0x00000000]).ShiftOutLeastSignificantZeros(),
            new BitMask([0x00000002, 0x00000000]).ShiftOutLeastSignificantZeros());
        Assert.AreEqual(
            new BitMask([0x00000001, 0x00000000]).ShiftOutLeastSignificantZeros(),
            new BitMask([0x00000000, 0x00000001]).ShiftOutLeastSignificantZeros());
        Assert.AreEqual(
            new BitMask([0x00001001, 0x00000000]).ShiftOutLeastSignificantZeros(),
            new BitMask([0x10010000, 0x00000000]).ShiftOutLeastSignificantZeros());
        Assert.AreEqual(
            new BitMask([0x00001001, 0x00000000]).ShiftOutLeastSignificantZeros(),
            new BitMask([0x00000000, 0x10010000]).ShiftOutLeastSignificantZeros());
    }

    [Fact]
    public void GetTwosComplementIsCorrect()
    {
        Assert.AreEqual(
            new BitMask([0x00000001], 5).GetTwosComplement(5),
            new BitMask([0x1F]));
        Assert.AreEqual(
            new BitMask([0x0000022C], 12).GetTwosComplement(12),
            new BitMask([0x00000DD4]));
    }

    [Fact]
    public void LengthOfRunOfBitsIsCorrect()
    {
        Assert.AreEqual(new BitMask([0x00000001]).LengthOfRunOfBits(32), 31);
        Assert.AreEqual(new BitMask([0x30000000]).LengthOfRunOfBits(32), 2);
        Assert.AreEqual(new BitMask([0x80000000]).LengthOfRunOfBits(32), 1);
        Assert.AreEqual(new BitMask([0x00000000]).LengthOfRunOfBits(32), 32);
        Assert.AreEqual(new BitMask([0x00000013]).LengthOfRunOfBits(5), 1);
        Assert.AreEqual(new BitMask([17]).LengthOfRunOfBits(5), 1);
    }
}
