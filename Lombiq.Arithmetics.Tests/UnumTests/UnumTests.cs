using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
    public class UnumTests
    {
        private readonly UnumEnvironment _warlpiriEnvironment;
        private readonly UnumEnvironment _environment_2_2;
        private readonly UnumEnvironment _environment_2_3;
        private readonly UnumEnvironment _environment_2_4;
        private readonly UnumEnvironment _environment_3_2;
        private readonly UnumEnvironment _environment_3_4;
        private readonly UnumEnvironment _environment_3_5;
        private readonly UnumEnvironment _environment_4_3;
        private readonly UnumEnvironment _environment_4_8;

        public UnumTests()
        {
            _warlpiriEnvironment = UnumEnvironment.FromStandardEnvironment(StandardEnvironment.Warlpiri);
            _environment_2_2 = new UnumEnvironment(2, 2);
            _environment_2_3 = new UnumEnvironment(2, 3);
            _environment_2_4 = new UnumEnvironment(2, 4);
            _environment_3_2 = new UnumEnvironment(3, 2);
            _environment_3_4 = new UnumEnvironment(3, 4);
            _environment_3_5 = new UnumEnvironment(3, 5);
            _environment_4_3 = new UnumEnvironment(4, 3);
            _environment_4_8 = new UnumEnvironment(4, 8);
        }

        [Fact]
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

            var unum0 = new Unum(_environment_4_8, new uint[] { 0 });
            Assert.AreEqual(unum0.IsZero(), true);

            var unumMinus1 = new Unum(_environment_4_8, new uint[] { 1 }, true);
            var bitMaskMinus1 = new BitMask(new uint[] { 0x2000, 0, 0, 0, 0, 0, 0, 0, 0x20000000 }, _environment_4_8.Size);
            Assert.AreEqual(unumMinus1.UnumBits, bitMaskMinus1);

            var unum10 = new Unum(_environment_2_2, new uint[] { 10 });
            var bitMask10 = new BitMask(new uint[] { 0x329 }, _environment_2_2.Size);
            Assert.AreEqual(unum10.UnumBits, bitMask10);

            var unum500000 = new Unum(_environment_4_8, new uint[] { 500000 }); // 0xC7A1250C9
            var bitMask500000 = new BitMask(new uint[] { 0xC7A1250C }, _environment_4_8.Size);
            Assert.AreEqual(unum500000.UnumBits, bitMask500000);

            var unumBig = new Unum(_environment_4_8, new uint[] { 594_967_295 });
            var bitMaskBig = new BitMask(new uint[] { 0xCF5FE51C, 0xF06E }, _environment_4_8.Size);
            Assert.AreEqual(unumBig.UnumBits, bitMaskBig);

            var minValue = new uint[8];
            for (var i = 0; i < 8; i++) minValue[i] = uint.MaxValue;
            minValue[7] >>= 1;
            var unumMin = new Unum(_environment_4_8, minValue, true);  // This is negative.
            var bitMaskMinValue = new BitMask(
                new uint[]
                {
                    0xFFFFE8FD , 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    0xFFFFFFFF , 0xFFFFFFFF,  0x200FEFFF,
                },
                _environment_4_8.Size);
            Assert.AreEqual(unumMin.UnumBits, bitMaskMinValue);

            var maxValue = new uint[8];
            for (int i = 0; i < 8; i++) maxValue[i] = uint.MaxValue;
            maxValue[7] >>= 1;

            var bitMaskMaxValue = new BitMask(
                    new uint[]
                {
                    0xFFFFE8FD , 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    0xFFFFFFFF , 0xFFFFFFFF,  0xFEFFF,
                },
                _environment_4_8.Size);
            var unumMax = new Unum(_environment_4_8, maxValue);

            Assert.AreEqual(unumMax.IsPositive(), true);
            Assert.AreEqual(unumMax.Size, _environment_4_8.Size);
            Assert.AreEqual(unumMax.FractionSizeWithHiddenBit(), 255);
            Assert.AreEqual(unumMax.ExponentValueWithBias(), 254);
            Assert.AreEqual(unumMax.FractionWithHiddenBit(), new BitMask(maxValue, _environment_4_8.Size));
            Assert.AreEqual(unumMax.UnumBits, bitMaskMaxValue);

            var tooBigUnum_warlpiri = new Unum(_warlpiriEnvironment, 3);
            var tooBigBitMask_warlpiri = _warlpiriEnvironment.LargestPositive | _warlpiriEnvironment.UncertaintyBitMask;
            Assert.AreEqual(tooBigUnum_warlpiri.UnumBits, tooBigBitMask_warlpiri);

            var tooBigNegativeUnum_warlpiri = new Unum(_warlpiriEnvironment, -3);
            var tooBigNegativeBitMask_warlpiri = _warlpiriEnvironment.LargestNegative | _warlpiriEnvironment.UncertaintyBitMask;
            Assert.AreEqual(tooBigNegativeUnum_warlpiri.UnumBits, tooBigNegativeBitMask_warlpiri);

            var maxValue_2_2 = new Unum(_environment_2_2, 480);
            var maxBitMask_2_2 = new BitMask(new uint[] { 0xFEE }, _environment_2_2.Size);
            Assert.AreEqual(maxValue_2_2.UnumBits, maxBitMask_2_2);

            var minValue_2_2 = new Unum(_environment_2_2, -480);
            var bitMaskMinValue_2_2 = new BitMask(new uint[] { 0x2FEE }, _environment_2_2.Size);
            Assert.AreEqual(minValue_2_2.UnumBits, bitMaskMinValue_2_2);

            var tooBigUnum_2_2 = new Unum(_environment_2_2, 481);
            var tooBigBitMask_2_2 = _environment_2_2.LargestPositive | _environment_2_2.UncertaintyBitMask;
            Assert.AreEqual(tooBigUnum_2_2.UnumBits, tooBigBitMask_2_2);

            var tooBigNegativeUnum_2_2 = new Unum(_environment_2_2, -481);
            var tooBigNegativeBitMask_2_2 = _environment_2_2.LargestNegative | _environment_2_2.UncertaintyBitMask;
            Assert.AreEqual(tooBigNegativeUnum_2_2.UnumBits, tooBigNegativeBitMask_2_2);

            var maxValue_2_3 = new Unum(_environment_2_3, 510);
            var maxBitMask_2_3 = new BitMask(new uint[] { 0x1FFDE }, _environment_2_3.Size);
            Assert.AreEqual(maxValue_2_3.UnumBits, maxBitMask_2_3);

            var minValue_2_3 = new Unum(_environment_2_3, -510);
            var bitMaskMinValue_2_3 = new BitMask(new uint[] { 0x5FFDE }, _environment_2_3.Size);
            Assert.AreEqual(minValue_2_3.UnumBits, bitMaskMinValue_2_3);

            var tooBigUnum_2_3 = new Unum(_environment_2_3, 511);
            var tooBigBitMask_2_3 = _environment_2_3.LargestPositive | _environment_2_3.UncertaintyBitMask;
            Assert.AreEqual(tooBigUnum_2_3.UnumBits, tooBigBitMask_2_3);

            var tooBigNegativeUnum_2_3 = new Unum(_environment_2_3, -511);
            var tooBigNegativeBitMask_2_3 = _environment_2_3.LargestNegative | _environment_2_3.UncertaintyBitMask;
            Assert.AreEqual(tooBigNegativeUnum_2_3.UnumBits, tooBigNegativeBitMask_2_3);

            // Testing in an environment where the biggest representable value isn't an integer.
            var maxValue_2_4 = new Unum(_environment_2_4, 511);
            var maxBitMask_2_4 = new BitMask(new uint[] { 0x7FFB7 }, _environment_2_4.Size);
            Assert.AreEqual(maxValue_2_4.UnumBits, maxBitMask_2_4);

            var minValue_2_4 = new Unum(_environment_2_4, -511);
            var bitMaskMinValue_2_4 = new BitMask(new uint[] { 0x807FFB7 }, _environment_2_4.Size);
            Assert.AreEqual(minValue_2_4.UnumBits, bitMaskMinValue_2_4);

            var tooBigUnum_2_4 = new Unum(_environment_2_4, 512);
            var tooBigBitMask_2_4 = _environment_2_4.LargestPositive | _environment_2_4.UncertaintyBitMask;
            Assert.AreEqual(tooBigUnum_2_4.UnumBits, tooBigBitMask_2_4);

            var tooBigNegativeUnum_2_4 = new Unum(_environment_2_4, -512);
            var tooBigNegativeBitMask_2_4 = _environment_2_4.LargestNegative | _environment_2_4.UncertaintyBitMask;
            Assert.AreEqual(tooBigNegativeUnum_2_4.UnumBits, tooBigNegativeBitMask_2_4);
        }

        [Fact]
        public void FractionToUintArrayIsCorrect()
        {
            var unumZero = new Unum(_environment_4_8, new uint[] { 0 });
            Assert.AreEqual(unumZero.FractionToUintArray(), new uint[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 });

            var unum1 = new Unum(_environment_4_8, new uint[] { 1 });
            Assert.AreEqual(unum1.FractionToUintArray(), new uint[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 });

            var unum500000 = new Unum(_environment_4_8, new uint[] { 500000 }); //0xC7A1250C
            Assert.AreEqual(unum500000.FractionToUintArray(), new uint[] { 500000, 0, 0, 0, 0, 0, 0, 0, 0 });

            var unumBig = new Unum(_environment_4_8, new uint[] { 594_967_295 });
            Assert.AreEqual(unumBig.FractionToUintArray(), new uint[] { 594_967_295, 0, 0, 0, 0, 0, 0, 0, 0 });

            var maxValue = new uint[8];
            for (var i = 0; i < 8; i++)
            {
                maxValue[i] = uint.MaxValue;
            }
            maxValue[7] >>= 1;
            var unumMax = new Unum(_environment_4_8, maxValue);
            Assert.AreEqual(unumMax.FractionToUintArray(), new uint[]
                { 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0x7FFFFFFF, 0 });
        }

        //[Fact]
        //public void UnumIsCorrectlyConstructedFromFloat()
        //{
        //    var first = new Unum(_metaData_3_4, (float)30.0);
        //    var second = new Unum(_metaData_3_4, (float)9999999999999);
        //    var third = new Unum(_metaData_3_4, (float)1.5);
        //    var five = new Unum(_metaData_3_5, (float)5);
        //    var fourth = new Unum(_metaData_3_5, 0.75F);

        //    var bitMask_1 = new BitMask(new uint[] { 0x3F22 }, _metaData_3_4.Size);
        //    var bitMask_2 = new BitMask(new uint[] { 0x6A2309EF }, _metaData_3_4.Size);
        //    var bitMask_5 = new BitMask(new uint[] { 0x1A21 }, _metaData_3_5.Size);
        //    var bitMask_3 = new BitMask(new uint[] { 0x660 }, _metaData_3_4.Size);
        //    var bitMask_4 = new BitMask(new uint[] { 0xA40 }, _metaData_3_5.Size);

        //    Assert.AreEqual(first.UnumBits, bitMask_1);
        //    Assert.AreEqual(second.UnumBits, bitMask_2);
        //    Assert.AreEqual(five.UnumBits, bitMask_5);
        //    Assert.AreEqual(fourth.UnumBits, bitMask_4);
        //    Assert.AreEqual(third.UnumBits, bitMask_3);

        //}

        [Fact]
        public void UnumIsCorrectlyConstructedFromInt()
        {
            var unum0 = new Unum(_environment_3_4, 0);
            var bitMask0 = new BitMask(new uint[] { 0 }, _environment_3_4.Size);
            Assert.AreEqual(unum0.UnumBits, bitMask0);

            var unum1 = new Unum(_environment_3_4, 1);
            var bitMask1 = new BitMask(new uint[] { 0x100 }, _environment_3_4.Size);
            Assert.AreEqual(unum1.UnumBits, bitMask1);

            var unum30 = new Unum(_environment_3_4, 30);
            var bitMask30 = new BitMask(new uint[] { 0x3F22 }, _environment_3_4.Size);
            Assert.AreEqual(unum30.UnumBits, bitMask30);

            var unum1000 = new Unum(_environment_3_4, 1000);
            var bitMask1000 = new BitMask(new uint[] { 0x63D45 }, _environment_3_4.Size);
            Assert.AreEqual(unum1000.UnumBits, bitMask1000);

            var unum5000 = new Unum(_environment_3_4, 5000);
            var bitMask5000 = new BitMask(new uint[] { 0x367148 }, _environment_3_4.Size);
            Assert.AreEqual(unum5000.UnumBits, bitMask5000);

            var unum6000 = new Unum(_environment_3_4, 6000);
            var bitMask6000 = new BitMask(new uint[] { 0x1B7747 }, _environment_3_4.Size);
            Assert.AreEqual(unum6000.UnumBits, bitMask6000);

            var unumNegative30 = new Unum(_environment_3_4, -30);
            var bitMaskMinus30 = new BitMask(new uint[] { 0x3F22, 1 }, _environment_3_4.Size);
            Assert.AreEqual(unumNegative30.UnumBits, bitMaskMinus30);

            var unumNegative1000 = new Unum(_environment_3_4, -1000);
            var bitMaskMinus1000 = new BitMask(new uint[] { 0x63D45, 1 }, _environment_3_4.Size);
            Assert.AreEqual(unumNegative1000.UnumBits, bitMaskMinus1000);
        }

        [Fact]
        public void UnumIsCorrectlyConstructedFromUInt()
        {
            var unum0 = new Unum(_environment_3_4, 0U);
            var bitMask0 = new BitMask(new uint[] { 0 }, _environment_3_4.Size);
            Assert.AreEqual(bitMask0, unum0.UnumBits);

            var unum1 = new Unum(_environment_3_4, 1U);
            var bitMask1 = new BitMask(new uint[] { 0x100 }, _environment_3_4.Size);
            Assert.AreEqual(bitMask1, unum1.UnumBits);

            var unum2 = new Unum(_environment_3_4, 2U);
            var bitMask2 = new BitMask(new uint[] { 0x200 }, _environment_3_4.Size);
            Assert.AreEqual(bitMask2, unum2.UnumBits);

            var unum4 = new Unum(_environment_3_4, 4U);
            var bitMask4 = new BitMask(new uint[] { 0x610 }, _environment_3_4.Size);
            Assert.AreEqual(bitMask4, unum4.UnumBits);

            var unum8 = new Unum(_environment_3_4, 8U);
            var bitMask8 = new BitMask(new uint[] { 0xC20 }, _environment_3_4.Size);
            Assert.AreEqual(bitMask8, unum8.UnumBits);

            var unum10 = new Unum(_environment_2_2, 10U);
            var bitMask10 = new BitMask(new uint[] { 0x329 }, _environment_2_2.Size);
            Assert.AreEqual(bitMask10, unum10.UnumBits);

            var unum13 = new Unum(_environment_3_4, 13U);
            var bitMask13 = new BitMask(new uint[] { 0x3522 }, _environment_3_4.Size);
            Assert.AreEqual(bitMask13, unum13.UnumBits);

            var unum30 = new Unum(_environment_3_4, 30U);
            var bitMask30 = new BitMask(new uint[] { 0x3F22 }, _environment_3_4.Size);
            Assert.AreEqual(bitMask30, unum30.UnumBits);

            var unum1000 = new Unum(_environment_3_4, 1000U);
            var bitMask1000 = new BitMask(new uint[] { 0x63D45 }, _environment_3_4.Size);
            Assert.AreEqual(bitMask1000, unum1000.UnumBits);

            var unum5000 = new Unum(_environment_3_4, 5000U);
            var bitMask5000 = new BitMask(new uint[] { 0x367148 }, _environment_3_4.Size);
            Assert.AreEqual(bitMask5000, unum5000.UnumBits);

            var unum6000 = new Unum(_environment_3_4, 6000U);
            var bitMask6000 = new BitMask(new uint[] { 0x1B7747 }, _environment_3_4.Size);
            Assert.AreEqual(bitMask6000, unum6000.UnumBits);
        }

        //[Fact]
        //public void UnumIsCorrectlyConstructedFromDouble()
        //{
        //    var first = new Unum(_metaData_3_4, (double)30.0);
        //    var second = new Unum(_metaData_3_4, (double)9999999999999);

        //    var bitMask_1 = new BitMask(new uint[] { 0x3F22 }, _metaData_3_4.Size);
        //    var bitMask_2 = new BitMask(new uint[] { 0x6A2309EF }, _metaData_3_4.Size);

        //    Assert.AreEqual(first.UnumBits, bitMask_1);
        //    Assert.AreEqual(second.UnumBits, bitMask_2);
        //}

        //[Fact]
        //public void UnumIsCorrectlyConstructedFromLong()
        //{
        //    var first = new Unum(_metaData_3_4, (long)30);
        //    var second = new Unum(_metaData_3_4, (long)9999999999999);

        //    var bitMask_1 = new BitMask(new uint[] { 0x3F22 }, _metaData_3_4.Size);
        //    var bitMask_2 = new BitMask(new uint[] { 0x6A2309EF }, _metaData_3_4.Size);

        //    Assert.AreEqual(first.UnumBits, bitMask_1);
        //    Assert.AreEqual(second.UnumBits, bitMask_2);
        //}

        [Fact]
        public void IsExactIsCorrect()
        {
            // 0  0000 0000  0000  1 000 00
            var bitMask_3_2_uncertain = new BitMask(new uint[] { 0x20 }, 19);
            var unum_3_2_uncertain = new Unum(_environment_3_2, bitMask_3_2_uncertain);
            Assert.AreEqual(false, unum_3_2_uncertain.IsExact());

            var bitMask_3_2_certain = new BitMask(19, false);
            var unum_3_2_certain = new Unum(_environment_3_2, bitMask_3_2_certain);
            Assert.AreEqual(true, unum_3_2_certain.IsExact());

            var bitMask_3_4_uncertain = new BitMask(new uint[] { 0x80, 0 }, 33);
            var unum_3_4_uncertain = new Unum(_environment_3_4, bitMask_3_4_uncertain);
            Assert.AreEqual(false, unum_3_4_uncertain.IsExact());

            var bitMask_3_4_certain = new BitMask(33, false);
            var unum_3_4_certain = new Unum(_environment_3_4, bitMask_3_4_certain);
            Assert.AreEqual(true, unum_3_4_certain.IsExact());
        }

        [Fact]
        public void FractionSizeIsCorrect()
        {
            var bitMask_3_2_allOne = new BitMask(19, true);
            var unum_3_2_allOne = new Unum(_environment_3_2, bitMask_3_2_allOne);
            Assert.AreEqual(4, unum_3_2_allOne.FractionSize());

            var bitMask_3_4_allOne = new BitMask(33, true);
            var unum_3_4_allOne = new Unum(_environment_3_4, bitMask_3_4_allOne);
            Assert.AreEqual(16, unum_3_4_allOne.FractionSize());
        }

        [Fact]
        public void ExponentSizeIsCorrect()
        {
            var bitMask_3_2_allOne = new BitMask(19, true);
            var unum_3_2_allOne = new Unum(_environment_3_2, bitMask_3_2_allOne);
            Assert.AreEqual(8, unum_3_2_allOne.ExponentSize());

            var bitMask_3_4_allOne = new BitMask(33, true);
            var unum_3_4_allOne = new Unum(_environment_3_4, bitMask_3_4_allOne);
            Assert.AreEqual(8, unum_3_4_allOne.ExponentSize());

            var bitMask_4_3_allOne = new BitMask(33, true);
            var unum_4_3_allOne = new Unum(_environment_4_3, bitMask_4_3_allOne);
            Assert.AreEqual(16, unum_4_3_allOne.ExponentSize());
        }

        [Fact]
        public void FractionMaskIsCorrect()
        {
            // 0  0000 0000  1111  0000 00
            var bitMask_3_2_allOne = new BitMask(19, true);
            var unum_3_2_allOne = new Unum(_environment_3_2, bitMask_3_2_allOne);
            var bitMask_3_2_FractionMask = new BitMask(new uint[] { 0x3C0 }, 19);
            Assert.AreEqual(bitMask_3_2_FractionMask, unum_3_2_allOne.FractionMask());

            // 0  0000 0000  1111 1111 1111 1111  0000 0000
            var bitMask_3_4_allOne = new BitMask(33, true);
            var unum_3_4_allOne = new Unum(_environment_3_4, bitMask_3_4_allOne);
            var bitMask_3_4_FractionMask = new BitMask(new uint[] { 0xFFFF00 }, 33);
            Assert.AreEqual(bitMask_3_4_FractionMask, unum_3_4_allOne.FractionMask());
        }

        [Fact]
        public void ExponentMaskIsCorrect()
        {
            // 0  1111 1111  0000  0 000 00
            var bitMask_3_2_allOne = new BitMask(19, true);
            var unum_3_2_allOne = new Unum(_environment_3_2, bitMask_3_2_allOne);
            var bitMask_3_2_ExponentMask = new BitMask(new uint[] { 0x3FC00 }, 19);
            Assert.AreEqual(bitMask_3_2_ExponentMask, unum_3_2_allOne.ExponentMask());

            // 0  1111 1111  0000 0000 0000 0000  0 000 0000
            var bitMask_3_4_allOne = new BitMask(33, true);
            var unum_3_4_allOne = new Unum(_environment_3_4, bitMask_3_4_allOne);
            var bitMask_3_4_ExponentMask = new BitMask(new uint[] { 0xFF000000 }, 33);
            Assert.AreEqual(bitMask_3_4_ExponentMask, unum_3_4_allOne.ExponentMask());
        }

        [Fact]
        public void ExponentValueWithBiasIsCorrect()
        {
            var bitMask1 = new BitMask(new uint[] { 0xE40 }, 33);
            var unum1 = new Unum(_environment_3_4, bitMask1);
            Assert.AreEqual(unum1.ExponentValueWithBias(), -8);

            var unumZero = new Unum(_environment_3_4, 0);
            Assert.AreEqual(unumZero.ExponentValueWithBias(), 1);
        }

        [Fact]
        public void FractionWithHiddenBitIsCorrect()
        {
            var bitMask1 = new BitMask(new uint[] { 0xE40 }, 33);
            var unum1 = new Unum(_environment_3_4, bitMask1);
            Assert.AreEqual(new BitMask(new uint[] { 2 }, 33), unum1.FractionWithHiddenBit());

            var bitMask2 = new BitMask(new uint[] { 0x3F22 }, 33);
            var unum2 = new Unum(_environment_3_4, bitMask2);
            Assert.AreEqual(new BitMask(new uint[] { 0xF }, 33), unum2.FractionWithHiddenBit());

            var bitMask3 = new BitMask(new uint[] { 0x7E012B }, 33);
            var unum3 = new Unum(_environment_3_4, bitMask3);
            Assert.AreEqual(new BitMask(new uint[] { 0x1E01 }, 33), unum3.FractionWithHiddenBit());
        }

        [Fact]
        public void AddExactUnumsIsCorrect()
        {
            // First example from The End of Error p. 117.
            var bitMask1 = new BitMask(new uint[] { 0xE40 }, 33);
            var bitMask2 = new BitMask(new uint[] { 0x3F22 }, 33);
            var unumFirst = new Unum(_environment_3_4, bitMask1);
            var unumSecond = new Unum(_environment_3_4, bitMask2);
            var bitMaskSum = new BitMask(new uint[] { 0x7E012B }, 33);
            var unumSum1 = Unum.AddExactUnums(unumFirst, unumSecond);
            Assert.AreEqual(unumSum1.UnumBits, bitMaskSum);

            // Addition should be commutative.
            var unumSum2 = Unum.AddExactUnums(unumSecond, unumFirst);
            Assert.AreEqual(unumSum1.UnumBits, unumSum2.UnumBits);

            var unum0 = new Unum(_environment_3_4, 0);
            var unum1 = new Unum(_environment_3_4, 1);
            var unum0Plus1 = unum0 + unum1;
            var unum31 = new Unum(_environment_3_4, 30) + unum1;
            var unum0PlusUnum1 = Unum.AddExactUnums(unum0, unum1);
            Assert.AreEqual(unum31.UnumBits, new Unum(_environment_3_4, 31).UnumBits);
            Assert.AreEqual(unum1.UnumBits, unum0Plus1.UnumBits);
            Assert.AreEqual(unum1.UnumBits, unum0PlusUnum1.UnumBits);

            // Case of inexact result, second example from The End or Error, p. 117.
            var bitMask4 = new BitMask(new uint[] { 0x18F400CF }, 33); // 1000.0078125
            var unum1000 = new Unum(_environment_3_4, 1000);
            var unum6 = Unum.AddExactUnums(unum1000, unumFirst); // 1/256
            Assert.AreEqual(unum6.UnumBits, bitMask4);

            var unum5000 = new Unum(_environment_3_4, 5000);
            var unum6000 = new Unum(_environment_3_4, 6000);
            Assert.AreEqual(Unum.AddExactUnums(unum5000, unum1000).UnumBits, unum6000.UnumBits);
            //var unumTest = new Unum(_metaData_3_5, 30.401);
            //var unumTest2 = new Unum(_metaData_3_5, -30.300);
            //var res = unumTest + unumTest2;
            //var resF = (double)res;
            //Assert.AreEqual(resF, (30.401 - 30.300));// The result is correct, but the precision is too poor to show that.

            var unumNegativeThirty = new Unum(_environment_3_4, -30);
            var unum30 = new Unum(_environment_3_4, 30);
            Assert.AreEqual(Unum.AddExactUnums(unum30, unumNegativeThirty).UnumBits, unum0.UnumBits);
        }

        [Fact]
        public void AdditionIsCorrectForIntegers()
        {
            var result = new Unum(_environment_3_5, 0);
            var count = 100;

            for (int i = 1; i <= count; i++) result += new Unum(_environment_3_5, i * 1000);
            for (int i = 1; i <= count; i++) result -= new Unum(_environment_3_5, i * 1000);

            Assert.AreEqual(result.UnumBits, new Unum(_environment_3_5, 0).UnumBits);
        }

        //[Fact]
        //public void AdditionIsCorrectForFloats()
        //{
        //    var res = new Unum(_metaData_3_5, 0);
        //    var res2 = new Unum(_metaData_3_5, 0);
        //    var end = 3;
        //    float facc = 0;

        //    for (int i = 1; i <= end; i++)
        //    {
        //        facc += (float)(i * 0.5F);
        //        res += new Unum(_metaData_3_5, (float)(i * 0.5F));
        //    }
        //    //res += new Unum(_metaData_3_5, 0);
        //    for (int i = 1; i <= end; i++)
        //    {

        //        facc -= (float)(i * 0.5F);
        //        res -= new Unum(_metaData_3_5, (float)(i * 0.5F));
        //    }
        //    var f = (float)res;
        //    //Assert.AreEqual(res.IsZero(), true);
        //    Assert.AreEqual(facc, f);
        //    Assert.AreEqual(res.UnumBits, new Unum(_metaData_3_5, 0).UnumBits);

        //    var res3 = new Unum(_metaData_3_5, 0.5F);
        //    var res1 = new Unum(_metaData_3_5, (float)13.0);
        //    var res2 = new Unum(_metaData_3_5, (float)4.0);
        //    res3 = res1 + res2;
        //    res3 -= res2;
        //    res3 -= res1;
        //    Assert.AreEqual((float)res3, 0.5F);
        //}

        [Fact]
        public void SubtractExactUnumsIsCorrect()
        {
            var bitMask1 = new BitMask(new uint[] { 0x7E012B }, 33); // 30.00390625
            var bitMask2 = new BitMask(new uint[] { 0xE40 }, 33);    // 0.00390625
            var bitMask3 = new BitMask(new uint[] { 0x3F22 }, 33);   // 30

            var unum1 = new Unum(_environment_3_4, bitMask1);
            var unum2 = new Unum(_environment_3_4, bitMask2);
            var unum3 = Unum.SubtractExactUnums(unum1, unum2);
            Assert.AreEqual(unum3.UnumBits, bitMask3);

            var unum5000 = new Unum(_environment_3_4, 5000);
            var unum6000 = new Unum(_environment_3_4, 6000);
            var unum1000 = new Unum(_environment_3_4, 1000);

            var unumRes = Unum.SubtractExactUnums(unum6000, unum5000);
            Assert.AreEqual(unumRes.UnumBits, unum1000.UnumBits);

            Unum unum30 = new Unum(_environment_3_4, 30);
            Unum unumZero = new Unum(_environment_3_4, 0);
            Assert.AreEqual(Unum.SubtractExactUnums(unum30, unum30).UnumBits, unumZero.UnumBits);
        }

        [Fact]
        public void IntToUnumIsCorrect()
        {
            var unum0 = new Unum(_environment_3_4, 0);
            var bitMask0 = new BitMask(new uint[] { 0 }, 33);
            Assert.AreEqual(unum0.UnumBits, bitMask0);

            var unum1 = new Unum(_environment_3_4, 1);
            Assert.AreEqual(unum1.UnumBits, new BitMask(new uint[] { 0x100 }, 33));

            var unum2 = new Unum(_environment_3_4, 2);
            Assert.AreEqual(unum2.UnumBits, new BitMask(new uint[] { 0x200 }, 33));

            var unum30 = new Unum(_environment_3_4, 30);
            var bitMask30 = new BitMask(new uint[] { 0x3F22 }, 33);
            Assert.AreEqual(unum30.UnumBits, bitMask30);

            var unum1000 = new Unum(_environment_3_4, 1000);
            var bitMask1000 = new BitMask(new uint[] { 0x63D45 }, 33);
            Assert.AreEqual(unum1000.UnumBits, bitMask1000);

            var unum5000 = new Unum(_environment_3_4, 5000);
            var bitMask5000 = new BitMask(new uint[] { 0x367148 }, 33);
            Assert.AreEqual(unum5000.UnumBits, bitMask5000);

            var bitMask6000 = new BitMask(new uint[] { 0x1B7747 }, 33);
            var unum6000 = new Unum(_environment_3_4, 6000);
            Assert.AreEqual(unum6000.UnumBits, bitMask6000);

            var unumNegative30 = new Unum(_environment_3_4, -30);
            var bitMaskNegative30 = new BitMask(new uint[] { 0x3F22, 1 }, 33);
            Assert.AreEqual(unumNegative30.UnumBits, bitMaskNegative30);

            var unumNegative1000 = new Unum(_environment_3_4, -1000);
            var bitMaskNegative1000 = new BitMask(new uint[] { 0x63D45, 1 }, 33);
            Assert.AreEqual(unumNegative1000.UnumBits, bitMaskNegative1000);
        }

        [Fact]
        public void UnumToUintIsCorrect()
        {
            var unum1 = new Unum(_environment_3_4, 1);
            var number1 = (uint)unum1;
            Assert.AreEqual(number1, 1);

            var unum2 = new Unum(_environment_3_4, 2);
            var number2 = (uint)unum2;
            Assert.AreEqual(number2, 2);

            var unum30 = new Unum(_environment_3_4, 30);
            var number30 = (uint)unum30;
            Assert.AreEqual(number30, 30);

            var unum1000 = new Unum(_environment_3_4, 1000);
            var number1000 = (uint)unum1000;
            Assert.AreEqual(number1000, 1000);

            var unum5000 = new Unum(_environment_3_4, 5000);
            var number5000 = (uint)unum5000;
            Assert.AreEqual(number5000, 5000);

            var unum6000 = new Unum(_environment_3_4, 6000);
            var number6000 = (uint)unum6000;
            Assert.AreEqual(number6000, 6000);
        }

        [Fact]
        public void UnumToIntIsCorrect()
        {
            var unum1 = new Unum(_environment_3_4, 1);
            var one = (int)unum1;
            Assert.AreEqual(one, 1);

            var unum30 = new Unum(_environment_3_4, 30);
            var number30 = (int)unum30;
            Assert.AreEqual(number30, 30);

            var unum1000 = new Unum(_environment_3_4, 1000);
            var number1000 = (int)unum1000;
            Assert.AreEqual(number1000, 1000);

            var unumNegative30 = new Unum(_environment_3_4, -30);
            var numberNegative30 = (int)unumNegative30;
            Assert.AreEqual(numberNegative30, -30);

            var unumNegative1000 = new Unum(_environment_3_4, -1000);
            var numberNegative1000 = (int)unumNegative1000;
            Assert.AreEqual(numberNegative1000, -1000);
        }

        //[Fact]
        //public void FloatToUnumIsCorrect()
        //{
        //    var first = new Unum(_environment_3_4, (float)30.0);
        //    var second = new Unum(_environment_3_4, (float)9999999999999);

        //    var bitMask_1 = new BitMask(new uint[] { 0x3F22 }, 33);
        //    var bitMask_2 = new BitMask(new uint[] { 0x6A2309EF }, 33);

        //    Assert.AreEqual(first.UnumBits, bitMask_1);
        //    Assert.AreEqual(second.UnumBits, bitMask_2);
        //}

        [Fact]
        public void UnumToFloatIsCorrect()
        {
            var unum30 = new Unum(_environment_3_4, 30);
            var numbe30 = (float)unum30;
            Assert.AreEqual(numbe30, 30F);

            var unum1000 = new Unum(_environment_3_4, 1000);
            var number1000 = (float)unum1000;
            Assert.AreEqual(number1000, 1000);

            var unumNegative30 = new Unum(_environment_3_4, -30);
            var numberNegative30 = (float)unumNegative30;
            Assert.AreEqual(numberNegative30, -30);

            var unumNegative1000 = new Unum(_environment_3_4, -1000);
            var numberNegative1000 = (float)unumNegative1000;
            Assert.AreEqual(numberNegative1000, -1000);
        }

        //[Fact]
        //public void UnumToDoubleIsCorrect()
        //{
        //    var unum30 = new Unum(_metaData_3_4, 30);
        //    var unum1000 = new Unum(_metaData_3_4, 1000);
        //    var unumNegativeThirty = new Unum(_metaData_3_4, -30);
        //    var unumNegativeThousand = new Unum(_metaData_3_4, -1000);

        //    var tooBigUnum = new Unum(_metaData_3_4, (double)9999999999999);
        //    var tooBigNegativeUnum = new Unum(_metaData_3_4, (double)-9999999999999);

        //    var thirty = (double)unum30;
        //    var thousand = (double)unum1000;
        //    var negativeThirty = (double)unumNegativeThirty;
        //    var negativeThousand = (double)unumNegativeThousand;
        //    var tooBig = (double)tooBigUnum;
        //    var tooBigNegative = (double)tooBigNegativeUnum;

        //    Assert.AreEqual(thirty, (double)30);
        //    Assert.AreEqual(thousand, (double)1000);
        //    Assert.AreEqual(negativeThirty, (double)-30);
        //    Assert.AreEqual(negativeThousand, (double)-1000);
        //    Assert.AreEqual(tooBig, (double)9999891824640); // Some information is lost due to limited size of Unum.
        //    Assert.AreEqual(tooBigNegative, (double)-9999891824640); // Some information is lost due to limited size of Unum.
        //}

    }
}
