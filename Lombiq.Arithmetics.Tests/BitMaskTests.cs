using NUnit.Framework;
using System;

namespace Lombiq.Arithmetics.Tests
{
    [TestFixture]
    public class BitMaskTests
    {
        [Test]
        public void BitMaskSegmentCountIsCorrectlyCalculatedFromSize()
        {
            var sizesAndSegmentCounts = new Tuple<BitMask, uint>[]
            {
                Tuple.Create(new BitMask(0), (uint)0),
                Tuple.Create(new BitMask(31), (uint)1),
                Tuple.Create(new BitMask(32), (uint)1),
                Tuple.Create(new BitMask(33), (uint)2),
                Tuple.Create(new BitMask(1023), (uint)32),
                Tuple.Create(new BitMask(1024), (uint)32),
                Tuple.Create(new BitMask(1025), (uint)33)
            };

            foreach (var item in sizesAndSegmentCounts) Assert.AreEqual(item.Item2, item.Item1.SegmentCount, $"Size: {item.Item1.Size}");
        }

        [Test]
        public void BitMaskSizeIsCorrectlySetWithSegments()
        {
            var sizesAndSegmentCounts = new Tuple<BitMask, uint>[]
            {
                Tuple.Create(new BitMask(new uint[0]), (uint)0),
                Tuple.Create(new BitMask(new uint[] { 1 }), (uint)32),
                Tuple.Create(new BitMask(new uint[] { 2, 2 }), (uint)64),
                Tuple.Create(new BitMask(new uint[] { 3, 3, 3 }), (uint)96),
                Tuple.Create(new BitMask(new uint[] { 4, 4, 4, 4 }), (uint)128),
                Tuple.Create(new BitMask(new uint[] { 0 }, 222), (uint)222)
            };

            foreach (var item in sizesAndSegmentCounts) Assert.AreEqual(item.Item2, item.Item1.Size, $"Mask: {item.Item1}");
        }

        [Test]
        public void BitMaskSetOneIsCorrect()
        {
            Assert.AreEqual(1, new BitMask(new uint[] { 0 }).SetOne(0).Segments[0]);
            Assert.AreEqual(1 << 5, new BitMask(32).SetOne(5).Segments[0]);
            Assert.AreEqual(0xFFFF, new BitMask(new uint[] { 0xFFFF }).SetOne(5).Segments[0]);
            Assert.AreEqual(0xFFFF + (1 << 30), new BitMask(new uint[] { 0xFFFF }).SetOne(30).Segments[0]);
            Assert.AreEqual((uint)(1 << 30) << 1, new BitMask(new uint[] { 0 }).SetOne(31).Segments[0]);
            Assert.AreEqual(uint.MaxValue, new BitMask(new uint[] { 0xFFFFFFFE }).SetOne(0).Segments[0]);
            Assert.AreEqual(uint.MaxValue, new BitMask(new uint[] { 0x7FFFFFFF }).SetOne(31).Segments[0]);
            Assert.AreEqual(new BitMask(new uint[] { 0, 0, 0xFFFF }), new BitMask(new uint[] { 0, 0, 0xFFFF }).SetOne(79));
            Assert.AreEqual(new BitMask(new uint[] { 0, 0, 0x1FFFF }), new BitMask(new uint[] { 0, 0, 0xFFFF }).SetOne(80));
        }
        [Test]
        public void BitMaskSetZeroIsCorrect()
        {
            Assert.AreEqual(0, new BitMask(new uint[] { 1 }).SetZero(0).Segments[0]);
            Assert.AreEqual(0x7FFF, new BitMask(new uint[] { 0xFFFF }).SetZero(15).Segments[0]);

        }

        [Test]
        public void BitMaskConstructorCorrectlyCopiesBitMask()
        {
            var masks = new BitMask[]
            {
                new BitMask(new uint[] { 0x42, 0x42 }), new BitMask(new uint[] { 0x88, 0x88, 0x88 })
            };

            foreach (var mask in masks) Assert.AreEqual(mask, new BitMask(mask));
        }

