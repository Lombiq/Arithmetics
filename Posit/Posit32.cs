﻿using System;

namespace Lombiq.Arithmetics
{
    public class Posit32
    {
        public uint PositBits { get; }

        #region Posit structure
        public const byte MaximumExponentSize = 2;

        public const byte Size = 32;

        public const byte Useed = 16;

        public const byte FirstRegimeBitIndex = 30;

        public const byte FirstRegimeBitPosition = 31;

        public const byte SizeMinusFixedBits = 28;

        public const short QuireSize = 512;

        #endregion

        #region Posit Masks

        public const uint SignBitMask = (uint)1 << 31;

        public const uint FirstRegimeBitBitMask = (uint)1 << 30;

        public const uint EmptyBitMask = 0;

        public const uint MaxValueBitMask = uint.MaxValue;

        public const uint MinValueBitMask = uint.MinValue;

        public const uint NaNBitMask = SignBitMask;

        #endregion

        #region Posit constructors

        public Posit32()
        {
            PositBits = 0;
        }

        public Posit32(uint bits, bool fromBitMask)
        {
            if (fromBitMask)
            {
                PositBits = bits;
            }
            else PositBits = new Posit32(bits).PositBits;
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

        public Posit32(int value)
        {
            PositBits = value >= 0 ? new Posit32((uint)value).PositBits : GetTwosComplement(new Posit32((uint)-value).PositBits);
        }

        #endregion

        #region Posit numeric states

        public bool IsPositive() => (PositBits & SignBitMask) == EmptyBitMask;

        public bool IsNaN() => PositBits == NaNBitMask;

        public bool IsZero() => PositBits == EmptyBitMask;

        #endregion

        #region Methods to handle parts of the Posit 

        public uint EncodeRegimeBits(int regimeKValue)
        {
            uint regimeBits;
            if (regimeKValue > 0)
            {
                regimeBits = (uint)(1 << regimeKValue + 1) - 1;
                regimeBits <<= Size - GetMostSignificantOnePosition(regimeBits) - 1;
            }
            else regimeBits = (FirstRegimeBitBitMask >> -regimeKValue);

            return regimeBits;
        }

        private uint AssemblePositBits(bool signBit, int regimeKValue, uint exponentBits, uint fractionBits)
        {
            // Calculating the regime. 
            var wholePosit = EncodeRegimeBits(regimeKValue);

            // Attaching the exponent
            var regimeLength = LengthOfRunOfBits(wholePosit, FirstRegimeBitPosition);

            wholePosit += exponentBits << SizeMinusFixedBits - regimeLength;

            var fractionMostSignificantOneIndex = GetMostSignificantOnePosition(fractionBits) - 1;

            // Hiding the hidden bit. (It is always one.) 
            fractionBits = SetZero(fractionBits, (ushort)fractionMostSignificantOneIndex);

            wholePosit += fractionBits << SizeMinusFixedBits - fractionMostSignificantOneIndex - regimeLength;

            return !signBit ? wholePosit : GetTwosComplement(wholePosit);
        }

        private uint AssemblePositBitsWithRounding(bool signBit, int regimeKValue, uint exponentBits, uint fractionBits)
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

                if (exponentBits < SignBitMask) return !signBit ? wholePosit : GetTwosComplement(wholePosit);

                if (exponentBits == SignBitMask) wholePosit += (wholePosit % 2) == 1 ? 1 : (uint)0;
                else wholePosit += 1;

                return !signBit ? wholePosit : GetTwosComplement(wholePosit);
            }

            var fractionMostSignificantOneIndex = GetMostSignificantOnePosition(fractionBits) - 1;

            // Hiding the hidden bit. (It is always one.) 
            fractionBits = SetZero(fractionBits, (ushort)fractionMostSignificantOneIndex);

            var fractionShiftedLeftBy = SizeMinusFixedBits - fractionMostSignificantOneIndex - (regimeLength);
            // Attaching the fraction.
            wholePosit += fractionShiftedLeftBy >= 0 ? fractionBits << fractionShiftedLeftBy : fractionBits >> -fractionShiftedLeftBy;
            // Calculating rounding.
            if (fractionShiftedLeftBy < 0)
            {
                if (Size + fractionShiftedLeftBy >= 0) fractionBits <<= Size + fractionShiftedLeftBy;
                else fractionBits >>= -(Size - fractionShiftedLeftBy);
                //return !signBit ? wholePosit : GetTwosComplement(wholePosit);
                if (fractionBits >= SignBitMask)
                {
                    if (fractionBits == SignBitMask)
                    {
                        wholePosit += (wholePosit % 2) == 1 ? 1 : (uint)0;
                    }
                    else wholePosit += 1;
                }
            }

