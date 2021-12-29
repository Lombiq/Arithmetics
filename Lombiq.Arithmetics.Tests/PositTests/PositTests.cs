using Shouldly;
using System;
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
            new Posit(_environment_8_2).EncodeRegimeBits(0).ShouldBe(new BitMask(0x40, _environment_8_2.Size));

            new Posit(_environment_6_3).EncodeRegimeBits(-3).ShouldBe(new BitMask(0x2, _environment_6_3.Size));

            new Posit(_environment_6_3).EncodeRegimeBits(3).ShouldBe(new BitMask(0x1E, _environment_6_3.Size));

            new Posit(_environment_6_3).EncodeRegimeBits(2).ShouldBe(new BitMask(0x1C, _environment_6_3.Size));

            new Posit(_environment_8_2).EncodeRegimeBits(1).ShouldBe(new BitMask(0x60, _environment_6_3.Size));

            new Posit(_environment_8_2).EncodeRegimeBits(3).ShouldBe(new BitMask(0x78, _environment_8_2.Size));

            new Posit(_environment_8_2).EncodeRegimeBits(6).ShouldBe(new BitMask(0x7F, _environment_8_2.Size));
        }

        [Fact]
        public void PositIsCorrectlyConstructedFromUint()
        {
            new Posit(_environment_6_3, 0U).PositBits.ShouldBe(new BitMask(0x0, _environment_6_3.Size));

            new Posit(_environment_6_3, 2).PositBits.ShouldBe(new BitMask(17, _environment_6_3.Size));

            new Posit(_environment_6_3, 8U).PositBits.ShouldBe(new BitMask(0x13, _environment_6_3.Size));

            new Posit(_environment_6_3, 16384U).PositBits.ShouldBe(new BitMask(0x1B, _environment_6_3.Size));

            new Posit(_environment_6_3, 1_048_576U).PositBits.ShouldBe(new BitMask(0x1D, _environment_6_3.Size));

            new Posit(_environment_8_2, 13U).PositBits.ShouldBe(new BitMask(0x5D, _environment_8_2.Size));

            new Posit(_environment_32_2, 17U).PositBits.ShouldBe(new BitMask(0x60400000, _environment_32_2.Size));

            new Posit(_environment_12_2, 172U).PositBits.ShouldBe(new BitMask(0x6D6, _environment_12_2.Size));

            new Posit(_environment_12_2, 173U).PositBits.ShouldBe(new BitMask(0x6D6, _environment_12_2.Size));

            new Posit(_environment_16_3, 48U).PositBits.ShouldBe(new BitMask(22016, _environment_16_3.Size));

            new Posit(_environment_16_3, 13200U).PositBits.ShouldBe(new BitMask(27449, _environment_16_3.Size));

            new Posit(_environment_16_3, 500U).PositBits.ShouldBe(new BitMask(25064, _environment_16_3.Size));

            new Posit(_environment_32_3, 1U).PositBits.ShouldBe(new BitMask(0x40000000, _environment_32_3.Size));

            // examples of Posit rounding
            new Posit(_environment_8_2, 90U).PositBits.ShouldBe(new BitMask(0x6A, _environment_12_2.Size));
            new Posit(_environment_8_2, 82U).PositBits.ShouldBe(new BitMask(0x69, _environment_12_2.Size));

            // Numbers out of range don't get rounded up infinity. They get rounded to the biggest representable
            // finite value (MaxValue).
            new Posit(_environment_6_1, 500U).PositBits.ShouldBe(_environment_6_1.MaxValueBitMask);
        }

        [Fact]
        public void PositIsCorrectlyConstructedFromInt()
        {
            new Posit(_environment_6_3, 8).PositBits.ShouldBe(new BitMask(0x13, _environment_6_3.Size));

            new Posit(_environment_6_3, 16384).PositBits.ShouldBe(new BitMask(0x1B, _environment_6_3.Size));

            new Posit(_environment_8_2, 13).PositBits.ShouldBe(new BitMask(0x5D, _environment_8_2.Size));

            new Posit(_environment_6_3, -8).PositBits.ShouldBe(new BitMask(0x2D, _environment_6_3.Size));

            new Posit(_environment_8_2, -13).PositBits.ShouldBe(new BitMask(0xA3, _environment_8_2.Size));

            new Posit(_environment_32_3, -1).PositBits.ShouldBe(new BitMask(0xC0000000, _environment_32_3.Size));

            new Posit(_environment_6_3, -16384).PositBits.ShouldBe(new BitMask(0x25, _environment_6_3.Size));

            new Posit(_environment_16_3, -500).PositBits.ShouldBe(new BitMask(40472, _environment_16_3.Size));
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
            posit16384.FractionWithHiddenBit().ShouldBe(new BitMask(1, _environment_6_3.Size));

            var posit0 = new Posit(_environment_6_1, 0);
            posit0.FractionWithHiddenBit().ShouldBe(new BitMask(1, _environment_6_1.Size));

            var posit3 = new Posit(_environment_6_1, 3);
            posit3.FractionWithHiddenBit().ShouldBe(new BitMask(6, _environment_6_1.Size));

            var posit_negative13 = new Posit(_environment_8_2, -13);
            posit_negative13.FractionWithHiddenBit().ShouldBe(new BitMask(0xD, _environment_6_1.Size));
        }

        [Fact]
        public void GetRegimeKValueIsCorrect()
        {
            new Posit(_environment_6_3, 8).GetRegimeKValue().ShouldBe(0);

            new Posit(_environment_6_3, 16384).GetRegimeKValue().ShouldBe(1);

            new Posit(_environment_6_3, 0).GetRegimeKValue().ShouldBe(-5);

            new Posit(_environment_8_2, 13).GetRegimeKValue().ShouldBe(0);

            new Posit(_environment_6_3, -8).GetRegimeKValue().ShouldBe(0);

            new Posit(_environment_8_2, -13).GetRegimeKValue().ShouldBe(0);

            new Posit(_environment_6_3, -16384).GetRegimeKValue().ShouldBe(1);
        }

        [Fact]
        public void CalculateScaleFactorIsCorrect()
        {
            new Posit(_environment_16_3, 13200).CalculateScaleFactor().ShouldBe(13);
            new Posit(_environment_16_3, 48).CalculateScaleFactor().ShouldBe(5);
            new Posit(_environment_16_3, 13248).CalculateScaleFactor().ShouldBe(13);
            new Posit(_environment_16_3, 1).CalculateScaleFactor().ShouldBe(0);
            new Posit(_environment_16_3, 2).CalculateScaleFactor().ShouldBe(1);
        }

        [Fact]
        public void AdditionIsCorrect()
        {
            var posit0 = new Posit(_environment_6_3, 0);
            var posit = posit0 + 1;
            posit.PositBits.ShouldBe(new Posit(_environment_6_3, 1).PositBits);

            var posit_negative_1 = new Posit(_environment_6_3, -1);
            var posit_negative_2 = posit_negative_1 + posit_negative_1;

            posit_negative_2.PositBits.ShouldBe(new Posit(_environment_6_3, -2).PositBits);

            posit_negative_1 -= 1;
            posit_negative_1.PositBits.ShouldBe(new Posit(_environment_6_3, -2).PositBits);

            posit_negative_2 += 1;
            posit_negative_2.PositBits.ShouldBe(new Posit(_environment_6_3, -1).PositBits);

            var posit1 = new Posit(_environment_6_3, 1);
            var posit2 = posit1 + 1;
            posit2.PositBits.ShouldBe(new Posit(_environment_6_3, 2).PositBits);

            var isPosit0 = posit1 - 1;
            isPosit0.PositBits.ShouldBe(posit0.PositBits);

            var posit3 = new Posit(_environment_6_2, 3);
            var posit6 = posit3 + posit3;
            posit6.PositBits.ShouldBe(new Posit(_environment_6_2, 6).PositBits);

            var posit1_16_3 = new Posit(_environment_16_3, 1);
            var posit2_16_3 = new Posit(_environment_16_3, 2);
            var posit3_16_3 = posit2_16_3 + posit1_16_3;
            posit3_16_3.PositBits.ShouldBe(new Posit(_environment_16_3, 3).PositBits);

            var posit4_16_3 = new Posit(_environment_16_3, 3) + posit1_16_3;
            posit4_16_3.PositBits.ShouldBe(new Posit(_environment_16_3, 4).PositBits);

            // This will be OK, once the quire will be used.
            //// var posit66K_32_3 = new Posit.Posit(_environment_32_3, 66000);
            //// var posit66K1_32_3 = posit66K_32_3 + 1;
            //// posit66K1_32_3.PositBits.ShouldBe(new Posit.Posit(_environment_32_3, 66001).PositBits);

            var posit48 = new Posit(_environment_16_3, 48);
            var posit13200 = new Posit(_environment_16_3, 13200);
            var posit13248 = posit48 + posit13200;
            posit13248.PositBits.ShouldBe(new Posit(_environment_16_3, 13248).PositBits);

            var posit1_32_2 = new Posit(_environment_32_2, 1);
            var posit2_32_2 = posit1_32_2 + posit1_32_2;
            posit2_32_2.PositBits.ShouldBe(new Posit(_environment_32_2, 2).PositBits);

            var otherPosit13248 = posit13200 + posit48;
            otherPosit13248.PositBits.ShouldBe(new Posit(_environment_16_3, 13248).PositBits);
        }

        [Fact]
        public void AdditionIsCorrectForPositives()
        {
            var posit1 = new Posit(_environment_32_3, 1);

            for (var i = 1; i < 10000; i++)
            {
                posit1 += 1;
            }

            posit1.PositBits.ShouldBe(new Posit(_environment_32_3, 10000).PositBits);

            var posit1_32_2 = new Posit(_environment_32_2, 1);

            for (var i = 1; i < 10000; i++)
            {
                posit1_32_2 += 1;
            }

            posit1_32_2.PositBits.ShouldBe(new Posit(_environment_32_2, 10000).PositBits);
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

            posit1.PositBits.ShouldBe(new Posit(_environment_16_3, 0).PositBits);

            var positA = new Posit(_environment_32_3, 1);
            Console.WriteLine((int)positA);
            var positB = positA;
            Console.WriteLine((int)positB);

            for (var i = 1; i < 100000; i++)
            {
                positA += positB;
            }

            var result = (int)positA;
            Assert.AreEqual(result, 100000);
        }
    }
}
