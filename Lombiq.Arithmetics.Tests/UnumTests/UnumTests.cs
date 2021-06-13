using System.Diagnostics.CodeAnalysis;
using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
    public class UnumTests
    {
        private readonly UnumEnvironment _warlpiriEnvironment;
        private readonly UnumEnvironment _environment22;
        private readonly UnumEnvironment _environment23;
        private readonly UnumEnvironment _environment24;
        private readonly UnumEnvironment _environment32;
        private readonly UnumEnvironment _environment34;
        private readonly UnumEnvironment _environment35;
        private readonly UnumEnvironment _environment43;
        private readonly UnumEnvironment _environment48;

        public UnumTests()
        {
            _warlpiriEnvironment = UnumEnvironment.FromStandardEnvironment(StandardEnvironment.Warlpiri);
            _environment22 = new UnumEnvironment(2, 2);
            _environment23 = new UnumEnvironment(2, 3);
            _environment24 = new UnumEnvironment(2, 4);
            _environment32 = new UnumEnvironment(3, 2);
            _environment34 = new UnumEnvironment(3, 4);
            _environment35 = new UnumEnvironment(3, 5);
            _environment43 = new UnumEnvironment(4, 3);
            _environment48 = new UnumEnvironment(4, 8);
        }

        [Fact]
        [SuppressMessage(
            "Major Bug",
            "S1764:Identical expressions should not be used on both sides of a binary operator",
            Justification = "It is what we are testing.")]
        public void WarlpiriUnumValuesAndCalculationsAreCorrect()
        {
            var unumNegative2 = new Unum(_warlpiriEnvironment, -2);
            Assert.AreEqual(-2, (int)unumNegative2);

            var unumNegative1 = new Unum(_warlpiriEnvironment, -1);
            Assert.AreEqual(-1, (int)unumNegative1);

            var unumNegative0 = new Unum(_warlpiriEnvironment, new BitMask(new uint[] { 8 }, _warlpiriEnvironment.Size));
            Assert.AreEqual(0, (int)unumNegative0);

            var unum0 = new Unum(_warlpiriEnvironment, 0);
            Assert.AreEqual(0, (int)unum0);

            var unum1 = new Unum(_warlpiriEnvironment, 1);
            Assert.AreEqual(1, (int)unum1);

            var unum2 = new Unum(_warlpiriEnvironment, 2);
            Assert.AreEqual(2, (int)unum2);

            Assert.AreEqual(unumNegative0, unumNegative2 + unum2);

            Assert.AreEqual(unumNegative0, unumNegative1 + unum1);

            Assert.AreEqual(unum0, unum2 - unum2);

            Assert.AreEqual(unum0, unum1 - unum1);

            Assert.AreEqual(unum2, unum1 + unum1);

            Assert.AreEqual(unumNegative2, unumNegative1 + unumNegative1);

            Assert.AreEqual(unumNegative1, unumNegative2 + unum1);

            Assert.AreEqual(unum1, unum2 + unumNegative1);

            Assert.AreEqual(unumNegative0, unumNegative1 - unumNegative1);

            Assert.AreEqual(unumNegative0, unumNegative2 - unumNegative2);

            Assert.AreEqual(unum1, unumNegative1 - unumNegative2);

            Assert.AreEqual(unum1, unum0 - unumNegative1);

            Assert.AreEqual(unum1, unumNegative0 - unumNegative1);

            Assert.AreEqual(unum2, unum0 - unumNegative2);

            Assert.AreEqual(unum2, unumNegative0 - unumNegative2);
        }

        [Fact]
        public void UnumIsCorrectlyConstructedFromUintArray()
        {
            var unum0 = new Unum(_environment48, new uint[] { 0 });
            Assert.AreEqual(unum0.IsZero(), true);

            var unumMinus1 = new Unum(_environment48, new uint[] { 1 }, true);
            var bitMaskMinus1 = new BitMask(new uint[] { 0x_2000, 0, 0, 0, 0, 0, 0, 0, 0x_2000_0000 }, _environment48.Size);
            Assert.AreEqual(unumMinus1.UnumBits, bitMaskMinus1);

            var unum10 = new Unum(_environment22, new uint[] { 10 });
            var bitMask10 = new BitMask(new uint[] { 0x_329 }, _environment22.Size);
            Assert.AreEqual(unum10.UnumBits, bitMask10);

            var unum500000 = new Unum(_environment48, new uint[] { 500_000 }); // 0xC7A1250C9
            var bitMask500000 = new BitMask(new[] { 0x_C7A1_250C }, _environment48.Size);
            Assert.AreEqual(unum500000.UnumBits, bitMask500000);

            var unumBig = new Unum(_environment48, new uint[] { 594_967_295 });
            var bitMaskBig = new BitMask(new uint[] { 0x_CF5F_E51C, 0x_F06E }, _environment48.Size);
            Assert.AreEqual(unumBig.UnumBits, bitMaskBig);

            var minValue = new uint[8];
            for (var i = 0; i < 8; i++) minValue[i] = uint.MaxValue;
            minValue[7] >>= 1;
            var unumMin = new Unum(_environment48, minValue, true);  // This is negative.
            var bitMaskMinValue = new BitMask(
                new uint[]
                {
                    0x_FFFF_E8FD, 0x_FFFF_FFFF, 0x_FFFF_FFFF, 0x_FFFF_FFFF, 0x_FFFF_FFFF, 0x_FFFF_FFFF,
                    0x_FFFF_FFFF, 0x_FFFF_FFFF, 0x_200F_EFFF,
                },
                _environment48.Size);
            Assert.AreEqual(unumMin.UnumBits, bitMaskMinValue);

            var maxValue = new uint[8];
            for (int i = 0; i < 8; i++) maxValue[i] = uint.MaxValue;
            maxValue[7] >>= 1;

            var bitMaskMaxValue = new BitMask(
                new uint[]
                {
                    0x_FFFF_E8FD, 0x_FFFF_FFFF, 0x_FFFF_FFFF, 0x_FFFF_FFFF, 0x_FFFF_FFFF, 0x_FFFF_FFFF,
                    0x_FFFF_FFFF, 0x_FFFF_FFFF, 0x_000F_EFFF,
                },
                _environment48.Size);
            var unumMax = new Unum(_environment48, maxValue);

            Assert.AreEqual(unumMax.IsPositive(), true);
            Assert.AreEqual(unumMax.Size, _environment48.Size);
            Assert.AreEqual(unumMax.FractionSizeWithHiddenBit(), 255);
            Assert.AreEqual(unumMax.ExponentValueWithBias(), 254);
            Assert.AreEqual(unumMax.FractionWithHiddenBit(), new BitMask(maxValue, _environment48.Size));
            Assert.AreEqual(unumMax.UnumBits, bitMaskMaxValue);

            var tooBigUnumWarlpiri = new Unum(_warlpiriEnvironment, 3);
            var tooBigBitMaskWarlpiri = _warlpiriEnvironment.LargestPositive | _warlpiriEnvironment.UncertaintyBitMask;
            Assert.AreEqual(tooBigUnumWarlpiri.UnumBits, tooBigBitMaskWarlpiri);

            var tooBigNegativeUnumWarlpiri = new Unum(_warlpiriEnvironment, -3);
            var tooBigNegativeBitMaskWarlpiri = _warlpiriEnvironment.LargestNegative | _warlpiriEnvironment.UncertaintyBitMask;
            Assert.AreEqual(tooBigNegativeUnumWarlpiri.UnumBits, tooBigNegativeBitMaskWarlpiri);

            var maxValue22 = new Unum(_environment22, 480);
            var maxBitMask22 = new BitMask(new uint[] { 0x_FEE }, _environment22.Size);
            Assert.AreEqual(maxValue22.UnumBits, maxBitMask22);

            var minValue22 = new Unum(_environment22, -480);
            var bitMaskMinValue22 = new BitMask(new uint[] { 0x_2FEE }, _environment22.Size);
            Assert.AreEqual(minValue22.UnumBits, bitMaskMinValue22);

            var tooBigUnum22 = new Unum(_environment22, 481);
            var tooBigBitMask22 = _environment22.LargestPositive | _environment22.UncertaintyBitMask;
            Assert.AreEqual(tooBigUnum22.UnumBits, tooBigBitMask22);

            var tooBigNegativeUnum22 = new Unum(_environment22, -481);
            var tooBigNegativeBitMask22 = _environment22.LargestNegative | _environment22.UncertaintyBitMask;
            Assert.AreEqual(tooBigNegativeUnum22.UnumBits, tooBigNegativeBitMask22);

            var maxValue23 = new Unum(_environment23, 510);
            var maxBitMask23 = new BitMask(new uint[] { 0x_0001_FFDE }, _environment23.Size);
            Assert.AreEqual(maxValue23.UnumBits, maxBitMask23);

            var minValue23 = new Unum(_environment23, -510);
            var bitMaskMinValue23 = new BitMask(new uint[] { 0x_0005_FFDE }, _environment23.Size);
            Assert.AreEqual(minValue23.UnumBits, bitMaskMinValue23);

            var tooBigUnum23 = new Unum(_environment23, 511);
            var tooBigBitMask23 = _environment23.LargestPositive | _environment23.UncertaintyBitMask;
            Assert.AreEqual(tooBigUnum23.UnumBits, tooBigBitMask23);

            var tooBigNegativeUnum23 = new Unum(_environment23, -511);
            var tooBigNegativeBitMask23 = _environment23.LargestNegative | _environment23.UncertaintyBitMask;
            Assert.AreEqual(tooBigNegativeUnum23.UnumBits, tooBigNegativeBitMask23);

            // Testing in an environment where the biggest representable value isn't an integer.
            var maxValue24 = new Unum(_environment24, 511);
            var maxBitMask24 = new BitMask(new uint[] { 0x_0007_FFB7 }, _environment24.Size);
            Assert.AreEqual(maxValue24.UnumBits, maxBitMask24);

            var minValue24 = new Unum(_environment24, -511);
            var bitMaskMinValue24 = new BitMask(new uint[] { 0x_0807_FFB7 }, _environment24.Size);
            Assert.AreEqual(minValue24.UnumBits, bitMaskMinValue24);

            var tooBigUnum24 = new Unum(_environment24, 512);
            var tooBigBitMask24 = _environment24.LargestPositive | _environment24.UncertaintyBitMask;
            Assert.AreEqual(tooBigUnum24.UnumBits, tooBigBitMask24);

            var tooBigNegativeUnum24 = new Unum(_environment24, -512);
            var tooBigNegativeBitMask24 = _environment24.LargestNegative | _environment24.UncertaintyBitMask;
            Assert.AreEqual(tooBigNegativeUnum24.UnumBits, tooBigNegativeBitMask24);
        }

        [Fact]
        public void FractionToUintArrayIsCorrect()
        {
            var unumZero = new Unum(_environment48, new uint[] { 0 });
            Assert.AreEqual(unumZero.FractionToUintArray(), new uint[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 });

            var unum1 = new Unum(_environment48, new uint[] { 1 });
            Assert.AreEqual(unum1.FractionToUintArray(), new uint[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 });

            var unum500000 = new Unum(_environment48, new uint[] { 500_000 }); // 0xC7A1250C
            Assert.AreEqual(unum500000.FractionToUintArray(), new uint[] { 500_000, 0, 0, 0, 0, 0, 0, 0, 0 });

            var unumBig = new Unum(_environment48, new uint[] { 594_967_295 });
            Assert.AreEqual(unumBig.FractionToUintArray(), new uint[] { 594_967_295, 0, 0, 0, 0, 0, 0, 0, 0 });

            var maxValue = new uint[8];
            for (var i = 0; i < 8; i++)
            {
                maxValue[i] = uint.MaxValue;
            }

            maxValue[7] >>= 1;
            var unumMax = new Unum(_environment48, maxValue);
            Assert.AreEqual(unumMax.FractionToUintArray(), new uint[]
            {
                0x_FFFF_FFFF,
                0x_FFFF_FFFF,
                0x_FFFF_FFFF,
                0x_FFFF_FFFF,
                0x_FFFF_FFFF,
                0x_FFFF_FFFF,
                0x_FFFF_FFFF,
                0x_7FFF_FFFF,
                0,
            });
        }

        [Fact]
        public void UnumIsCorrectlyConstructedFromInt()
        {
            var unum0 = new Unum(_environment34, 0);
            var bitMask0 = new BitMask(new uint[] { 0 }, _environment34.Size);
            Assert.AreEqual(unum0.UnumBits, bitMask0);

            var unum1 = new Unum(_environment34, 1);
            var bitMask1 = new BitMask(new uint[] { 0x_100 }, _environment34.Size);
            Assert.AreEqual(unum1.UnumBits, bitMask1);

            var unum30 = new Unum(_environment34, 30);
            var bitMask30 = new BitMask(new uint[] { 0x_3F22 }, _environment34.Size);
            Assert.AreEqual(unum30.UnumBits, bitMask30);

            var unum1000 = new Unum(_environment34, 1_000);
            var bitMask1000 = new BitMask(new uint[] { 0x_0006_3D45 }, _environment34.Size);
            Assert.AreEqual(unum1000.UnumBits, bitMask1000);

            var unum5000 = new Unum(_environment34, 5_000);
            var bitMask5000 = new BitMask(new uint[] { 0x_0036_7148 }, _environment34.Size);
            Assert.AreEqual(unum5000.UnumBits, bitMask5000);

            var unum6000 = new Unum(_environment34, 6_000);
            var bitMask6000 = new BitMask(new uint[] { 0x_001B_7747 }, _environment34.Size);
            Assert.AreEqual(unum6000.UnumBits, bitMask6000);

            var unumNegative30 = new Unum(_environment34, -30);
            var bitMaskMinus30 = new BitMask(new uint[] { 0x_3F22, 1 }, _environment34.Size);
            Assert.AreEqual(unumNegative30.UnumBits, bitMaskMinus30);

            var unumNegative1000 = new Unum(_environment34, -1_000);
            var bitMaskMinus1000 = new BitMask(new uint[] { 0x_0006_3D45, 1 }, _environment34.Size);
            Assert.AreEqual(unumNegative1000.UnumBits, bitMaskMinus1000);
        }

        [Fact]
        public void UnumIsCorrectlyConstructedFromUInt()
        {
            var unum0 = new Unum(_environment34, 0U);
            var bitMask0 = new BitMask(new uint[] { 0 }, _environment34.Size);
            Assert.AreEqual(bitMask0, unum0.UnumBits);

            var unum1 = new Unum(_environment34, 1U);
            var bitMask1 = new BitMask(new uint[] { 0x_100 }, _environment34.Size);
            Assert.AreEqual(bitMask1, unum1.UnumBits);

            var unum2 = new Unum(_environment34, 2U);
            var bitMask2 = new BitMask(new uint[] { 0x_200 }, _environment34.Size);
            Assert.AreEqual(bitMask2, unum2.UnumBits);

            var unum4 = new Unum(_environment34, 4U);
            var bitMask4 = new BitMask(new uint[] { 0x_610 }, _environment34.Size);
            Assert.AreEqual(bitMask4, unum4.UnumBits);

            var unum8 = new Unum(_environment34, 8U);
            var bitMask8 = new BitMask(new uint[] { 0x_C20 }, _environment34.Size);
            Assert.AreEqual(bitMask8, unum8.UnumBits);

            var unum10 = new Unum(_environment22, 10U);
            var bitMask10 = new BitMask(new uint[] { 0x_329 }, _environment22.Size);
            Assert.AreEqual(bitMask10, unum10.UnumBits);

            var unum13 = new Unum(_environment34, 13U);
            var bitMask13 = new BitMask(new uint[] { 0x_3522 }, _environment34.Size);
            Assert.AreEqual(bitMask13, unum13.UnumBits);

            var unum30 = new Unum(_environment34, 30U);
            var bitMask30 = new BitMask(new uint[] { 0x_3F22 }, _environment34.Size);
            Assert.AreEqual(bitMask30, unum30.UnumBits);

            var unum1000 = new Unum(_environment34, 1_000U);
            var bitMask1000 = new BitMask(new uint[] { 0x_0006_3D45 }, _environment34.Size);
            Assert.AreEqual(bitMask1000, unum1000.UnumBits);

            var unum5000 = new Unum(_environment34, 5_000U);
            var bitMask5000 = new BitMask(new uint[] { 0x_0036_7148 }, _environment34.Size);
            Assert.AreEqual(bitMask5000, unum5000.UnumBits);

            var unum6000 = new Unum(_environment34, 6_000U);
            var bitMask6000 = new BitMask(new uint[] { 0x_001B_7747 }, _environment34.Size);
            Assert.AreEqual(bitMask6000, unum6000.UnumBits);
        }

        [Fact]
        public void IsExactIsCorrect()
        {
            // 0  0000 0000  0000  1 000 00
            var bitMask32Uncertain = new BitMask(new uint[] { 0x_20 }, 19);
            var unum32Uncertain = new Unum(_environment32, bitMask32Uncertain);
            Assert.AreEqual(false, unum32Uncertain.IsExact());

            var bitMask32Certain = new BitMask(19, false);
            var unum32Certain = new Unum(_environment32, bitMask32Certain);
            Assert.AreEqual(true, unum32Certain.IsExact());

            var bitMask34Uncertain = new BitMask(new uint[] { 0x_80, 0 }, 33);
            var unum34Uncertain = new Unum(_environment34, bitMask34Uncertain);
            Assert.AreEqual(false, unum34Uncertain.IsExact());

            var bitMask34Certain = new BitMask(33, false);
            var unum34Certain = new Unum(_environment34, bitMask34Certain);
            Assert.AreEqual(true, unum34Certain.IsExact());
        }

        [Fact]
        public void FractionSizeIsCorrect()
        {
            var bitMask32AllOne = new BitMask(19, true);
            var unum32AllOne = new Unum(_environment32, bitMask32AllOne);
            Assert.AreEqual(4, unum32AllOne.FractionSize());

            var bitMask34AllOne = new BitMask(33, true);
            var unum34AllOne = new Unum(_environment34, bitMask34AllOne);
            Assert.AreEqual(16, unum34AllOne.FractionSize());
        }

        [Fact]
        public void ExponentSizeIsCorrect()
        {
            var bitMask32AllOne = new BitMask(19, true);
            var unum32AllOne = new Unum(_environment32, bitMask32AllOne);
            Assert.AreEqual(8, unum32AllOne.ExponentSize());

            var bitMask34AllOne = new BitMask(33, true);
            var unum34AllOne = new Unum(_environment34, bitMask34AllOne);
            Assert.AreEqual(8, unum34AllOne.ExponentSize());

            var bitMask43AllOne = new BitMask(33, true);
            var unum43AllOne = new Unum(_environment43, bitMask43AllOne);
            Assert.AreEqual(16, unum43AllOne.ExponentSize());
        }

        [Fact]
        public void FractionMaskIsCorrect()
        {
            // 0  0000 0000  1111  0000 00
            var bitMask32AllOne = new BitMask(19, true);
            var unum32AllOne = new Unum(_environment32, bitMask32AllOne);
            var bitMask32FractionMask = new BitMask(new uint[] { 0x_03C0 }, 19);
            Assert.AreEqual(bitMask32FractionMask, unum32AllOne.FractionMask());

            // 0  0000 0000  1111 1111 1111 1111  0000 0000
            var bitMask34AllOne = new BitMask(33, true);
            var unum34AllOne = new Unum(_environment34, bitMask34AllOne);
            var bitMask34FractionMask = new BitMask(new uint[] { 0x_00FF_FF00 }, 33);
            Assert.AreEqual(bitMask34FractionMask, unum34AllOne.FractionMask());
        }

        [Fact]
        public void ExponentMaskIsCorrect()
        {
            // 0  1111 1111  0000  0 000 00
            var bitMask32AllOne = new BitMask(19, true);
            var unum32AllOne = new Unum(_environment32, bitMask32AllOne);
            var bitMask32ExponentMask = new BitMask(new uint[] { 0x_0003_FC00 }, 19);
            Assert.AreEqual(bitMask32ExponentMask, unum32AllOne.ExponentMask());

            // 0  1111 1111  0000 0000 0000 0000  0 000 0000
            var bitMask34AllOne = new BitMask(33, true);
            var unum34AllOne = new Unum(_environment34, bitMask34AllOne);
            var bitMask34ExponentMask = new BitMask(new[] { 0x_FF00_0000 }, 33);
            Assert.AreEqual(bitMask34ExponentMask, unum34AllOne.ExponentMask());
        }

        [Fact]
        public void ExponentValueWithBiasIsCorrect()
        {
            var bitMask1 = new BitMask(new uint[] { 0x_E40 }, 33);
            var unum1 = new Unum(_environment34, bitMask1);
            Assert.AreEqual(unum1.ExponentValueWithBias(), -8);

            var unumZero = new Unum(_environment34, 0);
            Assert.AreEqual(unumZero.ExponentValueWithBias(), 1);
        }

        [Fact]
        public void FractionWithHiddenBitIsCorrect()
        {
            var bitMask1 = new BitMask(new uint[] { 0x_E40 }, 33);
            var unum1 = new Unum(_environment34, bitMask1);
            Assert.AreEqual(new BitMask(new uint[] { 2 }, 33), unum1.FractionWithHiddenBit());

            var bitMask2 = new BitMask(new uint[] { 0x_3F22 }, 33);
            var unum2 = new Unum(_environment34, bitMask2);
            Assert.AreEqual(new BitMask(new uint[] { 0x_000F }, 33), unum2.FractionWithHiddenBit());

            var bitMask3 = new BitMask(new uint[] { 0x_007E_012B }, 33);
            var unum3 = new Unum(_environment34, bitMask3);
            Assert.AreEqual(new BitMask(new uint[] { 0x_1E01 }, 33), unum3.FractionWithHiddenBit());
        }

        [Fact]
        public void AddExactUnumsIsCorrect()
        {
            // First example from The End of Error p. 117.
            var bitMask1 = new BitMask(new uint[] { 0x_E40 }, 33);
            var bitMask2 = new BitMask(new uint[] { 0x_3F22 }, 33);
            var unumFirst = new Unum(_environment34, bitMask1);
            var unumSecond = new Unum(_environment34, bitMask2);
            var bitMaskSum = new BitMask(new uint[] { 0x_007E_012B }, 33);
            var unumSum1 = Unum.AddExactUnums(unumFirst, unumSecond);
            Assert.AreEqual(unumSum1.UnumBits, bitMaskSum);

            // Addition should be commutative.
            var unumSum2 = Unum.AddExactUnums(unumSecond, unumFirst);
            Assert.AreEqual(unumSum1.UnumBits, unumSum2.UnumBits);

            var unum0 = new Unum(_environment34, 0);
            var unum1 = new Unum(_environment34, 1);
            var unum0Plus1 = unum0 + unum1;
            var unum31 = new Unum(_environment34, 30) + unum1;
            var unum0PlusUnum1 = Unum.AddExactUnums(unum0, unum1);
            Assert.AreEqual(unum31.UnumBits, new Unum(_environment34, 31).UnumBits);
            Assert.AreEqual(unum1.UnumBits, unum0Plus1.UnumBits);
            Assert.AreEqual(unum1.UnumBits, unum0PlusUnum1.UnumBits);

            // Case of inexact result, second example from The End or Error, p. 117.
            var bitMask4 = new BitMask(new uint[] { 0x_18F4_00CF }, 33); // 1000.0078125
            var unum1000 = new Unum(_environment34, 1_000);
            var unum6 = Unum.AddExactUnums(unum1000, unumFirst); // 1/256
            Assert.AreEqual(unum6.UnumBits, bitMask4);

            var unum5000 = new Unum(_environment34, 5_000);
            var unum6000 = new Unum(_environment34, 6_000);
            Assert.AreEqual(Unum.AddExactUnums(unum5000, unum1000).UnumBits, unum6000.UnumBits);

            var unumNegativeThirty = new Unum(_environment34, -30);
            var unum30 = new Unum(_environment34, 30);
            Assert.AreEqual(Unum.AddExactUnums(unum30, unumNegativeThirty).UnumBits, unum0.UnumBits);
        }

        [Fact]
        public void AdditionIsCorrectForIntegers()
        {
            var result = new Unum(_environment35, 0);
            var count = 100;

            for (int i = 1; i <= count; i++) result += new Unum(_environment35, i * 1_000);
            for (int i = 1; i <= count; i++) result -= new Unum(_environment35, i * 1_000);

            Assert.AreEqual(result.UnumBits, new Unum(_environment35, 0).UnumBits);
        }

        [Fact]
        public void SubtractExactUnumsIsCorrect()
        {
            var bitMask1 = new BitMask(new uint[] { 0x_007E_012B }, 33); // 30.00390625
            var bitMask2 = new BitMask(new uint[] { 0x_0E40 }, 33);    // 0.00390625
            var bitMask3 = new BitMask(new uint[] { 0x_3F22 }, 33);   // 30

            var unum1 = new Unum(_environment34, bitMask1);
            var unum2 = new Unum(_environment34, bitMask2);
            var unum3 = Unum.SubtractExactUnums(unum1, unum2);
            Assert.AreEqual(unum3.UnumBits, bitMask3);

            var unum5000 = new Unum(_environment34, 5_000);
            var unum6000 = new Unum(_environment34, 6_000);
            var unum1000 = new Unum(_environment34, 1_000);

            var unumRes = Unum.SubtractExactUnums(unum6000, unum5000);
            Assert.AreEqual(unumRes.UnumBits, unum1000.UnumBits);

            var unum30 = new Unum(_environment34, 30);
            var unumZero = new Unum(_environment34, 0);
            Assert.AreEqual(Unum.SubtractExactUnums(unum30, unum30).UnumBits, unumZero.UnumBits);
        }

        [Fact]
        public void IntToUnumIsCorrect()
        {
            var unum0 = new Unum(_environment34, 0);
            var bitMask0 = new BitMask(new uint[] { 0 }, 33);
            Assert.AreEqual(unum0.UnumBits, bitMask0);

            var unum1 = new Unum(_environment34, 1);
            Assert.AreEqual(unum1.UnumBits, new BitMask(new uint[] { 0x_100 }, 33));

            var unum2 = new Unum(_environment34, 2);
            Assert.AreEqual(unum2.UnumBits, new BitMask(new uint[] { 0x_200 }, 33));

            var unum30 = new Unum(_environment34, 30);
            var bitMask30 = new BitMask(new uint[] { 0x_3F22 }, 33);
            Assert.AreEqual(unum30.UnumBits, bitMask30);

            var unum1000 = new Unum(_environment34, 1_000);
            var bitMask1000 = new BitMask(new uint[] { 0x_0006_3D45 }, 33);
            Assert.AreEqual(unum1000.UnumBits, bitMask1000);

            var unum5000 = new Unum(_environment34, 5_000);
            var bitMask5000 = new BitMask(new uint[] { 0x_0036_7148 }, 33);
            Assert.AreEqual(unum5000.UnumBits, bitMask5000);

            var bitMask6000 = new BitMask(new uint[] { 0x_001B_7747 }, 33);
            var unum6000 = new Unum(_environment34, 6_000);
            Assert.AreEqual(unum6000.UnumBits, bitMask6000);

            var unumNegative30 = new Unum(_environment34, -30);
            var bitMaskNegative30 = new BitMask(new uint[] { 0x_3F22, 1 }, 33);
            Assert.AreEqual(unumNegative30.UnumBits, bitMaskNegative30);

            var unumNegative1000 = new Unum(_environment34, -1_000);
            var bitMaskNegative1000 = new BitMask(new uint[] { 0x_0006_3D45, 1 }, 33);
            Assert.AreEqual(unumNegative1000.UnumBits, bitMaskNegative1000);
        }

        [Fact]
        public void UnumToUintIsCorrect()
        {
            var unum1 = new Unum(_environment34, 1);
            var number1 = (uint)unum1;
            Assert.AreEqual(number1, 1);

            var unum2 = new Unum(_environment34, 2);
            var number2 = (uint)unum2;
            Assert.AreEqual(number2, 2);

            var unum30 = new Unum(_environment34, 30);
            var number30 = (uint)unum30;
            Assert.AreEqual(number30, 30);

            var unum1000 = new Unum(_environment34, 1_000);
            var number1000 = (uint)unum1000;
            Assert.AreEqual(number1000, 1_000);

            var unum5000 = new Unum(_environment34, 5_000);
            var number5000 = (uint)unum5000;
            Assert.AreEqual(number5000, 5_000);

            var unum6000 = new Unum(_environment34, 6_000);
            var number6000 = (uint)unum6000;
            Assert.AreEqual(number6000, 6_000);
        }

        [Fact]
        public void UnumToIntIsCorrect()
        {
            var unum1 = new Unum(_environment34, 1);
            var one = (int)unum1;
            Assert.AreEqual(one, 1);

            var unum30 = new Unum(_environment34, 30);
            var number30 = (int)unum30;
            Assert.AreEqual(number30, 30);

            var unum1000 = new Unum(_environment34, 1_000);
            var number1000 = (int)unum1000;
            Assert.AreEqual(number1000, 1_000);

            var unumNegative30 = new Unum(_environment34, -30);
            var numberNegative30 = (int)unumNegative30;
            Assert.AreEqual(numberNegative30, -30);

            var unumNegative1000 = new Unum(_environment34, -1_000);
            var numberNegative1000 = (int)unumNegative1000;
            Assert.AreEqual(numberNegative1000, -1_000);
        }

        [Fact]
        public void UnumToFloatIsCorrect()
        {
            var unum30 = new Unum(_environment34, 30);
            var numbe30 = (float)unum30;
            Assert.AreEqual(numbe30, 30F);

            var unum1000 = new Unum(_environment34, 1_000);
            var number1000 = (float)unum1000;
            Assert.AreEqual(number1000, 1_000);

            var unumNegative30 = new Unum(_environment34, -30);
            var numberNegative30 = (float)unumNegative30;
            Assert.AreEqual(numberNegative30, -30);

            var unumNegative1000 = new Unum(_environment34, -1_000);
            var numberNegative1000 = (float)unumNegative1000;
            Assert.AreEqual(numberNegative1000, -1_000);
        }
    }
}
