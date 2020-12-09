using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
    public class UnumHelperTests
    {
        private readonly UnumEnvironment _warlpiriEnvironment;
        private readonly UnumEnvironment _environment_2_2;
        private readonly UnumEnvironment _environment_2_3;
        private readonly UnumEnvironment _environment_2_4;
        private readonly UnumEnvironment _environment_3_2;
        private readonly UnumEnvironment _environment_4_8;

        public UnumHelperTests()
        {
            _warlpiriEnvironment = UnumEnvironment.FromStandardEnvironment(StandardEnvironment.Warlpiri);
            _environment_2_2 = new UnumEnvironment(2, 2);
            _environment_2_3 = new UnumEnvironment(2, 3);
            _environment_2_4 = new UnumEnvironment(2, 4);
            _environment_3_2 = new UnumEnvironment(3, 2);
            _environment_4_8 = new UnumEnvironment(4, 8);
        }

        [Fact]
        public void BitsRequiredByLargestExpressablePositiveIntegerIsCorrect()
        {
            Assert.AreEqual(UnumHelper.BitsRequiredByLargestExpressablePositiveInteger(_warlpiriEnvironment), 2);
            Assert.AreEqual(UnumHelper.BitsRequiredByLargestExpressablePositiveInteger(_environment_2_2), 9);
            Assert.AreEqual(UnumHelper.BitsRequiredByLargestExpressablePositiveInteger(_environment_2_3), 9);
            Assert.AreEqual(UnumHelper.BitsRequiredByLargestExpressablePositiveInteger(_environment_2_4), 9);
            Assert.AreEqual(UnumHelper.BitsRequiredByLargestExpressablePositiveInteger(_environment_3_2), 129);

        }

        [Fact]
        public void LargestExpressablePositiveIntegerIsCorrect()
        {
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_environment_4_8), _environment_4_8.EmptyBitMask);
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_environment_3_2), _environment_3_2.EmptyBitMask);
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_warlpiriEnvironment), new BitMask(2, _warlpiriEnvironment.Size));
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_environment_2_2), new BitMask(480, _environment_2_2.Size));
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_environment_2_3), new BitMask(510, _environment_2_3.Size));
            Assert.AreEqual(UnumHelper.LargestExpressablePositiveInteger(_environment_2_4), new BitMask(511, _environment_2_4.Size));

        }
    }
}
