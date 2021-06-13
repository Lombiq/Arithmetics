using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
    public class UnumHelperTests
    {
        private readonly UnumEnvironment _warlpiriEnvironment;
        private readonly UnumEnvironment _environment22;
        private readonly UnumEnvironment _environment23;
        private readonly UnumEnvironment _environment24;
        private readonly UnumEnvironment _environment32;
        private readonly UnumEnvironment _environment48;

        public UnumHelperTests()
        {
            _warlpiriEnvironment = UnumEnvironment.FromStandardEnvironment(StandardEnvironment.Warlpiri);
            _environment22 = new UnumEnvironment(2, 2);
            _environment23 = new UnumEnvironment(2, 3);
            _environment24 = new UnumEnvironment(2, 4);
            _environment32 = new UnumEnvironment(3, 2);
            _environment48 = new UnumEnvironment(4, 8);
        }

        [Fact]
        public void BitsRequiredByLargestExpressablePositiveIntegerIsCorrect()
        {
            Assert.AreEqual(UnumHelper.BitsRequiredByLargestExpressablePositiveInteger(_warlpiriEnvironment), 2);
            Assert.AreEqual(UnumHelper.BitsRequiredByLargestExpressablePositiveInteger(_environment22), 9);
            Assert.AreEqual(UnumHelper.BitsRequiredByLargestExpressablePositiveInteger(_environment23), 9);
            Assert.AreEqual(UnumHelper.BitsRequiredByLargestExpressablePositiveInteger(_environment24), 9);
            Assert.AreEqual(UnumHelper.BitsRequiredByLargestExpressablePositiveInteger(_environment32), 129);
        }

        [Fact]
        public void LargestExpressablePositiveIntegerIsCorrect()
        {
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_environment48), _environment48.EmptyBitMask);
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_environment32), _environment32.EmptyBitMask);
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_warlpiriEnvironment), new BitMask(2, _warlpiriEnvironment.Size));
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_environment22), new BitMask(480, _environment22.Size));
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_environment23), new BitMask(510, _environment23.Size));
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_environment24), new BitMask(511, _environment24.Size));
        }
    }
}
