using System;

namespace Lombiq.Arithmetics
{
    public struct Unum
    {
        private readonly UnumEnvironment _environment;

        // Signbit Exponent Fraction Ubit ExponentSize FractionSize
        public BitMask UnumBits { get; }

        #region Unum structure

        /// <summary>
        /// Gets the number of bits allocated to store the maximum number of bits in the exponent field of a unum.
        /// </summary>
        public byte ExponentSizeSize => _environment.ExponentSizeSize; // "esizesize"

        /// <summary>
        /// Gets the number of bits allocated to store the maximum number of bits in the fraction field of a unum.
        /// </summary>
        public byte FractionSizeSize => _environment.FractionSizeSize; // "fsizesize"

        /// <summary>
        /// Gets the maximum number of bits usable to store the exponent.
        /// </summary>
        public byte ExponentSizeMax => _environment.ExponentSizeMax; // "esizemax"

        /// <summary>
        /// Gets the maximum number of bits usable to store the fraction.
        /// </summary>
        public ushort FractionSizeMax => _environment.FractionSizeMax; // "fsizemax"

        /// <summary>
        /// Gets the number of bits that are used storing the utag.
        /// </summary>
        public byte UnumTagSize => _environment.UnumTagSize; // "utagsize"

        /// <summary>
        /// Gets the maximum number of bits used by the environment.
        /// </summary>
        public ushort Size => _environment.Size; // "maxubits"

        #endregion

        #region Unum masks

        /// <summary>
        /// Gets a BitMask for picking out the UncertainityBit.
        /// </summary>
        public BitMask UncertaintyBitMask => _environment.UncertaintyBitMask; // "ubitmask"

        /// <summary>
        /// Gets a BitMask for picking out the ExponentSize.
        /// </summary>
        public BitMask ExponentSizeMask => _environment.ExponentSizeMask; // "esizemask"

        /// <summary>
        /// Gets a BitMask for picking out the FractionSize.
        /// </summary>
        public BitMask FractionSizeMask => _environment.FractionSizeMask; // "fsizemask"

        /// <summary>
        /// Gets a BitMask for picking out the ExponentSize and FractionSize.
        /// </summary>
        public BitMask ExponentAndFractionSizeMask => _environment.ExponentAndFractionSizeMask; // "efsizemask"

        /// <summary>
        /// Gets a BitMask for picking out the utag.
        /// </summary>
        public BitMask UnumTagMask => _environment.UnumTagMask; // "utagmask"

        /// <summary>
        /// Gets a BitMask for picking out the SignBit.
        /// </summary>
        public BitMask SignBitMask => _environment.SignBitMask; // "signbigu"

        #endregion

        #region Unum environment

        /// <summary>
        /// Gets a BitMask for the Unit in the Last Place or Unit of Least Precision.
        /// </summary>
        public BitMask ULP => _environment.ULP;

        /// <summary>
        /// Gets a BitMask for the unum notation of positive infinity.
        /// </summary>
        public BitMask PositiveInfinity => _environment.PositiveInfinity; // "posinfu"

        /// <summary>
        /// Gets a BitMask for the unum notation of negative infinity.
        /// </summary>
        public BitMask NegativeInfinity => _environment.NegativeInfinity; // "neginfu"

        /// <summary>
        /// Gets a BitMask for the unum notation of a quiet NaN value.
        /// </summary>
        public BitMask QuietNotANumber => _environment.QuietNotANumber; // "qNaNu"

        /// <summary>
        /// Gets a BitMask for the unum notation of a signaling NaN value.
        /// </summary>
        public BitMask SignalingNotANumber => _environment.SignalingNotANumber; // "sNaNu"

        /// <summary>
        /// Gets a BitMask for the largest expressable finite positive unum in the environment.
        /// </summary>
        public BitMask LargestPositive => _environment.LargestPositive; // "maxrealu"

        /// <summary>
        /// Gets a BitMask for the smallest expressable positive real unum in the environment.
        /// </summary>
        public BitMask SmallestPositive => _environment.SmallestPositive; // "smallsubnormalu"

        /// <summary>
        /// Gets a BitMask for the largest expressable finite negative unum in the environment.
        /// </summary>
        public BitMask LargestNegative => _environment.LargestNegative; // "negbigu"

        /// <summary>
        /// Gets a BitMask for the largest magnitude negative unum in the environment.
        /// </summary>
        public BitMask MinRealU => _environment.MinRealU; // "minrealu"

        #endregion

        #region Unum constructors

        public Unum(UnumEnvironment environment)
        {
            _environment = environment;

            UnumBits = new BitMask(_environment.Size);
        }

