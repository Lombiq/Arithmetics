namespace Lombiq.Arithmetics
{
    public class PositEnvironment
    {
        public byte MaximumExponentSize { get; } //es

        public ushort Size { get; } //nbits

        public uint Useed { get; }

        public ushort FirstRegimeBitIndex { get; }

        public BitMask SignBitMask { get; }

        public BitMask FirstRegimeBitBitMask { get; }

        public BitMask EmptyBitMask { get; }

        public BitMask MaxValueBitMask { get; }

        public BitMask MinValueBitMask { get; }

        public BitMask NaNBitMask { get; }

        public uint QuireSize { get; }


        public PositEnvironment(byte size, byte maximumExponentSize)
        {
            Size = size;
            MaximumExponentSize = maximumExponentSize;

            Useed = (uint)1 << (1 << MaximumExponentSize);
            SignBitMask = new BitMask(Size).SetOne((ushort)(Size - 1));
            FirstRegimeBitIndex = (ushort)(Size - 2);
            FirstRegimeBitBitMask = new BitMask(Size).SetOne(FirstRegimeBitIndex);
            EmptyBitMask = new BitMask(Size);
            MaxValueBitMask = new BitMask(Size, true) >> 1;
            MinValueBitMask = SignBitMask + 1;
            NaNBitMask = SignBitMask;
            QuireSize = new BitMask((uint)((Size - 2) * (1 << MaximumExponentSize) + 5), size).GetMostSignificantOnePosition();
        }
    }
}
