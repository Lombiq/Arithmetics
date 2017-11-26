using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lombiq.Arithmetics.Posit
{
    public struct Posit
    {
        private readonly PositEnvironment _environment;
        public BitMask PositBits { get; }

        #region Posit structure

        public byte MaximumExponentSize => _environment.MaximumExponentSize;

        public ushort Size => _environment.Size;

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

            var regimeBits = new BitMask(1, _environment.Size);
            var exponentValue = (uint)PositBits.GetMostSignificantOnePosition() - 1;

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
            var regimeLength = wholePosit.LengthOfRunOfBits(_environment.FirstRegimeBitPosition);
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
            var regimeLength = wholePosit.LengthOfRunOfBits(_environment.FirstRegimeBitPosition);

            var exponentShiftedLeftBy = _environment.Size - (regimeLength + 3) - _environment.MaximumExponentSize;
            wholePosit += exponentBits << exponentShiftedLeftBy;

            //calculating rounding
            if (exponentShiftedLeftBy < 0)
            {
                exponentBits <<= exponentBits.Size + exponentShiftedLeftBy;
                if (exponentBits > new BitMask(exponentBits.Size).SetOne((ushort)(exponentBits.Size - 1)))
                {
                    wholePosit += (wholePosit.GetLowest32Bits() % 2) == 1 ? 1 : (uint)0;
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
                    }else wholePosit += 1;
                }
               
            }


            return !signBit ? wholePosit : wholePosit.GetTwosComplement(_environment.Size);
        }

        public int GetRegimeKValue()
        {
            return PositBits.LengthOfRunOfBits(_environment.FirstRegimeBitPosition);
        }

        public uint GetExponentValue()
        {
            var exponentMask= PositBits << PositBits.LengthOfRunOfBits(_environment.FirstRegimeBitPosition) + 2;
            return exponentMask.GetLowest32Bits();
        }

        #endregion

        #region operators

        public static Posit operator +(Posit left, Posit right)
        {



            return new Posit();
        }

        #endregion

    }
}
