using Xunit.Sdk;

namespace Lombiq.Arithmetics.Tests
{
    public static class CompatibilityAssert
    {
        public static void AreEqual<T>(T actual, T expected) => Xunit.Assert.Equal(expected, actual);

        public static void AreEqual(uint actual, int expected) => Xunit.Assert.Equal((uint)expected, actual);

        public static void AreEqual(int actual, uint expected) => Xunit.Assert.Equal(expected, (uint)actual);

        public static void AreEqual<T>(T actual, T expected, string userMessage)
        {
            try
            {
                Xunit.Assert.Equal(expected, actual);
            }
            catch (EqualException)
            {
                Xunit.Assert.True(false, userMessage);
            }
        }
    }
}