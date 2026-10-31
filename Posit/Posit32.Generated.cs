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

    }
}
