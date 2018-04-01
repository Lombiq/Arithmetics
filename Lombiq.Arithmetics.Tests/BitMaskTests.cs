using NUnit.Framework;
using System;
using Shouldly;

namespace Lombiq.Arithmetics.Tests
{
    [TestFixture]
    public class BitMaskTests
    {
        [Test]
        public void BitMaskSegmentCountIsCorrectlyCalculatedFromSize()
        {
            var sizesAndSegmentCounts = new Tuple<BitMask, ulong>[]
            {
                Tuple.Create(new BitMask(0), (ulong)0),
                Tuple.Create(new BitMask(31), (ulong)1),
                Tuple.Create(new BitMask(32), (ulong)1),
                Tuple.Create(new BitMask(33), (ulong)2),
                Tuple.Create(new BitMask(1023), (ulong)32),
                Tuple.Create(new BitMask(1024), (ulong)32),
                Tuple.Create(new BitMask(1025), (ulong)33)
            };

            foreach (var item in sizesAndSegmentCounts) item.Item2.ShouldBe(item.Item1.SegmentCount, $"Size: {item.Item1.Size}");
        }

        [Test]
        public void BitMaskSizeIsCorrectlySetWithSegments()
        {
            var sizesAndSegmentCounts = new Tuple<BitMask, ulong>[]
            {
                Tuple.Create(new BitMask(new ulong[0]), (ulong)0),
                Tuple.Create(new BitMask(new ulong[] { 1 }), (ulong)32),
                Tuple.Create(new BitMask(new ulong[] { 2, 2 }), (ulong)64),
                Tuple.Create(new BitMask(new ulong[] { 3, 3, 3 }), (ulong)96),
                Tuple.Create(new BitMask(new ulong[] { 4, 4, 4, 4 }), (ulong)128),
                Tuple.Create(new BitMask(new ulong[] { 0 }, 222), (ulong)222)
            };

            foreach (var item in sizesAndSegmentCounts) item.Item2.ShouldBe(item.Item1.Size, $"Mask: {item.Item1}");
        }

        [Test]
        public void BitMaskSetOneIsCorrect()
        {
            Assert.AreEqual(1, new BitMask(new ulong[] { 0 }).SetOne(0).Segments[0]);
            Assert.AreEqual(1 << 5, new BitMask(32).SetOne(5).Segments[0]);
            Assert.AreEqual(0xFFFF, new BitMask(new ulong[] { 0xFFFF }).SetOne(5).Segments[0]);
            Assert.AreEqual(0xFFFF + (1 << 30), new BitMask(new ulong[] { 0xFFFF }).SetOne(30).Segments[0]);
            Assert.AreEqual((1 << 30), new BitMask(new ulong[] { 0 }).SetOne(31).Segments[0]);
            //Assert.AreEqual(uint.MaxValue, new BitMask(new ulong[] { 0xFFFFFFFE }).SetOne(0).Segments[0]);
            //Assert.AreEqual(uint.MaxValue, new BitMask(new ulong[] { 0x7FFFFFFF }).SetOne(31).Segments[0]);
            //Assert.AreEqual(new BitMask(new ulong[] { 0, 0, 0xFFFF }), new BitMask(new ulong[] { 0, 0, 0xFFFF }).SetOne(79));
            //Assert.AreEqual(new BitMask(new ulong[] { 0, 0, 0x1FFFF }), new BitMask(new ulong[] { 0, 0, 0xFFFF }).SetOne(80));
        }
        [Test]
        public void BitMaskSetZeroIsCorrect()
        {
            Assert.AreEqual(0, new BitMask(new ulong[] { 1 }).SetZero(0).Segments[0]);
            Assert.AreEqual(0x7FFF, new BitMask(new ulong[] { 0xFFFF }).SetZero(15).Segments[0]);
        }

        [Test]
        public void BitMaskConstructorCorrectlyCopiesBitMask()
        {
            var masks = new BitMask[]
            {
                new BitMask(new ulong[] { 0x42, 0x42 }), new BitMask(new ulong[] { 0x88, 0x88, 0x88 })
            };

            foreach (var mask in masks) mask.ShouldBe(new BitMask(mask));
        }

        [Test]
        public void BitMaskIntegerAdditionIsCorrect()
        {
            Assert.AreEqual(1, (new BitMask(new ulong[] { 0 }) + 1).Segments[0]);
            Assert.AreEqual(0x1FFFE, (new BitMask(new ulong[] { 0xFFFF }) + 0xFFFF).Segments[0]);
            Assert.AreEqual(0xFFFFFFFF, (new BitMask(new ulong[] { 0xFFFFFFFE }) + 1).Segments[0]);
            Assert.AreEqual(0xFFFFFFFF, (new BitMask(new ulong[] { 0xEFFFFFFF }) + 0x10000000).Segments[0]);
            Assert.AreEqual(0, (new BitMask(new ulong[] { 0xFFFFFFFF }) + 1).Segments[0]);
            Assert.AreEqual(1, (new BitMask(new ulong[] { 0xFFFFFFFF }) + 2).Segments[0]);
            Assert.AreEqual(new BitMask(new ulong[] { 0, 0, 1 }), new BitMask(new ulong[] { 0xFFFFFFFF, 0xFFFFFFFF, 0 }) + 1);
        }

