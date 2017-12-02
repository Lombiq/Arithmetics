using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lombiq.Arithmetics.Posit
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

            var exponentValue = (uint)PositBits.GetMostSignificantOnePosition() -1;

            ushort kValue = 0;
            while (exponentValue > 1 << environment.MaximumExponentSize && kValue < _environment.Size - 1)
            {
                exponentValue -= (uint)1 << environment.MaximumExponentSize;
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
                regimeBits = (new BitMask(1, _environment.Size) << regimeKValue + 1) - 1;
                regimeBits <<= _environment.Size - regimeBits.GetMostSignificantOnePosition() - 1;
            }
            else regimeBits = (_environment.FirstRegimeBitBitMask << regimeKValue);

            return regimeBits;
        }

        private BitMask AssemblePositBits(bool signBit, int regimeKValue, BitMask exponentBits,
            BitMask fractionBits)
        {
            // Calculating the regime. 
            var wholePosit = EncodeRegimeBits(regimeKValue);

            // Attaching the exponent
            var regimeLength = wholePosit.LengthOfRunOfBits(_environment.FirstRegimeBitIndex);
            wholePosit += exponentBits << _environment.Size - (regimeLength + 3) - _environment.MaximumExponentSize;


            var fractionMostSignificantOneIndex = fractionBits.GetMostSignificantOnePosition() - 1;

            // Hiding the hidden bit. (It is always one.) 
            fractionBits = fractionBits.SetZero((ushort)fractionMostSignificantOneIndex);


            wholePosit += fractionBits << _environment.Size - 2 - fractionMostSignificantOneIndex - (regimeLength + 1) -
                          _environment.MaximumExponentSize;


            return !signBit ? wholePosit : wholePosit.GetTwosComplement(_environment.Size);
        }

        private BitMask AssemblePositBitsWithRounding(bool signBit, int regimeKValue, BitMask exponentBits,
            BitMask fractionBits)
        {
            // Calculating the regime. 
            var wholePosit = EncodeRegimeBits(regimeKValue);

            // Attaching the exponent
            var regimeLength = wholePosit.LengthOfRunOfBits(FirstRegimeBitIndex);

            var exponentShiftedLeftBy = Size - (regimeLength + 3) - MaximumExponentSize;
            wholePosit += exponentBits << exponentShiftedLeftBy;

            //calculating rounding
            if (exponentShiftedLeftBy < 0)
            {
                exponentBits <<= exponentBits.Size + exponentShiftedLeftBy;
                if (exponentBits >= new BitMask(exponentBits.Size).SetOne((ushort)(exponentBits.Size - 1)))
                {
                    if (exponentBits == new BitMask(exponentBits.Size).SetOne((ushort)(exponentBits.Size - 1)))
                    {
                        wholePosit += (wholePosit.GetLowest32Bits() % 2) == 1 ? 1 : (uint)0;
                    }
                    else wholePosit += 1;

                }
                return !signBit ? wholePosit : wholePosit.GetTwosComplement(_environment.Size);
            }

            var fractionMostSignificantOneIndex = fractionBits.GetMostSignificantOnePosition() - 1;

            // Hiding the hidden bit. (It is always one.) 
            fractionBits = fractionBits.SetZero((ushort)fractionMostSignificantOneIndex);

            var fractionShiftedLeftBy = _environment.Size - 2 - fractionMostSignificantOneIndex - (regimeLength + 1) -
                                        _environment.MaximumExponentSize;
            // Attaching the fraction.
            wholePosit += fractionBits << fractionShiftedLeftBy;
            // Calculating rounding
            if (fractionShiftedLeftBy < 0)
            {
                fractionBits <<= fractionBits.Size + fractionShiftedLeftBy;
                if (fractionBits >= new BitMask(fractionBits.Size).SetOne((ushort)(fractionBits.Size - 1)))
                {
                    if (fractionBits == new BitMask(fractionBits.Size).SetOne((ushort)(fractionBits.Size - 1)))
                    {
                        wholePosit += (wholePosit.GetLowest32Bits() % 2) == 1 ? 1 : (uint)0;
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
            return (int)(GetRegimeKValue() * Useed + GetExponentValue()+1);
        }

        public uint ExponentSize()
        {
            return Size - (PositBits.LengthOfRunOfBits(FirstRegimeBitIndex) + 3) > MaximumExponentSize
                ? MaximumExponentSize : (uint)(Size - (PositBits.LengthOfRunOfBits(FirstRegimeBitIndex) + 3));

        }

        public uint GetExponentValue()
        {
            // var regimeLength = PositBits.LengthOfRunOfBits((ushort)(FirstRegimeBitIndex));
            //var exponentShiftedLeftBy = PositBits.SegmentCount * 32 - MaximumExponentSize;

            var exponentMask = (PositBits >> (int)FractionSize())
                << (int)(PositBits.SegmentCount * 32 - ExponentSize())
                >> (int)(PositBits.SegmentCount * 32 - MaximumExponentSize);
            return exponentMask.GetLowest32Bits();
        }

        public uint FractionSize()
        {
            var fractionSize = Size - (PositBits.LengthOfRunOfBits(FirstRegimeBitIndex) + 2 + MaximumExponentSize);
            return fractionSize > 0 ? (uint)fractionSize : 0;
        }

        #endregion

        #region Helper methods for operations and conversions

        public BitMask FractionWithHiddenBit()
        {
            var result = PositBits << (int)(PositBits.SegmentCount * 32 - FractionSize())
                >> (int)(PositBits.SegmentCount * 32 - FractionSize());
            return result.SetOne((ushort)(FractionSize()));
        }
        #endregion

        #region operators

        public static Posit operator +(Posit left, Posit right)
        {
            if (left.IsNaN() || right.IsNaN()) return new Posit(left._environment, left.NaNBitMask);
            var resultFractionBits = new BitMask(left._environment.Size); // Later on the quire will be used here.
            var resultSignBit = false;
            var scaleFactor = left.CalculateScaleFactor();
            var exponentValueDifference = scaleFactor - right.CalculateScaleFactor();
            if (exponentValueDifference == 0)
            {
                resultFractionBits += left.FractionWithHiddenBit() + right.FractionWithHiddenBit();
                scaleFactor += resultFractionBits.GetMostSignificantOnePosition() -left.FractionWithHiddenBit().GetMostSignificantOnePosition();
                //resultFractionBits = resultFractionBits.ShiftOutLeastSignificantZeros();
            }
            else if (exponentValueDifference > 0) // The scale factor of the left Posit is bigger.
            {
                resultFractionBits += left.FractionWithHiddenBit();
                var biggerPositMovedToLeft = left.Size - 1 - left.FractionWithHiddenBit().GetMostSignificantOnePosition();
                resultFractionBits <<= biggerPositMovedToLeft;
                resultFractionBits += right.FractionWithHiddenBit() << biggerPositMovedToLeft - exponentValueDifference;
                scaleFactor += left.Size-1 - resultFractionBits.GetMostSignificantOnePosition();
            }
            else // The scale factor of the right Posit is bigger.
            {
                resultFractionBits += right.FractionWithHiddenBit();
                var biggerPositMovedToLeft = right.Size - 1 - right.FractionWithHiddenBit().GetMostSignificantOnePosition();
                resultFractionBits <<= biggerPositMovedToLeft;
                resultFractionBits += left.FractionWithHiddenBit() << biggerPositMovedToLeft - exponentValueDifference;
                scaleFactor += right.Size - 1 - resultFractionBits.GetMostSignificantOnePosition();
            }
            resultFractionBits = resultFractionBits.SetZero((ushort)(resultFractionBits.GetMostSignificantOnePosition() - 1));
            var resultRegimeKValue =(int)(scaleFactor / left._environment.MaximumExponentSize);
            var resultExponentBits = new BitMask((uint)(scaleFactor % left._environment.MaximumExponentSize)-1, left._environment.Size);

            return new Posit(left._environment,
                left.AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, resultExponentBits, resultFractionBits));
        }

        public static Posit operator +(Posit left, int right)
        {
            return left + new Posit(left._environment, right);
        }

        public static Posit operator -(Posit left, Posit right)
        {
            return left + (-right);
        }

        public static Posit operator -(Posit x)
        {
            if (x.IsNaN() || x.IsZero()) return new Posit(x._environment, x.PositBits);
            return new Posit(x._environment, x.PositBits.GetTwosComplement(x.Size));
        }


        public static explicit operator int(Posit x)
        {
            uint result;

            if ((x.GetRegimeKValue() * (1 << x.MaximumExponentSize)) + x.GetExponentValue() + x.FractionSize() + 1 < 31) // The posit fits into the range
            {
                result = (x.FractionWithHiddenBit() << (int)((x.GetRegimeKValue() * (1 << x.MaximumExponentSize)) + x.GetExponentValue()))
                    .GetLowest32Bits();
            }
            else return (x.IsPositive()) ? int.MaxValue : int.MinValue;
            return x.IsPositive() ? (int)result : (int)-result;
        }

        #endregion

    }
}