        [Test]
        public void BitMaskIntegerAdditionIsCorrect()
        {
            Assert.AreEqual(1, (new BitMask(new uint[] { 0 }) + 1).Segments[0]);
            Assert.AreEqual(0x1FFFE, (new BitMask(new uint[] { 0xFFFF }) + 0xFFFF).Segments[0]);
            Assert.AreEqual(0xFFFFFFFF, (new BitMask(new uint[] { 0xFFFFFFFE }) + 1).Segments[0]);
            Assert.AreEqual(0xFFFFFFFF, (new BitMask(new uint[] { 0xEFFFFFFF }) + 0x10000000).Segments[0]);
            Assert.AreEqual(0, (new BitMask(new uint[] { 0xFFFFFFFF }) + 1).Segments[0]);
            Assert.AreEqual(1, (new BitMask(new uint[] { 0xFFFFFFFF }) + 2).Segments[0]);
            Assert.AreEqual(new BitMask(new uint[] { 0, 0, 1 }), new BitMask(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0 }) + 1);
        }

        [Test]
        public void BitMaskIntegerSubtractionIsCorrect()
        {
            Assert.AreEqual(0, (new BitMask(new uint[] { 1 }) - 1).Segments[0]);
            Assert.AreEqual(0, (new BitMask(new uint[] { 0xFFFFFFFF }) - 0xFFFFFFFF).Segments[0]);
            Assert.AreEqual(1, (new BitMask(new uint[] { 0xFFFFFFFF }) - 0xFFFFFFFE).Segments[0]);
            Assert.AreEqual(0xFFFFFFFF, (new BitMask(new uint[] { 0 }) - 1).Segments[0]);
            Assert.AreEqual(0xFFFFFFFE, (new BitMask(new uint[] { 0 }) - 2).Segments[0]);
            Assert.AreEqual(0xEFFFFFFF, (new BitMask(new uint[] { 0xFFFFFFFF }) - 0x10000000).Segments[0]);
            Assert.AreEqual(0xFFFFFF, (new BitMask(new uint[] { 0x017FFFFF }) - 0x800000).Segments[0]);
            Assert.AreEqual(new BitMask(new uint[] { 0xFFFFFFFF, 0 }, 33), new BitMask(new uint[] { 0x7FFFFFFF, 1 }, 33) - 0x80000000);
        }