        public Unum(UnumEnvironment environment, BitMask bits)
        {
            _environment = environment;

            // To be sure that UnumBits has the same size as the environment. Excess bits will be truncated.
            UnumBits = BitMask.FromImmutableArray(bits.Segments, _environment.Size);
        }

        // This doesn't work for all cases yet.
        //public Unum(UnumEnvironment environment, float number)
        //{
        //    _environment = environment;

        //    // Handling special cases first.
        //    if (float.IsNaN(number))
        //    {
        //        UnumBits = _environment.QuietNotANumber;
        //        return;
        //    }
        //    if (float.IsPositiveInfinity(number))
        //    {
        //        UnumBits = _environment.PositiveInfinity;
        //        return;
        //    }
        //    if (float.IsNegativeInfinity(number))
        //    {
        //        UnumBits = _environment.NegativeInfinity;
        //        return;
        //    }


        //    UnumBits = new BitMask(_environment.Size);
        //    var floatExponentBits = (BitConverter.ToUInt32(BitConverter.GetBytes(number), 0) << 1) >> 24;

        //    // These are the only uncertain cases that we can safely handle without Ubounds.
        //    if (ExponentSizeMax < ExponentValueToExponentSize((int)floatExponentBits - 127))
        //    {
        //        // The exponent is too big, so we express the number as the largest possible signed value,
        //        // but the Unum is uncertain, meaning that it's finite, but too big to express.
        //        if (floatExponentBits - 127 > 0)
        //            UnumBits = IsPositive() ? LargestPositive : LargestNegative;
        //        else // If the exponent is too small, we will handle it as a signed uncertain zero.
        //        {
        //            UnumBits = new BitMask(Size);
        //            if (!IsPositive()) Negate();
        //        }

        //        SetUncertainityBit(true);

        //        return;
        //    }


        //    var floatFractionBits = (BitConverter.ToUInt32(BitConverter.GetBytes(number), 0) << 9) >> 9;
        //    uint resultFractionSize = 23;
        //    uint floatFractionBitsSize = 23;

        //    if (floatFractionBits == 0) resultFractionSize = 0;
        //    else
        //        while (floatFractionBits % 2 == 0)
        //        {
        //            resultFractionSize -= 1;
        //            floatFractionBits >>= 1;
        //            floatFractionBitsSize = resultFractionSize;
        //        }


        //    var uncertainty = false;

        //    if (FractionSizeMax + 1 < resultFractionSize)
        //    {
        //        resultFractionSize = ((uint)FractionSizeMax - 1);
        //        uncertainty = true;
        //    }
        //    else if (resultFractionSize > 0) resultFractionSize = (resultFractionSize - 1);

        //    var resultFraction = uncertainty ?
        //        new BitMask(new uint[] { floatFractionBits >> (int)floatFractionBitsSize - FractionSizeMax }, Size) :
        //        new BitMask(new uint[] { floatFractionBits }, Size);
        //    var resultExponent = ExponentValueToExponentBits((int)(floatExponentBits - 127), Size);
        //    var floatBits = BitConverter.ToUInt32(BitConverter.GetBytes(number), 0);
        //    var resultSignBit = (floatBits > uint.MaxValue / 2);
        //    var resultExponentSize = (ExponentValueToExponentSize((int)floatExponentBits - 127) - 1);


        //    AssembleUnumBits(resultSignBit, resultExponent, resultFraction,
        //        uncertainty, resultExponentSize, resultFractionSize);
        //}

        /// <summary>
        /// Creates a Unum of the given environment initialized with the value of the uint.
        /// </summary>
        /// <param name="environment">The Unum environment.</param>
        /// <param name="value">The uint value to initialize the new Unum with.</param>
        public Unum(UnumEnvironment environment, uint value)
        {
            _environment = environment;
            // Creating an array of the size needed to call the other constructor.
            // This is necessary because in Hastlayer only arrays with dimensions defined at compile-time are supported.
            var valueArray = new uint[environment.EmptyBitMask.SegmentCount];
            valueArray[0] = value;

            UnumBits = new Unum(environment, valueArray).UnumBits;
        }

