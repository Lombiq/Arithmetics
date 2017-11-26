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
        private PositEnvironment _environment_6_3;
        private PositEnvironment _environment_8_2;
        private PositEnvironment _environment_12_2;

        [SetUp]
        public void Init()
        {
            _environment_6_1 = new PositEnvironment(6, 1);
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
            new Posit.Posit(_environment_6_3, (uint)8).PositBits.ShouldBe(new BitMask(0x13, _environment_6_3.Size));

            new Posit.Posit(_environment_6_3, (uint)16384).PositBits.ShouldBe(new BitMask(0x1B, _environment_6_3.Size));

            new Posit.Posit(_environment_8_2, (uint)13).PositBits.ShouldBe(new BitMask(0x5D, _environment_8_2.Size));

            new Posit.Posit(_environment_12_2, (uint)172).PositBits.ShouldBe(new BitMask(0x6D6, _environment_12_2.Size));

            new Posit.Posit(_environment_12_2, (uint)173).PositBits.ShouldBe(new BitMask(0x6D6, _environment_12_2.Size));

            // examples of Posit rounding
            new Posit.Posit(_environment_8_2, (uint)90).PositBits.ShouldBe(new BitMask(0x6A, _environment_12_2.Size));
            new Posit.Posit(_environment_8_2, (uint)82).PositBits.ShouldBe(new BitMask(0x69, _environment_12_2.Size));

            // Numbers out of range don't get rounded up infinity. They get rounded to the biggest representable
            // finite value (maxpos). 
            new Posit.Posit(_environment_6_1, (uint)500).PositBits.ShouldBe(new BitMask(0x1F, _environment_6_1.Size));
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


    }
}