using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lombiq.Arithmetics;
using NUnit.Framework;
using Shouldly;

namespace Lombiq.Arithmetics.Tests
{
    [TestFixture]
    class Posit32Tests
    {

        [Test]
        public void EncodeRegimeBitsIsCorrect()
        {
            Assert.AreEqual(new Posit32().EncodeRegimeBits(0), 0x40000000);
            Assert.AreEqual(new Posit32().EncodeRegimeBits(1), 0x60000000);
            Assert.AreEqual(new Posit32().EncodeRegimeBits(2), 0x70000000);
            Assert.AreEqual(new Posit32().EncodeRegimeBits(-3), 0x8000000);
        }

        [Test]
        public void Posit32IsCorrectlyConstructedFromInt()
        {
            Assert.AreEqual(new Posit32(1).PositBits, 0x40000000);

            Assert.AreEqual(new Posit32(-1).PositBits, 0xC0000000);

            Assert.AreEqual(new Posit32(2).PositBits, 0x48000000);

            Assert.AreEqual(new Posit32(13).PositBits, 0x5D000000);

            Assert.AreEqual(new Posit32(17).PositBits, 0x60400000);

            Assert.AreEqual(new Posit32(500).PositBits, 0x71E80000);

            Assert.AreEqual(new Posit32(-500).PositBits, 0x8E180000);

            Assert.AreEqual(new Posit32(-499).PositBits, 0x8E1A0000);
        }

        [Test]
        public void Posit32AdditionIsCorrect()
        {
            var posit16 = new Posit32(16);
            var posit17 = posit16 + 1;
            posit17.PositBits.ShouldBe(new Posit32(17).PositBits);

            var posit1 = new Posit32(1);
            var posit0 = posit1 - 1;
            posit0.PositBits.ShouldBe(new Posit32(0).PositBits);
            var positNegative_1 = posit0 - 1;
            positNegative_1.PositBits.ShouldBe(0xC0000000);

            var positNegative_500 = new Posit32(-500);
            var positNegative_499 = positNegative_500 + 1;
            positNegative_499.PositBits.ShouldBe(new Posit32(-499).PositBits);

            var positNegative_2 = positNegative_1 - 1;
            positNegative_2.PositBits.ShouldBe(new Posit32(-2).PositBits);
        }

        [Test]
        public void Posit32AdditionIsCorrectForPositives()
        {
            var posit1 = new Posit32(1);

            for (var i = 1; i < 50000; i++)
            {
                posit1 += 1;
            }
            posit1.PositBits.ShouldBe(new Posit32(50000).PositBits);
        }

        [Test]
        public void Posit32LengthOfRunOfBitsIsCorrect()
        {
            Assert.AreEqual(Posit32.LengthOfRunOfBits(1, 31), 30);
            Assert.AreEqual(Posit32.LengthOfRunOfBits(0x60000000, 31), 2);
        }

        [Test]
        public void Posit32AdditionIsCorrectForNegatives()
        {
            var posit1 = new Posit32(-500);

            for (var i = 1; i <= 500; i++)
            {
                posit1 += 1;
            }

            for (var j = 1; j <= 500; j++)
            {
                posit1 -= 1;
            }

            posit1.PositBits.ShouldBe(new Posit32(-500).PositBits);
        }

        [Test]
        public void Posit32ToIntIsCorrect()
        {
            var posit1 = new Posit32(1);
            Assert.AreEqual((int)posit1, 1);

            var posit8 = new Posit32(8);
            Assert.AreEqual((int)posit8, 8);

            var posit16384 = new Posit32(16384);
            Assert.AreEqual((int)posit16384, 16384);

            var positNegative_13 = new Posit32(-13);
            Assert.AreEqual((int)positNegative_13, -13);
        }
    }

}

