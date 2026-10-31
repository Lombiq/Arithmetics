using System.Globalization;

namespace Lombiq.Arithmetics
{
    public static class PositTemplateHelper
    {
        public static readonly string[] UnderlyingStructNames = new[] { "byte", "ushort", "uint", "ulong", "BitMask" };

        public static string GetPositStructName(byte positSize, int maximumExponentSize) =>
            $"Posit{positSize}E{maximumExponentSize.ToString(CultureInfo.InvariantCulture)}";
    }
}
