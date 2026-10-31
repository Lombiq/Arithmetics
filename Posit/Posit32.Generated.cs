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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Posit32 Negate(Posit32 input) =>
            new Posit32(GetTwosComplement(input.PositBits), fromBitMask: true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Posit32 Sign(Posit32 input)
        {
            if (input.IsNaR() || input.IsZero()) return input;
            return new Posit32(
                input.IsPositive() ? FirstRegimeBitBitMask : GetTwosComplement(FirstRegimeBitBitMask),
                fromBitMask: true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Posit32 NearestInt(Posit32 input)
        {
            if (input.IsNaR() || input.IsZero()) return input;
            var magnitude = Abs(input);
            var negative = !input.IsPositive();
            uint resultBits;
            if (magnitude.PositBits < FirstRegimeBitBitMask)
            {
                const uint halfBits = FirstRegimeBitBitMask - (1 << (Size - 3 - MaximumExponentSize));
                resultBits = magnitude.PositBits > halfBits ? FirstRegimeBitBitMask : EmptyBitMask;
            }
            else
            {
                var scale = magnitude.CalculateScaleFactor();
                var fractionalBits = (int)magnitude.FractionSizeWithoutSignCheck() - scale;
                // Integral values can exceed every machine integer type; preserve their encoding.
                if (fractionalBits <= 0) return input;

                var unit = 1U << fractionalBits;
                var remainder = magnitude.PositBits & (unit - 1);
                if (remainder == 0) return input;
                var truncated = magnitude.PositBits & ~(unit - 1);
                // At scale zero the integer's one bit is implicit, not an encoding bit.
                var odd = scale == 0 || ((truncated >> fractionalBits) & 1) != 0;
                if (remainder > (unit >> 1) || (remainder == (unit >> 1) && odd)) truncated += unit;
                resultBits = (uint)truncated;
            }

            return new Posit32(negative ? GetTwosComplement(resultBits) : resultBits, fromBitMask: true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Posit32 Ceiling(Posit32 input)
        {
            if (input.IsNaR() || input.IsZero()) return input;
            var magnitude = Abs(input);
            var negative = !input.IsPositive();
            uint resultBits;
            if (magnitude.PositBits < FirstRegimeBitBitMask)
            {
                resultBits = !negative ? FirstRegimeBitBitMask : EmptyBitMask;
            }
            else
            {
                var scale = magnitude.CalculateScaleFactor();
                var fractionalBits = (int)magnitude.FractionSizeWithoutSignCheck() - scale;
                // Integral values can exceed every machine integer type; preserve their encoding.
                if (fractionalBits <= 0) return input;

                var unit = 1U << fractionalBits;
                var remainder = magnitude.PositBits & (unit - 1);
                if (remainder == 0) return input;
                var truncated = magnitude.PositBits & ~(unit - 1);
                if (!negative) truncated += unit;
                resultBits = (uint)truncated;
            }

            return new Posit32(negative ? GetTwosComplement(resultBits) : resultBits, fromBitMask: true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Posit32 Floor(Posit32 input)
        {
            if (input.IsNaR() || input.IsZero()) return input;
            var magnitude = Abs(input);
            var negative = !input.IsPositive();
            uint resultBits;
            if (magnitude.PositBits < FirstRegimeBitBitMask)
            {
                resultBits = negative ? FirstRegimeBitBitMask : EmptyBitMask;
            }
            else
            {
                var scale = magnitude.CalculateScaleFactor();
                var fractionalBits = (int)magnitude.FractionSizeWithoutSignCheck() - scale;
                // Integral values can exceed every machine integer type; preserve their encoding.
                if (fractionalBits <= 0) return input;

                var unit = 1U << fractionalBits;
                var remainder = magnitude.PositBits & (unit - 1);
                if (remainder == 0) return input;
                var truncated = magnitude.PositBits & ~(unit - 1);
                if (negative) truncated += unit;
                resultBits = (uint)truncated;
            }

            return new Posit32(negative ? GetTwosComplement(resultBits) : resultBits, fromBitMask: true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Posit32 Next(Posit32 input) =>
            new Posit32(unchecked((uint)(input.PositBits + 1)), fromBitMask: true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Posit32 Prior(Posit32 input) =>
            new Posit32(unchecked((uint)(input.PositBits - 1)), fromBitMask: true);

        // Adds the exact signed operands, then rounds once as sections 4.1 and 5.4 of the Posit Standard (2022) require.
        // Significands keep the hidden bit at bit Size - 2, so only operands whose scales differ by more than three lose
        // bits to alignment. Their sum then exceeds half the larger operand, putting the lost bits below every (Size + 1)-bit
        // rounding boundary near the sum, so keeping their OR as a sticky bit preserves the correctly rounded result.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static partial uint AddBits(uint leftBits, uint rightBits)
        {
            if (leftBits == NaNBitMask || rightBits == NaNBitMask) return NaNBitMask;
            if (leftBits == EmptyBitMask) return rightBits;
            if (rightBits == EmptyBitMask) return leftBits;

            const ulong topBit = 1UL << 63;
            var leftIsNegative = (leftBits & SignBitMask) != EmptyBitMask;
            var rightIsNegative = (rightBits & SignBitMask) != EmptyBitMask;
            ulong left = leftIsNegative ? GetTwosComplement(leftBits) : leftBits;
            ulong right = rightIsNegative ? GetTwosComplement(rightBits) : rightBits;
            var resultIsNegative = leftIsNegative;
            if (left < right)
            {
                var smallerMagnitude = left;
                left = right;
                right = smallerMagnitude;
                resultIsNegative = rightIsNegative;
            }

            // Decode both magnitudes from their top-aligned regime; truncated exponent and fraction bits read as 0.
            var leftAligned = left << 33;
            var leftRegimeIsPositive = leftAligned >= topBit;
            var leftRunProbe = leftRegimeIsPositive ? ~leftAligned : leftAligned;
            var leftRunLength = 0;
            while (leftRunProbe < topBit)
            {
                leftRunProbe <<= 1;
                leftRunLength++;
            }

            var leftRest = leftAligned << (leftRunLength + 1);
            var leftScale = ((leftRegimeIsPositive ? leftRunLength - 1 : -leftRunLength) << 2) +
                (int)(leftRest >> 62);
            var leftSignificand = (1UL << 30) | ((leftRest << 2) >> 34);

            var rightAligned = right << 33;
            var rightRegimeIsPositive = rightAligned >= topBit;
            var rightRunProbe = rightRegimeIsPositive ? ~rightAligned : rightAligned;
            var rightRunLength = 0;
            while (rightRunProbe < topBit)
            {
                rightRunProbe <<= 1;
                rightRunLength++;
            }

            var rightRest = rightAligned << (rightRunLength + 1);
            var rightScale = ((rightRegimeIsPositive ? rightRunLength - 1 : -rightRunLength) << 2) +
                (int)(rightRest >> 62);
            var rightSignificand = (1UL << 30) | ((rightRest << 2) >> 34);

            var scaleDifference = leftScale - rightScale;
            ulong alignedRight = 1;
            if (scaleDifference < 31)
            {
                alignedRight = rightSignificand >> scaleDifference;
                if ((alignedRight << scaleDifference) != rightSignificand) alignedRight |= 1;
            }

            var sum = leftIsNegative == rightIsNegative
                ? leftSignificand + alignedRight
                : leftSignificand - alignedRight;
            if (sum == 0) return EmptyBitMask;

            var mostSignificantBit = 31;
            while ((sum >> mostSignificantBit) == 0) mostSignificantBit--;

            var scale = leftScale + mostSignificantBit - 30;
            ulong magnitude;
            if (scale >= 120)
            {
                magnitude = SignBitMask - 1;
            }
            else
            {
                // Every posit value is a multiple of minPos, so a nonzero sum never needs the minPos clamp. Lay out the
                // unbounded encoding from the top bit and round it to nearest, ties to an even encoding: since encodings
                // are ordered like values, this is the section 4.1 comparison with the U1 boundary.
                var regimeK = scale >> 2;
                var regime = regimeK >= 0 ? ~(ulong.MaxValue >> (regimeK + 1)) : topBit >> -regimeK;
                var regimeLength = regimeK >= 0 ? regimeK + 2 : 1 - regimeK;
                var exponentAndFraction = ((ulong)(scale & 3) << 62) |
                    (((sum << (63 - mostSignificantBit)) & (topBit - 1)) >> 1);
                var encoding = regime | (exponentAndFraction >> regimeLength);
                magnitude = encoding >> 33;
                if ((encoding & (1UL << 32)) != 0 &&
                    ((encoding & ((1UL << 32) - 1)) != 0 || (magnitude & 1) != 0))
                {
                    magnitude++;
                }
            }

            var resultBits = (uint)magnitude;
            return resultIsNegative ? GetTwosComplement(resultBits) : resultBits;
        }
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
