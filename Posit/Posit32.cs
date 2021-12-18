using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Lombiq.Arithmetics
{
    [SuppressMessage(
        "Major Bug",
        "S1244:Floating point numbers should not be tested for equality",
        Justification = "Only test zero, should be exactly zero.")]
    public readonly struct Posit32 : IComparable, IConvertible, IFormattable, IEquatable<Posit32>, IComparable<Posit32>
    {
        public uint PositBits { get; }

        #region Posit structure

        public const byte MaximumExponentSize = 2;

        public const byte Size = 32;

        public const byte Useed = 1 << (1 << MaximumExponentSize);

        public const byte FirstRegimeBitIndex = Size - 2;

        public const byte FirstRegimeBitPosition = Size - 1;

        public const byte SizeMinusFixedBits = Size - 4;

        public const short QuireSize = 512;

        #endregion

        #region Posit Masks

        public const uint SignBitMask = 1U << (Size - 1);

        public const uint FirstRegimeBitBitMask = 1U << (Size - 2);

        public const uint EmptyBitMask = 0;

        public const uint MaxValueBitMask = uint.MaxValue;

        public const uint MinValueBitMask = uint.MinValue;

        public const uint NaNBitMask = SignBitMask;

        public const uint Float32ExponentMask = 0x_7f80_0000;

        public const uint Float32FractionMask = 0x_007f_ffff;

        public const uint Float32HiddenBitMask = 0x_0080_0000;

        public const ulong Double64FractionMask = 0x_000F_FFFF_FFFF_FFFF;

        public const ulong Double64ExponentMask = 0x_7FF0_0000_0000_0000;

        public const ulong Double64HiddenBitMask = 0x_0010_0000_0000_0000;

        #endregion

        #region Posit constructors

        public Posit32(uint bits, bool fromBitMask) =>
            PositBits = fromBitMask ? bits : new Posit32(bits).PositBits;

        public Posit32(Quire q)
        {
            PositBits = NaNBitMask;
            var sign = false;
            var positionOfMostSigniFicantOne = 511;
            var firstSegment = (ulong)(q >> (QuireSize - 64));
            if (firstSegment >= 0x_8000_0000_0000_0000)
            {
                q = ~q;
                q += 1;
                sign = true;
            }

            firstSegment = (ulong)(q >> (QuireSize - 64));
            while (firstSegment < 0x_8000_0000_0000_0000)
            {
                q <<= 1;
                positionOfMostSigniFicantOne -= 1;
                firstSegment = (ulong)(q >> (QuireSize - 64));
            }

            var scaleFactor = positionOfMostSigniFicantOne - 240;
            if (positionOfMostSigniFicantOne == 0)
            {
                PositBits = 0;
                return;
            }

            var resultRegimeKValue = scaleFactor / (1 << MaximumExponentSize);
            var resultExponentBits = (uint)(scaleFactor % (1 << MaximumExponentSize));
            if (resultExponentBits < 0)
            {
                resultRegimeKValue -= 1;
                resultExponentBits += 1 << MaximumExponentSize;
            }

            PositBits = AssemblePositBitsWithRounding(sign, resultRegimeKValue, resultExponentBits, (uint)(q >> (QuireSize - 32)));
        }

        public Posit32(uint value)
        {
            PositBits = value;
            if (value == 0) return;

            var exponentValue = (byte)(GetMostSignificantOnePosition(PositBits) - 1);

            byte kValue = 0;
            while (exponentValue >= 1 << MaximumExponentSize && kValue < Size - 1)
            {
                exponentValue -= 1 << MaximumExponentSize;
                kValue++;
            }

            PositBits = AssemblePositBitsWithRounding(false, kValue, exponentValue, PositBits);
        }

        public Posit32(int value) =>
            PositBits = value >= 0
                ? new Posit32((uint)value).PositBits
                : GetTwosComplement(new Posit32((uint)-value).PositBits);

        public Posit32(float floatBits)
        {
            PositBits = NaNBitMask;
            if (float.IsInfinity(floatBits) || float.IsNaN(floatBits))
            {
                return;
            }

            if (floatBits == 0)
            {
                PositBits = 0;
                return;
            }

            uint uintRepresentation;
            unsafe
            {
                uint* floatPointer = (uint*)&floatBits;
                uintRepresentation = *floatPointer;
            }

            var signBit = (uintRepresentation & SignBitMask) != 0;
            int scaleFactor = (int)((uintRepresentation << 1) >> 24) - 127;
            var fractionBits = uintRepresentation & Float32FractionMask;

            // Adding the hidden bit if it is one.
            if (scaleFactor != -127) fractionBits += Float32HiddenBitMask;
            else scaleFactor += 1;

            var regimeKValue = scaleFactor / (1 << MaximumExponentSize);
            if (scaleFactor < 0) regimeKValue--;

            var exponentValue = (uint)(scaleFactor - (regimeKValue * (1 << MaximumExponentSize)));
            if (exponentValue == 1 << MaximumExponentSize)
            {
                regimeKValue += 1;
                exponentValue = 0;
            }

            if (regimeKValue < -(Size - 2))
            {
                regimeKValue = -(Size - 2);
                exponentValue = 0;
            }

            if (regimeKValue > (Size - 2))
            {
                regimeKValue = Size - 2;
                exponentValue = 0;
            }

            PositBits = AssemblePositBitsWithRounding(signBit, regimeKValue, exponentValue, fractionBits);
        }

        public Posit32(double doubleBits)
        {
            PositBits = NaNBitMask;
            if (double.IsInfinity(doubleBits) || double.IsNaN(doubleBits))
            {
                return;
            }

            if (doubleBits == 0)
            {
                PositBits = 0;
                return;
            }

            ulong ulongRepresentation;
            unsafe
            {
                ulong* doublePointer = (ulong*)&doubleBits;
                ulongRepresentation = *doublePointer;
            }

            var signBit = (ulongRepresentation & ((ulong)SignBitMask << 32)) != 0;
            int scaleFactor = (int)((ulongRepresentation << 1) >> 53) - 1_023;
            uint fractionBits = (uint)((ulongRepresentation & Double64FractionMask) >> 21);

            // Adding the hidden bit if it is one.
            if (scaleFactor != -1_023) fractionBits += (uint)(Double64HiddenBitMask >> 21);
            else scaleFactor += 1;

            var regimeKValue = scaleFactor / (1 << MaximumExponentSize);
            if (scaleFactor < 0) regimeKValue--;

            var exponentValue = (uint)(scaleFactor - (regimeKValue * (1 << MaximumExponentSize)));
            if (exponentValue == 1 << MaximumExponentSize)
            {
                regimeKValue += 1;
                exponentValue = 0;
            }

            if (regimeKValue < -(Size - 2))
            {
                regimeKValue = -(Size - 2);
                exponentValue = 0;
            }

            if (regimeKValue > (Size - 2))
            {
                regimeKValue = Size - 2;
                exponentValue = 0;
            }

            PositBits = AssemblePositBitsWithRounding(signBit, regimeKValue, exponentValue, fractionBits);
        }

        #endregion

        #region Posit numeric states

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPositive() => (PositBits & SignBitMask) == EmptyBitMask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNaN() => PositBits == NaNBitMask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero() => PositBits == EmptyBitMask;

        #endregion

        #region Methods to handle parts of the Posit

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint EncodeRegimeBits(int regimeKValue)
        {
            uint regimeBits;
            if (regimeKValue > 0)
            {
                regimeBits = (uint)(1 << (regimeKValue + 1)) - 1;
                regimeBits <<= Size - GetMostSignificantOnePosition(regimeBits) - 1;
            }
            else
            {
                regimeBits = FirstRegimeBitBitMask >> -regimeKValue;
            }

            return regimeBits;
        }

        [SuppressMessage(
            "Critical Code Smell",
            "S3776:Cognitive Complexity of methods should not be too high",
            Justification = "It's not so complicated.")]
        public static uint AssemblePositBitsWithRounding(bool signBit, int regimeKValue, uint exponentBits, uint fractionBits)
        {
            // Calculating the regime.
            var wholePosit = EncodeRegimeBits(regimeKValue);

            // Attaching the exponent.
            var regimeLength = LengthOfRunOfBits(wholePosit, FirstRegimeBitPosition);

            var exponentShiftedLeftBy = (sbyte)SizeMinusFixedBits - regimeLength;
            wholePosit += exponentShiftedLeftBy >= 0 ? exponentBits << exponentShiftedLeftBy : exponentBits >> -exponentShiftedLeftBy;

            // Calculating rounding.
            if (exponentShiftedLeftBy < 0)
            {
                if (exponentShiftedLeftBy <= SizeMinusFixedBits) exponentBits <<= Size + exponentShiftedLeftBy;
                else exponentBits >>= Size + exponentShiftedLeftBy;

                if (exponentBits < SignBitMask) return signBit ? GetTwosComplement(wholePosit) : wholePosit;

                if (exponentBits == SignBitMask) wholePosit += wholePosit & 1;
                else wholePosit += 1;

                return signBit ? GetTwosComplement(wholePosit) : wholePosit;
            }

            var fractionMostSignificantOneIndex = GetMostSignificantOnePosition(fractionBits) - 1;

            // Hiding the hidden bit. (It is always one.)
            fractionBits = SetZero(fractionBits, (ushort)fractionMostSignificantOneIndex);

            var fractionShiftedLeftBy = SizeMinusFixedBits - fractionMostSignificantOneIndex - regimeLength;
            // Attaching the fraction.
            wholePosit += fractionShiftedLeftBy >= 0 ? fractionBits << fractionShiftedLeftBy : fractionBits >> -fractionShiftedLeftBy;
            // Calculating rounding.
            if (fractionShiftedLeftBy < 0)
            {
                if (Size + fractionShiftedLeftBy >= 0) fractionBits <<= Size + fractionShiftedLeftBy;
                else fractionBits >>= -(Size - fractionShiftedLeftBy);

                if (fractionBits >= SignBitMask)
                {
                    wholePosit += fractionBits == SignBitMask ? wholePosit & 1 : 1;
                }
            }

            return signBit ? GetTwosComplement(wholePosit) : wholePosit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte GetRegimeKValue()
        {
            var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            var lengthOfRunOfBits = LengthOfRunOfBits(bits, FirstRegimeBitPosition);

            return (bits & FirstRegimeBitBitMask) == EmptyBitMask
                ? (sbyte)-lengthOfRunOfBits
                : (sbyte)(lengthOfRunOfBits - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte GetRegimeKValueWithoutSignCheck(byte lengthOfRunOfBits) =>
            (PositBits & FirstRegimeBitBitMask) == EmptyBitMask
                ? (sbyte)-lengthOfRunOfBits
                : (sbyte)(lengthOfRunOfBits - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short CalculateScaleFactor()
        {
            var regimeKvalue = GetRegimeKValue();
            return (regimeKvalue == -FirstRegimeBitPosition) ? (short)0 : (short)((regimeKvalue * (1 << MaximumExponentSize)) + GetExponentValue());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ExponentSize()
        {
            var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            var lengthOfRunOfBits = LengthOfRunOfBits(bits, FirstRegimeBitPosition);
            var result = (byte)(Size - lengthOfRunOfBits - 1);

            if (lengthOfRunOfBits + 2 <= Size)
            {
                result = Size - (lengthOfRunOfBits + 2) > MaximumExponentSize
                     ? MaximumExponentSize
                     : (byte)(Size - (lengthOfRunOfBits + 2));
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ExponentSizeWithoutSignCheck()
        {
            var lengthOfRunOfBits = LengthOfRunOfBits(PositBits, FirstRegimeBitPosition);
            return Size - (lengthOfRunOfBits + 2) > MaximumExponentSize
                ? MaximumExponentSize : (byte)(Size - (lengthOfRunOfBits + 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetExponentValue()
        {
            var exponentMask = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            var exponentSize = ExponentSize();
            exponentMask = (exponentMask >> (int)FractionSize())
                            << (Size - exponentSize)
                            >> (Size - MaximumExponentSize);
            return exponentSize == 0 ? 0 : exponentMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetExponentValueWithoutSignCheck() =>
            (PositBits >> (int)FractionSizeWithoutSignCheck()) << (Size - ExponentSize()) >> (Size - MaximumExponentSize);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetExponentValueWithoutSignCheck(uint fractionSize) =>
            (PositBits >> (int)fractionSize) << (Size - ExponentSize()) >> (Size - MaximumExponentSize);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint FractionSize()
        {
            var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            var fractionSize = Size - (LengthOfRunOfBits(bits, FirstRegimeBitPosition) + 2 + MaximumExponentSize);
            return fractionSize > 0 ? (uint)fractionSize : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint FractionSizeWithoutSignCheck()
        {
            var fractionSize = Size - (LengthOfRunOfBits(PositBits, FirstRegimeBitPosition) + 2 + MaximumExponentSize);
            return fractionSize > 0 ? (uint)fractionSize : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint FractionSizeWithoutSignCheck(byte lengthOfRunOfBits)
        {
            var fractionSize = Size - (lengthOfRunOfBits + 2 + MaximumExponentSize);
            return fractionSize > 0 ? (uint)fractionSize : 0;
        }

        #endregion

        #region Helper methods for operations and conversions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Fraction()
        {
            var fractionSize = FractionSize();
            var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            return bits << (int)(Size - fractionSize)
                          >> (int)(Size - fractionSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint FractionWithHiddenBit()
        {
            var fractionSize = FractionSize();
            var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            var result = bits << (int)(Size - fractionSize)
                         >> (int)(Size - fractionSize);
            return fractionSize == 0 ? 1 : SetOne(result, (ushort)fractionSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint FractionWithHiddenBit(uint fractionSize)
        {
            var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            var result = bits << (int)(Size - fractionSize)
                         >> (int)(Size - fractionSize);
            return SetOne(result, (ushort)fractionSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint FractionWithHiddenBitWithoutSignCheck()
        {
            var fractionSizeWithoutSignCheck = FractionSizeWithoutSignCheck();
            var result = PositBits << (int)(Size - fractionSizeWithoutSignCheck)
                         >> (int)(Size - fractionSizeWithoutSignCheck);
            return SetOne(result, (ushort)fractionSizeWithoutSignCheck);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint FractionWithHiddenBitWithoutSignCheck(uint fractionSize)
        {
            var numberOfNonFractionBits = (int)(Size - fractionSize);
            var result = PositBits << numberOfNonFractionBits
                         >> numberOfNonFractionBits;
            return SetOne(result, (ushort)fractionSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short CalculateScaleFactor(sbyte regimeKValue, uint exponentValue, byte maximumExponentSize) =>
            (short)((regimeKValue * (1 << maximumExponentSize)) + exponentValue);

        public static Quire MultiplyIntoQuire(Posit32 left, Posit32 right)
        {
            if (left.IsZero() || right.IsZero()) return new Quire((ushort)QuireSize);
            if (left.IsNaN() || right.IsNaN()) return new Quire(1, (ushort)QuireSize) << (QuireSize - 1);
            var leftIsPositive = left.IsPositive();
            var rightIsPositive = right.IsPositive();
            var resultSignBit = leftIsPositive != rightIsPositive;

            left = Abs(left);
            right = Abs(right);
            var leftFractionSize = left.FractionSizeWithoutSignCheck();
            var rightFractionSize = right.FractionSizeWithoutSignCheck();

            var longResultFractionBits = left.FractionWithHiddenBitWithoutSignCheck() *
                (ulong)right.FractionWithHiddenBitWithoutSignCheck();
            var fractionSizeChange = GetMostSignificantOnePosition(longResultFractionBits) - (leftFractionSize + rightFractionSize + 1);
            var scaleFactor =
                CalculateScaleFactor(left.GetRegimeKValue(), left.GetExponentValue(), MaximumExponentSize) +
                CalculateScaleFactor(right.GetRegimeKValue(), right.GetExponentValue(), MaximumExponentSize);

            scaleFactor += (int)fractionSizeChange;

            var quireArray = new ulong[QuireSize / 64];
            quireArray[0] = longResultFractionBits;
            var resultQuire = new Quire(quireArray);
            resultQuire <<= 240 - GetMostSignificantOnePosition(longResultFractionBits) + 1 + scaleFactor;

            return !resultSignBit ? resultQuire : (~resultQuire) + 1;
        }

        #endregion

        #region Bit level Helper Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetMostSignificantOnePosition(uint bits)
        {
            byte position = 0;
            while (bits != 0)
            {
                bits >>= 1;
                position++;
            }

            return position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetMostSignificantOnePosition(ulong bits)
        {
            byte position = 0;
            while (bits != 0)
            {
                bits >>= 1;
                position++;
            }

            return position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Posit32 Abs(Posit32 input)
        {
            var signBit = input.PositBits >> (Size - 1);
            var maskOfSignBits = 0 - signBit;
            return new Posit32((input.PositBits ^ maskOfSignBits) + signBit, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint SetOne(uint bits, ushort index) => bits | (uint)(1 << index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetZero(uint bits, ushort index) => bits & (uint)~(1 << index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte LengthOfRunOfBits(uint bits, byte startingPosition)
        {
            byte length = 1;
            bits <<= Size - startingPosition;
            var startingBit = (bits >> 31) & 1;
            bits <<= 1;
            for (var i = 0; i < startingPosition && bits >> 31 == startingBit; i++)
            {
                bits <<= 1;
                length++;
            }

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetTwosComplement(uint bits) => ~bits + 1;

        #endregion

        #region Algebraic functions

        public static Posit32 Sqrt(Posit32 number)
        {
            if (number.IsNaN() || number.IsZero()) return number;
            if (!number.IsPositive()) return new Posit32(NaNBitMask, true);

            var inputScaleFactor = number.CalculateScaleFactor();
            var inputFractionWithHiddenBit = number.FractionWithHiddenBitWithoutSignCheck();

            if ((inputScaleFactor & 1) != 0)
            {
                // if the scaleFactor is odd, shift the number to make it even
                inputScaleFactor -= 1;
                inputFractionWithHiddenBit += inputFractionWithHiddenBit;
            }

            inputScaleFactor >>= 1;

            uint resultFractionBits = 0;
            uint startingEstimate = 0;
            uint temporaryEstimate;
            uint estimateMaskingBit = 1U << (int)number.FractionSizeWithoutSignCheck();

            while (estimateMaskingBit != 0)
            {
                temporaryEstimate = startingEstimate + estimateMaskingBit;
                if (temporaryEstimate <= inputFractionWithHiddenBit)
                {
                    startingEstimate = temporaryEstimate + estimateMaskingBit;
                    inputFractionWithHiddenBit -= temporaryEstimate;
                    resultFractionBits += estimateMaskingBit;
                }

                inputFractionWithHiddenBit += inputFractionWithHiddenBit;
                estimateMaskingBit >>= 1;
            }

            var resultRegimeKValue = inputScaleFactor / (1 << MaximumExponentSize);
            var resultExponentBits = inputScaleFactor % (1 << MaximumExponentSize);
            if (resultExponentBits < 0)
            {
                resultRegimeKValue -= 1;
                resultExponentBits += 1 << MaximumExponentSize;
            }

            return new Posit32(AssemblePositBitsWithRounding(false, resultRegimeKValue, (uint)resultExponentBits, resultFractionBits), true);
        }

        #endregion

        #region Fused operations

        public static Posit32 FusedSum(Posit32[] posits)
        {
            var resultQuire = new Quire((ushort)QuireSize);

            for (var i = 0; i < posits.Length; i++)
            {
                if (posits[i].IsNaN()) return posits[i];
                resultQuire += (Quire)posits[i];
            }

            return new Posit32(resultQuire);
        }

        public static Quire FusedSum(Posit32[] posits, Quire startingValue)
        {
            var quireNaNMask = new Quire(1, (ushort)QuireSize) << (QuireSize - 1);

            if (startingValue == quireNaNMask) return quireNaNMask;
            for (var i = 0; i < posits.Length; i++)
            {
                if (posits[i].IsNaN()) return quireNaNMask;
                startingValue += (Quire)posits[i];
            }

            return startingValue;
        }

        public static Posit32 FusedDotProduct(Posit32[] positArray1, Posit32[] positArray2)
        {
            if (positArray1.Length != positArray2.Length) return new Posit32(NaNBitMask, true);

            var resultQuire = new Quire((ushort)QuireSize);

            for (var i = 0; i < positArray1.Length; i++)
            {
                if (positArray1[i].IsNaN()) return positArray1[i];
                if (positArray2[i].IsNaN()) return positArray2[i];
                resultQuire += MultiplyIntoQuire(positArray1[i], positArray2[i]);
            }

            return new Posit32(resultQuire);
        }

        public static Posit32 FusedMultiplyAdd(Posit32 a, Posit32 b, Posit32 c)
        {
            var positArray1 = new Posit32[2];
            var positArray2 = new Posit32[2];

            positArray1[0] = a;
            positArray1[1] = new Posit32(1);
            positArray2[0] = b;
            positArray2[1] = c;

            return FusedDotProduct(positArray1, positArray2);
        }

        public static Posit32 FusedAddMultiply(Posit32 a, Posit32 b, Posit32 c)
        {
            var positArray1 = new Posit32[2];
            var positArray2 = new Posit32[2];

            positArray1[0] = a;
            positArray1[1] = b;
            positArray2[0] = c;
            positArray2[1] = c;

            return FusedDotProduct(positArray1, positArray2);
        }

        public static Posit32 FusedMultiplyMultiplySubtract(Posit32 a, Posit32 b, Posit32 c, Posit32 d)
        {
            var positArray1 = new Posit32[2];
            var positArray2 = new Posit32[2];

            positArray1[0] = a;
            positArray1[1] = -c;
            positArray2[0] = b;
            positArray2[1] = d;

            return FusedDotProduct(positArray1, positArray2);
        }
        #endregion

        #region Operators

        [SuppressMessage(
            "Critical Code Smell",
            "S3776:Cognitive Complexity of methods should not be too high",
            Justification = "Breaking this method up wouldn't be too helpful.")]
        public static Posit32 operator +(Posit32 left, Posit32 right)
        {
            // Handling special cases first.
            if (left.IsNaN()) return left;
            if (right.IsNaN()) return right;
            if (left.IsZero()) return right;
            if (right.IsZero()) return left;

            var leftSignBit = left.PositBits >> (Size - 1);
            var leftMaskOfSignBits = 0 - leftSignBit;
            var leftAbsoluteValue = new Posit32((left.PositBits ^ leftMaskOfSignBits) + leftSignBit, true);

            var rightSignBit = right.PositBits >> (Size - 1);
            var rightMaskOfSignBits = 0 - rightSignBit;
            var rightAbsoluteValue = new Posit32((right.PositBits ^ rightMaskOfSignBits) + rightSignBit, true);

            var leftLengthOfRunOfBits = LengthOfRunOfBits(leftAbsoluteValue.PositBits, FirstRegimeBitPosition);
            var rightLengthOfRunOfBits = LengthOfRunOfBits(rightAbsoluteValue.PositBits, FirstRegimeBitPosition);

            var leftFractionSize = leftAbsoluteValue.FractionSizeWithoutSignCheck(leftLengthOfRunOfBits);
            var rightFractionSize = rightAbsoluteValue.FractionSizeWithoutSignCheck(rightLengthOfRunOfBits);

            var signBitsMatch = leftSignBit == rightSignBit;
            sbyte leftRegimeKValue = leftAbsoluteValue.GetRegimeKValueWithoutSignCheck(leftLengthOfRunOfBits);
            uint leftExponentValue = leftAbsoluteValue.GetExponentValueWithoutSignCheck(leftFractionSize);
            sbyte rightRegimeKValue = rightAbsoluteValue.GetRegimeKValueWithoutSignCheck(rightLengthOfRunOfBits);
            uint rightExponentValue = rightAbsoluteValue.GetExponentValueWithoutSignCheck(rightFractionSize);

            var resultSignBit = leftAbsoluteValue > rightAbsoluteValue ? leftSignBit == 1 : rightSignBit == 1;
            uint resultFractionBits = 0;

            var leftScaleFactor = CalculateScaleFactor(leftRegimeKValue, leftExponentValue, MaximumExponentSize);
            var rightScaleFactor = CalculateScaleFactor(rightRegimeKValue, rightExponentValue, MaximumExponentSize);

            var scaleFactorDifference = leftScaleFactor - rightScaleFactor;

            var scaleFactor =
                scaleFactorDifference >= 0
                    ? leftScaleFactor
                    : rightScaleFactor;

            var leftFraction = leftAbsoluteValue.FractionWithHiddenBitWithoutSignCheck(leftFractionSize);
            var rightFraction = rightAbsoluteValue.FractionWithHiddenBitWithoutSignCheck(rightFractionSize);

            if (scaleFactorDifference == 0)
            {
                // False positive, will cause nested ternary.
#pragma warning disable S3240 // The simplest possible condition syntax should be used
                if (signBitsMatch)
                {
                    resultFractionBits += leftFraction + rightFraction;
                }
                else
                {
                    resultFractionBits += leftFraction >= rightFraction
                        ? leftFraction - rightFraction
                        : rightFraction - leftFraction;
                }
#pragma warning restore S3240 // The simplest possible condition syntax should be used

                scaleFactor += (short)(GetMostSignificantOnePosition(resultFractionBits) -
                              leftFractionSize - 1);
            }
            else if (scaleFactorDifference > 0)
            {
                // The scale factor of the left Posit is bigger.
                var fractionSizeDifference = (int)(leftFractionSize - rightFractionSize);
                resultFractionBits += leftFraction;
                var biggerPositMovedToLeft = (int)(FirstRegimeBitPosition - leftFractionSize - 1);
                resultFractionBits <<= biggerPositMovedToLeft;
                var smallerPositMovedToLeft = biggerPositMovedToLeft - scaleFactorDifference + fractionSizeDifference;

                if (signBitsMatch)
                {
                    resultFractionBits += smallerPositMovedToLeft >= 0
                        ? rightFraction << smallerPositMovedToLeft
                        : rightFraction >> -smallerPositMovedToLeft;
                }
                else
                {
                    resultFractionBits -= smallerPositMovedToLeft >= 0
                        ? rightFraction << smallerPositMovedToLeft
                        : rightFraction >> -smallerPositMovedToLeft;
                }

                scaleFactor += (short)(GetMostSignificantOnePosition(resultFractionBits) - FirstRegimeBitPosition);
            }
            else
            {
                // The scale factor of the right Posit is bigger.
                var fractionSizeDifference = (int)(rightFractionSize - leftFractionSize);
                resultFractionBits += rightFraction;
                var biggerPositMovedToLeft = (int)(FirstRegimeBitPosition - rightFractionSize - 1);
                resultFractionBits <<= biggerPositMovedToLeft;

                if (signBitsMatch)
                {
                    resultFractionBits += biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference >= 0
                        ? leftFraction << (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference)
                        : leftFraction >> -(biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);
                }
                else if (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference >= 0)
                {
                    resultFractionBits -= leftFraction << (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);
                }
                else
                {
                    resultFractionBits -= leftFraction >> -(biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);
                }

                scaleFactor += (short)(GetMostSignificantOnePosition(resultFractionBits) - FirstRegimeBitPosition);
            }

            if (resultFractionBits == 0) return new Posit32(0, true);

            var resultRegimeKValue = scaleFactor / (1 << MaximumExponentSize);
            var resultExponentBits = scaleFactor % (1 << MaximumExponentSize);
            if (resultExponentBits < 0)
            {
                resultRegimeKValue -= 1;
                resultExponentBits += 1 << MaximumExponentSize;
            }

            return new Posit32(AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, (uint)resultExponentBits, resultFractionBits), true);
        }

        public static Posit32 operator +(Posit32 left, int right) => left + new Posit32(right);

        public static Posit32 operator -(Posit32 left, Posit32 right) => left + -right;

        public static Posit32 operator -(Posit32 left, int right) => left - new Posit32(right);

        public static Posit32 operator -(Posit32 x)
        {
            if (x.IsNaN() || x.IsZero()) return new Posit32(x.PositBits, true);
            return new Posit32(GetTwosComplement(x.PositBits), true);
        }

        public static bool operator ==(Posit32 left, Posit32 right) => left.PositBits == right.PositBits;

        public static bool operator >(Posit32 left, Posit32 right)
        {
            if (left.IsPositive() != right.IsPositive()) return left.IsPositive();
            return left.IsPositive() ? left.PositBits > right.PositBits : !(left.PositBits > right.PositBits);
        }

        public static bool operator <(Posit32 left, Posit32 right) => !(left.PositBits > right.PositBits);

        public static bool operator !=(Posit32 left, Posit32 right) => !(left == right);

        public static bool operator <=(Posit32 left, Posit32 right) => left.CompareTo(right) <= 0;

        public static bool operator >=(Posit32 left, Posit32 right) => left.CompareTo(right) >= 0;

        public static Posit32 operator *(Posit32 left, int right) => left * new Posit32(right);

        public static Posit32 operator *(Posit32 left, Posit32 right)
        {
            if (left.IsZero() || right.IsZero()) return new Posit32(0);
            var leftIsPositive = left.IsPositive();
            var rightIsPositive = right.IsPositive();
            var resultSignBit = leftIsPositive != rightIsPositive;

            left = Abs(left);
            right = Abs(right);
            var leftFractionSize = left.FractionSizeWithoutSignCheck();
            var rightFractionSize = right.FractionSizeWithoutSignCheck();

            var longResultFractionBits = left.FractionWithHiddenBitWithoutSignCheck() *
                (ulong)right.FractionWithHiddenBitWithoutSignCheck();
            var fractionSizeChange = GetMostSignificantOnePosition(longResultFractionBits) - (leftFractionSize + rightFractionSize + 1);
            var resultFractionBits = (uint)(longResultFractionBits >> (int)(leftFractionSize + 1 + rightFractionSize + 1 - 32));
            var scaleFactor =
                CalculateScaleFactor(left.GetRegimeKValue(), left.GetExponentValue(), MaximumExponentSize) +
                CalculateScaleFactor(right.GetRegimeKValue(), right.GetExponentValue(), MaximumExponentSize);

            scaleFactor += (int)fractionSizeChange;

            var resultRegimeKValue = scaleFactor / (1 << MaximumExponentSize);
            var resultExponentBits = scaleFactor % (1 << MaximumExponentSize);
            if (resultExponentBits < 0)
            {
                resultRegimeKValue -= 1;
                resultExponentBits += 1 << MaximumExponentSize;
            }

            return new Posit32(AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, (uint)resultExponentBits, resultFractionBits), true);
        }

        public static Posit32 operator /(Posit32 left, int right) => left / new Posit32(right);

        public static Posit32 operator /(Posit32 left, Posit32 right)
        {
            if (left.IsZero()) return new Posit32(0);
            if (right.IsZero()) return new Posit32(NaNBitMask, true);
            var leftIsPositive = left.IsPositive();
            var rightIsPositive = right.IsPositive();
            var resultSignBit = leftIsPositive != rightIsPositive;

            left = Abs(left);
            right = Abs(right);
            var leftFractionSize = left.FractionSizeWithoutSignCheck();
            var rightFractionSize = right.FractionSizeWithoutSignCheck();

            var longResultFractionBits = ((ulong)left.FractionWithHiddenBitWithoutSignCheck() << (int)(63 - leftFractionSize)) /
                (right.FractionWithHiddenBitWithoutSignCheck() << (int)(31 - rightFractionSize));
            var fractionSizeChange = GetMostSignificantOnePosition(longResultFractionBits) - 33;

            var scaleFactor =
                CalculateScaleFactor(left.GetRegimeKValue(), left.GetExponentValue(), MaximumExponentSize) -
                CalculateScaleFactor(right.GetRegimeKValue(), right.GetExponentValue(), MaximumExponentSize);
            scaleFactor += fractionSizeChange;

            var resultRegimeKValue = scaleFactor / (1 << MaximumExponentSize);
            var resultExponentBits = scaleFactor % (1 << MaximumExponentSize);
            if (resultExponentBits < 0)
            {
                resultRegimeKValue -= 1;
                resultExponentBits += 1 << MaximumExponentSize;
            }

            var resultFractionBits = (uint)(longResultFractionBits >> (resultRegimeKValue > 0 ? resultRegimeKValue + 1 : -resultRegimeKValue + 1));

            return new Posit32(AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, (uint)resultExponentBits, resultFractionBits), true);
        }

        public static explicit operator int(Posit32 x)
        {
            uint result;
            if (x.PositBits == 0) return 0;

            var scaleFactor = (x.GetRegimeKValue() * (1 << MaximumExponentSize)) + x.GetExponentValue();

            if (scaleFactor + 1 <= 31)
            {
                // The posit fits into the range
                var mostSignificantOnePosition = GetMostSignificantOnePosition(x.FractionWithHiddenBit());

                result = scaleFactor - mostSignificantOnePosition + 1 >= 0
                    ? x.FractionWithHiddenBit() << (int)(scaleFactor - mostSignificantOnePosition + 1)
                    : x.FractionWithHiddenBit() >> -(int)(scaleFactor - mostSignificantOnePosition + 1);
            }
            else
            {
                return x.IsPositive() ? int.MaxValue : int.MinValue;
            }

            return x.IsPositive() ? (int)result : (int)-result;
        }

        public static explicit operator float(Posit32 x)
        {
            if (x.IsNaN()) return float.NaN;
            if (x.IsZero()) return 0F;

            var floatBits = x.IsPositive() ? EmptyBitMask : SignBitMask;
            float floatRepresentation;
            var scaleFactor = (x.GetRegimeKValue() * (1 << MaximumExponentSize)) + x.GetExponentValue();

            if (scaleFactor > 127) return x.IsPositive() ? float.MaxValue : float.MinValue;
            if (scaleFactor < -127) return x.IsPositive() ? float.Epsilon : -float.Epsilon;

            var fraction = x.Fraction();

            if (scaleFactor == -127)
            {
                fraction >>= 1;
                fraction += Float32HiddenBitMask >> 1;
            }

            floatBits += (uint)((scaleFactor + 127) << 23);

            if (x.FractionSize() <= 23)
            {
                fraction <<= (int)(23 - x.FractionSize());
            }
            else
            {
                fraction >>= (int)-(23 - x.FractionSize());
            }

            floatBits += (fraction << (32 - GetMostSignificantOnePosition(fraction) - 1)) >> (32 - GetMostSignificantOnePosition(fraction) - 1);

            unsafe
            {
                float* floatPointer = (float*)&floatBits;
                floatRepresentation = *floatPointer;
            }

            return floatRepresentation;
        }

        public static explicit operator double(Posit32 x)
        {
            if (x.IsNaN()) return double.NaN;
            if (x.IsZero()) return 0D;

            ulong doubleBits = x.IsPositive() ? EmptyBitMask : ((ulong)SignBitMask) << 32;
            double doubleRepresentation;
            var scaleFactor = (x.GetRegimeKValue() * (1 << MaximumExponentSize)) + x.GetExponentValue();

            var fraction = (ulong)x.Fraction();

            doubleBits += (ulong)((scaleFactor + 1_023) << 52);

            fraction <<= (int)(52 - x.FractionSize());
            doubleBits += (fraction << (64 - GetMostSignificantOnePosition(fraction) - 1)) >> (64 - GetMostSignificantOnePosition(fraction) - 1);

            unsafe
            {
                double* doublePointer = (double*)&doubleBits;
                doubleRepresentation = *doublePointer;
            }

            return doubleRepresentation;
        }

        public static explicit operator Quire(Posit32 x)
        {
            if (x.IsNaN()) return new Quire(1, 512) << 511;
            var quireArray = new ulong[QuireSize / 64];
            quireArray[0] = x.FractionWithHiddenBit();
            var resultQuire = new Quire(quireArray);
            resultQuire <<= (int)(240 - x.FractionSize() + x.CalculateScaleFactor());
            // This is not a conditional expression return because Hastlayer would throw a "You can't at the moment
            // assign to a variable that you previously assigned to using a reference type-holding variable."
            if (x.IsPositive()) return resultQuire;
            return (~resultQuire) + 1;
        }

        #endregion

        #region Support methods

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is Posit32 positValue) return CompareTo(positValue);

            throw new ArgumentException("Argument must be an other posit");
        }

        public int CompareTo(Posit32 obj)
        {
            var otherIsNaN = obj.IsNaN();

            if (IsNaN()) return otherIsNaN ? 0 : -1;
            if (otherIsNaN) return 1;

            if (this < obj) return -1;
            if (this > obj) return 1;
            return 0;
        }

        public override string ToString() => ((double)this).ToString(CultureInfo.InvariantCulture);

        public string ToString(string format, IFormatProvider formatProvider) => ((double)this).ToString(format, formatProvider);

        public string ToString(IFormatProvider provider) => ((double)this).ToString(provider);

        public Posit32 Parse(string number) => new(double.Parse(number, CultureInfo.InvariantCulture));

        public bool TryParse(string number, out Posit32 positResult)
        {
            var returnValue = double.TryParse(number, out double result);
            positResult = new Posit32(result);
            return returnValue;
        }

        public override bool Equals(object obj) => obj is Posit32 other && this == other;

        public bool Equals(Posit32 other) => this == other;

        public TypeCode GetTypeCode() => throw new NotSupportedException();

        public bool ToBoolean(IFormatProvider provider) => !IsZero();

        public char ToChar(IFormatProvider provider) => throw new InvalidCastException();

        public sbyte ToSByte(IFormatProvider provider) => (sbyte)(int)this;

        public byte ToByte(IFormatProvider provider) => (byte)(uint)this;

        public short ToInt16(IFormatProvider provider) => (short)(int)this;

        public ushort ToUInt16(IFormatProvider provider) => (ushort)(uint)this;

        public int ToInt32(IFormatProvider provider) => (int)this;

        public uint ToUInt32(IFormatProvider provider) => (uint)this;

        public long ToInt64(IFormatProvider provider) => (long)this;

        public ulong ToUInt64(IFormatProvider provider) => (ulong)this;

        public float ToSingle(IFormatProvider provider) => (float)this;

        public double ToDouble(IFormatProvider provider) => (double)this;

        public decimal ToDecimal(IFormatProvider provider) => throw new NotSupportedException();

        public DateTime ToDateTime(IFormatProvider provider) => throw new InvalidCastException();

        public object ToType(Type conversionType, IFormatProvider provider) => throw new NotSupportedException();

        public override int GetHashCode() => (int)PositBits;

        #endregion
    }
}
