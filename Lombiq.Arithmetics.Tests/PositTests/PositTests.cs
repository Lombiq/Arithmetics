using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lombiq.Arithmetics.Posit;
using NUnit.Framework;
using Shouldly;

namespace Lombiq.Arithmetics.Tests
{
    [TestFixture]
    class PositTests
    {

        private PositEnvironment _environment_6_1;
        private PositEnvironment _environment_6_2;
        private PositEnvironment _environment_6_3;
        private PositEnvironment _environment_8_2;
        private PositEnvironment _environment_12_2;

        [SetUp]
        public void Init()
        {
            _environment_6_1 = new PositEnvironment(6, 1);
            _environment_6_2 = new PositEnvironment(6, 2);
            _environment_6_3 = new PositEnvironment(6, 3);
            _environment_8_2 = new PositEnvironment(8, 2);
            _environment_12_2 = new PositEnvironment(12, 2);

        }

        [TestFixtureTearDown]
        public void Clean()
        {
        }

        [Test]
        public void EncodeRegimeBitsIsCorrect()
        {
            new Posit.Posit(_environment_8_2).EncodeRegimeBits(0).ShouldBe(new BitMask(0x40, _environment_8_2.Size));

            new Posit.Posit(_environment_6_3).EncodeRegimeBits(-3).ShouldBe(new BitMask(0x2, _environment_6_3.Size));

            new Posit.Posit(_environment_6_3).EncodeRegimeBits(3).ShouldBe(new BitMask(0x1E, _environment_6_3.Size));

            new Posit.Posit(_environment_6_3).EncodeRegimeBits(2).ShouldBe(new BitMask(0x1C, _environment_6_3.Size));

            new Posit.Posit(_environment_8_2).EncodeRegimeBits(1).ShouldBe(new BitMask(0x60, _environment_6_3.Size));

            new Posit.Posit(_environment_8_2).EncodeRegimeBits(3).ShouldBe(new BitMask(0x78, _environment_8_2.Size));

            new Posit.Posit(_environment_8_2).EncodeRegimeBits(6).ShouldBe(new BitMask(0x7F, _environment_8_2.Size));
        }

        [Test]
        public void PositIsCorrectlyConstructedFromUint()
        {

            new Posit.Posit(_environment_6_3, (uint)0).PositBits.ShouldBe(new BitMask(0x0, _environment_6_3.Size));

            new Posit.Posit(_environment_6_3, (uint)8).PositBits.ShouldBe(new BitMask(0x13, _environment_6_3.Size));

            new Posit.Posit(_environment_6_3, (uint)16384).PositBits.ShouldBe(new BitMask(0x1B, _environment_6_3.Size));

            new Posit.Posit(_environment_6_3, (uint)1048576).PositBits.ShouldBe(new BitMask(0x1D, _environment_6_3.Size));

            new Posit.Posit(_environment_8_2, (uint)13).PositBits.ShouldBe(new BitMask(0x5D, _environment_8_2.Size));

            new Posit.Posit(_environment_12_2, (uint)172).PositBits.ShouldBe(new BitMask(0x6D6, _environment_12_2.Size));

            new Posit.Posit(_environment_12_2, (uint)173).PositBits.ShouldBe(new BitMask(0x6D6, _environment_12_2.Size));

            // examples of Posit rounding
            new Posit.Posit(_environment_8_2, (uint)90).PositBits.ShouldBe(new BitMask(0x6A, _environment_12_2.Size));
            new Posit.Posit(_environment_8_2, (uint)82).PositBits.ShouldBe(new BitMask(0x69, _environment_12_2.Size));

            // Numbers out of range don't get rounded up infinity. They get rounded to the biggest representable
            // finite value (MaxValue). 
            new Posit.Posit(_environment_6_1, (uint)500).PositBits.ShouldBe(_environment_6_1.MaxValueBitMask);
        }

        [Test]
        public void PositIsCorrectlyConstructedFromInt()
        {
            new Posit.Posit(_environment_6_3, 8).PositBits.ShouldBe(new BitMask(0x13, _environment_6_3.Size));

            new Posit.Posit(_environment_6_3, 16384).PositBits.ShouldBe(new BitMask(0x1B, _environment_6_3.Size));

            new Posit.Posit(_environment_8_2, 13).PositBits.ShouldBe(new BitMask(0x5D, _environment_8_2.Size));

            new Posit.Posit(_environment_6_3, -8).PositBits.ShouldBe(new BitMask(0x2D, _environment_6_3.Size));

            new Posit.Posit(_environment_8_2, -13).PositBits.ShouldBe(new BitMask(0xA3, _environment_8_2.Size));

            new Posit.Posit(_environment_6_3, -16384).PositBits.ShouldBe(new BitMask(0x25, _environment_6_3.Size));

        }

        [Test]
        public void PositToIntIsCorrect()
        {
            //var posit8 = new Posit.Posit(_environment_6_3, 8);
            //Assert.AreEqual((int)posit8, 8);

            var posit16384 = new Posit.Posit(_environment_6_3, 16384);
            Assert.AreEqual((int)posit16384, 16384);

        }

        [Test]
        public void FractionWithHiddenBitIsCorrect()
        {
            var posit16384 = new Posit.Posit(_environment_6_3, 16384);
            posit16384.FractionWithHiddenBit().ShouldBe(new BitMask(1, _environment_6_3.Size));

            var posit3 = new Posit.Posit(_environment_6_1, 3);
            posit3.FractionWithHiddenBit().ShouldBe(new BitMask(6, _environment_6_1.Size));
        }

        [Test]
        public void GetRegimeKValueIsCorrect()
        {
            new Posit.Posit(_environment_6_3, 8).GetRegimeKValue().ShouldBe(0);

            new Posit.Posit(_environment_6_3, 16384).GetRegimeKValue().ShouldBe(1);

            new Posit.Posit(_environment_8_2, 13).GetRegimeKValue().ShouldBe(0);

            new Posit.Posit(_environment_6_3, -8).GetRegimeKValue().ShouldBe(0);

            new Posit.Posit(_environment_8_2, -13).GetRegimeKValue().ShouldBe(0);

            new Posit.Posit(_environment_6_3, -16384).GetRegimeKValue().ShouldBe(1);

        }


        [Test]
        public void AdditionIsCorrect()
        {
            //var posit0 = new Posit.Posit(_environment_6_3, 0);
            var posit1 = new Posit.Posit(_environment_6_3, 1);
            var posit2 = posit1 + 1;
            posit2.PositBits.ShouldBe(new Posit.Posit(_environment_6_3, 2).PositBits);
            //var posit = posit0 + 1;
            ////posit.PositBits.ShouldBe(new Posit.Posit(_environment_6_3, 1).PositBits);

            //var posit3 = new Posit.Posit(_environment_6_2, 3);
            //var posit6 = posit3 + posit3;
            //posit6.PositBits.ShouldBe(new Posit.Posit(_environment_6_2, 6).PositBits);
        }
    }
}