        /// <summary>
        /// Creates a Unum initialized with a value that is defined by the bits in a uint array.
        /// </summary>
        /// <param name="environment">The Unum environment.</param>
        /// <param name="value">
        /// The uint array which defines the Unum's value as an integer.
        /// To use with Hastlayer this should be the same size as the BitMasks in the given environment.
        /// </param>
        /// <param name="negative">Defines whether the number is positive or not.</param>
        public Unum(UnumEnvironment environment, uint[] value, bool negative = false)
        {
            _environment = environment;

            UnumBits = new BitMask(value, environment.Size);
            if (UnumBits == _environment.EmptyBitMask) return;

            // Handling the case when the number wouldn't fit in the range of the environment.
            if (UnumHelper.LargestExpressablePositiveInteger(environment) != environment.EmptyBitMask)
            {
                if (UnumBits > UnumHelper.LargestExpressablePositiveInteger(environment))
                {
                    UnumBits = negative ? environment.LargestNegative | environment.UncertaintyBitMask
                        : environment.LargestPositive | environment.UncertaintyBitMask;
                    return;
                }
            }
            var uncertainityBit = false;

            // Putting the actual value in a BitMask.
            var exponent = new BitMask(value, Size);

            // The value of the exponent is one less than the number of binary digits in the integer.
            var exponentValue = new BitMask((uint)(exponent.GetMostSignificantOnePosition() - 1), Size);

            // Calculating the number of bits needed to represent the value of the exponent.
            var exponentSize = exponentValue.GetMostSignificantOnePosition();

            // If the value of the exponent is not a power of 2,
            // then one more bit is needed to represent the biased value.
            if ((exponentValue.GetLowest32Bits() & exponentValue.GetLowest32Bits() - 1) > 0) exponentSize++;

            // Handling input numbers that don't fit in the range of the given environment.
            if (exponentSize > ExponentSizeMax)
            {
                UnumBits = negative ? (environment.LargestNegative | environment.UncertaintyBitMask) - 1
                    : (environment.LargestPositive | environment.UncertaintyBitMask) - 1;
                return;
            }
            // Calculating the bias from the number of bits representing the exponent.
            var bias = exponentSize == 0 ? 0 : (1 << exponentSize - 1) - 1;

            // Applying the bias to the exponent.
            exponent = exponentValue + (uint)bias;

            // Putting the actual value in a BitMask.
            var fraction = new BitMask(value, Size);

            // Shifting out the zeros after the least significant 1-bit.
            fraction = fraction.ShiftOutLeastSignificantZeros();

            // Calculating the number of bits needed to represent the fraction.
            var fractionSize = fraction.GetMostSignificantOnePosition();


            /* If there's a hidden bit and it's 1,
              * then the most significant 1-bit of the fraction is stored there,
              * so we're removing it from the fraction and decreasing fraction size accordingly. */
            if (exponent.GetLowest32Bits() > 0)
            {
                fractionSize--;
                fraction = fraction.SetZero(fractionSize);
            }
            // Handling input numbers that fit in the range, but are too big to represent exactly.
            if (fractionSize > FractionSizeMax)
            {
                fraction = fraction >> FractionSizeMax - fractionSize;
                uncertainityBit = true;
            }

            UnumBits = AssembleUnumBits(negative, exponent, fraction,
                uncertainityBit, (byte)(exponentSize > 0 ? --exponentSize : 0), (ushort)(fractionSize > 0 ? --fractionSize : 0));
        }

        public Unum(UnumEnvironment environment, int value)
        {
            _environment = environment;

            // Creating an array of the size needed to call the other constructor.
            // This is necessary because in Hastlayer only arrays with dimensions defined at compile-time are supported.
            var valueArray = new uint[environment.EmptyBitMask.SegmentCount];

            if (value >= 0)
            {
                valueArray[0] = (uint)value;
                UnumBits = new Unum(environment, valueArray).UnumBits;
            }
            else
            {
                valueArray[0] = (uint)-value;
                UnumBits = new Unum(environment, valueArray, true).UnumBits;
            }

        }

        // This doesn't work for all cases yet.
        //public Unum(UnumEnvironment environment, double x)
        //{
        //    _environment = environment;
        //    UnumBits = _environment.EmptyBitMask;

        //    // Handling special cases first.
        //    if (double.IsNaN(x))
        //    {
        //        UnumBits = QuietNotANumber;
        //        return;
        //    }
        //    if (double.IsPositiveInfinity(x))
        //    {
        //        UnumBits = PositiveInfinity;
        //        return;
        //    }
        //    if (double.IsNegativeInfinity(x))
        //    {
        //        UnumBits = NegativeInfinity;
        //        return;
        //    }


        //    var doubleBits = BitConverter.ToUInt64(BitConverter.GetBytes(x), 0);
        //    SetSignBit((doubleBits > ulong.MaxValue / 2));