            return !signBit ? wholePosit : GetTwosComplement(wholePosit);
        }

        public sbyte GetRegimeKValue()
        {
            var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            return (bits & FirstRegimeBitBitMask) == EmptyBitMask
                ? (sbyte)-LengthOfRunOfBits(bits, FirstRegimeBitPosition)
                : (sbyte)(LengthOfRunOfBits(bits, FirstRegimeBitPosition) - 1);
        }

        public sbyte GetRegimeKValueWithoutSignCheck()
        {
            return (PositBits & FirstRegimeBitBitMask) == EmptyBitMask
                ? (sbyte)-LengthOfRunOfBits(PositBits, FirstRegimeBitPosition)
                : (sbyte)(LengthOfRunOfBits(PositBits, FirstRegimeBitPosition) - 1);
        }

        public short CalculateScaleFactor()
        {
            if (GetRegimeKValue() == -FirstRegimeBitPosition) return 0;
            //return (int)((GetRegimeKValue() == 0) ? 1 + GetExponentValue() : (GetRegimeKValue() * (1 << MaximumExponentSize) + GetExponentValue()));
            return (short)(GetRegimeKValue() * (1 << MaximumExponentSize) + GetExponentValue());
        }

        public byte ExponentSize()
        {
            var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            return Size - (LengthOfRunOfBits(bits, FirstRegimeBitPosition) + 2) > MaximumExponentSize
                ? MaximumExponentSize : (byte)(Size - (LengthOfRunOfBits(bits, FirstRegimeBitPosition) + 2));
        }

        public byte ExponentSizeWithoutSignCheck()
        {
            return Size - (LengthOfRunOfBits(PositBits, FirstRegimeBitPosition) + 2) > MaximumExponentSize
                ? MaximumExponentSize : (byte)(Size - (LengthOfRunOfBits(PositBits, FirstRegimeBitPosition) + 2));
        }

        public uint GetExponentValue()
        {
            var exponentMask = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            exponentMask = (exponentMask >> (int)FractionSize())
                            << (32 - ExponentSize())
                            >> (32 - MaximumExponentSize);
            return exponentMask;
        }

        public uint GetExponentValueWithoutSignCheck()
        {
            return (PositBits >> (int)FractionSizeWithoutSignCheck())
                            << (32 - ExponentSize())
                            >> (32 - MaximumExponentSize);
        }

        public uint FractionSize()
        {
            var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            var fractionSize = Size - (LengthOfRunOfBits(bits, FirstRegimeBitPosition) + 2 + MaximumExponentSize);
            return fractionSize > 0 ? (uint)fractionSize : 0;
        }
        public uint FractionSizeWithoutSignCheck()
        {
            var fractionSize = Size - (LengthOfRunOfBits(PositBits, FirstRegimeBitPosition) + 2 + MaximumExponentSize);
            return fractionSize > 0 ? (uint)fractionSize : 0;
        }

        #endregion

        #region Helper methods for operations and conversions

        public uint FractionWithHiddenBit()
        {
            var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
            var result = bits << (int)(Size - FractionSize())
                         >> (int)(Size - FractionSize());
            return SetOne(result, (ushort)FractionSize());
        }

        public uint FractionWithHiddenBitWithoutSignCheck()
        {
            var result = PositBits << (int)(Size - FractionSizeWithoutSignCheck())
                         >> (int)(Size - FractionSizeWithoutSignCheck());
            return SetOne(result, (ushort)FractionSizeWithoutSignCheck());
        }

        public static short CalculateScaleFactor(sbyte regimeKValue, uint exponentValue, byte maximumExponentSize) =>
            (short)(regimeKValue * (1 << maximumExponentSize) + exponentValue);

        #endregion

        #region Bit level Helper Methods

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

        public uint SetOne(uint bits, ushort index) => bits | (uint)(1 << index);

        public uint SetZero(uint bits, ushort index) => bits & (uint)~(1 << index);

