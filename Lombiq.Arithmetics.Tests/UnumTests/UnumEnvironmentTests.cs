using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
    public class UnumEnvironmentTests
    {
        private readonly UnumEnvironment _warlpiriEnvironment;
        private readonly UnumEnvironment _environment32;
        private readonly UnumEnvironment _environment34;
        private readonly Unum _unum32;
        private readonly Unum _unum34;

        public UnumEnvironmentTests()
        {
            _warlpiriEnvironment = UnumEnvironment.FromStandardEnvironment(StandardEnvironment.Warlpiri);
            _environment32 = new UnumEnvironment(3, 2);
            _environment34 = new UnumEnvironment(3, 4);
            _unum32 = new Unum(_environment32);
            _unum34 = new Unum(_environment34);
        }

        [Fact]
        public void WarlpiriUnumEnvironmentIsCorrect()
        {
            Assert.AreEqual(new BitMask(4), _warlpiriEnvironment.EmptyBitMask);
            Assert.AreEqual(_warlpiriEnvironment.EmptyBitMask, _warlpiriEnvironment.ExponentSizeMask);
            Assert.AreEqual(1, _warlpiriEnvironment.ExponentSizeMax);
            Assert.AreEqual(0, _warlpiriEnvironment.ExponentSizeSize);
            Assert.AreEqual(_warlpiriEnvironment.EmptyBitMask, _warlpiriEnvironment.FractionSizeMask);
            Assert.AreEqual(1, _warlpiriEnvironment.FractionSizeMax);
            Assert.AreEqual(0, _warlpiriEnvironment.FractionSizeSize);
            Assert.AreEqual(_warlpiriEnvironment.EmptyBitMask, _warlpiriEnvironment.ExponentAndFractionSizeMask);
            Assert.AreEqual(4, _warlpiriEnvironment.Size);

            Assert.AreEqual(12, _warlpiriEnvironment.LargestNegative.GetLowest32Bits()); // 1100
            Assert.AreEqual(4, _warlpiriEnvironment.LargestPositive.GetLowest32Bits()); // 0100
            Assert.AreEqual(14, _warlpiriEnvironment.NegativeInfinity.GetLowest32Bits()); // 1110
            Assert.AreEqual(6, _warlpiriEnvironment.PositiveInfinity.GetLowest32Bits()); // 0110
            Assert.AreEqual(7, _warlpiriEnvironment.QuietNotANumber.GetLowest32Bits()); // 0111
            Assert.AreEqual(15, _warlpiriEnvironment.SignalingNotANumber.GetLowest32Bits()); // 1111
            Assert.AreEqual(8, _warlpiriEnvironment.SignBitMask.GetLowest32Bits()); // 1000
            Assert.AreEqual(2, _warlpiriEnvironment.SmallestPositive.GetLowest32Bits()); // 0010
            Assert.AreEqual(2, _warlpiriEnvironment.Ulp.GetLowest32Bits()); // 0010
            Assert.AreEqual(1, _warlpiriEnvironment.UncertaintyBitMask.GetLowest32Bits()); // 0001
            Assert.AreEqual(1, _warlpiriEnvironment.UnumTagMask.GetLowest32Bits()); // 0001
            Assert.AreEqual(1, _warlpiriEnvironment.UnumTagSize);
        }

        [Fact]
        public void UnumExponentSizeSizeIsCorrect()
        {
            Assert.AreEqual(3, _unum32.ExponentSizeSize);
            Assert.AreEqual(3, _unum34.ExponentSizeSize);
        }

        [Fact]
        public void UnumFractionSizeSizeIsCorrect()
        {
            Assert.AreEqual(2, _unum32.FractionSizeSize);
            Assert.AreEqual(4, _unum34.FractionSizeSize);
        }

        [Fact]
        public void UnumTagSizeIsCorrect()
        {
            Assert.AreEqual(6, _unum32.UnumTagSize);
            Assert.AreEqual(8, _unum34.UnumTagSize);
        }

        [Fact]
        public void UnumSizeIsCorrect()
        {
            Assert.AreEqual(19, _unum32.Size);
            Assert.AreEqual(33, _unum34.Size);
        }

        [Fact]
        public void UnumUncertaintyBitMaskIsCorrect()
        {
            // 0  0000 0000  0000  1 000 00
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_20 }, _unum32.Size),
                _unum32.UncertaintyBitMask,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.UncertaintyBitMask)));

            // 0  0000 0000  0000 0000 0000 0000  1 000 0000
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_80, 0 }, _unum34.Size),
                _unum34.UncertaintyBitMask,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.UncertaintyBitMask)));
        }

        [Fact]
        public void UnumExponentSizeMaskIsCorrect()
        {
            // 0  0000 0000  0000  0 111 00
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_1C }, _unum32.Size),
                _unum32.ExponentSizeMask,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.ExponentSizeMask)));

            // 0  0000 0000  0000 0000 0000 0000  0 111 0000
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_70, 0 }, _unum34.Size),
                _unum34.ExponentSizeMask,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.ExponentSizeMask)));
        }

        [Fact]
        public void UnumFractionSizeMaskIsCorrect()
        {
            // 0  0000 0000  0000  0 000 11
            Assert.AreEqual(
                new BitMask(new uint[] { 3 }, _unum32.Size),
                _unum32.FractionSizeMask,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.FractionSizeMask)));

            // 0  0000 0000  0000 0000 0000 0000  0 000 1111
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_0F, 0 }, _unum34.Size),
                _unum34.FractionSizeMask,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.FractionSizeMask)));
        }

        [Fact]
        public void UnumExponentAndFractionSizeMaskIsCorrect()
        {
            // 0  0000 0000  0000  0 111 11
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_1F }, _unum32.Size),
                _unum32.ExponentAndFractionSizeMask,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.ExponentAndFractionSizeMask)));

            // 0  0000 0000  0000 0000 0000 0000  0 111 1111
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_7F, 0 }, _unum34.Size),
                _unum34.ExponentAndFractionSizeMask,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.ExponentAndFractionSizeMask)));
        }

        [Fact]
        public void UnumTagMaskIsCorrect()
        {
            // 0  0000 0000  0000  1 111 11
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_3F }, _unum32.Size),
                _unum32.UnumTagMask,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.UnumTagMask)));

            // 0  0000 0000  0000 0000 0000 0000  1 111 1111
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_FF, 0 }, _unum34.Size),
                _unum34.UnumTagMask,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.UnumTagMask)));
        }

        [Fact]
        public void UnumSignBitMaskIsCorrect()
        {
            // 1  0000 0000  0000  0 000 00
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_0004_0000 }, _unum32.Size),
                _unum32.SignBitMask,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.SignBitMask)));

            // 1  0000 0000  0000 0000 0000 0000  0 000 0000
            Assert.AreEqual(
                new BitMask(new uint[] { 0, 1 }, _unum34.Size),
                _unum34.SignBitMask,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.SignBitMask)));
        }

        [Fact]
        public void UnumPositiveInfinityIsCorrect()
        {
            // 0  1111 1111  1111  0 111 11
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_0003_FFDF }, _unum32.Size),
                _unum32.PositiveInfinity,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.PositiveInfinity)));

            // 0  1111 1111  1111 1111 1111 1111  0 111 1111
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_FFFF_FF7F, 0 }, _unum34.Size),
                _unum34.PositiveInfinity,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.PositiveInfinity)));
        }

        [Fact]
        public void UnumNegativeInfinityIsCorrect()
        {
            // 1  1111 1111  1111  0 111 11
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_0007_FFDF }, _unum32.Size),
                _unum32.NegativeInfinity,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.NegativeInfinity)));

            // 1  1111 1111  1111 1111 1111 1111  0 111 1111
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_FFFF_FF7F, 1 }, _unum34.Size),
                _unum34.NegativeInfinity,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.NegativeInfinity)));
        }

        [Fact]
        public void UnumQuietNotANumberIsCorrect()
        {
            // 0  1111 1111  1111  1 111 11
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_0003_FFFF }, _unum32.Size),
                _unum32.QuietNotANumber,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.QuietNotANumber)));

            // 0  1111 1111  1111 1111 1111 1111  1 111 1111
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_FFFF_FFFF, 0 }, _unum34.Size),
                _unum34.QuietNotANumber,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.QuietNotANumber)));
        }

        [Fact]
        public void UnumSignalingNotANumberIsCorrect()
        {
            // 1  1111 1111  1111  1 111 11
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_0007_FFFF }, _unum32.Size),
                _unum32.SignalingNotANumber,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.SignalingNotANumber)));

            // 1  1111 1111  1111 1111 1111 1111  1 111 1111
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_FFFF_FFFF, 1 }, _unum34.Size),
                _unum34.SignalingNotANumber,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.SignalingNotANumber)));
        }

        [Fact]
        public void UnumLargestPositiveIsCorrect()
        {
            // 0  1111 1111  1110  0 111 11
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_0003_FF9F }, _unum32.Size),
                _unum32.LargestPositive,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.LargestPositive)));

            // 0  1111 1111  1111 1111 1111 1110  0 111 1111
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_FFFF_FE7F, 0 }, _unum34.Size),
                _unum34.LargestPositive,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.LargestPositive)));
        }

        [Fact]
        public void UnumSmallestPositiveIsCorrect()
        {
            // 0  0000 0000  0001  0 111 11
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_5F }, _unum32.Size),
                _unum32.SmallestPositive,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.SmallestPositive)));

            // 0  0000 0000  0000 0000 0000 0001  0 111 1111
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_17F, 0 }, _unum34.Size),
                _unum34.SmallestPositive,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.SmallestPositive)));
        }

        [Fact]
        public void UnumLargestNegativeIsCorrect()
        {
            // 1  1111 1111  1110  0 111 11
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_0007_FF9F }, _unum32.Size),
                _unum32.LargestNegative,
                TestFailureMessageBuilder(_unum32, nameof(_unum32.LargestNegative)));

            // 1  1111 1111  1111 1111 1111 1110  0 111 1111
            Assert.AreEqual(
                new BitMask(new uint[] { 0x_FFFF_FE7F, 1 }, _unum34.Size),
                _unum34.LargestNegative,
                TestFailureMessageBuilder(_unum34, nameof(_unum34.LargestNegative)));
        }

        private static string TestFailureMessageBuilder(Unum unum, string propertyName) =>
            $"Testing the \"{propertyName}\" property of the Unum ({unum.ExponentSizeSize}, {unum.FractionSizeSize}) environment failed.";
    }
}
