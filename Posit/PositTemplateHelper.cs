using System.Globalization;

namespace Lombiq.Arithmetics
{
    public static class PositTemplateHelper
    {
        public static readonly byte[] PositSizes = new byte[] { 8, 16, 32, 64 };
        public static readonly string[] UnderlyingStructNames = new[] { "byte", "ushort", "uint", "ulong", "BitMask" };

        public static string GetPositStructName(byte positSize, int maximumExponentSize) =>
            $"Posit{positSize}E{maximumExponentSize.ToString(CultureInfo.InvariantCulture)}";
    }
}