        [Test]
        public void BitMaskIntegerSubtractionIsCorrect()
        {
            Assert.AreEqual(0, (new BitMask(new ulong[] { 1 }) - 1).Segments[0]);
            Assert.AreEqual(0, (new BitMask(new ulong[] { 0xFFFFFFFF }) - 0xFFFFFFFF).Segments[0]);
            Assert.AreEqual(1, (new BitMask(new ulong[] { 0xFFFFFFFF }) - 0xFFFFFFFE).Segments[0]);
            Assert.AreEqual(0xFFFFFFFF, (new BitMask(new ulong[] { 0 }) - 1).Segments[0]);
            Assert.AreEqual(0xFFFFFFFE, (new BitMask(new ulong[] { 0 }) - 2).Segments[0]);
            Assert.AreEqual(0xEFFFFFFF, (new BitMask(new ulong[] { 0xFFFFFFFF }) - 0x10000000).Segments[0]);
            Assert.AreEqual(0xFFFFFF, (new BitMask(new ulong[] { 0x017FFFFF }) - 0x800000).Segments[0]);
            Assert.AreEqual(new BitMask(new ulong[] { 0xFFFFFFFF, 0 }, 33), new BitMask(new ulong[] { 0x7FFFFFFF, 1 }, 33) - 0x80000000);
        }

        [Test]
        public void BitMaskAdditionIsCorrect()
        {
            (new BitMask(new BitMask(new ulong[] { 0x_5555_5555_5555_5555 }) + new BitMask(new ulong[] { 0x_AAAA_AAAA_AAAA_AAAA })))
                .ShouldBe(new BitMask(64, true));
            //new BitMask(new ulong[] { 0x_FFFF_FFFF_FFFF_FFFE, 1 }).ShouldBe(
            //                new BitMask(new ulong[] { 0x_FFFF_FFFF_FFFF_FFFF, 0 }) + new BitMask(new ulong[] { 0x_FFFF_FFFF_FFFF_FFFF, 0 }));
            //new BitMask(new ulong[] { 0x_FFFF_FFFF_FFFF_FFFE, 0x_FFFF_FFFF_FFFF_FFFF, 1 }).ShouldBe(
            //                new BitMask(new ulong[] { 0x_FFFF_FFFF_FFFF_FFFF, 0x_FFFF_FFFF_FFFF_FFFF, 0 }) + new BitMask(new ulong[] { 0x_FFFF_FFFF_FFFF_FFFF, 0x_FFFF_FFFF_FFFF_FFFF, 0 }));
        }

        [Test]
        public void BitMaskSubtractionIsCorrect()
        {
            new BitMask(new ulong[] { 0xAAAAAAAA }).ShouldBe(
                            new BitMask(new ulong[] { 0xFFFFFFFF }) - new BitMask(new ulong[] { 0x55555555 }));
            new BitMask(new ulong[] { 0xFFFFFFFF, 0 }).ShouldBe(
                            new BitMask(new ulong[] { 0xFFFFFFFE, 1 }) - new BitMask(new ulong[] { 0xFFFFFFFF, 0 }));
            new BitMask(new ulong[] { 0xFFFFFFFF, 0xFFFFFFFF, 0 }).ShouldBe(
                            new BitMask(new ulong[] { 0xFFFFFFFE, 0xFFFFFFFF, 1 }) - new BitMask(new ulong[] { 0xFFFFFFFF, 0xFFFFFFFF, 0 }));
        }

        [Test]
        public void BitMaskBitShiftLeftIsCorrect()
        {
            new BitMask(new ulong[] { 0x80000000 }).ShouldBe(
                            new BitMask(new ulong[] { 1 }) << 31);
            new BitMask(new ulong[] { 0x00000003 }).ShouldBe(
                new BitMask(new ulong[] { 6 }) << -1);
            new BitMask(new ulong[] { 0x_8000_0000_0000_0000, 0x_0000_0000_0000_0000 }).ShouldBe(
                            new BitMask(new ulong[] { 1, 0 }) << 63);
            new BitMask(new ulong[] { 0 }).ShouldBe(
                            new BitMask(new ulong[] { 1 }) << 64);
            new BitMask(new ulong[] { 0x_0000_0000_8000_0000, 0 }).ShouldBe(
                             new BitMask(new ulong[] { 0x_0000_0000_0080_0000, 0 }) << 8);
            new BitMask(new ulong[] { 1 }).ShouldBe(
                            new BitMask(new ulong[] { 0x80000000 }) << -31);
            (new BitMask(new ulong[] { 0x_0000_0000_8000_0000, 0 }) << 33).ShouldBe(
                            new BitMask(new ulong[] { 0, 1 }));
        }

