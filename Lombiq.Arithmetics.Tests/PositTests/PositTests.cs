using Shouldly;
using System;
using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
    public class PositTests
    {
        private readonly PositEnvironment _environment61;
        private readonly PositEnvironment _environment62;
        private readonly PositEnvironment _environment63;
        private readonly PositEnvironment _environment82;
        private readonly PositEnvironment _environment122;
        private readonly PositEnvironment _environment163;
        private readonly PositEnvironment _environment323;
        private readonly PositEnvironment _environment322;

        public PositTests()
        {
            _environment61 = new PositEnvironment(6, 1);
            _environment62 = new PositEnvironment(6, 2);
            _environment63 = new PositEnvironment(6, 3);
            _environment82 = new PositEnvironment(8, 2);
            _environment122 = new PositEnvironment(12, 2);
            _environment163 = new PositEnvironment(16, 3);
            _environment323 = new PositEnvironment(32, 3);
            _environment322 = new PositEnvironment(32, 2);
        }

        [Fact]
        public void EncodeRegimeBitsIsCorrect()
        {
            new Posit(_environment82).EncodeRegimeBits(0).ShouldBe(new BitMask(0x_40, _environment82.Size));

            new Posit(_environment63).EncodeRegimeBits(-3).ShouldBe(new BitMask(0x_02, _environment63.Size));

            new Posit(_environment63).EncodeRegimeBits(3).ShouldBe(new BitMask(0x_1E, _environment63.Size));

            new Posit(_environment63).EncodeRegimeBits(2).ShouldBe(new BitMask(0x_1C, _environment63.Size));

            new Posit(_environment82).EncodeRegimeBits(1).ShouldBe(new BitMask(0x_60, _environment63.Size));

            new Posit(_environment82).EncodeRegimeBits(3).ShouldBe(new BitMask(0x_78, _environment82.Size));

            new Posit(_environment82).EncodeRegimeBits(6).ShouldBe(new BitMask(0x_7F, _environment82.Size));
        }

        [Fact]
        public void PositIsCorrectlyConstructedFromUint()
        {
            new Posit(_environment63, 0U).PositBits.ShouldBe(new BitMask(0x_00, _environment63.Size));

            new Posit(_environment63, 2).PositBits.ShouldBe(new BitMask(17, _environment63.Size));

            new Posit(_environment63, 8U).PositBits.ShouldBe(new BitMask(0x_13, _environment63.Size));

            new Posit(_environment63, 16_384U).PositBits.ShouldBe(new BitMask(0x_1B, _environment63.Size));

            new Posit(_environment63, 1_048_576U).PositBits.ShouldBe(new BitMask(0x_1D, _environment63.Size));

            new Posit(_environment82, 13U).PositBits.ShouldBe(new BitMask(0x_5D, _environment82.Size));

            new Posit(_environment322, 17U).PositBits.ShouldBe(new BitMask(0x_6040_0000, _environment322.Size));

            new Posit(_environment122, 172U).PositBits.ShouldBe(new BitMask(0x_6D6, _environment122.Size));

            new Posit(_environment122, 173U).PositBits.ShouldBe(new BitMask(0x_6D6, _environment122.Size));

            new Posit(_environment163, 48U).PositBits.ShouldBe(new BitMask(22_016, _environment163.Size));

            new Posit(_environment163, 13_200U).PositBits.ShouldBe(new BitMask(27_449, _environment163.Size));

            new Posit(_environment163, 500U).PositBits.ShouldBe(new BitMask(25_064, _environment163.Size));

            new Posit(_environment323, 1U).PositBits.ShouldBe(new BitMask(0x_4000_0000, _environment323.Size));

            // examples of Posit rounding
            new Posit(_environment82, 90U).PositBits.ShouldBe(new BitMask(0x_6A, _environment122.Size));
            new Posit(_environment82, 82U).PositBits.ShouldBe(new BitMask(0x_69, _environment122.Size));

            // Numbers out of range don't get rounded up infinity. They get rounded to the biggest representable
            // finite value (MaxValue).
            new Posit(_environment61, 500U).PositBits.ShouldBe(_environment61.MaxValueBitMask);
        }

        [Fact]
        public void PositIsCorrectlyConstructedFromInt()
        {
            new Posit(_environment63, 8).PositBits.ShouldBe(new BitMask(0x_13, _environment63.Size));

            new Posit(_environment63, 16_384).PositBits.ShouldBe(new BitMask(0x_1B, _environment63.Size));

            new Posit(_environment82, 13).PositBits.ShouldBe(new BitMask(0x_5D, _environment82.Size));

            new Posit(_environment63, -8).PositBits.ShouldBe(new BitMask(0x_2D, _environment63.Size));

            new Posit(_environment82, -13).PositBits.ShouldBe(new BitMask(0x_A3, _environment82.Size));

            new Posit(_environment323, -1).PositBits.ShouldBe(new BitMask(0x_C000_0000, _environment323.Size));

            new Posit(_environment63, -16_384).PositBits.ShouldBe(new BitMask(0x_25, _environment63.Size));

            new Posit(_environment163, -500).PositBits.ShouldBe(new BitMask(40_472, _environment163.Size));
        }

        [Fact]
        public void PositToIntIsCorrect()
        {
            var posit8 = new Posit(_environment63, 8);
            Assert.AreEqual((int)posit8, 8);

            var posit16384 = new Posit(_environment63, 16_384);
            Assert.AreEqual((int)posit16384, 16_384);

            var posit1323 = new Posit(_environment323, 1);
            Assert.AreEqual((int)posit1323, 1);
        }

        [Fact]
        public void ExponentSizeIsCorrect()
        {
            var posit16384 = new Posit(_environment63, 16_384);
            Assert.AreEqual(posit16384.ExponentSize(), 2);

            var posit2 = new Posit(_environment63, 2);
            Assert.AreEqual(posit2.ExponentSize(), 3);

            var posit3 = new Posit(_environment61, 3);
            Assert.AreEqual(posit3.ExponentSize(), 1);

            var posit362 = new Posit(_environment62, 3);
            Assert.AreEqual(posit362.ExponentSize(), 2);

            var positNegative13 = new Posit(_environment82, -13);
            Assert.AreEqual(positNegative13.ExponentSize(), 2);
        }

        [Fact]
        public void GetExponentValueIsCorrect()
        {
            var posit16384 = new Posit(_environment63, 16_384);
            Assert.AreEqual(posit16384.GetExponentValue(), 6);

            var posit2 = new Posit(_environment63, 2);
            Assert.AreEqual(posit2.GetExponentValue(), 1);

            var posit3 = new Posit(_environment61, 3);
            Assert.AreEqual(posit3.GetExponentValue(), 1);

            var posit362 = new Posit(_environment62, 3);
            Assert.AreEqual(posit362.GetExponentValue(), 1);

            var positNegative13 = new Posit(_environment82, -13);
            Assert.AreEqual(positNegative13.GetExponentValue(), 3);

            var posit13248 = new Posit(_environment163, 13_248);
            Assert.AreEqual(posit13248.GetExponentValue(), 5);
        }

        [Fact]
        public void FractionSizeIsCorrect()
        {
            var posit16384 = new Posit(_environment63, 16_384);
            Assert.AreEqual(posit16384.FractionSize(), 0);

            var posit0 = new Posit(_environment63, 0);
            Assert.AreEqual(posit0.FractionSize(), 0);

            var posit2 = new Posit(_environment63, 2);
            Assert.AreEqual(posit2.FractionSize(), 0);

            var posit3 = new Posit(_environment61, 3);
            Assert.AreEqual(posit3.FractionSize(), 2);

            var positNegative13 = new Posit(_environment82, -13);
            Assert.AreEqual(positNegative13.FractionSize(), 3);

            var posit13248 = new Posit(_environment163, 13_248);
            Assert.AreEqual(posit13248.FractionSize(), 9);
        }

        [Fact]
        public void FractionWithHiddenBitIsCorrect()
        {
            var posit16384 = new Posit(_environment63, 16_384);
            posit16384.FractionWithHiddenBit().ShouldBe(new BitMask(1, _environment63.Size));

            var posit0 = new Posit(_environment61, 0);
            posit0.FractionWithHiddenBit().ShouldBe(new BitMask(1, _environment61.Size));

            var posit3 = new Posit(_environment61, 3);
            posit3.FractionWithHiddenBit().ShouldBe(new BitMask(6, _environment61.Size));

            var positNegative13 = new Posit(_environment82, -13);
            positNegative13.FractionWithHiddenBit().ShouldBe(new BitMask(0x_0D, _environment61.Size));
        }

        [Fact]
        public void GetRegimeKValueIsCorrect()
        {
            new Posit(_environment63, 8).GetRegimeKValue().ShouldBe(0);

            new Posit(_environment63, 16_384).GetRegimeKValue().ShouldBe(1);

            new Posit(_environment63, 0).GetRegimeKValue().ShouldBe(-5);

            new Posit(_environment82, 13).GetRegimeKValue().ShouldBe(0);

            new Posit(_environment63, -8).GetRegimeKValue().ShouldBe(0);

            new Posit(_environment82, -13).GetRegimeKValue().ShouldBe(0);

            new Posit(_environment63, -16_384).GetRegimeKValue().ShouldBe(1);
        }

        [Fact]
        public void CalculateScaleFactorIsCorrect()
        {
            new Posit(_environment163, 13_200).CalculateScaleFactor().ShouldBe(13);
            new Posit(_environment163, 48).CalculateScaleFactor().ShouldBe(5);
            new Posit(_environment163, 13_248).CalculateScaleFactor().ShouldBe(13);
            new Posit(_environment163, 1).CalculateScaleFactor().ShouldBe(0);
            new Posit(_environment163, 2).CalculateScaleFactor().ShouldBe(1);
        }

        [Fact]
        public void AdditionIsCorrect()
        {
            var posit0 = new Posit(_environment63, 0);
            var posit = posit0 + 1;
            posit.PositBits.ShouldBe(new Posit(_environment63, 1).PositBits);

            var positNegative1 = new Posit(_environment63, -1);
            var positNegative2 = positNegative1 + positNegative1;

            positNegative2.PositBits.ShouldBe(new Posit(_environment63, -2).PositBits);

            positNegative1 -= 1;
            positNegative1.PositBits.ShouldBe(new Posit(_environment63, -2).PositBits);

            positNegative2 += 1;
            positNegative2.PositBits.ShouldBe(new Posit(_environment63, -1).PositBits);

            var posit1 = new Posit(_environment63, 1);
            var posit2 = posit1 + 1;
            posit2.PositBits.ShouldBe(new Posit(_environment63, 2).PositBits);

            var isPosit0 = posit1 - 1;
            isPosit0.PositBits.ShouldBe(posit0.PositBits);

            var posit3 = new Posit(_environment62, 3);
            var posit6 = posit3 + posit3;
            posit6.PositBits.ShouldBe(new Posit(_environment62, 6).PositBits);

            var posit1163 = new Posit(_environment163, 1);
            var posit2163 = new Posit(_environment163, 2);
            var posit3163 = posit2163 + posit1163;
            posit3163.PositBits.ShouldBe(new Posit(_environment163, 3).PositBits);

            var posit4163 = new Posit(_environment163, 3) + posit1163;
            posit4163.PositBits.ShouldBe(new Posit(_environment163, 4).PositBits);

            var posit48 = new Posit(_environment163, 48);
            var posit13200 = new Posit(_environment163, 13_200);
            var posit13248 = posit48 + posit13200;
            posit13248.PositBits.ShouldBe(new Posit(_environment163, 13_248).PositBits);

            var posit1322 = new Posit(_environment322, 1);
            var posit2322 = posit1322 + posit1322;
            posit2322.PositBits.ShouldBe(new Posit(_environment322, 2).PositBits);

            var otherPosit13248 = posit13200 + posit48;
            otherPosit13248.PositBits.ShouldBe(new Posit(_environment163, 13_248).PositBits);
        }

        [Fact]
        public void AdditionIsCorrectForPositives()
        {
            var posit1 = new Posit(_environment323, 1);

            for (var i = 1; i < 10_000; i++)
            {
                posit1 += 1;
            }

            posit1.PositBits.ShouldBe(new Posit(_environment323, 10_000).PositBits);

            var posit1322 = new Posit(_environment322, 1);

            for (var i = 1; i < 10_000; i++)
            {
                posit1322 += 1;
            }

            posit1322.PositBits.ShouldBe(new Posit(_environment322, 10_000).PositBits);
        }

        [Fact]
        public void AdditionIsCorrectForNegatives()
        {
            var posit1 = new Posit(_environment163, -500);

            for (var i = 1; i <= 1_000; i++)
            {
                posit1 += 1;
            }

            for (var j = 1; j <= 500; j++)
            {
                posit1 -= 1;
            }

            posit1.PositBits.ShouldBe(new Posit(_environment163, 0).PositBits);

            var positA = new Posit(_environment323, 1);
            Console.WriteLine((int)positA);
            var positB = positA;
            Console.WriteLine((int)positB);

            for (var i = 1; i < 100_000; i++)
            {
                positA += positB;
            }

            var result = (int)positA;
            Assert.AreEqual(result, 100_000);
        }
    }
}