        public static byte LengthOfRunOfBits(uint bits, byte startingPosition)
        {
            byte length = 1;
            bits <<= Size - startingPosition;
            var startingBit = bits >> 31 > 0;
            bits <<= 1;
            for (var i = 0; i < startingPosition; i++)
            {
                if (bits >> 31 > 0 != startingBit) return length;
                bits <<= 1;
                length++;
            }
            return (length > startingPosition) ? startingPosition : length;
        }

        public static uint GetTwosComplement(uint bits) => ~bits + 1;

        #endregion

        #region operators

        public static Posit32 operator +(Posit32 left, Posit32 right)
        {
            // Handling special cases first.
            if (left.IsNaN()) return left;
            if (right.IsNaN()) return right;
            if (left.IsZero()) return right;
            if (right.IsZero()) return left;

            var leftIsPositive = left.IsPositive();
            var rightIsPositive = right.IsPositive();

            var signBitsMatch = leftIsPositive == rightIsPositive;

            sbyte leftRegimeKValue;
            uint leftExponentValue;
            sbyte rightRegimeKValue;
            uint rightExponentValue;

            if (!leftIsPositive)
            {
                left = -left;
                leftRegimeKValue = left.GetRegimeKValueWithoutSignCheck();
                leftExponentValue = left.GetExponentValueWithoutSignCheck();
            }
            else
            {
                leftRegimeKValue = left.GetRegimeKValueWithoutSignCheck();
                leftExponentValue = left.GetExponentValueWithoutSignCheck();
            }
            if (!rightIsPositive)
            {
                right = -right;
                rightRegimeKValue = right.GetRegimeKValueWithoutSignCheck();
                rightExponentValue = right.GetExponentValueWithoutSignCheck();
            }
            else
            {
                rightRegimeKValue = right.GetRegimeKValueWithoutSignCheck();
                rightExponentValue = right.GetExponentValueWithoutSignCheck();
            }

            var resultSignBit = left > right ? !leftIsPositive : !rightIsPositive;

            uint resultFractionBits = 0;

            var scaleFactorDifference = CalculateScaleFactor(leftRegimeKValue, leftExponentValue, MaximumExponentSize)
                                        - CalculateScaleFactor(rightRegimeKValue, rightExponentValue, MaximumExponentSize);

            var scaleFactor =
                scaleFactorDifference >= 0
                    ? CalculateScaleFactor(leftRegimeKValue, leftExponentValue, MaximumExponentSize)
                    : CalculateScaleFactor(rightRegimeKValue, rightExponentValue, MaximumExponentSize);

            if (scaleFactorDifference == 0)
            {
                if (signBitsMatch)
                {
                    resultFractionBits += left.FractionWithHiddenBitWithoutSignCheck() + right.FractionWithHiddenBitWithoutSignCheck();
                }
                else
                {
                    if (left.FractionWithHiddenBitWithoutSignCheck() >= right.FractionWithHiddenBitWithoutSignCheck())
                    {
                        resultFractionBits += left.FractionWithHiddenBitWithoutSignCheck() - right.FractionWithHiddenBitWithoutSignCheck();
                    }
                    else
                    {
                        resultFractionBits += right.FractionWithHiddenBitWithoutSignCheck() - left.FractionWithHiddenBitWithoutSignCheck();
                    }
                }

                scaleFactor += (short)(GetMostSignificantOnePosition(resultFractionBits) -
                               GetMostSignificantOnePosition(left.FractionWithHiddenBitWithoutSignCheck()));
            }
            else if (scaleFactorDifference > 0) // The scale factor of the left Posit is bigger.
            {
                var fractionSizeDifference = (int)(left.FractionSizeWithoutSignCheck() - right.FractionSizeWithoutSignCheck());
                resultFractionBits += left.FractionWithHiddenBitWithoutSignCheck();
                var biggerPositMovedToLeft = FirstRegimeBitPosition - GetMostSignificantOnePosition(left.FractionWithHiddenBit());
                resultFractionBits <<= biggerPositMovedToLeft;
                var smallerPositMovedToLeft = biggerPositMovedToLeft - scaleFactorDifference + fractionSizeDifference;

                if (signBitsMatch)
                {
                    if (smallerPositMovedToLeft >= 0)
                    {
                        resultFractionBits += right.FractionWithHiddenBitWithoutSignCheck() << smallerPositMovedToLeft;
                    }
                    else resultFractionBits += right.FractionWithHiddenBitWithoutSignCheck() >> -smallerPositMovedToLeft;
                }
                else resultFractionBits -= smallerPositMovedToLeft >= 0
                        ? right.FractionWithHiddenBitWithoutSignCheck() << smallerPositMovedToLeft
                        : right.FractionWithHiddenBitWithoutSignCheck() >> -smallerPositMovedToLeft;

                scaleFactor += (short)(GetMostSignificantOnePosition(resultFractionBits) - FirstRegimeBitPosition);
            }
            else // The scale factor of the right Posit is bigger.
            {
                var fractionSizeDifference = (int)(right.FractionSizeWithoutSignCheck() - left.FractionSizeWithoutSignCheck());
                resultFractionBits += right.FractionWithHiddenBitWithoutSignCheck();
                var biggerPositMovedToLeft = FirstRegimeBitPosition - GetMostSignificantOnePosition(right.FractionWithHiddenBitWithoutSignCheck());
                resultFractionBits <<= biggerPositMovedToLeft;

                if (signBitsMatch)
                {
                    if (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference >= 0)
                    {
                        resultFractionBits += left.FractionWithHiddenBitWithoutSignCheck() << (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);
                    }
                    else resultFractionBits += left.FractionWithHiddenBitWithoutSignCheck() >> -(biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);

                }
                else if (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference >= 0)
                {
                    resultFractionBits -= left.FractionWithHiddenBitWithoutSignCheck() << (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);
                }
                else resultFractionBits -= left.FractionWithHiddenBitWithoutSignCheck() >> -(biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);

                scaleFactor += (short)(GetMostSignificantOnePosition(resultFractionBits) - FirstRegimeBitPosition);
            }
            if (resultFractionBits == 0) return new Posit32(0);

            var resultRegimeKValue = scaleFactor / (1 << MaximumExponentSize);
            var resultExponentBits = (uint)(scaleFactor % (1 << MaximumExponentSize));

            return new Posit32(left.AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, resultExponentBits, resultFractionBits), true);
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

