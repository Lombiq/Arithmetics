using Shouldly;
using System;
using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
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
                Tuple.Create(new BitMask(1_023), 32U),
                Tuple.Create(new BitMask(1_024), 32U),
                Tuple.Create(new BitMask(1_025), 33U),
            };

            foreach (var item in sizesAndSegmentCounts) item.Item2.ShouldBe(item.Item1.SegmentCount, $"Size: {item.Item1.Size}");
        }

        [Fact]
        public void BitMaskSizeIsCorrectlySetWithSegments()
        {
            var sizesAndSegmentCounts = new[]
            {
                Tuple.Create(new BitMask(new uint[0]), 0U),
                Tuple.Create(new BitMask(new uint[] { 1 }), 32U),
                Tuple.Create(new BitMask(new uint[] { 2, 2 }), 64U),
                Tuple.Create(new BitMask(new uint[] { 3, 3, 3 }), 96U),
                Tuple.Create(new BitMask(new uint[] { 4, 4, 4, 4 }), 128U),
                Tuple.Create(new BitMask(new uint[] { 0 }, 222), 222U),
            };

            foreach (var item in sizesAndSegmentCounts) item.Item2.ShouldBe(item.Item1.Size, $"Mask: {item.Item1}");
        }

        [Fact]
        public void BitMaskSetOneIsCorrect()
        {
            Assert.AreEqual(1, new BitMask(new uint[] { 0 }).SetOne(0).Segments[0]);
            Assert.AreEqual(1 << 5, new BitMask(32).SetOne(5).Segments[0]);
            Assert.AreEqual(0x_FFFF, new BitMask(new uint[] { 0x_FFFF }).SetOne(5).Segments[0]);
            Assert.AreEqual(0x_FFFF + (1 << 30), new BitMask(new uint[] { 0x_FFFF }).SetOne(30).Segments[0]);
            Assert.AreEqual((uint)(1 << 30) << 1, new BitMask(new uint[] { 0 }).SetOne(31).Segments[0]);
            Assert.AreEqual(uint.MaxValue, new BitMask(new[] { 0x_FFFF_FFFE }).SetOne(0).Segments[0]);
            Assert.AreEqual(uint.MaxValue, new BitMask(new uint[] { 0x_7FFF_FFFF }).SetOne(31).Segments[0]);
            Assert.AreEqual(new BitMask(new uint[] { 0, 0, 0x_FFFF }), new BitMask(new uint[] { 0, 0, 0x_FFFF }).SetOne(79));
            Assert.AreEqual(new BitMask(new uint[] { 0, 0, 0x_1_FFFF }), new BitMask(new uint[] { 0, 0, 0x_FFFF }).SetOne(80));
        }

        [Fact]
        public void BitMaskSetZeroIsCorrect()
        {
            Assert.AreEqual(0, new BitMask(new uint[] { 1 }).SetZero(0).Segments[0]);
            Assert.AreEqual(0x_7FFF, new BitMask(new uint[] { 0x_FFFF }).SetZero(15).Segments[0]);
        }

        [Fact]
        public void BitMaskConstructorCorrectlyCopiesBitMask()
        {
            var masks = new[]
            {
                new BitMask(new uint[] { 0x_42, 0x_42 }), new BitMask(new uint[] { 0x_88, 0x_88, 0x_88 }),
            };

            foreach (var mask in masks) mask.ShouldBe(new BitMask(mask));
        }

        [Fact]
        public void BitMaskIntegerAdditionIsCorrect()
        {
            Assert.AreEqual(1, (new BitMask(new uint[] { 0 }) + 1).Segments[0]);
            Assert.AreEqual(0x_1_FFFE, (new BitMask(new uint[] { 0x_FFFF }) + 0x_FFFF).Segments[0]);
            Assert.AreEqual(0x_FFFF_FFFF, (new BitMask(new[] { 0x_FFFF_FFFE }) + 1).Segments[0]);
            Assert.AreEqual(0x_FFFF_FFFF, (new BitMask(new[] { 0x_EFFF_FFFF }) + 0x_1000_0000).Segments[0]);
            Assert.AreEqual(0, (new BitMask(new[] { 0x_FFFF_FFFF }) + 1).Segments[0]);
            Assert.AreEqual(1, (new BitMask(new[] { 0x_FFFF_FFFF }) + 2).Segments[0]);
            Assert.AreEqual(new BitMask(new uint[] { 0, 0, 1 }), new BitMask(new uint[] { 0x_FFFF_FFFF, 0x_FFFF_FFFF, 0 }) + 1);
        }

        [Fact]
        public void BitMaskIntegerSubtractionIsCorrect()
        {
            Assert.AreEqual(0, (new BitMask(new uint[] { 1 }) - 1).Segments[0]);
            Assert.AreEqual(0, (new BitMask(new[] { 0x_FFFF_FFFF }) - 0x_FFFF_FFFF).Segments[0]);
            Assert.AreEqual(1, (new BitMask(new[] { 0x_FFFF_FFFF }) - 0x_FFFF_FFFE).Segments[0]);
            Assert.AreEqual(0x_FFFF_FFFF, (new BitMask(new uint[] { 0 }) - 1).Segments[0]);
            Assert.AreEqual(0x_FFFF_FFFE, (new BitMask(new uint[] { 0 }) - 2).Segments[0]);
            Assert.AreEqual(0x_EFFF_FFFF, (new BitMask(new[] { 0x_FFFF_FFFF }) - 0x_1000_0000).Segments[0]);
            Assert.AreEqual(0x_FF_FFFF, (new BitMask(new uint[] { 0x_017F_FFFF }) - 0x_80_0000).Segments[0]);
            Assert.AreEqual(new BitMask(new uint[] { 0x_FFFF_FFFF, 0 }, 33), new BitMask(new uint[] { 0x_7FFF_FFFF, 1 }, 33) - 0x_8000_0000);
        }

        [Fact]
        public void BitMaskAdditionIsCorrect()
        {
            new BitMask(new[] { 0x_FFFF_FFFF }).ShouldBe(
                            new BitMask(new uint[] { 0x_5555_5555 }) + new BitMask(new[] { 0x_AAAA_AAAA }));
            new BitMask(new uint[] { 0x_FFFF_FFFE, 1 }).ShouldBe(
                            new BitMask(new uint[] { 0x_FFFF_FFFF, 0 }) + new BitMask(new uint[] { 0x_FFFF_FFFF, 0 }));
            new BitMask(new uint[] { 0x_FFFF_FFFE, 0x_FFFF_FFFF, 1 }).ShouldBe(
                            new BitMask(new uint[] { 0x_FFFF_FFFF, 0x_FFFF_FFFF, 0 }) + new BitMask(new uint[] { 0x_FFFF_FFFF, 0x_FFFF_FFFF, 0 }));
        }

        [Fact]
        public void BitMaskSubtractionIsCorrect()
        {
            new BitMask(new[] { 0x_AAAA_AAAA }).ShouldBe(
                            new BitMask(new[] { 0x_FFFF_FFFF }) - new BitMask(new uint[] { 0x_5555_5555 }));
            new BitMask(new uint[] { 0x_FFFF_FFFF, 0 }).ShouldBe(
                            new BitMask(new uint[] { 0x_FFFF_FFFE, 1 }) - new BitMask(new uint[] { 0x_FFFF_FFFF, 0 }));
            new BitMask(new uint[] { 0x_FFFF_FFFF, 0x_FFFF_FFFF, 0 }).ShouldBe(
                            new BitMask(new uint[] { 0x_FFFF_FFFE, 0x_FFFF_FFFF, 1 }) - new BitMask(new uint[] { 0x_FFFF_FFFF, 0x_FFFF_FFFF, 0 }));
        }

        [Fact]
        public void BitMaskBitShiftLeftIsCorrect()
        {
            new BitMask(new[] { 0x_8000_0000 }).ShouldBe(
                            new BitMask(new uint[] { 1 }) << 31);
            new BitMask(new uint[] { 0x_0000_0003 }).ShouldBe(
                new BitMask(new uint[] { 6 }) << -1);
            new BitMask(new uint[] { 0x_0000_0000, 0x_8000_0000 }).ShouldBe(
                            new BitMask(new uint[] { 1, 0 }) << 63);
            new BitMask(new uint[] { 0 }).ShouldBe(
                            new BitMask(new uint[] { 0x_0000_0001 }) << 32);
            new BitMask(new uint[] { 0x_8000_0000, 0x_0000_0000 }).ShouldBe(
                             new BitMask(new uint[] { 0x_0080_0000, 0x_0000_0000 }) << 8);
            new BitMask(new uint[] { 1 }).ShouldBe(
                            new BitMask(new[] { 0x_8000_0000 }) << -31);
            (new BitMask(new uint[] { 1, 0 }) << 63).ShouldBe(
                            new BitMask(new uint[] { 0x_0000_0000, 0x_8000_0000 }));
        }

        [Fact]
        public void BitMaskBitShiftRightIsCorrect()
        {
            new BitMask(new uint[] { 0x_0080_0000, 0x_0000_0000 }).ShouldBe(
                            new BitMask(new uint[] { 0x_8000_0000, 0x_0000_0000 }) >> 8);
            new BitMask(new uint[] { 1 }).ShouldBe(
                            new BitMask(new[] { 0x_8000_0000 }) >> 31);
            new BitMask(new uint[] { 1, 0 }).ShouldBe(
                            new BitMask(new uint[] { 0x_0000_0000, 0x_8000_0000 }) >> 63);
            new BitMask(new uint[] { 0x_1000_0010, 0x_0000_0000 }).ShouldBe(
                            new BitMask(new uint[] { 0x_0000_0100, 0x_0000_0001 }) >> 4);
            new BitMask(new uint[] { 0 }).ShouldBe(
                            new BitMask(new[] { 0x_8000_0000 }) >> 32);
            new BitMask(new[] { 0x_8000_0000 }).ShouldBe(
                            new BitMask(new uint[] { 1 }) >> -31);
        }

        [Fact]
        public void FindMostSignificantOneIsCorrect()
        {
            Assert.AreEqual(0, new BitMask(new uint[] { 0x_0000_0000, 0x_0000_0000 }).GetMostSignificantOnePosition());
            Assert.AreEqual(1, new BitMask(new uint[] { 0x_0000_0001, 0x_0000_0000 }).GetMostSignificantOnePosition());
            Assert.AreEqual(2, new BitMask(new uint[] { 0x_0000_0002, 0x_0000_0000 }).GetMostSignificantOnePosition());
            Assert.AreEqual(33, new BitMask(new uint[] { 0x_0000_0002, 0x_0000_0001 }).GetMostSignificantOnePosition());
        }

        [Fact]
        public void FindLeastSignificantOneIsCorrect()
        {
            Assert.AreEqual(0, new BitMask(new uint[] { 0x_0000_0000, 0x_0000_0000 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(1, new BitMask(new uint[] { 0x_0000_0001, 0x_0000_0000 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(2, new BitMask(new uint[] { 0x_0000_0002, 0x_0000_0000 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(2, new BitMask(new uint[] { 0x_0000_0002, 0x_0000_0001 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(33, new BitMask(new uint[] { 0x_0000_0000, 0x_0000_0001 }).GetLeastSignificantOnePosition());
        }

        [Fact]
        public void ShiftToRightEndIsCorrect()
        {
            new BitMask(new uint[] { 0x_0000_0000, 0x_0000_0000 }).ShouldBe(new BitMask(new uint[] { 0x_0000_0000, 0x_0000_0000 }).ShiftOutLeastSignificantZeros());
            new BitMask(new uint[] { 0x_0000_0001, 0x_0000_0000 }).ShiftOutLeastSignificantZeros().ShouldBe(new BitMask(new uint[] { 0x_0000_0001, 0x_0000_0000 }).ShiftOutLeastSignificantZeros());
            new BitMask(new uint[] { 0x_0000_0001, 0x_0000_0000 }).ShiftOutLeastSignificantZeros().ShouldBe(new BitMask(new uint[] { 0x_0000_0002, 0x_0000_0000 }).ShiftOutLeastSignificantZeros());
            new BitMask(new uint[] { 0x_0000_0001, 0x_0000_0000 }).ShiftOutLeastSignificantZeros().ShouldBe(new BitMask(new uint[] { 0x_0000_0000, 0x_0000_0001 }).ShiftOutLeastSignificantZeros());
            new BitMask(new uint[] { 0x_0000_1001, 0x_0000_0000 }).ShiftOutLeastSignificantZeros().ShouldBe(new BitMask(new uint[] { 0x_1001_0000, 0x_0000_0000 }).ShiftOutLeastSignificantZeros());
            new BitMask(new uint[] { 0x_0000_1001, 0x_0000_0000 }).ShiftOutLeastSignificantZeros().ShouldBe(new BitMask(new uint[] { 0x_0000_0000, 0x_1001_0000 }).ShiftOutLeastSignificantZeros());
        }

        [Fact]
        public void GetTwosComplementIsCorrect()
        {
            new BitMask(new uint[] { 0x_0000_0001 }, 5).GetTwosComplement(5).ShouldBe(new BitMask(new uint[] { 0x_1F }));
            new BitMask(new uint[] { 0x_0000_022C }, 12).GetTwosComplement(12).ShouldBe(new BitMask(new uint[]
            {
                0x_0000_0DD4,
            }));
        }

        [Fact]
        public void LengthOfRunOfBitsIsCorrect()
        {
            new BitMask(new uint[] { 0x_0000_0001 }).LengthOfRunOfBits(32).ShouldBe((ushort)31);
            new BitMask(new uint[] { 0x_3000_0000 }).LengthOfRunOfBits(32).ShouldBe((ushort)2);
            new BitMask(new[] { 0x_8000_0000 }).LengthOfRunOfBits(32).ShouldBe((ushort)1);
            new BitMask(new uint[] { 0x_0000_0000 }).LengthOfRunOfBits(32).ShouldBe((ushort)32);
            new BitMask(new uint[] { 0x_0000_0013 }).LengthOfRunOfBits(5).ShouldBe((ushort)1);
            new BitMask(new uint[] { 17 }).LengthOfRunOfBits(5).ShouldBe((ushort)1);
        }

    }
}
