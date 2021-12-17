using Shouldly;

namespace Lombiq.Arithmetics.Tests
{
    public static class CompatibilityAssert
    {
        public static void AreEqual<T>(T actual, T expected) => actual.ShouldBe(expected);

        public static void AreEqual(uint actual, int expected) => actual.ShouldBe((uint)expected);

        public static void AreEqual(int actual, uint expected)
        {
            var actualUInt = (uint)actual;
            actualUInt.ShouldBe(expected);
        }

        public static void AreEqual<T>(T actual, T expected, string userMessage) => actual.ShouldBe(expected, userMessage);
    }
}