        //    var doubleFractionBits = (BitConverter.ToUInt64(BitConverter.GetBytes(x), 0) << 12) >> 12;
        //    uint resultFractionSize = 52;

        //    if (doubleFractionBits == 0) resultFractionSize = 0;
        //    else
        //    {
        //        while (doubleFractionBits % 2 == 0)
        //        {
        //            resultFractionSize -= 1;
        //            doubleFractionBits >>= 1;
        //        }

        //    }


        //    var uncertainty = false;

        //    if (FractionSizeMax < resultFractionSize - 1)
        //    {
        //        SetFractionSizeBits((uint)(FractionSizeMax - 1));
        //        uncertainty = true;
        //    }
        //    else SetFractionSizeBits(resultFractionSize - 1);


        //    var doubleExponentBits = (BitConverter.ToUInt64(BitConverter.GetBytes(x), 0) << 1) >> 53;

        //    // These are the only uncertain cases that we can safely handle without Ubounds.
        //    if (ExponentSizeMax < ExponentValueToExponentSize((int)doubleExponentBits - 1023))
        //    {
        //        // The exponent is too big, so we express the number as the largest possible signed value,
        //        // but the Unum is uncertain, meaning that it's finite, but too big to express.
        //        if (doubleExponentBits - 1023 > 0)
        //            UnumBits = IsPositive() ? LargestPositive : LargestNegative;
        //        else // If the exponent is too small, we will handle it as a signed uncertain zero.
        //        {
        //            UnumBits = _environment.EmptyBitMask;
        //            if (!IsPositive()) Negate();
        //        }

        //        SetUncertainityBit(true);

        //        return;
        //    }


        //    var exponentSizeBits = ExponentValueToExponentSize((int)doubleExponentBits - 1023) - 1;
        //    SetExponentSizeBits(exponentSizeBits);

        //    var doubleFraction = new uint[2];
        //    doubleFraction[0] = (uint)((doubleFractionBits << 32) >> 32);
        //    doubleFraction[1] = (uint)((doubleFractionBits >> 32));

        //    if (uncertainty)
        //    {
        //        SetFractionBits(Size > 32 ?
        //            // This is necessary because Hastlayer enables only one size of BitMasks.
        //            new BitMask(doubleFraction, Size) >> ((int)resultFractionSize - (int)FractionSize()) :
        //            // The lower 32 bits wouldn't fit in anyway.
        //            new BitMask(new uint[] { doubleFraction[1] }, Size) >> ((int)resultFractionSize - FractionSizeMax));

        //        SetUncertainityBit(true);
        //    }
        //    else
        //        SetFractionBits(Size > 32 ?
        //            // This is necessary because Hastlayer enables only one size of BitMasks.
        //            new BitMask(doubleFraction, Size) :
        //            // The lower 32 bits wouldn't fit in anyway.
        //            new BitMask(new uint[] { doubleFraction[1] }, Size));


        //    SetExponentBits(ExponentValueToExponentBits((int)(doubleExponentBits - 1023), Size));
        //}

        #endregion

        #region Methods to set the values of individual Unum structure elements



        /// <summary>
        /// Assembles the Unum from its pre-computed parts.
        /// </summary>
        /// <param name="signBit">The SignBit of the Unum.</param>
        /// <param name="exponent">The biased notation of the exponent of the Unum.</param>
        /// <param name="fraction">The fraction of the Unum without the hidden bit.</param>
        /// <param name="uncertainityBit">The value of the uncertainity bit (Ubit).</param>
        /// <param name="exponentSize">The Unum's exponent size, in a notation that is one less than the actual value.</param>
        /// <param name="fractionSize">The Unum's fraction size, in a notation that is one less than the actual value.</param>
        /// <returns>The BitMask representing the whole Unum with all the parts set.</returns>
        private BitMask AssembleUnumBits(bool signBit, BitMask exponent, BitMask fraction,
             bool uncertainityBit, byte exponentSize, ushort fractionSize)
        {
            var wholeUnum = new BitMask(exponentSize, Size) << FractionSizeSize;
            wholeUnum += fractionSize;

            if (uncertainityBit) wholeUnum += UncertaintyBitMask;

            wholeUnum += fraction << UnumTagSize;
            wholeUnum += exponent << (UnumTagSize + fractionSize + 1);

            if (signBit) wholeUnum += SignBitMask;

            return wholeUnum;
        }

        /// <summary>
        /// Sets the SignBit to the given value and leaves everything else as is.
        /// </summary>
        /// <param name="signBit">The desired SignBit.</param>
        /// <returns>The BitMask representing the Unum with its SignBit set to the given value.</returns>
        public Unum SetSignBit(bool signBit)
        {
            var newUnumBits = signBit ? UnumBits | SignBitMask : UnumBits & (new BitMask(Size, true) ^ (SignBitMask));
            return new Unum(_environment, newUnumBits);
        }

