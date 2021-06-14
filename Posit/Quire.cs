using System;
using System.Collections.Generic;

namespace Lombiq.Arithmetics
{
    public class Quire
    {
        private const ulong SegmentMaskWithLeadingOne = 0x8000_0000_0000_0000;

        private readonly ulong[] _segments;

        public ushort Size { get; }

        public ushort SegmentCount { get; }

        public IList<ulong> Segments => _segments;

        public Quire(ushort size)
        {
            var partialSegment = size % 64;
            SegmentCount = (ushort)((size >> 6) + (partialSegment == 0 ? 0 : 1));
            Size = size;
            _segments = new ulong[SegmentCount];
        }

        public Quire(ulong[] segments, ushort size = 0)
        {
            SegmentCount = (ushort)segments.Length;
            Size = size;
            if (size > SegmentCount << 6)
            {
                SegmentCount = (ushort)((size >> 6) + (size % 32 == 0 ? 0 : 1));
            }

            _segments = new ulong[SegmentCount];

            Array.Copy(segments, _segments, segments.Length);
            for (int i = segments.Length; i < SegmentCount; i++) _segments[i] = 0;
        }

        public Quire(uint firstSegment, ushort size)
        {
            Size = size;
            SegmentCount = (ushort)((size >> 6) + (size % 32 == 0 ? 0 : 1));
            _segments = new ulong[SegmentCount];
            _segments[0] = firstSegment;
            for (int i = 1; i < SegmentCount; i++) _segments[i] = 0;
        }

        public static Quire operator +(Quire left, Quire right)
        {
            if (left.SegmentCount == 0 || right.SegmentCount == 0) return left;
            var result = new ulong[left.SegmentCount];
            bool carry = false, leftBit, rightBit;
            byte buffer;
            ushort segmentPosition = 0, position = 0;

            for (ushort i = 0; i < left.SegmentCount << 6; i++)
            {
                leftBit = ((left._segments[segmentPosition] >> position) & 1) == 1;
                rightBit = ((right._segments[segmentPosition] >> position) & 1) == 1;

                buffer = (byte)((leftBit ? 1 : 0) + (rightBit ? 1 : 0) + (carry ? 1 : 0));

                if ((buffer & 1) == 1) result[segmentPosition] += 1UL << position;
                carry = buffer >> 1 == 1;

                position++;
                if (position >> 6 == 1)
                {
                    position = 0;
                    segmentPosition++;
                }
            }

            return new Quire(result);
        }

        public static Quire operator +(Quire left, uint right) => left + new Quire(right, (ushort)(left.SegmentCount << 6));
        public static Quire operator -(Quire left, Quire right)
        {
            if (left.SegmentCount == 0 || right.SegmentCount == 0) return left;

            var result = new ulong[left.SegmentCount];
            bool carry = false, leftBit, rightBit;
            byte buffer;
            ushort segmentPosition = 0, position = 0;

            for (ushort i = 0; i < left.SegmentCount << 6; i++)
            {
                leftBit = ((left._segments[segmentPosition] >> position) & 1) == 1;
                rightBit = ((right._segments[segmentPosition] >> position) & 1) == 1;

                buffer = (byte)(2 + (leftBit ? 1 : 0) - (rightBit ? 1 : 0) - (carry ? 1 : 0));

                if ((buffer & 1) == 1) result[segmentPosition] += 1UL << position;
                carry = buffer >> 1 == 0;

                position++;
                if (position >> 6 == 1)
                {
                    position = 0;
                    segmentPosition++;
                }
            }

            return new Quire(result);
        }

        public static Quire operator ~(Quire q)
        {
            for (ushort i = 0; i < q.SegmentCount; i++)
            {
                q._segments[i] = ~q.Segments[i];
            }

            return q;
        }

        public static bool operator ==(Quire left, Quire right)
        {
            if (left.SegmentCount != right.SegmentCount) return false;
            for (ushort i = 0; i < left.SegmentCount; i++)
            {
                if (left.Segments[i] != right.Segments[i]) return false;
            }

            return true;
        }

        public static bool operator !=(Quire left, Quire right) => !(left == right);

        public static Quire operator >>(Quire left, int right)
        {
            right &= (1 << (left.SegmentCount * 6)) - 1;

            bool carryOld, carryNew;
            var segments = new ulong[left.SegmentCount];
            Array.Copy(left._segments, segments, left._segments.Length);
            ushort currentIndex;

            for (ushort i = 0; i < right; i++)
            {
                carryOld = false;

                for (ushort j = 1; j <= segments.Length; j++)
                {
                    currentIndex = (ushort)(segments.Length - j);
                    carryNew = (segments[currentIndex] & 1) == 1;
                    segments[currentIndex] >>= 1;
                    if (carryOld) segments[currentIndex] |= SegmentMaskWithLeadingOne;
                    carryOld = carryNew;
                }
            }

            return new Quire(segments);
        }

        public static Quire operator <<(Quire left, int right)
        {
            right &= (1 << (left.SegmentCount * 6)) - 1;

            bool carryOld, carryNew;
            var segments = new ulong[left.SegmentCount];
            Array.Copy(left._segments, segments, left._segments.Length);
            const uint segmentMaskWithClosingOne = 1;

            for (ushort i = 0; i < right; i++)
            {
                carryOld = false;

                for (ushort j = 0; j < segments.Length; j++)
                {
                    carryNew = (segments[j] & SegmentMaskWithLeadingOne) == SegmentMaskWithLeadingOne;
                    segments[j] <<= 1;
                    if (carryOld) segments[j] |= segmentMaskWithClosingOne;
                    carryOld = carryNew;
                }
            }

            return new Quire(segments);
        }

        public static explicit operator ulong(Quire x) => x.Segments[0];

        public static explicit operator uint(Quire x) => (uint)x.Segments[0];

        public override bool Equals(object obj) => obj is Quire other && this == other;

        public override int GetHashCode()
        {
            int code = Size;
            unchecked
            {
                for (int i = 0; i < SegmentCount; i++)
                    code = (code * 397) ^ Segments[i].GetHashCode();
            }

            return code;
        }
    }
}