        public static Posit32 operator *(Posit32 left, int right) => left * new Posit32(right);
        public static Posit32 operator *(Posit32 left, Posit32 right)
        {
            var leftIsPositive = left.IsPositive();
            var rightIsPositive = right.IsPositive();
            var resultSignBit = leftIsPositive != rightIsPositive;

            if (!leftIsPositive)
            {
                left = -left;
            }
            if (!rightIsPositive)
            {
                right = -right;
            }

            uint resultFractionBits = (uint)((ulong)left.FractionWithHiddenBitWithoutSignCheck() * (ulong)right.FractionWithHiddenBitWithoutSignCheck() >> 32);

            var scaleFactor = CalculateScaleFactor(left.GetRegimeKValue(), left.GetExponentValue(), MaximumExponentSize) + CalculateScaleFactor(right.GetRegimeKValue(), right.GetExponentValue(), MaximumExponentSize);

            var resultRegimeKValue = (int)(scaleFactor / (1 << MaximumExponentSize));
            var resultExponentBits = (uint)scaleFactor % (1 << MaximumExponentSize);

            return new Posit32(left.AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, resultExponentBits, resultFractionBits), true);

        }

        public static explicit operator int(Posit32 x)
        {
            uint result;
            var scaleFactor = x.GetRegimeKValue() * (1 << MaximumExponentSize) + x.GetExponentValue();
            if (scaleFactor + 1 < 31) // The posit fits into the range
            {
                if (scaleFactor - GetMostSignificantOnePosition(x.FractionWithHiddenBit()) + 1 >= 0)
                {
                    result = x.FractionWithHiddenBit() <<
                        (int)(scaleFactor - GetMostSignificantOnePosition(x.FractionWithHiddenBit()) + 1);
                }
                else
                {
                    result = (x.FractionWithHiddenBit() >>
                               -(int)(scaleFactor - GetMostSignificantOnePosition(x.FractionWithHiddenBit()) + 1));
                }
            }
            else return (x.IsPositive()) ? int.MaxValue : int.MinValue;
            return x.IsPositive() ? (int)result : (int)-result;
        }

        #endregion
    }
}

