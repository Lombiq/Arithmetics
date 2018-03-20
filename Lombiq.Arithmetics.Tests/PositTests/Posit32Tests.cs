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
            Assert.AreEqual(Posit32.EncodeRegimeBits(0), 0x40000000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(1), 0x60000000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(2), 0x70000000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(-3), 0x8000000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(-30), 0x00000001);
            Assert.AreEqual(Posit32.EncodeRegimeBits(30), 0x7FFFFFFF);
        }

        [Test]
        public void Posit32IsCorrectlyConstructedFromInt()
        {
            Assert.AreEqual(new Posit32(0).PositBits, 0x00000000);

            Assert.AreEqual(new Posit32(1).PositBits, 0x40000000);

            Assert.AreEqual(new Posit32(-1).PositBits, 0xC0000000);

            Assert.AreEqual(new Posit32(2).PositBits, 0x48000000);

            Assert.AreEqual(new Posit32(13).PositBits, 0x5D000000);

            Assert.AreEqual(new Posit32(17).PositBits, 0x60400000);

            Assert.AreEqual(new Posit32(500).PositBits, 0x71E80000);

            Assert.AreEqual(new Posit32(-500).PositBits, 0x8E180000);

            Assert.AreEqual(new Posit32(-499).PositBits, 0x8E1A0000);

            Assert.AreEqual(new Posit32(int.MaxValue).PositBits, 0b_0_111111110_11_00000000000000000000);

            Assert.AreEqual(new Posit32(int.MinValue).PositBits, 0b_1_000000001_01_00000000000000000000);

            Assert.AreEqual(new Posit32(int.MaxValue - 1).PositBits, 0b_0_1111111101100000000000000000000);

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

            (new Posit32(500) + new Posit32(-500)).PositBits.ShouldBe(new Posit32(0).PositBits);
            (new Posit32(99988) + new Posit32(-88999)).PositBits.ShouldBe(new Posit32(10989).PositBits);
            (new Posit32((float)0.75) + new Posit32((float)0.75)).PositBits.ShouldBe(new Posit32((float)1.5).PositBits);
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
        public void Posit32MultiplicationIsCorrect()
        {
            var posit1 = new Posit32(1);
            posit1 *= 5;
            posit1.PositBits.ShouldBe(new Posit32(5).PositBits);

            var posit55 = new Posit32(int.MaxValue - 1);
            posit55 *= new Posit32((float)0.25);
            posit55.PositBits.ShouldBe(new Posit32((int.MaxValue - 1) / 4).PositBits);

            posit55 *= new Posit32(0);
            posit55.PositBits.ShouldBe(new Posit32(0).PositBits);

            var positReal1 = new Posit32((float)1.0001);
            var positReal2 = new Posit32((float)1.0001);
            var pr3 = positReal1 * positReal2;
            pr3.PositBits.ShouldBe(new Posit32((float)1.0002).PositBits);
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

            var positIntMaxValue = new Posit32(int.MaxValue);
            Assert.AreEqual((int)positIntMaxValue, int.MaxValue);
            var positCloseToIntMaxValue = new Posit32(2147481600);
            Assert.AreEqual((int)positCloseToIntMaxValue, 2147481600);

        }

        [Test]
        public void Posit32IsCorrectlyConstructedFromFloat()
        {
            Assert.AreEqual(new Posit32((float)0).PositBits, 0x00000000);
            Assert.AreEqual(new Posit32((float)-0).PositBits, 0x00000000);
            Assert.AreEqual(new Posit32((float)0.75).PositBits, 0b0_01111_00_000000000000000000000000);

            Assert.AreEqual(new Posit32((float)0.0500000007450580596923828125).PositBits, 0b0_001_11_10011001100110011001101000);
            Assert.AreEqual(new Posit32((float)-0.00179999996908009052276611328125).PositBits, 0b1_1110_01_0010100000100100000011000);

            Assert.AreEqual(new Posit32((float)-134.75).PositBits, 0x93CA0000);
            Assert.AreEqual(new Posit32((float)100000.5).PositBits, 0b0_111110_00_10000110101000001000000);
            Assert.AreEqual(new Posit32((float)-2000000.5).PositBits, 0b1_0000001_11_0001011110110111111110);

            Assert.AreEqual(new Posit32((float)1.065291755432698054096667486857660145523165660824704316367306233814815641380846500396728515625E-38).PositBits, 0b0_0000000000000000000000000000001);
            Assert.AreEqual(new Posit32((float)2.7647944E+38).PositBits, 0b0_1111111111111111111111111111111);
        }

        [Test]
        public void Posit32ToFloatIsCorrect()
        {
            var posit1 = new Posit32(1);
            Assert.AreEqual((float)posit1, 1);

            var positNegative_1234 = new Posit32(-1234);
            Assert.AreEqual((float)positNegative_1234, -1234);

            var posit3 = new Posit32((float)0.75);
            Assert.AreEqual((float)posit3, 0.75);

            var posit4 = new Posit32((float)-134.75);
            Assert.AreEqual((float)posit4, -134.75);

            var posit5 = new Posit32((float)100000.5);
            Assert.AreEqual((float)posit5, 100000.5);

            var posit6 = new Posit32((float)-2000000.5);
            Assert.AreEqual((float)posit6, -2000000.5);

            var posit7 = new Posit32((float)-0.00179999996908009052276611328125);
            Assert.AreEqual((float)posit7, -0.00179999996908009052276611328125);

            var posit8 = new Posit32((float)0.0500000007450580596923828125);
            Assert.AreEqual((float)posit8, 0.0500000007450580596923828125);
        }

    }

}

