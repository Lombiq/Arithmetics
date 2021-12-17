namespace Lombiq.Arithmetics
{
    //signbit regime exponent(?) fraction(?)
    public struct Posit
    {
        private readonly PositEnvironment _environment;
        public BitMask PositBits { get; }

        #region Posit structure
        public byte MaximumExponentSize => _environment.MaximumExponentSize;
        public ushort Size => _environment.Size;
        public uint Useed => _environment.Useed;
        public ushort FirstRegimeBitIndex => _environment.FirstRegimeBitIndex;

        #endregion

        #region Posit Masks

        public BitMask SignBitMask => _environment.SignBitMask;

        public BitMask FirstRegimeBitBitMask => _environment.FirstRegimeBitBitMask;

        public BitMask EmptyBitMask => _environment.EmptyBitMask;

        public BitMask MaxValueBitMask => _environment.MaxValueBitMask;

        public BitMask MinValueBitMask => _environment.MinValueBitMask;

        public BitMask NaNBitMask => _environment.NaNBitMask;

        #endregion

        #region Posit constructors

        public Posit(PositEnvironment environment)
        {
            _environment = environment;

            PositBits = new BitMask(_environment.Size);
        }

        public Posit(PositEnvironment environment, BitMask bits)
        {
            _environment = environment;

            PositBits = BitMask.FromImmutableArray(bits.Segments, _environment.Size);
        }

        public Posit(PositEnvironment environment, uint value)
        {
            _environment = environment;

            PositBits = new BitMask(value, _environment.Size);
            if (value == 0) return;

            var exponentValue = (uint)PositBits.GetMostSignificantOnePosition() - 1;

            ushort kValue = 0;
            while (exponentValue >= 1 << environment.MaximumExponentSize && kValue < _environment.Size - 1)
            {
                exponentValue -= 1U << environment.MaximumExponentSize;
                kValue++;
            }

            PositBits = AssemblePositBitsWithRounding(false, kValue, new BitMask(exponentValue, 32), PositBits);
        }

        public Posit(PositEnvironment environment, int value)
        {
            _environment = environment;
            PositBits = value >= 0 ? new Posit(environment, (uint)value).PositBits :
                new Posit(environment, (uint)-value).PositBits.GetTwosComplement(_environment.Size);
        }

        #endregion

        #region Posit numeric states

        public bool IsPositive() => (PositBits & SignBitMask) == EmptyBitMask;

        public bool IsNaN() => PositBits == NaNBitMask;

        public bool IsZero() => PositBits == EmptyBitMask;

        #endregion

        #region Methods to handle parts of the Posit

        public BitMask EncodeRegimeBits(int regimeKValue)
        {
            BitMask regimeBits;
            if (regimeKValue > 0)
            {
                regimeBits = (new BitMask(1, _environment.Size) << (regimeKValue + 1)) - 1;
                regimeBits <<= _environment.Size - regimeBits.GetMostSignificantOnePosition() - 1;
            }
            else regimeBits = _environment.FirstRegimeBitBitMask << regimeKValue;

            return regimeBits;
        }

        private BitMask AssemblePositBits(bool signBit, int regimeKValue, BitMask exponentBits, BitMask fractionBits)
        {
            // Calculating the regime.
            var wholePosit = EncodeRegimeBits(regimeKValue);

            // Attaching the exponent
            var regimeLength = wholePosit.LengthOfRunOfBits((ushort)(_environment.FirstRegimeBitIndex + 1));
            wholePosit += exponentBits << (_environment.Size - (regimeLength + 2) - _environment.MaximumExponentSize);

            var fractionMostSignificantOneIndex = fractionBits.GetMostSignificantOnePosition() - 1;

            // Hiding the hidden bit. (It is always one.)
            fractionBits = fractionBits.SetZero((ushort)fractionMostSignificantOneIndex);

            wholePosit += fractionBits << (_environment.Size - 2 - fractionMostSignificantOneIndex - regimeLength -
                          _environment.MaximumExponentSize);

            return !signBit ? wholePosit : wholePosit.GetTwosComplement(_environment.Size);
        }

        private BitMask AssemblePositBitsWithRounding(bool signBit, int regimeKValue, BitMask exponentBits, BitMask fractionBits)
        {
            // Calculating the regime.
            var wholePosit = EncodeRegimeBits(regimeKValue);

            // Attaching the exponent.
            var regimeLength = wholePosit.LengthOfRunOfBits((ushort)(FirstRegimeBitIndex + 1));

            var exponentShiftedLeftBy = Size - (regimeLength + 2) - MaximumExponentSize;
            wholePosit += exponentBits << exponentShiftedLeftBy;

            // Calculating rounding.
            if (exponentShiftedLeftBy < 0)
            {
                exponentBits <<= exponentBits.Size + exponentShiftedLeftBy;
                if (exponentBits >= new BitMask(exponentBits.Size).SetOne((ushort)(exponentBits.Size - 1)))
                {
                    if (exponentBits == new BitMask(exponentBits.Size).SetOne((ushort)(exponentBits.Size - 1)))
                    {
                        wholePosit += wholePosit.GetLowest32Bits() & 1;
                    }
                    else wholePosit += 1;
                }

                return !signBit ? wholePosit : wholePosit.GetTwosComplement(_environment.Size);
            }

            var fractionMostSignificantOneIndex = fractionBits.GetMostSignificantOnePosition() - 1;

            // Hiding the hidden bit. (It is always one.)
            fractionBits = fractionBits.SetZero((ushort)fractionMostSignificantOneIndex);

            var fractionShiftedLeftBy = _environment.Size - 2 - fractionMostSignificantOneIndex - regimeLength -
                                        _environment.MaximumExponentSize;
            // Attaching the fraction.
            wholePosit += fractionBits << fractionShiftedLeftBy;
            // Calculating rounding.
            if (fractionShiftedLeftBy < 0)
            {
                fractionBits <<= fractionBits.Size + fractionShiftedLeftBy;
                if (fractionBits >= new BitMask(fractionBits.Size).SetOne((ushort)(fractionBits.Size - 1)))
                {
                    if (fractionBits == new BitMask(fractionBits.Size).SetOne((ushort)(fractionBits.Size - 1)))
                    {
                        wholePosit += wholePosit.GetLowest32Bits() & 1;
                    }
                    else wholePosit += 1;
                }
            }

            return !signBit ? wholePosit : wholePosit.GetTwosComplement(_environment.Size);
        }

        public int GetRegimeKValue()
        {
            var bits = IsPositive() ? PositBits : PositBits.GetTwosComplement(Size);
            return (bits & FirstRegimeBitBitMask) == EmptyBitMask
                ? -bits.LengthOfRunOfBits((ushort)(FirstRegimeBitIndex + 1))
                : bits.LengthOfRunOfBits((ushort)(FirstRegimeBitIndex + 1)) - 1;
        }

        public int CalculateScaleFactor()
        {
            if (GetRegimeKValue() == -(Size - 1)) return 0;
            //return (int)((GetRegimeKValue() == 0) ? 1 + GetExponentValue() : (GetRegimeKValue() * (1 << MaximumExponentSize) + GetExponentValue()));
            return (int)((GetRegimeKValue() * (1 << MaximumExponentSize)) + GetExponentValue());
        }

        public uint ExponentSize()
        {
            var bits = IsPositive() ? PositBits : PositBits.GetTwosComplement(Size);
            return Size - (bits.LengthOfRunOfBits((ushort)(FirstRegimeBitIndex + 1)) + 2) > MaximumExponentSize
                ? MaximumExponentSize : (uint)(Size - (bits.LengthOfRunOfBits((ushort)(FirstRegimeBitIndex + 1)) + 2));
        }

        public uint GetExponentValue()
        {
            var exponentMask = IsPositive() ? PositBits : PositBits.GetTwosComplement(Size);
            exponentMask = (exponentMask >> (int)FractionSize())
                << (int)((PositBits.SegmentCount * 32) - ExponentSize())
                >> (int)((PositBits.SegmentCount * 32) - MaximumExponentSize);
            return exponentMask.GetLowest32Bits();
        }

        public uint FractionSize()
        {
            var bits = IsPositive() ? PositBits : PositBits.GetTwosComplement(Size);
            var fractionSize = Size - (bits.LengthOfRunOfBits((ushort)(FirstRegimeBitIndex + 1)) + 2 + MaximumExponentSize);
            return fractionSize > 0 ? (uint)fractionSize : 0;
        }

        #endregion

        #region Helper methods for operations and conversions

        public BitMask FractionWithHiddenBit()
        {
            var bits = IsPositive() ? PositBits : PositBits.GetTwosComplement(Size);
            var result = bits << (int)((PositBits.SegmentCount * 32) - FractionSize())
                >> (int)((PositBits.SegmentCount * 32) - FractionSize());
            return result.SetOne((ushort)FractionSize());
        }

        public static int CalculateScaleFactor(int regimeKValue, uint exponentValue, byte maximumExponentSize) =>
            (int)((regimeKValue * (1 << maximumExponentSize)) + exponentValue);

        #endregion

        #region operators

        public static Posit operator +(Posit left, Posit right)
        {
            var leftIsPositive = left.IsPositive();
            var rightIsPositive = right.IsPositive();
            var resultSignBit = (left.PositBits + right.PositBits).GetMostSignificantOnePosition() < left.PositBits.Size
                ? !leftIsPositive
                : !rightIsPositive;

            var signBitsMatch = leftIsPositive == rightIsPositive;

            int leftRegimeKValue;
            uint leftExponentValue;
            int rightRegimeKValue;
            uint rightExponentValue;

            if (!leftIsPositive)
            {
                var negatedLeft = -left;
                leftRegimeKValue = negatedLeft.GetRegimeKValue();
                leftExponentValue = negatedLeft.GetExponentValue();
            }
            else
            {
                leftRegimeKValue = left.GetRegimeKValue();
                leftExponentValue = left.GetExponentValue();
            }

            if (!rightIsPositive)
            {
                var negatedRight = -right;
                rightRegimeKValue = negatedRight.GetRegimeKValue();
                rightExponentValue = negatedRight.GetExponentValue();
            }
            else
            {
                rightRegimeKValue = right.GetRegimeKValue();
                rightExponentValue = right.GetExponentValue();
            }

            // Handling special cases first.
            if (leftRegimeKValue == -(left.Size - 1))
            {
                return leftIsPositive ? right : left;
            }

            if (rightRegimeKValue == -(right.Size - 1))
            {
                return rightIsPositive ? left : right;
            }

            var resultFractionBits = new BitMask(left._environment.Size); // Later on the quire will be used here.

            var scaleFactorDifference = CalculateScaleFactor(leftRegimeKValue, leftExponentValue, left.MaximumExponentSize)
                - CalculateScaleFactor(rightRegimeKValue, rightExponentValue, right.MaximumExponentSize);

            var scaleFactor =
                scaleFactorDifference >= 0
                    ? CalculateScaleFactor(leftRegimeKValue, leftExponentValue, left.MaximumExponentSize)
                    : CalculateScaleFactor(rightRegimeKValue, rightExponentValue, right.MaximumExponentSize);

            if (scaleFactorDifference == 0)
            {
                if (signBitsMatch)
                {
                    resultFractionBits += left.FractionWithHiddenBit() + right.FractionWithHiddenBit();
                }
                else
                {
                    if (left.FractionWithHiddenBit() >= right.FractionWithHiddenBit())
                    {
                        resultFractionBits += left.FractionWithHiddenBit() - right.FractionWithHiddenBit();
                    }
                    else
                    {
                        resultFractionBits += right.FractionWithHiddenBit() - left.FractionWithHiddenBit();
                    }
                }

                scaleFactor += resultFractionBits.GetMostSignificantOnePosition() -
                                      left.FractionWithHiddenBit().GetMostSignificantOnePosition();
            }
            else if (scaleFactorDifference > 0) // The scale factor of the left Posit is bigger.
            {
                var fractionSizeDifference = (int)(left.FractionSize() - right.FractionSize());
                resultFractionBits += left.FractionWithHiddenBit();
                var biggerPositMovedToLeft = left.Size - 1 - left.FractionWithHiddenBit().GetMostSignificantOnePosition();
                resultFractionBits <<= biggerPositMovedToLeft;

                if (signBitsMatch)
                {
                    resultFractionBits += right.FractionWithHiddenBit() << (biggerPositMovedToLeft - scaleFactorDifference + fractionSizeDifference);

                }
                else resultFractionBits -= right.FractionWithHiddenBit() << (biggerPositMovedToLeft - scaleFactorDifference + fractionSizeDifference);

                scaleFactor += resultFractionBits.GetMostSignificantOnePosition() - (left.Size - 1);
            }
            else // The scale factor of the right Posit is bigger.
            {
                var fractionSizeDifference = (int)(right.FractionSize() - left.FractionSize());
                resultFractionBits += right.FractionWithHiddenBit();
                var biggerPositMovedToLeft = right.Size - 1 - right.FractionWithHiddenBit().GetMostSignificantOnePosition();
                resultFractionBits <<= biggerPositMovedToLeft;

                if (signBitsMatch)
                {
                    resultFractionBits += left.FractionWithHiddenBit() << (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);
                }
                else resultFractionBits -= left.FractionWithHiddenBit() << (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference);

                scaleFactor += resultFractionBits.GetMostSignificantOnePosition() - (right.Size - 1);
            }

            if (resultFractionBits.GetMostSignificantOnePosition() == 0) return new Posit(left._environment, left.EmptyBitMask);

            var resultRegimeKValue = scaleFactor / (1 << left.MaximumExponentSize);
            var resultExponentBits = new BitMask((uint)(scaleFactor % (1 << left.MaximumExponentSize)), left._environment.Size);

            return new Posit(
                left._environment,
                left.AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, resultExponentBits, resultFractionBits));
        }

        public static Posit operator +(Posit left, int right) => left + new Posit(left._environment, right);

        public static Posit operator -(Posit left, Posit right) => left + (-right);

        public static Posit operator -(Posit left, int right) => left - new Posit(left._environment, right);

        public static Posit operator -(Posit x)
        {
            if (x.IsNaN() || x.IsZero()) return new Posit(x._environment, x.PositBits);
            return new Posit(x._environment, x.PositBits.GetTwosComplement(x.Size));
        }

        public static bool operator ==(Posit left, Posit right) => left.PositBits == right.PositBits;

        public static bool operator >(Posit left, Posit right)
        {
            if (!left.IsPositive()) left = -left;
            if (!right.IsPositive()) right = -right;
            return (left.PositBits + right.PositBits).GetMostSignificantOnePosition() > left.PositBits.Size;

        }

        public static bool operator <(Posit left, Posit right) => !(left.PositBits > right.PositBits);

        public static bool operator !=(Posit left, Posit right) => !(left == right);

        public static explicit operator int(Posit x)
        {
            uint result;

            // The posit fits into the range
            if ((x.GetRegimeKValue() * (1 << x.MaximumExponentSize)) + x.GetExponentValue() + 1 < 31)
            {
                result = (x.FractionWithHiddenBit() << (
                        (int)((x.GetRegimeKValue() * (1 << x.MaximumExponentSize)) + x.GetExponentValue()) -
                        x.FractionWithHiddenBit().GetMostSignificantOnePosition() +
                        1))
                    .GetLowest32Bits();
            }
            else
            {
                return x.IsPositive() ? int.MaxValue : int.MinValue;
            }

            return x.IsPositive() ? (int)result : (int)-result;
        }

        #endregion

    }
}
