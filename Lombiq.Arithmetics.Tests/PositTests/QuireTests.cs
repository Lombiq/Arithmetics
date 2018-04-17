using NUnit.Framework;
using System;
using Shouldly;

namespace Lombiq.Arithmetics.Tests
{
    class QuireTests
    {

        [Test]
        public void QuireBitShiftLeftIsCorrect()
        {
            Assert.AreEqual(new Quire(new ulong[] { 0x80000000 }).Segments,
                            (new Quire(new ulong[] { 1 }) << 31).Segments);
            Assert.AreEqual(new Quire(new ulong[] { 0x0000000000000003 }).Segments,
                (new Quire(new ulong[] { 6 }) << -1).Segments);
            Assert.AreEqual(new Quire(new ulong[] {0, 0x8000000000000000 }).Segments,
                            (new Quire(new ulong[] { 1, 0 }) << 127).Segments);
            Assert.AreEqual(new Quire(new ulong[] { 0 }).Segments,
                            (new Quire(new ulong[] { 0x00000001 }) << 64).Segments);
            Assert.AreEqual(new Quire(new ulong[] { 0x80000000, 0x00000000 }).Segments,
                             (new Quire(new ulong[] { 0x00800000, 0x00000000 }) << 8).Segments);
            Assert.AreEqual(new Quire(new ulong[] { 1 }).Segments,
                            (new Quire(new ulong[] { 0x80000000 }) << -31).Segments);          
        }

        [Test]
        public void QuireBitShiftRightIsCorrect()
        {
            Assert.AreEqual(new Quire(new ulong[] { 0x00800000, 0x00000000 }).Segments,
                            (new Quire(new ulong[] { 0x80000000, 0x00000000 }) >> 8).Segments);
            Assert.AreEqual(new Quire(new ulong[] { 1 }).Segments,
                            (new Quire(new ulong[] { 0x80000000 }) >> 31).Segments);
            Assert.AreEqual(new Quire(new ulong[] { 1, 0 }).Segments,
                            (new Quire(new ulong[] { 0, 0x8000000000000000 }) >> 127).Segments);
            Assert.AreEqual(new Quire(new ulong[] { 0x1000000000000010, 0 }).Segments,
                            (new Quire(new ulong[] { 0x0000000000000100, 0x0000000000000001 }) >> 4).Segments);
            Assert.AreEqual(new Quire(new ulong[] { 0 }).Segments,
                            (new Quire(new ulong[] { 0x8000000000000000 }) >> 64).Segments);
            Assert.AreEqual(new Quire(new ulong[] { 0x8000000000000000 }).Segments,
                            (new Quire(new ulong[] { 1 }) >> -63).Segments);
        }

    }
}
