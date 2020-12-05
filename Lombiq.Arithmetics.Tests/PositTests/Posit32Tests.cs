using Shouldly;
using System.Diagnostics;
using System.Globalization;
using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
    public class Posit32Tests
    {
        [Fact]
        public void EncodeRegimeBitsIsCorrect()
        {
            Assert.AreEqual(Posit32.EncodeRegimeBits(0), 0x40000000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(1), 0x60000000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(2), 0x70000000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(-3), 0x8000000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(-30), 0x00000001);
            Assert.AreEqual(Posit32.EncodeRegimeBits(30), 0x7FFFFFFF);
        }

        [Fact]
        public void Posit32IsCorrectlyConstructedFromInt()
        {
            Assert.AreEqual(new Posit32(0).PositBits, 0x00000000);

            Assert.AreEqual(new Posit32(1).PositBits, 0x40000000);

            Assert.AreEqual(new Posit32(-1).PositBits, 0xC0000000);

            Assert.AreEqual(new Posit32(2).PositBits, 0x48000000);

            Assert.AreEqual(new Posit32(13).PositBits, 0x5D000000);

            Assert.AreEqual(new Posit32(17).PositBits, 0x60400000);

            Assert.AreEqual(new Posit32(500).PositBits, 0x71E80000);

            Assert.AreEqual(new Posit32(100).PositBits, 0b01101010010000000000000000000000);

            Assert.AreEqual(new Posit32(-500).PositBits, 0x8E180000);

            Assert.AreEqual(new Posit32(-499).PositBits, 0x8E1A0000);

            Assert.AreEqual(new Posit32(int.MaxValue).PositBits, 0b_0_111111110_11_00000000000000000000);

            Assert.AreEqual(new Posit32(int.MinValue).PositBits, 0b_1_000000001_01_00000000000000000000);

            Assert.AreEqual(new Posit32(int.MaxValue - 1).PositBits, 0b_0_1111111101100000000000000000000);
        }

        [Fact]
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

            var posit3 = new Posit32((float)100.0125);
            var posit4 = posit3 - 100;

            posit4.PositBits.ShouldBe(new Posit32(0b00010110011001100000000000000000, true).PositBits);

            (new Posit32(500) + new Posit32(-500)).PositBits.ShouldBe(new Posit32(0).PositBits);
            (new Posit32(99988) + new Posit32(-88999)).PositBits.ShouldBe(new Posit32(10989).PositBits);
            (new Posit32((float)0.75) + new Posit32((float)0.75)).PositBits.ShouldBe(new Posit32((float)1.5).PositBits);
            (new Posit32((float)4) + new Posit32((float)-3.75)).PositBits.ShouldBe(new Posit32((float)0.25).PositBits);
        }

        [Fact]
        public void Posit32AdditionIsCorrectForPositives()
        {
            var posit1 = new Posit32(1);

            for (var i = 1; i < 50000; i++)
            {
                posit1 += 1;
            }

            posit1.PositBits.ShouldBe(new Posit32(50000).PositBits);
        }

        [Fact]
        public void Posit32LengthOfRunOfBitsIsCorrect()
        {
            Assert.AreEqual(Posit32.LengthOfRunOfBits(1, 31), 30);
            Assert.AreEqual(Posit32.LengthOfRunOfBits(0x60000000, 31), 2);
            Assert.AreEqual(Posit32.LengthOfRunOfBits(0b00010000100011110101001111000101, 31), 2);
        }

        [Fact]
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

        [Fact]
        public void Posit32MultiplicationIsCorrect()
        {
            var posit1 = new Posit32(1);
            posit1 *= 5;
            posit1.PositBits.ShouldBe(new Posit32(5).PositBits);

            var posit2 = new Posit32(2);
            posit2 *= new Posit32((float)0.25);
            posit2.PositBits.ShouldBe(new Posit32((float)0.5).PositBits);

            var posit3 = new Posit32((float)0.1);
            posit3 *= new Posit32((float)0.01);
            posit3.PositBits.ShouldBe(new Posit32(0b00001100000011000100100110111010, true).PositBits);

            var posit55 = new Posit32(int.MaxValue - 1);
            posit55 *= new Posit32((float)0.25);
            posit55.PositBits.ShouldBe(new Posit32((int.MaxValue - 1) / 4).PositBits);

            posit55 *= new Posit32(0);
            posit55.PositBits.ShouldBe(new Posit32(0).PositBits);

            var positReal1 = new Posit32(0b01000000000000000011010001101110, true);
            var positReal2 = new Posit32(0b01000000000000000011010001101110, true);
            var pr3 = positReal1 * positReal2;

            Assert.AreEqual(pr3.PositBits, 0b01000000000000000110100011011101);
        }

        [Fact]
        public void Posit32DivisionIsCorrect()
        {
            var posit6 = new Posit32(6);
            posit6 /= 4;
            posit6.PositBits.ShouldBe(new Posit32((float)1.5).PositBits);

            var posit2 = new Posit32(2);
            posit2 /= 4;
            posit2.PositBits.ShouldBe(new Posit32((float)0.5).PositBits);

            var posit55 = new Posit32(int.MaxValue - 1);
            posit55 /= new Posit32(4);
            posit55.PositBits.ShouldBe(new Posit32((int.MaxValue - 1) / 4).PositBits);

            posit55 /= new Posit32(0);
            posit55.PositBits.ShouldBe(new Posit32(Posit32.NaNBitMask, true).PositBits);

            var posit12345 = new Posit32(12345);
            posit12345 /= 100;
            posit12345.PositBits.ShouldBe(new Posit32(0b01101011101101110011001100110011, true).PositBits);

            var positBig = new Posit32(5000000);
            positBig /= 1000000;
            positBig.PositBits.ShouldBe(new Posit32(5).PositBits);

            var positBig2 = new Posit32(50000000);
            positBig2 /= 50000000;
            positBig2.PositBits.ShouldBe(new Posit32(1).PositBits);

            var positSmall = new Posit32((float)0.02);
            positSmall /= new Posit32((float)0.05);
            positSmall.PositBits.ShouldBe(new Posit32(0b00110100110011001100110011000101, true).PositBits);

            var positSmall2 = new Posit32((float)0.1);

            positSmall2 /= 100;
            positSmall2.PositBits.ShouldBe(new Posit32(0b00001100000011000100100110111011, true).PositBits);
        }

        [Fact]
        public void Posit32ToIntIsCorrect()
        {
            var posit0 = new Posit32(0);
            Assert.AreEqual((int)posit0, 0);

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

        [Fact]
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

        [Fact]
        public void Posit32IsCorrectlyConstructedFromDouble()
        {
            Assert.AreEqual(new Posit32((double)0).PositBits, 0x00000000);
            Assert.AreEqual(new Posit32((double)-0).PositBits, 0x00000000);
            Assert.AreEqual(new Posit32(0.75).PositBits, 0b0_01111_00_000000000000000000000000);

            Assert.AreEqual(new Posit32(0.0500000007450580596923828125).PositBits, 0b00011110011001100110011001101000);
            Assert.AreEqual(new Posit32(-0.00179999996908009052276611328125).PositBits, 0b1_1110010010100000100100000011000);

            Assert.AreEqual(new Posit32(-134.75).PositBits, 0b10010011110010100000000000000000);
            Assert.AreEqual(new Posit32(100000.5).PositBits, 0b0_111110_00_10000110101000001000000);
            Assert.AreEqual(new Posit32(-2000000.5).PositBits, 0b1_0000001_11_0001011110110111111110);

            Assert.AreEqual(new Posit32(1.065291755432698054096667486857660145523165660824704316367306233814815641380846500396728515625E-38).PositBits, 0b0_0000000000000000000000000000001);
            Assert.AreEqual(new Posit32(2.7647944E+38).PositBits, 0b0_1111111111111111111111111111111);
        }

        [Fact]
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

            var posit11 = new Posit32((float)0.002);
            Assert.AreEqual((float)posit11, (float)0.002);

            var posit9 = new Posit32((float)0.005);
            Assert.AreEqual((float)posit9, (float)0.005);

            var posit10 = new Posit32((float)0.1);
            Assert.AreEqual((float)posit10, (float)0.1);

            var posit12 = new Posit32((float)0.707106781);
            Assert.AreEqual((float)posit12, (float)0.707106781);
            //Debug.WriteLine((float)0.707106781);
        }

        [Fact]
        public void Posit32ToDoubleIsCorrect()
        {
            var posit1 = new Posit32(1);
            Assert.AreEqual((double)posit1, 1);

            var positNegative_1234 = new Posit32(-1234);
            Assert.AreEqual((double)positNegative_1234, -1234);

            var posit3 = new Posit32((double)0.75);
            Assert.AreEqual((double)posit3, 0.75);

            var posit4 = new Posit32((double)-134.75);
            Assert.AreEqual((double)posit4, -134.75);

            var posit5 = new Posit32((double)100000.5);
            Assert.AreEqual((double)posit5, 100000.5);

            var posit6 = new Posit32((double)-2000000.5);
            Assert.AreEqual((float)posit6, -2000000.5);

            var posit7 = new Posit32((double)-0.00179999996908009052276611328125);
            Assert.AreEqual((double)posit7, -0.00179999996908009052276611328125);

            var posit8 = new Posit32((double)0.0500000007450580596923828125);
            Assert.AreEqual((double)posit8, 0.0500000007450580596923828125);

            var posit11 = new Posit32((double)0.002);
            Assert.AreEqual((double)posit11, (double)0.001999999978579581);

            var posit9 = new Posit32((double)0.005);
            Assert.AreEqual((double)posit9, (double)0.005000000004656613);

            var posit10 = new Posit32((double)0.1);
            Assert.AreEqual((double)posit10, (double)0.10000000009313226);

            var posit12 = new Posit32((double)0.707106781);
            Assert.AreEqual((double)posit12, (double)0.7071067802608013);
            //Debug.WriteLine((float)0.707106781);
        }

        [Fact]
        public void Posit32ToQuireIsCorrect()
        {
            var posit1 = new Posit32(1);
            Assert.AreEqual(((Quire)posit1).Segments, (new Quire(new ulong[] { 1 }, 512) << 240).Segments);

            var positNegative1 = new Posit32(-1);
            Assert.AreEqual(((Quire)positNegative1).Segments,
                (new Quire(new ulong[] { 0, 0, 0, 0xFFFF000000000000, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue,
                    ulong.MaxValue, }, 512)).Segments);

            var positNegative3 = new Posit32(-3);
            Assert.AreEqual(((Quire)positNegative3).Segments,
                (new Quire(new ulong[] { 0, 0, 0, 0xFFFD000000000000, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue,
                    ulong.MaxValue, }, 512)).Segments);

            var positMax = new Posit32(0x7FFFFFFF, true);
            Assert.AreEqual(((Quire)positMax).Segments, (new Quire(new ulong[] { 1 }, 512) << 360).Segments);

            var positNaN = new Posit32(Posit32.NaNBitMask, true);
            var QuireNaN = (Quire)positNaN;
            var QuireNaNFromMask = new Quire(new ulong[] { 1 }, 512) << 511;

            Assert.AreEqual(QuireNaN.Segments, QuireNaNFromMask.Segments);
        }

        [Fact]
        public void Posit32FusedSumIsCorrect()
        {
            var positArray = new Posit32[3];
            positArray[0] = new Posit32(1);
            positArray[1] = new Posit32(16777216);
            positArray[2] = new Posit32(4);
            Assert.AreEqual(Posit32.FusedSum(positArray).PositBits, new Posit32(16777224).PositBits);

            positArray[2] = new Posit32(Posit32.NaNBitMask, true);
            Assert.AreEqual(Posit32.FusedSum(positArray).PositBits, positArray[2].PositBits);
        }

        [Fact]
        public void Posit32MultiplyIntoQuireIsCorrect()
        {
            var posit1 = new Posit32(3);
            var posit2 = new Posit32(4);
            var posit3 = new Posit32(-1);

            Assert.AreEqual((new Posit32(Posit32.MultiplyIntoQuire(posit1, posit2))).PositBits, new Posit32(12).PositBits);

            Assert.AreEqual((new Posit32(Posit32.MultiplyIntoQuire(posit1, posit3))).PositBits, new Posit32(-3).PositBits);
        }

        [Fact]
        public void Posit32FusedDotProductIsCorrect()
        {
            var positArray1 = new Posit32[3];
            var positArray2 = new Posit32[3];
            positArray1[0] = new Posit32(1);
            positArray1[1] = new Posit32(2);
            positArray1[2] = new Posit32(3);

            positArray2[0] = new Posit32(1);
            positArray2[1] = new Posit32(2);
            positArray2[2] = new Posit32(4);
            Assert.AreEqual(Posit32.FusedDotProduct(positArray1, positArray2).PositBits, new Posit32(17).PositBits);

            var positArray3 = new Posit32[3];
            positArray3[0] = new Posit32(-1);
            positArray3[1] = new Posit32(2);
            positArray3[2] = new Posit32(-100);
            Assert.AreEqual(Posit32.FusedDotProduct(positArray1, positArray3).PositBits, new Posit32(-297).PositBits);
        }

        [Fact]
        public void Posit32FusedMultiplyAddIsCorrect()
        {
            var posit1 = new Posit32(300);
            var posit2 = new Posit32((float)0.5);
            var posit3 = new Posit32(-1);

            Assert.AreEqual((Posit32.FusedMultiplyAdd(posit1, posit2, posit3)).PositBits, new Posit32(149).PositBits);
            Assert.AreEqual((Posit32.FusedMultiplyAdd(posit1, posit3, posit2)).PositBits, new Posit32((float)-299.5).PositBits);
        }

        [Fact]
        public void Posit32FusedAddMultiplyIsCorrect()
        {
            var posit1 = new Posit32((float)0.75);
            var posit2 = new Posit32((float)0.5);
            var posit3 = new Posit32(-2);

            Assert.AreEqual((Posit32.FusedAddMultiply(posit1, posit2, posit3)).PositBits, new Posit32((float)-2.5).PositBits);
            Assert.AreEqual((Posit32.FusedAddMultiply(posit2, posit3, posit1)).PositBits, new Posit32((float)-1.125).PositBits);
        }

        [Fact]
        public void Posit32FusedMultiplyMultiplySubtractIsCorrect()
        {
            var posit1 = new Posit32((float)0.75);
            var posit2 = new Posit32((float)0.5);
            var posit3 = new Posit32(-2);
            var posit4 = new Posit32((float)125.125);

            Assert.AreEqual((Posit32.FusedMultiplyMultiplySubtract(posit1, posit2, posit3, posit4)).PositBits, new Posit32((float)250.625).PositBits);
            Assert.AreEqual((Posit32.FusedMultiplyMultiplySubtract(posit2, posit3, posit1, posit4)).PositBits, new Posit32((float)-94.84375).PositBits);
        }

        [Fact]
        public void Posit32SquareRootIsCorrect()
        {
            var positNaN = new Posit32(Posit32.NaNBitMask, true);
            Posit32.Sqrt(positNaN).PositBits.ShouldBe(new Posit32(Posit32.NaNBitMask, true).PositBits);

            var positZero = new Posit32(0);
            Posit32.Sqrt(positZero).PositBits.ShouldBe(new Posit32(0).PositBits);

            var positOne = new Posit32(1);
            Posit32.Sqrt(positOne).PositBits.ShouldBe(new Posit32(1).PositBits);

            var positNegative = new Posit32(-1);
            Posit32.Sqrt(positNegative).PositBits.ShouldBe(new Posit32(Posit32.NaNBitMask, true).PositBits);

            var posit4 = new Posit32(4);
            Posit32.Sqrt(posit4).PositBits.ShouldBe(new Posit32(2).PositBits);

            var posit9 = new Posit32(9);
            Posit32.Sqrt(posit9).PositBits.ShouldBe(new Posit32(3).PositBits);

            var posit625 = new Posit32(625);
            Posit32.Sqrt(posit625).PositBits.ShouldBe(new Posit32(25).PositBits);

            var positSmallerThanOne1 = new Posit32((float)0.5);
            Debug.WriteLine(((float)Posit32.Sqrt(positSmallerThanOne1)).ToString("0.0000000000", CultureInfo.InvariantCulture));
            Posit32.Sqrt(positSmallerThanOne1).PositBits.ShouldBe(new Posit32(0b00111011010100000100111100110011, true).PositBits);

            var positBig = new Posit32(1004004);
            Posit32.Sqrt(positBig).PositBits.ShouldBe(new Posit32(1002).PositBits);
        }

        [Fact]
        public void Posit32ToStringIsCorrect()
        {
            var posit1 = new Posit32((float)0.75);
            var posit2 = new Posit32(-200000);
            var posit3 = new Posit32((float)125.12545);
            var posit4 = new Posit32((double)0.999);

            posit1.ToString(CultureInfo.InvariantCulture).ShouldBe("0.75");
            posit2.ToString(CultureInfo.InvariantCulture).ShouldBe("-200000");
            posit3.ToString("0.############", CultureInfo.InvariantCulture).ShouldBe("125.125450134277");
            posit4.ToString("0.###############", CultureInfo.InvariantCulture).ShouldBe("0.998999997973442");

        }
    }
}
