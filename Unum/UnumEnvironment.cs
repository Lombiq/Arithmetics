namespace Lombiq.Arithmetics
{
    public class UnumEnvironment
    {
        #region Unum structure

        /// <summary>
        /// Gets the number of bits allocated to store the maximum number of bits in the exponent field of a unum.
        /// </summary>
        public byte ExponentSizeSize { get; } // "esizesize"

        /// <summary>
        /// Gets the number of bits allocated to store the maximum number of bits in the fraction field of a unum.
        /// </summary>
        public byte FractionSizeSize { get; } // "fsizesize"

        /// <summary>
        /// Gets the maximum number of bits usable to store the exponent.
        /// </summary>
        public byte ExponentSizeMax { get; } // "esizemax"

        /// <summary>
        /// Gets the maximum number of bits usable to store the fraction.
        /// </summary>
        public ushort FractionSizeMax { get; } // "fsizemax"

        /// <summary>
        /// Gets the number of bits that are used for storing the utag.
        /// </summary>
        public byte UnumTagSize { get; } // "utagsize"

        /// <summary>
        /// Gets the maximum number of bits used by the environment.
        /// </summary>
        public ushort Size { get; } // "maxubits"
        #endregion

        #region Unum masks

        /// <summary>
        /// Gets an empty BitMask the size of the environment.
        /// </summary>
        public BitMask EmptyBitMask { get; }

        /// <summary>
        /// Gets a BitMask for picking out the UncertainityBit.
        /// </summary>
        public BitMask UncertaintyBitMask { get; } // "ubitmask"

        /// <summary>
        /// Gets a BitMask for picking out the ExponentSize.
        /// </summary>
        public BitMask ExponentSizeMask { get; } // "esizemask"

        /// <summary>
        /// Gets a BitMask for picking out the FractionSize.
        /// </summary>
        public BitMask FractionSizeMask { get; } // "fsizemask"

        /// <summary>
        /// Gets a BitMask for picking out the ExponentSize and FractionSize.
        /// </summary>
        public BitMask ExponentAndFractionSizeMask { get; } // "efsizemask"

        /// <summary>
        /// Gets a BitMask for picking out the utag.
        /// </summary>
        public BitMask UnumTagMask { get; } // "utagmask"

        /// <summary>
        /// Gets a BitMask for picking out the SignBit.
        /// </summary>
        public BitMask SignBitMask { get; } // "signbigu"
        #endregion

        #region Unum special values

        /// <summary>
        /// Gets a BitMask for the Unit in the Last Place or Unit of Least Precision.
        /// </summary>
        public BitMask ULP { get; }

        /// <summary>
        /// Gets the positive infinity for the given unum environment.
        /// </summary>
        public BitMask PositiveInfinity { get; } // "posinfu"

        /// <summary>
        /// Gets the negative infinity for the given unum environment.
        /// </summary>
        public BitMask NegativeInfinity { get; } // "neginfu"

        /// <summary>
        /// Gets a BitMask for the notation of a quiet NaN value in the environment.
        /// </summary>
        public BitMask QuietNotANumber { get; } // "qNaNu"

        /// <summary>
        /// Gets a BitMask for the notation of a signaling NaN value in the environment.
        /// </summary>
        public BitMask SignalingNotANumber { get; } // "sNaNu"

        /// <summary>
        /// Gets the largest magnitude positive real number. One ULP less than infinity.
        /// </summary>
        public BitMask LargestPositive { get; } // "maxrealu"

        /// <summary>
        /// Gets the smallest magnitude positive real number. One ULP more than 0.
        /// </summary>
        public BitMask SmallestPositive { get; } // "smallsubnormalu"

        /// <summary>
        /// Gets the largest magnitude negative real number. One ULP more than negative infinity.
        /// </summary>
        public BitMask LargestNegative { get; } // "negbigu"

        /// <summary>
        /// Gets a BitMask for the largest magnitude negative unum in the environment.
        /// </summary>
        public BitMask MinRealU { get; } // "minrealu"

        #endregion

        public UnumEnvironment(byte exponentSizeSize, byte fractionSizeSize)
        {
            // Initializing structure.
            ExponentSizeSize = exponentSizeSize;
            FractionSizeSize = fractionSizeSize;

            ExponentSizeMax = (byte)UnumHelper.SegmentSizeSizeToSegmentSize(ExponentSizeSize);
            FractionSizeMax = UnumHelper.SegmentSizeSizeToSegmentSize(FractionSizeSize);

            UnumTagSize = (byte)(1 + ExponentSizeSize + FractionSizeSize);
            Size = (ushort)(1 + ExponentSizeMax + FractionSizeMax + UnumTagSize);

            // Initializing masks.
            EmptyBitMask = new BitMask(Size);

            UncertaintyBitMask = new BitMask(Size).SetOne((ushort)(UnumTagSize - 1));

            FractionSizeMask = new BitMask(Size).SetOne(FractionSizeSize) - 1;

            ExponentSizeMask = UncertaintyBitMask - 1 - FractionSizeMask;

            ExponentAndFractionSizeMask = ExponentSizeMask | FractionSizeMask;
            UnumTagMask = UncertaintyBitMask + ExponentAndFractionSizeMask;

            SignBitMask = new BitMask(Size).SetOne((ushort)(Size - 1));

            // Initializing environment.
            ULP = new BitMask(Size).SetOne(UnumTagSize);

            PositiveInfinity = new BitMask(Size).SetOne((ushort)(Size - 1)) - 1 - UncertaintyBitMask;

            NegativeInfinity = new BitMask(Size).SetOne((ushort)(Size - 1)) + PositiveInfinity;

            LargestPositive = PositiveInfinity - ULP;
            SmallestPositive = ExponentAndFractionSizeMask + ULP;

            LargestNegative = NegativeInfinity - ULP;

            MinRealU = LargestPositive + (1U << (Size - 1));

            QuietNotANumber = PositiveInfinity + UncertaintyBitMask;
            SignalingNotANumber = NegativeInfinity + UncertaintyBitMask;
        }

        public static UnumEnvironment FromConfigurationValues(byte eSize, ushort fSize) =>
            new(UnumHelper.SegmentSizeToSegmentSizeSize(eSize), UnumHelper.SegmentSizeToSegmentSizeSize(fSize));

        public static UnumEnvironment FromConfiguration(UnumConfiguration configuration) =>
            FromConfigurationValues(configuration.ExponentSize, configuration.FractionSize);

        public static UnumEnvironment FromStandardEnvironment(StandardEnvironment environment)
        {
            switch (environment)
            {
                case StandardEnvironment.Warlpiri:
                    return new UnumEnvironment(0, 0);
                case StandardEnvironment.HalfPrecisionLike:
                    return FromConfiguration(UnumConfiguration.FromIeeeConfiguration(IeeeConfiguration.HalfPrecision));
                case StandardEnvironment.SinglePrecisionLike:
                    return FromConfiguration(UnumConfiguration.FromIeeeConfiguration(IeeeConfiguration.SinglePrecision));
                case StandardEnvironment.DoublePrecisionLike:
                    return FromConfiguration(UnumConfiguration.FromIeeeConfiguration(IeeeConfiguration.DoublePrecision));
                case StandardEnvironment.ExtendedPrecisionLike:
                    return FromConfiguration(UnumConfiguration.FromIeeeConfiguration(IeeeConfiguration.ExtendedPrecision));
                case StandardEnvironment.QuadPrecisionLike:
                    return FromConfiguration(UnumConfiguration.FromIeeeConfiguration(IeeeConfiguration.QuadPrecision));
                default:
                    return FromConfiguration(UnumConfiguration.FromIeeeConfiguration(IeeeConfiguration.SinglePrecision));
            }
        }

        public static UnumEnvironment GetDefaultEnvironment() => FromStandardEnvironment(StandardEnvironment.SinglePrecisionLike);
    }

    public enum StandardEnvironment
    {
        Warlpiri,              // 4-bit.
        HalfPrecisionLike,     // 33-bit.
        SinglePrecisionLike,   // 50-bit.
        DoublePrecisionLike,   // 92-bit.
        ExtendedPrecisionLike, // 92-bit.
        QuadPrecisionLike,      // 157-bit.
    }
}