        /// <summary>
        /// Sets the Ubit to the given value and leaves everything else as is.
        /// </summary>
        /// <param name="uncertainityBit">The desired UBit.</param>
        /// <returns>The BitMask representing the Unum with its UBit set to the given value.</returns>
        public Unum SetUncertainityBit(bool uncertainityBit)
        {
            var newUnumBits = uncertainityBit ? UnumBits | UncertaintyBitMask : UnumBits & (~UncertaintyBitMask);
            return new Unum(_environment, newUnumBits);
        }

        /// <summary>
        /// Changes the exponent to the bitstring given in the input BitMask and leaves everything else as is.
        /// </summary>
        /// <param name="exponent">The desired exponent in biased notation.</param>
        /// <returns>The BitMask representing the Unum with its exponent set to the given value.</returns>
        public Unum SetExponentBits(BitMask exponent)
        {
            var newUnumBits = (UnumBits & (new BitMask(Size, true) ^ ExponentMask())) |
                   (exponent << (FractionSizeSize + ExponentSizeSize + 1 + FractionSize()));
            return new Unum(_environment, newUnumBits);
        }


        /// <summary>
        /// Sets the fraction to the given value and leaves everything else as is.
        /// </summary>
        /// <param name="fraction">The desired fraction without the hidden bit.</param>
        /// <returns>The BitMask representing the Unum with its fraction set to the given value.</returns>
        public Unum SetFractionBits(BitMask fraction)
        {
            var newUnumBits = (UnumBits & (new BitMask(Size, true) ^ FractionMask())) |
                   (fraction << FractionSizeSize + ExponentSizeSize + 1);
            return new Unum(_environment, newUnumBits);
        }

        /// <summary>
        /// Sets the fractionSize to the given value and leaves everything else as is.
        /// </summary>
        /// <param name="fractionSize">
        /// The desired fractionSize in a notation that is one less than the actual value.
        /// </param>
        /// <returns>The BitMask representing the Unum with its fractionSize set to the given value.</returns>
        public Unum SetFractionSizeBits(byte fractionSize)
        {
            var newUnumBits = (UnumBits & (new BitMask(Size, true) ^ FractionSizeMask)) |
                  new BitMask(fractionSize, Size);
            return new Unum(_environment, newUnumBits);
        }

        /// <summary>
        /// Sets the exponentSize to the given value and leaves everything else as is.
        /// </summary>
        /// <param name="fraction">
        /// The desired exponentSize in a notation that is one less than the actual value.
        /// </param>
        /// <returns>The BitMask representing the Unum with its exponentSize set to the given value.</returns>
        public Unum SetExponentSizeBits(byte exponentSize)
        {
            var newUnumBits = (UnumBits & (new BitMask(Size, true) ^ ExponentSizeMask) |
                   (new BitMask(exponentSize, Size) << FractionSizeSize));
            return new Unum(_environment, newUnumBits);
        }

        #endregion

        #region Binary data extraction

        /// <summary>
        /// Copies the actual integer value represented by the Unum into an array of unsigned integers with the 
        /// most significant bit of the last element functioning as the signbit.
        /// </summary>
        /// <returns>
        /// An array of unsigned integers that together represent the integer value of the Unum with the most 
        /// significant bit of the last uint functioning as a signbit.
        /// </returns>
        public uint[] FractionToUintArray()
        {
            var resultMask = FractionWithHiddenBit() << ExponentValueWithBias() - (int)FractionSize();
            var result = new uint[resultMask.SegmentCount];

            for (var i = 0; i < resultMask.SegmentCount; i++) result[i] = resultMask.Segments[i];
            if (!IsPositive()) result[resultMask.SegmentCount - 1] |= 0x80000000;
            else
            {
                result[resultMask.SegmentCount - 1] <<= 1;
                result[resultMask.SegmentCount - 1] >>= 1;
            }

            return result;
        }

        #endregion

        #region Binary data manipulation

        public Unum Negate()
        {
            var newUnumBits = UnumBits ^ SignBitMask;
            return new Unum(_environment, newUnumBits);
        }

        #endregion

        #region Unum numeric states

        public bool IsExact() => (UnumBits & UncertaintyBitMask) == _environment.EmptyBitMask;

        public bool IsPositive() => (UnumBits & SignBitMask) == _environment.EmptyBitMask;

