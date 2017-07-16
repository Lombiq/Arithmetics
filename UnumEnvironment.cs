namespace Lombiq.Unum
{
    public class UnumEnvironment
    {
        #region Unum structure
        public byte ExponentSizeSize { get; } // "esizesize"
        public byte FractionSizeSize { get; } // "fsizesize"

        public byte ExponentSizeMax { get; } // "esizemax"
        public ushort FractionSizeMax { get; } // "fsizemax"

        public byte UnumTagSize { get; } // "utagsize"
        public ushort Size { get; } // "maxubits"
        #endregion

        #region Unum masks
        public BitMask EmptyBitMask { get; }

        public BitMask UncertaintyBitMask { get; } // "ubitmask"
        public BitMask ExponentSizeMask { get; } // "esizemask"
        public BitMask FractionSizeMask { get; } // "fsizemask"
        public BitMask ExponentAndFractionSizeMask { get; } // "efsizemask"
        public BitMask UnumTagMask { get; } // "utagmask"
        public BitMask SignBitMask { get; } // "signbigu", a unum in which all bits are zero except the sign bit;
        #endregion

        #region Unum special values
        public BitMask ULP { get; } // Unit in the Last Place or Unit of Least Precision.

        public BitMask PositiveInfinity { get; } // "posinfu", the positive infinity for the given unum environment.
        public BitMask NegativeInfinity { get; } // "neginfu", the negative infinity for the given unum environment.

        public BitMask QuietNotANumber { get; } // "qNaNu"
        public BitMask SignalingNotANumber { get; } // "sNaNu"

        public BitMask LargestPositive { get; } // "maxrealu", the largest magnitude positive real number. One ULP less than infinity.
        public BitMask SmallestPositive { get; } // "smallsubnormalu", the smallest magnitude positive real number. One ULP more than 0.

        public BitMask LargestNegative { get; } // "negbigu", the largest maginude negative real number. One ULP more than negative infinity.
        public BitMask MinRealU { get; } // "minrealu", looks like to be exactly the same as "negbigu".

        //private uint _smallNormal; // "smallnormalu"
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

            MinRealU = LargestPositive + ((uint)1 << Size - 1);

            QuietNotANumber = PositiveInfinity + UncertaintyBitMask;
            SignalingNotANumber = NegativeInfinity + UncertaintyBitMask;

            //_smallNormal = _exponentAndFractionSizeMask + 1 << _size - 1 - _exponentSizeMax;
        }


        public static UnumEnvironment FromConfigurationValues(byte eSize, ushort fSize) =>
            new UnumEnvironment(UnumHelper.SegmentSizeToSegmentSizeSize(eSize), UnumHelper.SegmentSizeToSegmentSizeSize(fSize));

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
        QuadPrecisionLike      // 157-bit.
    }
}
