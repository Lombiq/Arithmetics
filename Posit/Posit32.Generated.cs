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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint ConvertE2PrecisionBits(uint sourceBits, int sourceSize, int destinationSize)
        {
            if (sourceSize <= destinationSize) return sourceBits << (destinationSize - sourceSize);

            var sourceNaR = 1U << (sourceSize - 1);
            var destinationSign = 1U << (destinationSize - 1);
            var destinationMask = (destinationSign << 1) - 1;
            if (sourceBits == sourceNaR) return destinationSign;
            if (sourceBits == 0) return 0;

            var shift = sourceSize - destinationSize;
            var sourceValue = sourceSize == 32
                ? unchecked((int)sourceBits)
                : (int)(sourceBits << (32 - sourceSize)) >> (32 - sourceSize);
            var destinationMaximum = destinationSign - 1;
            var maximumAtSourcePrecision = (int)destinationMaximum << shift;
            if (sourceValue > maximumAtSourcePrecision) return destinationMaximum;
            if (sourceValue < -maximumAtSourcePrecision) return unchecked(0U - destinationMaximum) & destinationMask;

            var minimumAtSourcePrecision = 1 << shift;
            if (sourceValue > -minimumAtSourcePrecision && sourceValue < minimumAtSourcePrecision)
            {
                return sourceValue < 0 ? destinationMask : 1;
            }

            var discardedMask = (1U << shift) - 1;
            if ((sourceBits & discardedMask) == 0) return (sourceBits >> shift) & destinationMask;

            var lower = sourceValue >> shift;
            var lowerBits = (uint)lower & destinationMask;
            var extendedMask = (1U << (destinationSize + 1)) - 1;
            var boundaryBits = ((lowerBits << 1) | 1) & extendedMask;
            var boundarySign = 1U << destinationSize;
            var boundary = (boundaryBits & boundarySign) == 0
                ? (int)boundaryBits
                : unchecked((int)(boundaryBits | ~extendedMask));
            var boundaryAtSourcePrecision = boundary << (shift - 1);

            // Signed encoding order is numeric order; U1 is the literal section 4.1 boundary.
            return sourceValue < boundaryAtSourcePrecision ||
                (sourceValue == boundaryAtSourcePrecision && (lowerBits & 1) == 0)
                    ? lowerBits
                    : (lowerBits + 1) & destinationMask;
        }

        public static explicit operator Posit32(Posit8E2 value) =>
            new Posit32(ConvertE2PrecisionBits(value.PositBits, 8, 32), fromBitMask: true);

        public static explicit operator Posit32(Posit16E2 value) =>
            new Posit32(ConvertE2PrecisionBits(value.PositBits, 16, 32), fromBitMask: true);

        public static explicit operator Posit32(Posit32E2 value) =>
            new Posit32(value.PositBits, fromBitMask: true);

        public static explicit operator Posit8E2(Posit32 value) =>
            new Posit8E2((byte)ConvertE2PrecisionBits(value.PositBits, 32, 8), fromBitMask: true);

        public static explicit operator Posit16E2(Posit32 value) =>
            new Posit16E2((ushort)ConvertE2PrecisionBits(value.PositBits, 32, 16), fromBitMask: true);

        public static explicit operator Posit32E2(Posit32 value) =>
            new Posit32E2(value.PositBits, fromBitMask: true);

    }
}
