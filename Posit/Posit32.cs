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

        public Posit32(uint bits, bool FromBitMask)
        {
            PositBits = bits;
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
            PositBits = value >= 0 ? (uint)value : GetTwosComplement((uint)-value);
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
            else regimeBits = (FirstRegimeBitBitMask << regimeKValue);

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
            wholePosit += exponentBits << exponentShiftedLeftBy;

            // Calculating rounding.
            if (exponentShiftedLeftBy < 0)
            {
                exponentBits <<= Size + exponentShiftedLeftBy;
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
            wholePosit += fractionBits << fractionShiftedLeftBy;
            // Calculating rounding.
            if (fractionShiftedLeftBy >= 0) return !signBit ? wholePosit : GetTwosComplement(wholePosit);
            fractionBits <<= Size + fractionShiftedLeftBy;
            if (fractionBits < SignBitMask) return !signBit ? wholePosit : GetTwosComplement(wholePosit);

            if (fractionBits == SignBitMask) wholePosit += (wholePosit % 2) == 1 ? 1 : (uint)0;
            else wholePosit += 1;

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
            var result = bits << (int)(32 - FractionSize())
                         >> (int)(32 - FractionSize());
            return SetOne(result, (ushort)FractionSize());
        }

        public uint FractionWithHiddenBitWithoutSignCheck()
        {
            var result = PositBits << (int)(32 - FractionSizeWithoutSignCheck())
                         >> (int)(32 - FractionSizeWithoutSignCheck());
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


        public byte LengthOfRunOfBits(uint bits, byte startingPosition)
        {
            byte length = 1;
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
            var leftIsPositive = left.IsPositive();
            var rightIsPositive = right.IsPositive();
            var resultSignBit = left > right ? leftIsPositive : rightIsPositive;

            var signBitsMatch = leftIsPositive == rightIsPositive;

            sbyte leftRegimeKValue;
            uint leftExponentValue;
            sbyte rightRegimeKValue;
            uint rightExponentValue;

            if (!leftIsPositive)
            {
                var negatedLeft = -left;
                leftRegimeKValue = negatedLeft.GetRegimeKValueWithoutSignCheck();
                leftExponentValue = negatedLeft.GetExponentValueWithoutSignCheck();
            }
            else
            {
                leftRegimeKValue = left.GetRegimeKValueWithoutSignCheck();
                leftExponentValue = left.GetExponentValueWithoutSignCheck();
            }
            if (!rightIsPositive)
            {
                var negatedRight = -right;
                rightRegimeKValue = negatedRight.GetRegimeKValueWithoutSignCheck();
                rightExponentValue = negatedRight.GetExponentValueWithoutSignCheck();
            }
            else
            {
                rightRegimeKValue = right.GetRegimeKValueWithoutSignCheck();
                rightExponentValue = right.GetExponentValueWithoutSignCheck();
            }

            // Handling special cases first.
            if (leftRegimeKValue == -FirstRegimeBitPosition)
            {
                return leftIsPositive ? right : left;
            }
            if (rightRegimeKValue == -FirstRegimeBitPosition)
            {
                return rightIsPositive ? left : right;
            }

            ulong resultFractionBits = 0;

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

                if (signBitsMatch)
                {
                    resultFractionBits += right.FractionWithHiddenBitWithoutSignCheck() << (biggerPositMovedToLeft - scaleFactorDifference + fractionSizeDifference);

                }
                else resultFractionBits -= right.FractionWithHiddenBitWithoutSignCheck() << (biggerPositMovedToLeft - scaleFactorDifference + fractionSizeDifference);

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
                    resultFractionBits += left.FractionWithHiddenBitWithoutSignCheck() << (int)(biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);
                }
                else resultFractionBits -= left.FractionWithHiddenBitWithoutSignCheck() << (int)(biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);

                scaleFactor += (short)(GetMostSignificantOnePosition(resultFractionBits) - FirstRegimeBitPosition);
            }
            if (GetMostSignificantOnePosition(resultFractionBits) == 0) return new Posit32(0);

            var resultRegimeKValue = (int)(scaleFactor / (1 << MaximumExponentSize));
            var resultExponentBits = (uint)(scaleFactor % (1 << MaximumExponentSize));

            return new Posit32(left.AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, resultExponentBits, (uint)resultFractionBits));
        }

        public static Posit32 operator +(Posit32 left, int right) => left + new Posit32(right);

        public static Posit32 operator -(Posit32 left, Posit32 right) => left + (-right);

        public static Posit32 operator -(Posit32 left, int right) => left - new Posit32(right);


        public static Posit32 operator -(Posit32 x)
        {
            if (x.IsNaN() || x.IsZero()) return new Posit32(x.PositBits);
            return new Posit32(GetTwosComplement(x.PositBits));
        }
        public static bool operator ==(Posit32 left, Posit32 right) => left.PositBits == right.PositBits;

        public static bool operator >(Posit32 left, Posit32 right)
        {
            if (left.IsPositive() != right.IsPositive()) return left.IsPositive();
            return left.IsPositive() ? left.PositBits > right.PositBits : !(left.PositBits > right.PositBits);
        }

        public static bool operator <(Posit32 left, Posit32 right) => !(left.PositBits > right.PositBits);

        public static bool operator !=(Posit32 left, Posit32 right) => !(left == right);

        //public static Posit operator *(Posit left, Posit right)
        //{
        //    var leftIsPositive = left.IsPositive();
        //    var rightIsPositive = right.IsPositive();
        //    var resultSignBit = leftIsPositive != rightIsPositive;

        //    if (!leftIsPositive)
        //    {
        //        left = -left;
        //    }
        //    if (!rightIsPositive)
        //    {
        //        right = -right;
        //    }

        //    var rightRegimeKValue = right.GetRegimeKValue();
        //    var rightExponentValue = right.GetExponentValue();
        //    var leftRegimeKValue = left.GetRegimeKValue();
        //    var leftExponentValue = left.GetExponentValue();


        //    // Handling special cases first.
        //    if (leftRegimeKValue == -(left.Size - 1))
        //    {
        //        return right.IsNaN()? right : left;
        //    }
        //    if (rightRegimeKValue == -(right.Size - 1))
        //    {
        //        return right;
        //    }

        //    var resultFractionBits = new Quire(left.Size); 

        //    var scaleFactor = CalculateScaleFactor(leftRegimeKValue, leftExponentValue, left.MaximumExponentSize) + CalculateScaleFactor(rightRegimeKValue, rightExponentValue, right.MaximumExponentSize);

        //    if (scaleFactorDifference == 0)
        //    {
        //        if (signBitsMatch)
        //        {
        //            resultFractionBits += left.FractionWithHiddenBit() + right.FractionWithHiddenBit();
        //        }
        //        else
        //        {
        //            if (left.FractionWithHiddenBit() >= right.FractionWithHiddenBit())
        //            {
        //                resultFractionBits += left.FractionWithHiddenBit() - right.FractionWithHiddenBit();
        //            }
        //            else
        //            {
        //                resultFractionBits += right.FractionWithHiddenBit() - left.FractionWithHiddenBit();
        //            }
        //        }

        //        scaleFactor += resultFractionBits.GetMostSignificantOnePosition() -
        //                       left.FractionWithHiddenBit().GetMostSignificantOnePosition();
        //    }
        //    else if (scaleFactorDifference > 0) // The scale factor of the left Posit is bigger.
        //    {
        //        var fractionSizeDifference = (int)(left.FractionSize() - right.FractionSize());
        //        resultFractionBits += left.FractionWithHiddenBit();
        //        var biggerPositMovedToLeft = left.Size - 1 - left.FractionWithHiddenBit().GetMostSignificantOnePosition();
        //        resultFractionBits <<= biggerPositMovedToLeft;

        //        if (signBitsMatch)
        //        {
        //            resultFractionBits += right.FractionWithHiddenBit() << (int)(biggerPositMovedToLeft - scaleFactorDifference + fractionSizeDifference);

        //        }
        //        else resultFractionBits -= right.FractionWithHiddenBit() << (int)(biggerPositMovedToLeft - scaleFactorDifference + fractionSizeDifference);

        //        scaleFactor += resultFractionBits.GetMostSignificantOnePosition() - (left.Size - 1);
        //    }
        //    else // The scale factor of the right Posit is bigger.
        //    {
        //        var fractionSizeDifference = (int)(right.FractionSize() - left.FractionSize());
        //        resultFractionBits += right.FractionWithHiddenBit();
        //        var biggerPositMovedToLeft = right.Size - 1 - right.FractionWithHiddenBit().GetMostSignificantOnePosition();
        //        resultFractionBits <<= biggerPositMovedToLeft;

        //        if (signBitsMatch)
        //        {
        //            resultFractionBits += left.FractionWithHiddenBit() << (int)(biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);
        //        }
        //        else resultFractionBits -= left.FractionWithHiddenBit() << (int)(biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);

        //        scaleFactor += resultFractionBits.GetMostSignificantOnePosition() - (right.Size - 1);
        //    }
        //    if (resultFractionBits.GetMostSignificantOnePosition() == 0) return new Posit(left._environment, left.EmptyBitMask);

        //    var resultRegimeKValue = (int)(scaleFactor / (1 << left.MaximumExponentSize));
        //    var resultExponentBits = new BitMask((uint)((scaleFactor % (1 << left.MaximumExponentSize))), left._environment.Size);

        //    return new Posit(left._environment,
        //        left.AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, resultExponentBits, resultFractionBits));

        //}


        public static explicit operator int(Posit32 x)
        {
            uint result;

            if ((x.GetRegimeKValue() * (1 << MaximumExponentSize)) + x.GetExponentValue() + 1 < 31) // The posit fits into the range
            {
                result = (x.FractionWithHiddenBit() << (int)((x.GetRegimeKValue() * (1 << MaximumExponentSize)) + x.GetExponentValue()) - GetMostSignificantOnePosition(x.FractionWithHiddenBit()) + 1);
            }
            else return (x.IsPositive()) ? int.MaxValue : int.MinValue;
            return x.IsPositive() ? (int)result : (int)-result;
        }

        #endregion

    }
}