        // This is needed because there are many valid representations of zero in an Unum environment.
        public bool IsZero() =>
            (UnumBits & UncertaintyBitMask) == _environment.EmptyBitMask &&
            (UnumBits & FractionMask()) == _environment.EmptyBitMask &&
            (UnumBits & ExponentMask()) == _environment.EmptyBitMask;

        #endregion

        #region  Methods for Utag independent Masks and values

        public byte ExponentSize() => (byte)(((UnumBits & ExponentSizeMask) >> FractionSizeSize) + 1).GetLowest32Bits();

        public ushort FractionSize() => (ushort)((UnumBits & FractionSizeMask) + 1).GetLowest32Bits();

        public BitMask FractionMask()
        {
            var fractionMask = new BitMask(1, Size);
            return ((fractionMask << FractionSize()) - 1) << UnumTagSize;
        }

        public BitMask ExponentMask()
        {
            var exponentMask = new BitMask(1, Size);
            return ((exponentMask << ExponentSize()) - 1) << (FractionSize() + UnumTagSize);
        }

        #endregion

        #region Methods for Utag dependent Masks and values

        public BitMask Exponent() => (ExponentMask() & UnumBits) >> (UnumTagSize + FractionSize());

        public BitMask Fraction() => (FractionMask() & UnumBits) >> UnumTagSize;

        public BitMask FractionWithHiddenBit() =>
            HiddenBitIsOne() ? Fraction().SetOne(FractionSize()) : Fraction();

        public ushort FractionSizeWithHiddenBit() => HiddenBitIsOne() ? (ushort)(FractionSize() + 1) : FractionSize();

        public int Bias() => (1 << (ExponentSize() - 1)) - 1;

        public bool HiddenBitIsOne() => Exponent().GetLowest32Bits() > 0;

        public int ExponentValueWithBias() => (int)Exponent().GetLowest32Bits() - Bias() + (HiddenBitIsOne() ? 0 : 1);

        public bool IsNan() => UnumBits == SignalingNotANumber || UnumBits == QuietNotANumber;

        public bool IsPositiveInfinity() => UnumBits == PositiveInfinity;

        public bool IsNegativeInfinity() => UnumBits == NegativeInfinity;

        #endregion

        #region Operations for exact Unums

