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
            Assert.AreEqual(Posit32.EncodeRegimeBits(0), 0x_4000_0000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(1), 0x_6000_0000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(2), 0x_7000_0000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(-3), 0x_800_0000);
            Assert.AreEqual(Posit32.EncodeRegimeBits(-30), 0x_0000_0001);
            Assert.AreEqual(Posit32.EncodeRegimeBits(30), 0x_7FFF_FFFF);
        }

        [Fact]
        public void Posit32IsCorrectlyConstructedFromInt()
        {
            Assert.AreEqual(new Posit32(0).PositBits, 0x_0000_0000);

            Assert.AreEqual(new Posit32(1).PositBits, 0x_4000_0000);

            Assert.AreEqual(new Posit32(-1).PositBits, 0x_C000_0000);

            Assert.AreEqual(new Posit32(2).PositBits, 0x_4800_0000);

            Assert.AreEqual(new Posit32(13).PositBits, 0x_5D00_0000);

            Assert.AreEqual(new Posit32(17).PositBits, 0x_6040_0000);

            Assert.AreEqual(new Posit32(500).PositBits, 0x_71E8_0000);

            Assert.AreEqual(new Posit32(100).PositBits, 0b_01101010_01000000_00000000_00000000);

            Assert.AreEqual(new Posit32(-500).PositBits, 0x_8E18_0000);

            Assert.AreEqual(new Posit32(-499).PositBits, 0x_8E1A_0000);

            Assert.AreEqual(new Posit32(int.MaxValue).PositBits, 0b_01111111_10110000_00000000_00000000);

            Assert.AreEqual(new Posit32(int.MinValue).PositBits, 0b_10000000_01010000_00000000_00000000);

            Assert.AreEqual(new Posit32(int.MaxValue - 1).PositBits, 0b_01111111_10110000_00000000_00000000);
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
            positNegative_1.PositBits.ShouldBe(0x_C000_0000);

            var positNegative_500 = new Posit32(-500);
            var positNegative_499 = positNegative_500 + 1;
            positNegative_499.PositBits.ShouldBe(new Posit32(-499).PositBits);

            var positNegative_2 = positNegative_1 - 1;
            positNegative_2.PositBits.ShouldBe(new Posit32(-2).PositBits);

            var posit3 = new Posit32(100.0125F);
            var posit4 = posit3 - 100;

            posit4.PositBits.ShouldBe(new Posit32(0b_00010110_01100110_00000000_00000000, true).PositBits);

            (new Posit32(500) + new Posit32(-500)).PositBits.ShouldBe(new Posit32(0).PositBits);
            (new Posit32(99_988) + new Posit32(-88_999)).PositBits.ShouldBe(new Posit32(10_989).PositBits);
            (new Posit32(0.75F) + new Posit32(0.75F)).PositBits.ShouldBe(new Posit32(1.5F).PositBits);
            (new Posit32(4F) + new Posit32(-3.75F)).PositBits.ShouldBe(new Posit32(0.25F).PositBits);
        }

        [Fact]
        public void Posit32AdditionIsCorrectForPositives()
        {
            var posit1 = new Posit32(1);

            for (var i = 1; i < 50_000; i++)
            {
                posit1 += 1;
            }

            posit1.PositBits.ShouldBe(new Posit32(50_000).PositBits);
        }

        [Fact]
        public void Posit32LengthOfRunOfBitsIsCorrect()
        {
            Assert.AreEqual(Posit32.LengthOfRunOfBits(1, 31), 30);
            Assert.AreEqual(Posit32.LengthOfRunOfBits(0x_6000_0000, 31), 2);
            Assert.AreEqual(Posit32.LengthOfRunOfBits(0b_00010000_10001111_01010011_11000101, 31), 2);
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
            posit2 *= new Posit32(0.25F);
            posit2.PositBits.ShouldBe(new Posit32(0.5F).PositBits);

            var posit3 = new Posit32(0.1F);
            posit3 *= new Posit32(0.01F);
            posit3.PositBits.ShouldBe(new Posit32(0b_00001100_00001100_01001001_10111010, true).PositBits);

            var posit55 = new Posit32(int.MaxValue - 1);
            posit55 *= new Posit32(0.25F);
            posit55.PositBits.ShouldBe(new Posit32((int.MaxValue - 1) / 4).PositBits);

            posit55 *= new Posit32(0);
            posit55.PositBits.ShouldBe(new Posit32(0).PositBits);

            var positReal1 = new Posit32(0b_01000000_00000000_00110100_01101110, true);
            var positReal2 = new Posit32(0b_01000000_00000000_00110100_01101110, true);
            var pr3 = positReal1 * positReal2;

            Assert.AreEqual(pr3.PositBits, 0b_01000000_00000000_01101000_11011101);
        }

        [Fact]
        public void Posit32DivisionIsCorrect()
        {
            var posit6 = new Posit32(6);
            posit6 /= 4;
            posit6.PositBits.ShouldBe(new Posit32(1.5F).PositBits);

            var posit2 = new Posit32(2);
            posit2 /= 4;
            posit2.PositBits.ShouldBe(new Posit32(0.5F).PositBits);

            var posit55 = new Posit32(int.MaxValue - 1);
            posit55 /= new Posit32(4);
            posit55.PositBits.ShouldBe(new Posit32((int.MaxValue - 1) / 4).PositBits);

            posit55 /= new Posit32(0);
            posit55.PositBits.ShouldBe(new Posit32(Posit32.NaNBitMask, true).PositBits);

            var posit12345 = new Posit32(12_345);
            posit12345 /= 100;
            posit12345.PositBits.ShouldBe(new Posit32(0b_01101011_10110111_00110011_00110011, true).PositBits);

            var positBig = new Posit32(5_000_000);
            positBig /= 1_000_000;
            positBig.PositBits.ShouldBe(new Posit32(5).PositBits);

            var positBig2 = new Posit32(50_000_000);
            positBig2 /= 50_000_000;
            positBig2.PositBits.ShouldBe(new Posit32(1).PositBits);

            var positSmall = new Posit32(0.02F);
            positSmall /= new Posit32(0.05F);
            positSmall.PositBits.ShouldBe(new Posit32(0b_00110100_11001100_11001100_11000101, true).PositBits);

            var positSmall2 = new Posit32(0.1F);

            positSmall2 /= 100;
            positSmall2.PositBits.ShouldBe(new Posit32(0b_00001100_00001100_01001001_10111011, true).PositBits);
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

            var posit16384 = new Posit32(16_384);
            Assert.AreEqual((int)posit16384, 16_384);

            var positNegative_13 = new Posit32(-13);
            Assert.AreEqual((int)positNegative_13, -13);

            var positIntMaxValue = new Posit32(int.MaxValue);
            Assert.AreEqual((int)positIntMaxValue, int.MaxValue);
            var positCloseToIntMaxValue = new Posit32(2_147_481_600);
            Assert.AreEqual((int)positCloseToIntMaxValue, 2_147_481_600);
        }

        [Fact]
        public void Posit32IsCorrectlyConstructedFromFloat()
        {
            Assert.AreEqual(new Posit32(0F).PositBits, 0x_0000_0000);
            Assert.AreEqual(new Posit32(-0F).PositBits, 0x_0000_0000);
            Assert.AreEqual(new Posit32(0.75F).PositBits, 0b_00111100_00000000_00000000_00000000);

            Assert.AreEqual(new Posit32(0.0500000007450580596923828125F).PositBits, 0b_00011110_01100110_01100110_01101000);
            Assert.AreEqual(new Posit32(-0.00179999996908009052276611328125F).PositBits, 0b_11110010_01010000_01001000_00011000);

            Assert.AreEqual(new Posit32(-134.75F).PositBits, 0x_93CA_0000);
            Assert.AreEqual(new Posit32(100000.5F).PositBits, 0b_01111100_01000011_01010000_01000000);
            Assert.AreEqual(new Posit32(-2000000.5F).PositBits, 0b_10000001_11000101_11101101_11111110);

            Assert.AreEqual(new Posit32(1.065291755432698054096667486857660145523165660824704316367306233814815641380846500396728515625E-38F).PositBits, 0b_00000000_00000000_00000000_00000001);
            Assert.AreEqual(new Posit32(2.7647944F+38F).PositBits, 0b_01111111_11111111_11111111_11111111);
        }

        [Fact]
        public void Posit32IsCorrectlyConstructedFromDouble()
        {
            Assert.AreEqual(new Posit32(0D).PositBits, 0x_0000_0000);
            Assert.AreEqual(new Posit32(-0D).PositBits, 0x_0000_0000);
            Assert.AreEqual(new Posit32(0.75).PositBits, 0b_00111100_00000000_00000000_00000000);

            Assert.AreEqual(new Posit32(0.0500000007450580596923828125).PositBits, 0b_00011110_01100110_01100110_01101000);
            Assert.AreEqual(new Posit32(-0.00179999996908009052276611328125).PositBits, 0b_11110010_01010000_01001000_00011000);

            Assert.AreEqual(new Posit32(-134.75).PositBits, 0b_10010011_11001010_00000000_00000000);
            Assert.AreEqual(new Posit32(100000.5).PositBits, 0b_01111100_01000011_01010000_01000000);
            Assert.AreEqual(new Posit32(-2000000.5).PositBits, 0b_10000001_11000101_11101101_11111110);

            Assert.AreEqual(new Posit32(1.065291755432698054096667486857660145523165660824704316367306233814815641380846500396728515625E-38).PositBits, 0b_00000000_00000000_00000000_00000001);
            Assert.AreEqual(new Posit32(2.7647944E+38).PositBits, 0b_01111111_11111111_11111111_11111111);
        }

        [Fact]
        public void Posit32ToFloatIsCorrect()
        {
            var posit1 = new Posit32(1);
            Assert.AreEqual((float)posit1, 1);

            var positNegative_1234 = new Posit32(-1_234);
            Assert.AreEqual((float)positNegative_1234, -1_234);

            var posit3 = new Posit32(0.75F);
            Assert.AreEqual((float)posit3, 0.75);

            var posit4 = new Posit32(-134.75F);
            Assert.AreEqual((float)posit4, -134.75);

            var posit5 = new Posit32(100000.5F);
            Assert.AreEqual((float)posit5, 100000.5);

            var posit6 = new Posit32(-2000000.5F);
            Assert.AreEqual((float)posit6, -2000000.5);

            var posit7 = new Posit32(-0.00179999996908009052276611328125F);
            Assert.AreEqual((float)posit7, -0.00179999996908009052276611328125);

            var posit8 = new Posit32(0.0500000007450580596923828125F);
            Assert.AreEqual((float)posit8, 0.0500000007450580596923828125);

            var posit11 = new Posit32(0.002F);
            Assert.AreEqual((float)posit11, 0.002F);

            var posit9 = new Posit32(0.005F);
            Assert.AreEqual((float)posit9, 0.005F);

            var posit10 = new Posit32(0.1F);
            Assert.AreEqual((float)posit10, 0.1F);

            var posit12 = new Posit32(0.707106781F);
            Assert.AreEqual((float)posit12, 0.707106781F);
            // Debug.WriteLine(0.707106781F);
        }

        [Fact]
        public void Posit32ToDoubleIsCorrect()
        {
            var posit1 = new Posit32(1);
            Assert.AreEqual((double)posit1, 1);

            var positNegative_1234 = new Posit32(-1_234);
            Assert.AreEqual((double)positNegative_1234, -1_234);

            var posit3 = new Posit32(0.75);
            Assert.AreEqual((double)posit3, 0.75);

            var posit4 = new Posit32(-134.75);
            Assert.AreEqual((double)posit4, -134.75);

            var posit5 = new Posit32(100000.5);
            Assert.AreEqual((double)posit5, 100000.5);

            var posit6 = new Posit32(-2000000.5);
            Assert.AreEqual((float)posit6, -2000000.5);

            var posit7 = new Posit32(-0.00179999996908009052276611328125);
            Assert.AreEqual((double)posit7, -0.00179999996908009052276611328125);

            var posit8 = new Posit32(0.0500000007450580596923828125);
            Assert.AreEqual((double)posit8, 0.0500000007450580596923828125);

            var posit11 = new Posit32(0.002);
            Assert.AreEqual((double)posit11, 0.001999999978579581);

            var posit9 = new Posit32(0.005);
            Assert.AreEqual((double)posit9, 0.005000000004656613);

            var posit10 = new Posit32(0.1);
            Assert.AreEqual((double)posit10, 0.10000000009313226);

            var posit12 = new Posit32(0.707106781);
            Assert.AreEqual((double)posit12, 0.7071067802608013);
            // Debug.WriteLine(0.707106781F);
        }

        [Fact]
        public void Posit32ToQuireIsCorrect()
        {
            var posit1 = new Posit32(1);
            Assert.AreEqual(((Quire)posit1).Segments, (new Quire(new ulong[] { 1 }, 512) << 240).Segments);

            var positNegative1 = new Posit32(-1);
            Assert.AreEqual(
                ((Quire)positNegative1).Segments,
                (new Quire(
                    new ulong[] { 0, 0, 0, 0x_FFFF_0000_0000_0000, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue,
                    ulong.MaxValue, }, 512)).Segments);

            var positNegative3 = new Posit32(-3);
            Assert.AreEqual(
                ((Quire)positNegative3).Segments,
                (new Quire(
                    new ulong[] { 0, 0, 0, 0x_FFFD_0000_0000_0000, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue,
                    ulong.MaxValue, }, 512)).Segments);

            var positMax = new Posit32(0x_7FFF_FFFF, true);
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
            positArray[1] = new Posit32(16_777_216);
            positArray[2] = new Posit32(4);
            Assert.AreEqual(Posit32.FusedSum(positArray).PositBits, new Posit32(16_777_224).PositBits);

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
            var posit2 = new Posit32(0.5F);
            var posit3 = new Posit32(-1);

            Assert.AreEqual((Posit32.FusedMultiplyAdd(posit1, posit2, posit3)).PositBits, new Posit32(149).PositBits);
            Assert.AreEqual((Posit32.FusedMultiplyAdd(posit1, posit3, posit2)).PositBits, new Posit32(-299.5F).PositBits);
        }

        [Fact]
        public void Posit32FusedAddMultiplyIsCorrect()
        {
            var posit1 = new Posit32(0.75F);
            var posit2 = new Posit32(0.5F);
            var posit3 = new Posit32(-2);

            Assert.AreEqual((Posit32.FusedAddMultiply(posit1, posit2, posit3)).PositBits, new Posit32(-2.5F).PositBits);
            Assert.AreEqual((Posit32.FusedAddMultiply(posit2, posit3, posit1)).PositBits, new Posit32(-1.125F).PositBits);
        }

        [Fact]
        public void Posit32FusedMultiplyMultiplySubtractIsCorrect()
        {
            var posit1 = new Posit32(0.75F);
            var posit2 = new Posit32(0.5F);
            var posit3 = new Posit32(-2);
            var posit4 = new Posit32(125.125F);

            Assert.AreEqual((Posit32.FusedMultiplyMultiplySubtract(posit1, posit2, posit3, posit4)).PositBits, new Posit32(250.625F).PositBits);
            Assert.AreEqual((Posit32.FusedMultiplyMultiplySubtract(posit2, posit3, posit1, posit4)).PositBits, new Posit32(-94.84375F).PositBits);
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

            var positSmallerThanOne1 = new Posit32(0.5F);
            Debug.WriteLine(((float)Posit32.Sqrt(positSmallerThanOne1)).ToString("0.0000000000", CultureInfo.InvariantCulture));
            Posit32.Sqrt(positSmallerThanOne1).PositBits.ShouldBe(new Posit32(0b_00111011_01010000_01001111_00110011, true).PositBits);

            var positBig = new Posit32(1_004_004);
            Posit32.Sqrt(positBig).PositBits.ShouldBe(new Posit32(1_002).PositBits);
        }

        [Fact]
        public void Posit32ToStringIsCorrect()
        {
            var posit1 = new Posit32(0.75F);
            var posit2 = new Posit32(-200_000);
            var posit3 = new Posit32(125.12545F);
            var posit4 = new Posit32(0.999);

            posit1.ToString(CultureInfo.InvariantCulture).ShouldBe("0.75");
            posit2.ToString(CultureInfo.InvariantCulture).ShouldBe("-200000");
            posit3.ToString("0.############", CultureInfo.InvariantCulture).ShouldBe("125.125450134277");
            posit4.ToString("0.###############", CultureInfo.InvariantCulture).ShouldBe("0.998999997973442");

        }
    }
}
