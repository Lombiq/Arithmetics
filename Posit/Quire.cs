using System;
using System.Collections.Immutable;


namespace Lombiq.Arithmetics
{
    public struct Quire
    {
        public ushort Size { get; }
        public ushort SegmentCount { get; }
        public ImmutableArray<uint> Segments { get; }

        public Quire(ushort size)
        {
            var partialSegment = size % 32;
            SegmentCount = (ushort)((size >> 5) + (partialSegment == 0 ? 0 : 1));
            Size = size;

            // Creating a temporary array, so the items aren't added using ImmutableArray.Add, because that instantiates 
            // a new array for each execution.
            var segments = new uint[SegmentCount];
            Segments = ImmutableArray.CreateRange(segments);
        }

        public Quire(uint[] segments, ushort size = 0)
        {
            var segmentBits = (ushort)(segments.Length << 5);

            Size = size < segmentBits ? segmentBits : size;
            SegmentCount = size > segmentBits ?
                (ushort)((size >> 5) + (size % 32 == 0 ? 0 : 1)) :
                (ushort)segments.Length;

            if (SegmentCount > segments.Length)
            {
                /* Creating a new, temporary array once that will be used to initialize the ImmutableArray,
                 * so the "extension" items (i.e. the 0-value items on top of the original segments) aren't added
                 * using ImmutableArray.Add, which would instantiate a new array for each addition. */
                var extendedSegments = new uint[SegmentCount];
                Array.Copy(segments, extendedSegments, segments.Length);
                Segments = ImmutableArray.CreateRange(extendedSegments);
            }
            else Segments = ImmutableArray.CreateRange(segments);
        }

        // This is temporary, there we will try carry-lookahead.
        public static Quire operator +(Quire left, BitMask right)
        {
            if (left.SegmentCount == 0 || right.SegmentCount == 0) return left;
            bool carry = false, leftBit, rightBit;
            byte buffer;
            ushort segmentPosition = 0, position = 0;
            var segments = new uint[left.SegmentCount];

            for (ushort i = 0; i < left.Size; i++)
            {
                leftBit = (left.Segments[segmentPosition] >> position) % 2 == 1;
                rightBit = i >= right.Size ? false : (right.Segments[segmentPosition] >> position) % 2 == 1;

                buffer = (byte)((leftBit ? 1 : 0) + (rightBit ? 1 : 0) + (carry ? 1 : 0));

                if (buffer % 2 == 1) segments[segmentPosition] += (uint)(1 << position);
                carry = buffer >> 1 == 1;

                position++;
                if (position >> 5 == 1)
                {
                    position = 0;
                    segmentPosition++;
                }
            }

            return new Quire(segments);
        }

        public static Quire operator >>(Quire left, int right)
        {
            if (right < 0) return left << -right;

            bool carryOld, carryNew;
            var segmentMaskWithLeadingOne = 0x80000000; // 1000 0000 0000 0000 0000 0000 0000 0000
            var segments = new uint[left.SegmentCount];
            left.Segments.CopyTo(segments);
            ushort currentIndex;

            for (ushort i = 0; i < right; i++)
            {
                carryOld = false;

                for (ushort j = 1; j <= segments.Length; j++)
                {
                    currentIndex = (ushort)(segments.Length - j);
                    carryNew = segments[currentIndex] % 2 == 1;
                    segments[currentIndex] >>= 1;
                    if (carryOld) segments[currentIndex] |= segmentMaskWithLeadingOne;
                    carryOld = carryNew;
                }
            }

            return new Quire(segments);
        }

        public static Quire operator <<(Quire left, int right)
        {
            if (right < 0) return left >> -right;

            bool carryOld, carryNew;
            var segmentMaskWithLeadingOne = 0x80000000; // 1000 0000 0000 0000 0000 0000 0000 0000
            uint segmentMaskWithClosingOne = 1;         // 0000 0000 0000 0000 0000 0000 0000 0001
            var segments = new uint[left.SegmentCount];
            left.Segments.CopyTo(segments);

            for (ushort i = 0; i < right; i++)
            {
                carryOld = false;

                for (ushort j = 0; j < segments.Length; j++)
                {
                    carryNew = ((segments[j] & segmentMaskWithLeadingOne) == segmentMaskWithLeadingOne);
                    segments[j] <<= 1;
                    if (carryOld) segments[j] |= segmentMaskWithClosingOne;
                    carryOld = carryNew;
                }
            }

            return new Quire(segments);
        }
    }
}


