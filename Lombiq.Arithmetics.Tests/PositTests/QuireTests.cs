using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests;

public class QuireTests
{
    [Fact]
    public void QuireBitShiftLeftIsCorrect()
    {
        Assert.AreEqual(
            new Quire([0x80000000]).Segments,
            (new Quire([1]) << 31).Segments);
        Assert.AreEqual(
            new Quire([0]).Segments,
            (new Quire([6]) << -1).Segments);
        Assert.AreEqual(
            new Quire([0, 0x_8000_0000_0000_0000]).Segments,
            (new Quire([1, 0]) << 127).Segments);
        Assert.AreEqual(
            new Quire([1]).Segments,
            (new Quire([0x00000001]) << 64).Segments);
        Assert.AreEqual(
            new Quire([0x80000000, 0x00000000]).Segments,
            (new Quire([0x00800000, 0x00000000]) << 8).Segments);
        Assert.AreEqual(
            new Quire([0x200000000]).Segments,
            (new Quire([0x00000001]) << -31).Segments);
    }

    [Fact]
    public void QuireBitShiftRightIsCorrect()
    {
        Assert.AreEqual(
            new Quire([0x00800000, 0x00000000]).Segments,
            (new Quire([0x80000000, 0x00000000]) >> 8).Segments);
        Assert.AreEqual(
            new Quire([1]).Segments,
            (new Quire([0x80000000]) >> 31).Segments);
        Assert.AreEqual(
            new Quire([1, 0]).Segments,
            (new Quire([0, 0x_8000_0000_0000_0000]) >> 127).Segments);
        Assert.AreEqual(
            new Quire([1_152_921_504_606_846_992, 0]).Segments,
            (new Quire([0x_0000_0000_0000_0100, 0x_0000_0000_0000_0001]) >> 4).Segments);
        Assert.AreEqual(
            new Quire([0x_8000_0000_0000_0000]).Segments,
            (new Quire([0x_8000_0000_0000_0000]) >> 64).Segments);
        Assert.AreEqual(
            new Quire([0x_4000_0000_0000_0000]).Segments,
            (new Quire([0x_8000_0000_0000_0000]) >> -63).Segments);
    }

    [Fact]
    public void QuireAdditionIsCorrect()
    {
        Assert.AreEqual(
            new Quire([5]).Segments,
            (new Quire([4]) + new Quire([1])).Segments);
        Assert.AreEqual(
            new Quire([0, 2]).Segments,
            (new Quire([ulong.MaxValue, 1]) + new Quire([1, 0])).Segments);
        Assert.AreEqual(
            new Quire([2, 0, 0, 1, 2]).Segments,
            (new Quire([1, 0, 0, 0, 1]) + new Quire([1, 0, 0, 1, 1])).Segments);
    }

    [Fact]
    public void QuireToIntegerAdditionIsCorrect() =>
        Assert.AreEqual(
            new Quire([5]).Segments,
            (new Quire([4]) + 1).Segments);

    [Fact]
    public void QuireSubtractionIsCorrect()
    {
        Assert.AreEqual(
            new Quire([4]).Segments,
            (new Quire([5]) - new Quire([1])).Segments);
        Assert.AreEqual(
            new Quire([1, 0, 0, 0, 1]).Segments,
            (new Quire([2, 0, 0, 1, 2]) - new Quire([1, 0, 0, 1, 1])).Segments);
        Assert.AreEqual(
            new Quire([ulong.MaxValue, 1]).Segments,
            (new Quire([0, 2]) - new Quire([1, 0])).Segments);
    }
}
