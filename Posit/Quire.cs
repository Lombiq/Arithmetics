using System;
using System.Collections.Immutable;


namespace Lombiq.Arithmetics
{
    public class Quire
    {
        public ushort Size { get; }
        public ushort SegmentCount { get; }
        public ulong[] Segments { get; }

        public Quire(ushort size)
        {
            var partialSegment = size % 64;
            SegmentCount = (ushort)((size >> 6) + (partialSegment == 0 ? 0 : 1));
            Size = size;
            for (int i = 0; i < SegmentCount; i++)
                Segments[i] = 0;
        }

        public Quire(ulong[] segments, ushort size = 0)
        {
            SegmentCount = (ushort)segments.Length;
            Size = size;
            if (size > SegmentCount << 6)
            {

                SegmentCount = (ushort)((size >> 6) + (size % 32 == 0 ? 0 : 1));
            }
            Segments = new ulong[SegmentCount];

            Array.Copy(segments, Segments, segments.Length);
            for (int i = segments.Length; i < SegmentCount; i++)
                Segments[i] = 0;
        }

        public static Quire operator +(Quire left, BitMask right)
        {
            if (left.SegmentCount == 0 || right.SegmentCount == 0) return left;
            bool carry = false, leftBit, rightBit;
            byte buffer;
            ushort segmentPosition = 0, position = 0;


            for (ushort i = 0; i < left.Size; i++)
            {
                leftBit = (left.Segments[segmentPosition] >> position) % 2 == 1;
                rightBit = i >= right.Size ? false : ((right.Segments[segmentPosition] >> position) & 1) == 1;

                buffer = (byte)((leftBit ? 1 : 0) + (rightBit ? 1 : 0) + (carry ? 1 : 0));

                if ((buffer & 1) == 1) left.Segments[segmentPosition] += (uint)(1 << position);
                carry = buffer >> 1 == 1;

                position++;
                if (position >> 6 == 1)
                {
                    position = 0;
                    segmentPosition++;
                }
            }

            return left;
        }

        public static Quire operator >>(Quire left, int right)
        {
            if (right < 0) return left << -right;

            bool carryOld, carryNew;
            var segmentMaskWithLeadingOne = 0x8000000000000000;
            ushort currentIndex;

            for (ushort i = 0; i < right; i++)
            {
                carryOld = false;

                for (ushort j = 1; j <= left.Segments.Length; j++)
                {
                    currentIndex = (ushort)(left.Segments.Length - j);
                    carryNew = (left.Segments[currentIndex] & 1) == 1;
                    left.Segments[currentIndex] >>= 1;
                    if (carryOld) left.Segments[currentIndex] |= segmentMaskWithLeadingOne;
                    carryOld = carryNew;
                }
            }

            return left;
        }

        public static Quire operator <<(Quire left, int right)
        {
            if (right < 0) return left >> -right;

            bool carryOld, carryNew;
            var segmentMaskWithLeadingOne = 0x8000000000000000;
            uint segmentMaskWithClosingOne = 1;

            for (ushort i = 0; i < right; i++)
            {
                carryOld = false;

                for (ushort j = 0; j < left.Segments.Length; j++)
                {
                    carryNew = ((left.Segments[j] & segmentMaskWithLeadingOne) == segmentMaskWithLeadingOne);
                    left.Segments[j] <<= 1;
                    if (carryOld) left.Segments[j] |= segmentMaskWithClosingOne;
                    carryOld = carryNew;
                }
            }

            return left;
        }
    }
}


