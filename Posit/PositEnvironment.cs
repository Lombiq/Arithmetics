using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lombiq.Arithmetics.Posit
{
    public class PositEnvironment
    {
        public byte MaximumExponentSize { get; } //es

        public ushort Size { get; } //nbits

        public uint Useed { get; }

        public ushort FirstRegimeBitPosition { get; }
        public BitMask SignBitMask { get; }

        public BitMask FirstRegimeBitBitMask { get; }


        public PositEnvironment(byte size, byte maximumExponentSize)
        {
            Size = size;
            MaximumExponentSize = maximumExponentSize;

            Useed = (uint)2 << (2 << MaximumExponentSize);
            SignBitMask = new BitMask(Size).SetOne((ushort)(Size - 1));
            FirstRegimeBitPosition = (ushort)(Size - 2);
            FirstRegimeBitBitMask = new BitMask(Size).SetOne(FirstRegimeBitPosition);
        }

    }
}