        public static Unum AddExactUnums(Unum left, Unum right)
        {
            // It could be only FractionSizeMax + 2 long if that would be Hastlayer-compatible (it isn't due to static
            // array sizes).
            var scratchPad = new BitMask(left._environment.Size);

            // Handling special cases first.
            if (left.IsNan() || right.IsNan())
                return new Unum(left._environment, left.QuietNotANumber);

            if ((left.IsPositiveInfinity() && right.IsNegativeInfinity()) ||
                (left.IsNegativeInfinity() && right.IsPositiveInfinity()))
                return new Unum(left._environment, left.QuietNotANumber);

            if (left.IsPositiveInfinity() || right.IsPositiveInfinity())
                return new Unum(left._environment, left.PositiveInfinity);

            if (left.IsNegativeInfinity() || right.IsNegativeInfinity())
                return new Unum(left._environment, left.NegativeInfinity);

            var exponentValueDifference = left.ExponentValueWithBias() - right.ExponentValueWithBias();
            var signBitsMatch = left.IsPositive() == right.IsPositive();
            var resultSignBit = false;
            var biggerBitsMovedToLeft = 0;
            var smallerBitsMovedToLeft = 0;
            var resultExponentValue = 0;


            if (exponentValueDifference == 0) // Exponents are equal.
            {
                resultExponentValue = left.ExponentValueWithBias();

                // We align the fractions so their Most Significant Bit gets to the leftmost position that the 
                // FractionSize allows. This way the digits that won't fit automatically get lost.
                biggerBitsMovedToLeft = left.FractionSizeMax + 1 - (left.FractionSize() + 1);
                smallerBitsMovedToLeft = left.FractionSizeMax + 1 - (right.FractionSize() + 1);
                // Adding the aligned Fractions.
                scratchPad = AddAlignedFractions(
                    left.FractionWithHiddenBit() << biggerBitsMovedToLeft,
                    right.FractionWithHiddenBit() << smallerBitsMovedToLeft,
                    signBitsMatch);


                if (!signBitsMatch)
                {
                    // If the value of the Hidden Bits match we just compare the fractions,
                    // and get the Sign of the bigger one.
                    if (left.HiddenBitIsOne() == right.HiddenBitIsOne())
                    {
                        resultSignBit = left.Fraction() >= right.Fraction()
                            ? !left.IsPositive() // Left Fraction is bigger.
                            : !right.IsPositive(); // Right Fraction is bigger.

                    }
                    // Otherwise we get the Sign of the number that has a Hidden Bit set.
                    else resultSignBit = left.HiddenBitIsOne() ? !left.IsPositive() : !right.IsPositive();
                }
                else resultSignBit = !left.IsPositive();

            }
            else if (exponentValueDifference > 0) // Left Exponent is bigger.
            {
                // We align the fractions according to their exponent values so the Most Significant Bit  of the bigger 
                // number gets to the leftmost position that the  FractionSize allows. 
                // This way the digits that won't fit automatically get lost.
                resultSignBit = !left.IsPositive();
                resultExponentValue = left.ExponentValueWithBias();
                biggerBitsMovedToLeft = left.FractionSizeMax + 1 - (left.FractionSize() + 1);
                smallerBitsMovedToLeft = left.FractionSizeMax + 1 - (right.FractionSize() + 1) - exponentValueDifference;

                scratchPad = left.FractionWithHiddenBit() << biggerBitsMovedToLeft;
                // Adding the aligned Fractions.
                scratchPad = AddAlignedFractions(scratchPad,
                    right.FractionWithHiddenBit() << smallerBitsMovedToLeft, signBitsMatch);
            }
            else // Right Exponent is bigger.
            {

                // We align the fractions according to their exponent values so the Most Significant Bit  of the bigger 
                // number gets to the leftmost position that the  FractionSize allows. 
                // This way the digits that won't fit automatically get lost.
                resultSignBit = !right.IsPositive();
                resultExponentValue = right.ExponentValueWithBias();
                biggerBitsMovedToLeft = left.FractionSizeMax + 1 - (right.FractionSize() + 1);
                smallerBitsMovedToLeft = left.FractionSizeMax + 1 - (left.FractionSize() + 1) + exponentValueDifference;

                scratchPad = right.FractionWithHiddenBit() << biggerBitsMovedToLeft;
                // Adding the aligned Fractions.
                scratchPad = AddAlignedFractions(scratchPad,
                    left.FractionWithHiddenBit() << smallerBitsMovedToLeft, signBitsMatch);
            }

            // Calculating how the addition changed the exponent of the result.
            var exponentChange = scratchPad.GetMostSignificantOnePosition() - (left.FractionSizeMax + 1);
            var resultExponent = new BitMask(left._environment.Size) +
                ExponentValueToExponentBits(resultExponentValue + exponentChange, left.Size);
            // Calculating the ExponentSize needed to the excess-k notation of the results Exponent value. 
            var resultExponentSize = (byte)(ExponentValueToExponentSize(resultExponentValue + exponentChange) - 1);

            var resultUbit = false;
            if (smallerBitsMovedToLeft < 0) resultUbit = true; // There are lost digits, so we set the ubit to 1.
            // If there are no lost digits, we can shift out the least significant zeros to save space.
            else scratchPad = scratchPad.ShiftOutLeastSignificantZeros();

            ushort resultFractionSize = 0;

            //Calculating the results FractionSize.
            if (scratchPad.GetMostSignificantOnePosition() == 0) // If the Fraction is zero, so is the FractionSize.
            {
                resultExponent = scratchPad; // 0
                resultExponentSize = 0; // If the Fraction is zero, so is the ExponentSize.
            }
            else resultFractionSize = (ushort)(scratchPad.GetMostSignificantOnePosition() - 1);


            if (resultExponent.GetMostSignificantOnePosition() != 0) // Erase the hidden bit if it is set.
            {
                scratchPad = scratchPad.SetZero((ushort)(scratchPad.GetMostSignificantOnePosition() - 1));
                resultFractionSize = (ushort)(resultFractionSize == 0 ? 0 : resultFractionSize - 1);
            }

            // This is temporary, for the imitation of float behaviour. Now the Ubit works as a flag for rounded values.
            // When Ubounds will be implemented this should be handled in the addition operator.
            if ((!left.IsExact()) || (!right.IsExact())) resultUbit = true;

            // Setting the parts of the result Unum to the calculated values.
            var resultBitMask = left.AssembleUnumBits(resultSignBit, resultExponent, scratchPad,
                resultUbit, resultExponentSize, resultFractionSize);

            return new Unum(left._environment, resultBitMask);
        }

        public static Unum SubtractExactUnums(Unum left, Unum right) => AddExactUnums(left, NegateExactUnum(right));

        public static Unum NegateExactUnum(Unum input) => input.Negate();

