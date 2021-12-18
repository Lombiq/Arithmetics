using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
    public class QuireTests
    {
        [Fact]
        public void QuireBitShiftLeftIsCorrect()
        {
            Assert.AreEqual(
                new Quire(new ulong[] { 0x80000000 }).GetSegments(),
                (new Quire(new ulong[] { 1 }) << 31).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 0 }).GetSegments(),
                (new Quire(new ulong[] { 6 }) << -1).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 0, 0x_8000_0000_0000_0000 }).GetSegments(),
                (new Quire(new ulong[] { 1, 0 }) << 127).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 1 }).GetSegments(),
                (new Quire(new ulong[] { 0x00000001 }) << 64).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 0x80000000, 0x00000000 }).GetSegments(),
                (new Quire(new ulong[] { 0x00800000, 0x00000000 }) << 8).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 0x200000000 }).GetSegments(),
                (new Quire(new ulong[] { 0x00000001 }) << -31).GetSegments());
        }

        [Fact]
        public void QuireBitShiftRightIsCorrect()
        {
            Assert.AreEqual(
                new Quire(new ulong[] { 0x00800000, 0x00000000 }).GetSegments(),
                (new Quire(new ulong[] { 0x80000000, 0x00000000 }) >> 8).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 1 }).GetSegments(),
                (new Quire(new ulong[] { 0x80000000 }) >> 31).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 1, 0 }).GetSegments(),
                (new Quire(new ulong[] { 0, 0x_8000_0000_0000_0000 }) >> 127).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 1_152_921_504_606_846_992, 0 }).GetSegments(),
                (new Quire(new ulong[] { 0x_0000_0000_0000_0100, 0x_0000_0000_0000_0001 }) >> 4).GetSegments());
            Assert.AreEqual(
                new Quire(new[] { 0x_8000_0000_0000_0000 }).GetSegments(),
                (new Quire(new[] { 0x_8000_0000_0000_0000 }) >> 64).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 0x_4000_0000_0000_0000 }).GetSegments(),
                (new Quire(new[] { 0x_8000_0000_0000_0000 }) >> -63).GetSegments());
        }

        [Fact]
        public void QuireAdditionIsCorrect()
        {
            Assert.AreEqual(
                new Quire(new ulong[] { 5 }).GetSegments(),
                (new Quire(new ulong[] { 4 }) + new Quire(new ulong[] { 1 })).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 0, 2 }).GetSegments(),
                (new Quire(new ulong[] { ulong.MaxValue, 1 }) + new Quire(new ulong[] { 1, 0 })).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 2, 0, 0, 1, 2 }).GetSegments(),
                (new Quire(new ulong[] { 1, 0, 0, 0, 1 }) + new Quire(new ulong[] { 1, 0, 0, 1, 1 })).GetSegments());
        }

        [Fact]
        public void QuireToIntegerAdditionIsCorrect() =>
            Assert.AreEqual(
                new Quire(new ulong[] { 5 }).GetSegments(),
                (new Quire(new ulong[] { 4 }) + 1).GetSegments());

        [Fact]
        public void QuireSubtractionIsCorrect()
        {
            Assert.AreEqual(
                new Quire(new ulong[] { 4 }).GetSegments(),
                (new Quire(new ulong[] { 5 }) - new Quire(new ulong[] { 1 })).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { 1, 0, 0, 0, 1 }).GetSegments(),
                (new Quire(new ulong[] { 2, 0, 0, 1, 2 }) - new Quire(new ulong[] { 1, 0, 0, 1, 1 })).GetSegments());
            Assert.AreEqual(
                new Quire(new ulong[] { ulong.MaxValue, 1 }).GetSegments(),
                (new Quire(new ulong[] { 0, 2 }) - new Quire(new ulong[] { 1, 0 })).GetSegments());
        }
    }
}
