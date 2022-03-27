using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
    public class PositTests
    {
        private readonly PositEnvironment _environment_6_1;
        private readonly PositEnvironment _environment_6_2;
        private readonly PositEnvironment _environment_6_3;
        private readonly PositEnvironment _environment_8_2;
        private readonly PositEnvironment _environment_12_2;
        private readonly PositEnvironment _environment_16_3;
        private readonly PositEnvironment _environment_32_3;
        private readonly PositEnvironment _environment_32_2;

        public PositTests()
        {
            _environment_6_1 = new PositEnvironment(6, 1);
            _environment_6_2 = new PositEnvironment(6, 2);
            _environment_6_3 = new PositEnvironment(6, 3);
            _environment_8_2 = new PositEnvironment(8, 2);
            _environment_12_2 = new PositEnvironment(12, 2);
            _environment_16_3 = new PositEnvironment(16, 3);
            _environment_32_3 = new PositEnvironment(32, 3);
            _environment_32_2 = new PositEnvironment(32, 2);
        }

        [Fact]
        public void EncodeRegimeBitsIsCorrect()
        {
            Assert.AreEqual(new Posit(_environment_8_2).EncodeRegimeBits(0), new BitMask(0x40, _environment_8_2.Size));

            Assert.AreEqual(new Posit(_environment_6_3).EncodeRegimeBits(-3), new BitMask(0x2, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_6_3).EncodeRegimeBits(3), new BitMask(0x1E, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_6_3).EncodeRegimeBits(2), new BitMask(0x1C, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_8_2).EncodeRegimeBits(1), new BitMask(0x60, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_8_2).EncodeRegimeBits(3), new BitMask(0x78, _environment_8_2.Size));

            Assert.AreEqual(new Posit(_environment_8_2).EncodeRegimeBits(6), new BitMask(0x7F, _environment_8_2.Size));
        }

        [Fact]
        public void PositIsCorrectlyConstructedFromUint()
        {
            Assert.AreEqual(new Posit(_environment_6_3, 0U).PositBits, new BitMask(0x0, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_6_3, 2).PositBits, new BitMask(17, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_6_3, 8U).PositBits, new BitMask(0x13, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_6_3, 16384U).PositBits, new BitMask(0x1B, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_6_3, 1_048_576U).PositBits, new BitMask(0x1D, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_8_2, 13U).PositBits, new BitMask(0x5D, _environment_8_2.Size));

            Assert.AreEqual(new Posit(_environment_32_2, 17U).PositBits, new BitMask(0x60400000, _environment_32_2.Size));

            Assert.AreEqual(new Posit(_environment_12_2, 172U).PositBits, new BitMask(0x6D6, _environment_12_2.Size));

            Assert.AreEqual(new Posit(_environment_12_2, 173U).PositBits, new BitMask(0x6D6, _environment_12_2.Size));

            Assert.AreEqual(new Posit(_environment_16_3, 48U).PositBits, new BitMask(22016, _environment_16_3.Size));

            Assert.AreEqual(new Posit(_environment_16_3, 13200U).PositBits, new BitMask(27449, _environment_16_3.Size));

            Assert.AreEqual(new Posit(_environment_16_3, 500U).PositBits, new BitMask(25064, _environment_16_3.Size));

            Assert.AreEqual(new Posit(_environment_32_3, 1U).PositBits, new BitMask(0x40000000, _environment_32_3.Size));

            // examples of Posit rounding
            Assert.AreEqual(new Posit(_environment_8_2, 90U).PositBits, new BitMask(0x6A, _environment_12_2.Size));
            Assert.AreEqual(new Posit(_environment_8_2, 82U).PositBits, new BitMask(0x69, _environment_12_2.Size));

            // Numbers out of range don't get rounded up infinity. They get rounded to the biggest representable
            // finite value (MaxValue).
            Assert.AreEqual(new Posit(_environment_6_1, 500U).PositBits, _environment_6_1.MaxValueBitMask);
        }

        [Fact]
        public void PositIsCorrectlyConstructedFromInt()
        {
            Assert.AreEqual(new Posit(_environment_6_3, 8).PositBits, new BitMask(0x13, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_6_3, 16384).PositBits, new BitMask(0x1B, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_8_2, 13).PositBits, new BitMask(0x5D, _environment_8_2.Size));

            Assert.AreEqual(new Posit(_environment_6_3, -8).PositBits, new BitMask(0x2D, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_8_2, -13).PositBits, new BitMask(0xA3, _environment_8_2.Size));

            Assert.AreEqual(new Posit(_environment_32_3, -1).PositBits, new BitMask(0xC0000000, _environment_32_3.Size));

            Assert.AreEqual(new Posit(_environment_6_3, -16384).PositBits, new BitMask(0x25, _environment_6_3.Size));

            Assert.AreEqual(new Posit(_environment_16_3, -500).PositBits, new BitMask(40472, _environment_16_3.Size));
        }

        [Fact]
        public void PositToIntIsCorrect()
        {
            var posit8 = new Posit(_environment_6_3, 8);
            Assert.AreEqual((int)posit8, 8);

            var posit16384 = new Posit(_environment_6_3, 16384);
            Assert.AreEqual((int)posit16384, 16384);

            var posit1_32_3 = new Posit(_environment_32_3, 1);
            Assert.AreEqual((int)posit1_32_3, 1);
        }

        [Fact]
        public void ExponentSizeIsCorrect()
        {
            var posit16384 = new Posit(_environment_6_3, 16384);
            Assert.AreEqual(posit16384.ExponentSize(), 2);

            var posit2 = new Posit(_environment_6_3, 2);
            Assert.AreEqual(posit2.ExponentSize(), 3);

            var posit3 = new Posit(_environment_6_1, 3);
            Assert.AreEqual(posit3.ExponentSize(), 1);

            var posit3_6_2 = new Posit(_environment_6_2, 3);
            Assert.AreEqual(posit3_6_2.ExponentSize(), 2);

            var posit_negative13 = new Posit(_environment_8_2, -13);
            Assert.AreEqual(posit_negative13.ExponentSize(), 2);
        }

        [Fact]
        public void GetExponentValueIsCorrect()
        {
            var posit16384 = new Posit(_environment_6_3, 16384);
            Assert.AreEqual(posit16384.GetExponentValue(), 6);

            var posit2 = new Posit(_environment_6_3, 2);
            Assert.AreEqual(posit2.GetExponentValue(), 1);

            var posit3 = new Posit(_environment_6_1, 3);
            Assert.AreEqual(posit3.GetExponentValue(), 1);

            var posit3_6_2 = new Posit(_environment_6_2, 3);
            Assert.AreEqual(posit3_6_2.GetExponentValue(), 1);

            var posit_negative13 = new Posit(_environment_8_2, -13);
            Assert.AreEqual(posit_negative13.GetExponentValue(), 3);

            var posit13248 = new Posit(_environment_16_3, 13248);
            Assert.AreEqual(posit13248.GetExponentValue(), 5);
        }

        [Fact]
        public void FractionSizeIsCorrect()
        {
            var posit16384 = new Posit(_environment_6_3, 16384);
            Assert.AreEqual(posit16384.FractionSize(), 0);

            var posit0 = new Posit(_environment_6_3, 0);
            Assert.AreEqual(posit0.FractionSize(), 0);

            var posit2 = new Posit(_environment_6_3, 2);
            Assert.AreEqual(posit2.FractionSize(), 0);

            var posit3 = new Posit(_environment_6_1, 3);
            Assert.AreEqual(posit3.FractionSize(), 2);

            var posit_negative13 = new Posit(_environment_8_2, -13);
            Assert.AreEqual(posit_negative13.FractionSize(), 3);

            var posit13248 = new Posit(_environment_16_3, 13248);
            Assert.AreEqual(posit13248.FractionSize(), 9);
        }

        [Fact]
        public void FractionWithHiddenBitIsCorrect()
        {
            var posit16384 = new Posit(_environment_6_3, 16384);
            Assert.AreEqual(posit16384.FractionWithHiddenBit(), new BitMask(1, _environment_6_3.Size));

            var posit0 = new Posit(_environment_6_1, 0);
            Assert.AreEqual(posit0.FractionWithHiddenBit(), new BitMask(1, _environment_6_1.Size));

            var posit3 = new Posit(_environment_6_1, 3);
            Assert.AreEqual(posit3.FractionWithHiddenBit(), new BitMask(6, _environment_6_1.Size));

            var posit_negative13 = new Posit(_environment_8_2, -13);
            Assert.AreEqual(posit_negative13.FractionWithHiddenBit(), new BitMask(0xD, _environment_6_1.Size));
        }

        [Fact]
        public void GetRegimeKValueIsCorrect()
        {
            Assert.AreEqual(new Posit(_environment_6_3, 8).GetRegimeKValue(), 0);

            Assert.AreEqual(new Posit(_environment_6_3, 16384).GetRegimeKValue(), 1);

            Assert.AreEqual(new Posit(_environment_6_3, 0).GetRegimeKValue(), -5);

            Assert.AreEqual(new Posit(_environment_8_2, 13).GetRegimeKValue(), 0);

            Assert.AreEqual(new Posit(_environment_6_3, -8).GetRegimeKValue(), 0);

            Assert.AreEqual(new Posit(_environment_8_2, -13).GetRegimeKValue(), 0);

            Assert.AreEqual(new Posit(_environment_6_3, -16384).GetRegimeKValue(), 1);
        }

        [Fact]
        public void CalculateScaleFactorIsCorrect()
        {
            Assert.AreEqual(new Posit(_environment_16_3, 13200).CalculateScaleFactor(), 13);
            Assert.AreEqual(new Posit(_environment_16_3, 48).CalculateScaleFactor(), 5);
            Assert.AreEqual(new Posit(_environment_16_3, 13248).CalculateScaleFactor(), 13);
            Assert.AreEqual(new Posit(_environment_16_3, 1).CalculateScaleFactor(), 0);
            Assert.AreEqual(new Posit(_environment_16_3, 2).CalculateScaleFactor(), 1);
        }

        [Fact]
        public void AdditionIsCorrect()
        {
            var posit0 = new Posit(_environment_6_3, 0);
            var posit = posit0 + 1;
            Assert.AreEqual(posit.PositBits, new Posit(_environment_6_3, 1).PositBits);

            var posit_negative_1 = new Posit(_environment_6_3, -1);
            var posit_negative_2 = posit_negative_1 + posit_negative_1;

            Assert.AreEqual(posit_negative_2.PositBits, new Posit(_environment_6_3, -2).PositBits);

            posit_negative_1 -= 1;
            Assert.AreEqual(posit_negative_1.PositBits, new Posit(_environment_6_3, -2).PositBits);

            posit_negative_2 += 1;
            Assert.AreEqual(posit_negative_2.PositBits, new Posit(_environment_6_3, -1).PositBits);

            var posit1 = new Posit(_environment_6_3, 1);
            var posit2 = posit1 + 1;
            Assert.AreEqual(posit2.PositBits, new Posit(_environment_6_3, 2).PositBits);

            var isPosit0 = posit1 - 1;
            Assert.AreEqual(isPosit0.PositBits, posit0.PositBits);

            var posit3 = new Posit(_environment_6_2, 3);
            var posit6 = posit3 + posit3;
            Assert.AreEqual(posit6.PositBits, new Posit(_environment_6_2, 6).PositBits);

            var posit1_16_3 = new Posit(_environment_16_3, 1);
            var posit2_16_3 = new Posit(_environment_16_3, 2);
            var posit3_16_3 = posit2_16_3 + posit1_16_3;
            Assert.AreEqual(posit3_16_3.PositBits, new Posit(_environment_16_3, 3).PositBits);

            var posit4_16_3 = new Posit(_environment_16_3, 3) + posit1_16_3;
            Assert.AreEqual(posit4_16_3.PositBits, new Posit(_environment_16_3, 4).PositBits);

            // This will be OK, once the quire will be used.
            //// var posit66K_32_3 = new Posit.Posit(_environment_32_3, 66000);
            //// var posit66K1_32_3 = posit66K_32_3 + 1;
            //// posit66K1_32_3.PositBits, new Posit.Posit(_environment_32_3, 66001).PositBits);

            var posit48 = new Posit(_environment_16_3, 48);
            var posit13200 = new Posit(_environment_16_3, 13200);
            var posit13248 = posit48 + posit13200;
            Assert.AreEqual(posit13248.PositBits, new Posit(_environment_16_3, 13248).PositBits);

            var posit1_32_2 = new Posit(_environment_32_2, 1);
            var posit2_32_2 = posit1_32_2 + posit1_32_2;
            Assert.AreEqual(posit2_32_2.PositBits, new Posit(_environment_32_2, 2).PositBits);

            var otherPosit13248 = posit13200 + posit48;
            Assert.AreEqual(otherPosit13248.PositBits, new Posit(_environment_16_3, 13248).PositBits);
        }

        [Fact]
        public void AdditionIsCorrectForPositives()
        {
            var posit1 = new Posit(_environment_32_3, 1);

            for (var i = 1; i < 10000; i++)
            {
                posit1 += 1;
            }

            Assert.AreEqual(posit1.PositBits, new Posit(_environment_32_3, 10000).PositBits);

            var posit1_32_2 = new Posit(_environment_32_2, 1);

            for (var i = 1; i < 10000; i++)
            {
                posit1_32_2 += 1;
            }

            Assert.AreEqual(posit1_32_2.PositBits, new Posit(_environment_32_2, 10000).PositBits);
        }

        [Fact]
        public void AdditionIsCorrectForNegatives()
        {
            var posit1 = new Posit(_environment_16_3, -500);

            for (var i = 1; i <= 1000; i++)
            {
                posit1 += 1;
            }

            for (var j = 1; j <= 500; j++)
            {
                posit1 -= 1;
            }

            Assert.AreEqual(posit1.PositBits, new Posit(_environment_16_3, 0).PositBits);

            var positA = new Posit(_environment_32_3, 1);
            var positB = positA;

            for (var i = 1; i < 10000; i++)
            {
                positA += positB;
            }

            var result = (int)positA;
            Assert.AreEqual(result, 10000);
        }
    }
}