        [Test]
        public void BitMaskAdditionIsCorrect()
        {
            Assert.AreEqual(new BitMask(new uint[] { 0xFFFFFFFF }),
                            new BitMask(new uint[] { 0x55555555 }) + new BitMask(new uint[] { 0xAAAAAAAA }));
            Assert.AreEqual(new BitMask(new uint[] { 0xFFFFFFFE, 1 }),
                            new BitMask(new uint[] { 0xFFFFFFFF, 0 }) + new BitMask(new uint[] { 0xFFFFFFFF, 0 }));
            Assert.AreEqual(new BitMask(new uint[] { 0xFFFFFFFE, 0xFFFFFFFF, 1 }),
                            new BitMask(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0 }) + new BitMask(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0 }));
        }

        [Test]
        public void BitMaskSubtractionIsCorrect()
        {
            Assert.AreEqual(new BitMask(new uint[] { 0xAAAAAAAA }),
                            new BitMask(new uint[] { 0xFFFFFFFF }) - new BitMask(new uint[] { 0x55555555 }));
            Assert.AreEqual(new BitMask(new uint[] { 0xFFFFFFFF, 0 }),
                            new BitMask(new uint[] { 0xFFFFFFFE, 1 }) - new BitMask(new uint[] { 0xFFFFFFFF, 0 }));
            Assert.AreEqual(new BitMask(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0 }),
                            new BitMask(new uint[] { 0xFFFFFFFE, 0xFFFFFFFF, 1 }) - new BitMask(new uint[] { 0xFFFFFFFF, 0xFFFFFFFF, 0 }));
        }

        [Test]
        public void BitMaskBitShiftLeftIsCorrect()
        {
            Assert.AreEqual(new BitMask(new uint[] { 0x80000000 }),
                            new BitMask(new uint[] { 1 }) << 31);
            Assert.AreEqual(new BitMask(new uint[] { 0x00000000, 0x80000000 }),
                            new BitMask(new uint[] { 1, 0 }) << 63);
            Assert.AreEqual(new BitMask(new uint[] { 0 }),
                            new BitMask(new uint[] { 0x00000001 }) << 32);
            Assert.AreEqual(new BitMask(new uint[] { 0x80000000, 0x00000000 }),
                            new BitMask(new uint[] { 0x00800000, 0x00000000 }) << 8);
            Assert.AreEqual(new BitMask(new uint[] { 1 }),
                            new BitMask(new uint[] { 0x80000000 }) << -31);
            Assert.AreEqual(new BitMask(new uint[] { 1, 0 }) << 63,
                            new BitMask(new uint[] { 0x00000000, 0x80000000 }));

        }

        [Test]
        public void BitMaskBitShiftRightIsCorrect()
        {
            Assert.AreEqual(new BitMask(new uint[] { 0x00800000, 0x00000000 }),
                            new BitMask(new uint[] { 0x80000000, 0x00000000 }) >> 8);
            Assert.AreEqual(new BitMask(new uint[] { 1 }),
                            new BitMask(new uint[] { 0x80000000 }) >> 31);
            Assert.AreEqual(new BitMask(new uint[] { 1, 0 }),
                            new BitMask(new uint[] { 0x00000000, 0x80000000 }) >> 63);
            Assert.AreEqual(new BitMask(new uint[] { 0x10000010, 0x00000000 }),
                            new BitMask(new uint[] { 0x00000100, 0x00000001 }) >> 4);
            Assert.AreEqual(new BitMask(new uint[] { 0 }),
                            new BitMask(new uint[] { 0x80000000 }) >> 32);
            Assert.AreEqual(new BitMask(new uint[] { 0x80000000 }),
                            new BitMask(new uint[] { 1 }) >> -31);


        }

        [Test]
        public void FindMostSignificantOneIsCorrect()
        {
            Assert.AreEqual(0, new BitMask(new uint[] { 0x00000000, 0x00000000 }).GetMostSignificantOnePosition());
            Assert.AreEqual(1, new BitMask(new uint[] { 0x00000001, 0x00000000 }).GetMostSignificantOnePosition());
            Assert.AreEqual(2, new BitMask(new uint[] { 0x00000002, 0x00000000 }).GetMostSignificantOnePosition());
            Assert.AreEqual(33, new BitMask(new uint[] { 0x00000002, 0x00000001 }).GetMostSignificantOnePosition());
        }

        [Test]
        public void FindLeastSignificantOneIsCorrect()
        {
            Assert.AreEqual(0, new BitMask(new uint[] { 0x00000000, 0x00000000 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(1, new BitMask(new uint[] { 0x00000001, 0x00000000 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(2, new BitMask(new uint[] { 0x00000002, 0x00000000 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(2, new BitMask(new uint[] { 0x00000002, 0x00000001 }).GetLeastSignificantOnePosition());
            Assert.AreEqual(33, new BitMask(new uint[] { 0x00000000, 0x00000001 }).GetLeastSignificantOnePosition());
        }

        [Test]
        public void ShiftToRightEndIsCorrect()
        {
            Assert.AreEqual(new BitMask(new uint[] { 0x00000000, 0x00000000 }), new BitMask(new uint[] { 0x00000000, 0x00000000 }).ShiftOutLeastSignificantZeros());
            Assert.AreEqual(new BitMask(new uint[] { 0x00000001, 0x00000000 }).ShiftOutLeastSignificantZeros(), new BitMask(new uint[] { 0x00000001, 0x00000000 }).ShiftOutLeastSignificantZeros());
            Assert.AreEqual(new BitMask(new uint[] { 0x00000001, 0x00000000 }).ShiftOutLeastSignificantZeros(), new BitMask(new uint[] { 0x00000002, 0x00000000 }).ShiftOutLeastSignificantZeros());
            Assert.AreEqual(new BitMask(new uint[] { 0x00000001, 0x00000000 }).ShiftOutLeastSignificantZeros(), new BitMask(new uint[] { 0x00000000, 0x00000001 }).ShiftOutLeastSignificantZeros());
            Assert.AreEqual(new BitMask(new uint[] { 0x00001001, 0x00000000 }).ShiftOutLeastSignificantZeros(), new BitMask(new uint[] { 0x10010000, 0x00000000 }).ShiftOutLeastSignificantZeros());
            Assert.AreEqual(new BitMask(new uint[] { 0x00001001, 0x00000000 }).ShiftOutLeastSignificantZeros(), new BitMask(new uint[] { 0x00000000, 0x10010000 }).ShiftOutLeastSignificantZeros());
        }
    }
}