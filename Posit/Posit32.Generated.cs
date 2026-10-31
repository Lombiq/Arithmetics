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

        // Standard (2022), sections 4.1 and 5.4, for every E2 size (8, 16 or 32 bits in the low bits of a uint).
        // The significands have at most 28 bits, so their 64-bit product is exact and is rounded only once.
        // Rounding the long-form encoding (regime, exponent, fraction; zero beyond the LSB) to nearest with ties
        // to even is the literal U1 rule: encodings are monotonic and U1 is U followed by a 1. Negation maps u, w,
        // U1 and the parity of U consistently, so the magnitude is rounded and then negated. NaR wins over zero.
        // A scale of at least 4 * size - 8 clamps to maxPos and a scale below -(4 * size - 8) to minPos; in between
        // the regime terminator is inside the payload, so rounding reaches neither 0 nor NaR.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint MultiplyE2Bits(uint leftBits, uint rightBits, int size)
        {
            var narBits = 1U << (size - 1);
            if (leftBits == narBits || rightBits == narBits) return narBits;
            if (leftBits == 0 || rightBits == 0) return 0;

            var sizeMask = unchecked((narBits << 1) - 1);
            var leftMagnitude = (leftBits & narBits) == 0 ? leftBits : unchecked(0U - leftBits) & sizeMask;
            var rightMagnitude = (rightBits & narBits) == 0 ? rightBits : unchecked(0U - rightBits) & sizeMask;

            // A magnitude is 2^(4 * regime + exponent) * 1.fraction. The payload is shifted up to bit 63 and the
            // regime run with its terminator is shifted out, so missing exponent and fraction bits read as zero.
            var leftRun = PositHelper.LengthOfRunOfBits(leftMagnitude, (byte)(size - 1));
            var leftPayload = ((ulong)leftMagnitude << (65 - size)) << (leftRun + 1);
            var leftScale = (((leftMagnitude >> (size - 2)) == 0 ? -leftRun : leftRun - 1) * 4) + (int)(leftPayload >> 62);
            var leftSignificand = 0x8000_0000U | (uint)((leftPayload << 2) >> 33);

            var rightRun = PositHelper.LengthOfRunOfBits(rightMagnitude, (byte)(size - 1));
            var rightPayload = ((ulong)rightMagnitude << (65 - size)) << (rightRun + 1);
            var rightScale = (((rightMagnitude >> (size - 2)) == 0 ? -rightRun : rightRun - 1) * 4) + (int)(rightPayload >> 62);
            var rightSignificand = 0x8000_0000U | (uint)((rightPayload << 2) >> 33);

            // The product of the 32-bit significands is exact, in [2^62, 2^64); carry tells if it is at least 2^63.
            var product = (ulong)leftSignificand * rightSignificand;
            var carry = (int)(product >> 63);
            var significand = product << (1 - carry);
            var scale = leftScale + rightScale + carry;

            var maximumScale = (4 * size) - 8;
            uint magnitude;
            if (scale >= maximumScale) magnitude = narBits - 1;
            else if (scale < -maximumScale) magnitude = 1;
            else
            {
                var regime = scale >> 2;
                var regimeLength = regime >= 0 ? regime + 2 : 1 - regime;
                var regimeBits = regime >= 0 ? ((1U << (regime + 1)) - 1) << 1 : 1U;
                var tailLength = size - regimeLength;

                // The exponent and the fraction below the hidden bit, left-aligned in 64 bits; the lowest fraction
                // bit goes to the sticky bit, so no product bit is dropped before the rounding.
                var tail = ((ulong)(scale & 3) << 62) | ((significand << 1) >> 2);
                var encoding = (regimeBits << tailLength) | (uint)(tail >> (64 - tailLength));
                var sticky = (tail << tailLength) != 0 || (significand & 1) != 0;
                magnitude = (encoding >> 1) + ((encoding & 1) != 0 && (sticky || (encoding & 2) != 0) ? 1U : 0U);
            }

            return ((leftBits ^ rightBits) & narBits) == 0 ? magnitude : unchecked(0U - magnitude) & sizeMask;
        }

    }
}
