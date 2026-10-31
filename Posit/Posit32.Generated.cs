using System.Runtime.CompilerServices;

namespace Lombiq.Arithmetics
{
    public readonly partial struct Posit32
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static partial Posit32 Abs(Posit32 input)
        {
            var signBit = input.PositBits >> (Size - 1);
            var maskOfSignBits = 0 - signBit;

            return new Posit32((input.PositBits ^ maskOfSignBits) + signBit, fromBitMask: true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static partial uint GetTwosComplement(uint bits) =>
            ~bits + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public partial int CompareTo(Posit32 obj)
        {
            var left = unchecked((int)PositBits);
            var right = unchecked((int)obj.PositBits);
            if (left < right) return -1;
            if (left > right) return 1;
            return 0;
        }

        public const uint NaRBitMask = NaNBitMask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNaR() => PositBits == NaRBitMask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNegative() => (PositBits & SignBitMask) != EmptyBitMask && !IsNaR();
    }
}