        public static bool AreEqualExactUnums(Unum left, Unum right) =>
            left.IsZero() && right.IsZero() ? true : left.UnumBits == right.UnumBits;


        #endregion

        #region Helper methods for operations and conversions

        public static BitMask ExponentValueToExponentBits(int value, ushort size)
        {

            var exponent = new BitMask((uint)((value < 0) ? -value : value), size);
            var exponentSize = ExponentValueToExponentSize(value);
            exponent += (uint)(1 << (exponentSize - 1)) - 1; // Applying bias

            if (value < 0) // In case of a negative exponent the 
            {
                exponent -= (uint)(-2 * value);

            }

            return exponent;

        }

        public static byte ExponentValueToExponentSize(int value)
        {
            byte size = 1;

            if (value > 0) while (value > 1 << (size - 1)) size++;
            else while (-value >= 1 << (size - 1)) size++;

            return size;
        }

        public static BitMask AddAlignedFractions(BitMask left, BitMask right, bool signBitsMatch)
        {
            var mask = new BitMask(left.Size);

            if (signBitsMatch) mask = left + right;
            else mask = left > right ? left - right : right - left;

            return mask;
        }

        #endregion

        #region Operators

        public static Unum operator +(Unum left, Unum right) => AddExactUnums(left, right);

        public static Unum operator -(Unum x) => NegateExactUnum(x);

        public static Unum operator -(Unum left, Unum right) => SubtractExactUnums(left, right);

        //public static Unum operator *(Unum left, Unum right)
        //{
        //    if (left.IsExact() && right.IsExact()) return MultiplyExactUnums(left, right);

        //    return new Unum();
        //}

        //public static Unum operator /(Unum left, Unum right)
        //{

        //}

        public static bool operator ==(Unum left, Unum right) => AreEqualExactUnums(left, right);

        public static bool operator !=(Unum left, Unum right) => !(left == right);

        //public static bool operator <(Unum left, Unum right)
        // {
        //     if (left.IsPositive() != right.IsPositive()) return left.IsPositive();
        //     if (left.ExponentValueWithBias() > right.ExponentValueWithBias()) return left.IsPositive();
        //     if (left.ExponentValueWithBias() < right.ExponentValueWithBias()) return right.IsPositive();
        //     // if (left.FractionWithHiddenBit())

        //     return false;
        // }

        //public static bool operator >(Unum left, Unum right)
        //{

        //}

        //public static bool operator <=(Unum left, Unum right)
        //{

        //}

        //public static bool operator >=(Unum left, Unum right)
        //{

        //}

        //public static implicit operator Unum(short x)
        //{

        //}

        //public static implicit operator Unum(ushort x)
        //{

        //}

        //Converting from an Unum to int results in information loss, so only allowing it explicitly (with a cast).
        public static explicit operator int(Unum x)
        {
            uint result;

            if ((x.ExponentValueWithBias() + (int)x.FractionSizeWithHiddenBit()) < 31) //The Unum fits into the range.
                result = (x.FractionWithHiddenBit() << x.ExponentValueWithBias() - (int)x.FractionSize()).GetLowest32Bits();
            else return (x.IsPositive()) ? int.MaxValue : int.MinValue; // The absolute value of the Unum is too large.

            return x.IsPositive() ? (int)result : -(int)result;
        }

        public static explicit operator uint(Unum x) =>
            (x.FractionWithHiddenBit() << x.ExponentValueWithBias() - ((int)x.FractionSize())).GetLowest32Bits();

        // This is not well tested yet.
        public static explicit operator float(Unum x)
        {
            // Handling special cases first.
            if (x.IsNan()) return float.NaN;
            if (x.IsNegativeInfinity()) return float.NegativeInfinity;
            if (x.IsPositiveInfinity()) return float.PositiveInfinity;
            if (x.ExponentValueWithBias() > 127) // Exponent is too big for float format.
                return (x.IsPositive()) ? float.PositiveInfinity : float.NegativeInfinity;
            if (x.ExponentValueWithBias() < -126) return (x.IsPositive()) ? 0 : -0; // Exponent is too small for float format.

            var result = (x.Fraction() << 23 - ((int)x.FractionSize())).GetLowest32Bits();
            result |= (uint)(x.ExponentValueWithBias() + 127) << 23;

            return x.IsPositive() ?
                BitConverter.ToSingle(BitConverter.GetBytes(result), 0) :
                -BitConverter.ToSingle(BitConverter.GetBytes(result), 0);
        }

        #endregion

        #region Overrides
        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => base.ToString();

        #endregion
    }
}
