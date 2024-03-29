namespace Lombiq.Arithmetics;

public class PositEnvironment
{
    public byte MaximumExponentSize { get; }

    public ushort Size { get; }

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

        Useed = 1U << (1 << MaximumExponentSize);
        SignBitMask = new BitMask(Size).SetOne((ushort)(Size - 1));
        FirstRegimeBitIndex = (ushort)(Size - 2);
        FirstRegimeBitBitMask = new BitMask(Size).SetOne(FirstRegimeBitIndex);
        EmptyBitMask = new BitMask(Size);
        MaxValueBitMask = new BitMask(Size, allOne: true) >> 1;
        MinValueBitMask = SignBitMask + 1;
        NaNBitMask = SignBitMask;
        QuireSize = new BitMask((uint)(((Size - 2) * (1 << MaximumExponentSize)) + 5), size).FindMostSignificantOnePosition();
    }
}
