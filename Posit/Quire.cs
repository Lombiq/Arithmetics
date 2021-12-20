using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.Arithmetics
{
    [SuppressMessage("Major Code Smell", "S4035:Classes implementing \"IEquatable<T>\" should be sealed", Justification = "False Positive")]
    public class Quire : IEqualityComparer<Quire>
    {
        private const ulong SegmentMaskWithLeadingOne = 0x_8000_0000_0000_0000;
        private const ulong SegmentMaskWithClosingOne = 1;

        public ushort Size { get; }
        public ushort SegmentCount { get; }

        [SuppressMessage(
            "Performance",
            "CA1819:Properties should not return arrays",
            Justification = "Not currently posisble due to IArraySizeHolder limitations.")]
        // See: https://github.com/Lombiq/Hastlayer-SDK/issues/63
        public ulong[] Segments { get; }

        public Quire(ushort size)
        {
            var partialSegment = size % 64;
            SegmentCount = (ushort)((size >> 6) + (partialSegment == 0 ? 0 : 1));
            Size = size;
            Segments = new ulong[SegmentCount];
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

        public Quire(uint firstSegment, ushort size)
        {
            Size = size;
            SegmentCount = (ushort)((size >> 6) + (size % 32 == 0 ? 0 : 1));
            Segments = new ulong[SegmentCount];
            Segments[0] = firstSegment;
            for (int i = 1; i < SegmentCount; i++)
                Segments[i] = 0;
        }

        public static Quire operator +(Quire left, Quire right)
        {
            if (left.SegmentCount == 0 || right.SegmentCount == 0) return left;
            var result = new ulong[left.SegmentCount];
            bool carry = false;
            ushort segmentPosition = 0, position = 0;

            for (ushort i = 0; i < left.SegmentCount << 6; i++)
            {
                bool leftBit = ((left.Segments[segmentPosition] >> position) & 1) == 1;
                bool rightBit = ((right.Segments[segmentPosition] >> position) & 1) == 1;
                byte buffer = (byte)((leftBit ? 1 : 0) + (rightBit ? 1 : 0) + (carry ? 1 : 0));

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
            bool carry = false;
            ushort segmentPosition = 0, position = 0;

            for (ushort i = 0; i < left.SegmentCount << 6; i++)
            {
                bool leftBit = ((left.Segments[segmentPosition] >> position) & 1) == 1;
                bool rightBit = ((right.Segments[segmentPosition] >> position) & 1) == 1;

                byte buffer = (byte)(2 + (leftBit ? 1 : 0) - (rightBit ? 1 : 0) - (carry ? 1 : 0));

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
                q.Segments[i] = ~q.Segments[i];
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

            var segments = new ulong[left.SegmentCount];
            Array.Copy(left.Segments, segments, left.Segments.Length);

            for (ushort i = 0; i < right; i++)
            {
                bool carryOld = false;

                for (ushort j = 1; j <= segments.Length; j++)
                {
                    ushort currentIndex = (ushort)(segments.Length - j);
                    bool carryNew = (segments[currentIndex] & 1) == 1;
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

            var segments = new ulong[left.SegmentCount];
            Array.Copy(left.Segments, segments, left.Segments.Length);

            for (ushort i = 0; i < right; i++)
            {
                bool carryOld = false;

                for (ushort j = 0; j < segments.Length; j++)
                {
                    bool carryNew = (segments[j] & SegmentMaskWithLeadingOne) == SegmentMaskWithLeadingOne;
                    segments[j] <<= 1;
                    if (carryOld) segments[j] |= SegmentMaskWithClosingOne;
                    carryOld = carryNew;
                }
            }

            return new Quire(segments);
        }

        public static explicit operator ulong(Quire x) => x.Segments[0];

        public static explicit operator uint(Quire x) => (uint)x.Segments[0];

        protected bool Equals(Quire other) => this == other;
        public bool Equals(Quire x, Quire y) => x == y;
        public override bool Equals(object obj) => obj is Quire other && this == other;

        public int GetHashCode(Quire obj) => obj.GetHashCode();

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Segments != null ? Segments.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ Size.GetHashCode();
                hashCode = (hashCode * 397) ^ SegmentCount.GetHashCode();
                return hashCode;
            }
        }
    }
}