        [Test]
        public void BitMaskBitShiftRightIsCorrect()
        {
            new BitMask(new ulong[] { 0x00800000, 0x00000000 }).ShouldBe(
                            new BitMask(new ulong[] { 0x80000000, 0x00000000 }) >> 8);
            new BitMask(new ulong[] { 1 }).ShouldBe(
                            new BitMask(new ulong[] { 0x80000000 }) >> 31);
            new BitMask(new ulong[] { 1, 0 }).ShouldBe(
                            new BitMask(new ulong[] { 0x00000000, 0x80000000 }) >> 63);
            new BitMask(new ulong[] { 0x10000010, 0x00000000 }).ShouldBe(
                            new BitMask(new ulong[] { 0x00000100, 0x00000001 }) >> 4);
            new BitMask(new ulong[] { 0 }).ShouldBe(
                            new BitMask(new ulong[] { 0x80000000 }) >> 32);
            new BitMask(new ulong[] { 0x80000000 }).ShouldBe(
                            new BitMask(new ulong[] { 1 }) >> -31);
        }

        [Test]
        public void FindMostSignificantOneIsCorrect()
        {
            Assert.AreEqual(0, new BitMask(new ulong[] { 0x00000000, 0x00000000 }).GetMostSignificantOnePosition());
            Assert.AreEqual(1, new BitMask(new ulong[] { 0x00000001, 0x00000000 }).GetMostSignificantOnePosition());
            Assert.AreEqual(2, new BitMask(new ulong[] { 0x00000002, 0x00000000 }).GetMostSignificantOnePosition());
            Assert.AreEqual(33, new BitMask(new ulong[] { 0x00000002, 0x00000001 }).GetMostSignificantOnePosition());
        }

        [Test]
        public void FindLeastSignificantOneIsCorrect()
        {
            Assert.AreEqual(0, new BitMask(new ulong[] { 0x00000000, 0x00000000 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(1, new BitMask(new ulong[] { 0x00000001, 0x00000000 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(2, new BitMask(new ulong[] { 0x00000002, 0x00000000 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(2, new BitMask(new ulong[] { 0x00000002, 0x00000001 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(33, new BitMask(new ulong[] { 0x00000000, 0x00000001 }).GetLeastSignificantOnePosition());
        }

        [Test]
        public void ShiftToRightEndIsCorrect()
        {
            new BitMask(new ulong[] { 0x00000000, 0x00000000 }).ShouldBe(new BitMask(new ulong[] { 0x00000000, 0x00000000 }).ShiftOutLeastSignificantZeros());
            new BitMask(new ulong[] { 0x00000001, 0x00000000 }).ShiftOutLeastSignificantZeros().ShouldBe(new BitMask(new ulong[] { 0x00000001, 0x00000000 }).ShiftOutLeastSignificantZeros());
            new BitMask(new ulong[] { 0x00000001, 0x00000000 }).ShiftOutLeastSignificantZeros().ShouldBe(new BitMask(new ulong[] { 0x00000002, 0x00000000 }).ShiftOutLeastSignificantZeros());
            new BitMask(new ulong[] { 0x00000001, 0x00000000 }).ShiftOutLeastSignificantZeros().ShouldBe(new BitMask(new ulong[] { 0x00000000, 0x00000001 }).ShiftOutLeastSignificantZeros());
            new BitMask(new ulong[] { 0x00001001, 0x00000000 }).ShiftOutLeastSignificantZeros().ShouldBe(new BitMask(new ulong[] { 0x10010000, 0x00000000 }).ShiftOutLeastSignificantZeros());
            new BitMask(new ulong[] { 0x00001001, 0x00000000 }).ShiftOutLeastSignificantZeros().ShouldBe(new BitMask(new ulong[] { 0x00000000, 0x10010000 }).ShiftOutLeastSignificantZeros());
        }

        [Test]
        public void GetTwosComplementIsCorrect()
        {
            new BitMask(new ulong[] { 0x00000001 }, 5).GetTwosComplement(5).ShouldBe(new BitMask(new ulong[] { 0x1F }));
            new BitMask(new ulong[] { 0x0000022C }, 12).GetTwosComplement(12).ShouldBe(new BitMask(new ulong[] { 0x00000DD4 }));
        }

        [Test]
        public void LengthOfRunOfBitsIsCorrect()
        {
            new BitMask(new ulong[] { 0x00000001 }).LengthOfRunOfBits(32).ShouldBe((ushort)31);
            new BitMask(new ulong[] { 0x30000000 }).LengthOfRunOfBits(32).ShouldBe((ushort)2);
            new BitMask(new ulong[] { 0x80000000 }).LengthOfRunOfBits(32).ShouldBe((ushort)1);
            new BitMask(new ulong[] { 0x00000000 }).LengthOfRunOfBits(32).ShouldBe((ushort)32);
            new BitMask(new ulong[] { 0x00000013 }).LengthOfRunOfBits(5).ShouldBe((ushort)1);
            new BitMask(new ulong[] { 17 }).LengthOfRunOfBits(5).ShouldBe((ushort)1);
        }

    }
}