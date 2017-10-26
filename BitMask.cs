using System;
using System.Collections.Immutable;

namespace Lombiq.Arithmetics
{
    public struct BitMask
    {
        public ushort Size { get; }
        public ushort SegmentCount { get; }
        public ImmutableArray<uint> Segments { get; }


        #region Constructors


        public BitMask(uint segment, ushort size)
        {
            Size = size;
            SegmentCount = (ushort)((size >> 5) + (size % 32 == 0 ? 0 : 1));

            /* Creating a new, temporary array once that will be used to initialize the ImmutableArray,
             * so the "extension" items (i.e. the 0-value items on top of the original segments) aren't added
             * using ImmutableArray.Add, which would instantiate a new array for each addition. */
            var extendedSegments = new uint[SegmentCount];
            extendedSegments[0] = segment;
            Segments = ImmutableArray.CreateRange(extendedSegments);
        }

        public BitMask(uint[] segments, ushort size = 0)
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

        public BitMask(ushort size, bool allOne = false)
        {
            var partialSegment = size % 32;
            SegmentCount = (ushort)((size >> 5) + (partialSegment == 0 ? 0 : 1));
            Size = size;

            // Creating a temporary array, so the items aren't added using ImmutableArray.Add, because that instantiates 
            // a new array for each execution.
            var segments = new uint[SegmentCount];

            if (allOne)
            {
                ushort i = 0;
                for (; i < SegmentCount - 1; i++) segments[i] = uint.MaxValue;

                // The last segment is special in a way that it might not be necessary to have all 1 bits.
                segments[i] = partialSegment > 0 ? (uint)(1 << partialSegment) - 1 : uint.MaxValue;
            }

            Segments = ImmutableArray.CreateRange(segments);
        }

        public BitMask(BitMask source)
        {
            Size = source.Size;
            SegmentCount = source.SegmentCount;
            Segments = source.Segments;
        }

        #endregion

        #region Static factories

        public static BitMask FromImmutableArray(ImmutableArray<uint> segments, ushort size = 0)
        {
            var intermediarySegments = new uint[segments.Length];
            segments.CopyTo(intermediarySegments);

            return new BitMask(intermediarySegments, size);
        }

        #endregion

        #region BitMask manipulation functions

        /// <summary>
        /// Returns a new BitMask, where the given index is set to one.
        /// </summary>
        /// <param name="index">The index of the bit to set.</param>
        /// <returns>A BitMask where the given bit is set to one.</returns>
        public BitMask SetOne(ushort index)
        {
            if (index > Size) return new BitMask(this);

            var bitPosition = index % 32;
            var segmentPosition = index >> 5;

            if ((Segments[segmentPosition] >> bitPosition) % 2 == 0)
                return FromImmutableArray(Segments.SetItem(segmentPosition, Segments[segmentPosition] | (uint)(1 << bitPosition)), Size);

            return new BitMask(this);
        }
        /// <summary>
        /// Returns a new BitMask, where the given bit is set to zero.
        /// </summary>
        /// <param name="index">The index of the bit to set to zero.</param>
        /// <returns>A BitMask where the given bit is set to zero.</returns>
        public BitMask SetZero(ushort index)
        {
            if (index > Size) return new BitMask(this);

            var bitPosition = index % 32;
            var segmentPosition = index >> 5;

            if ((Segments[segmentPosition] >> bitPosition) % 2 == 1)
                return FromImmutableArray(Segments.SetItem(segmentPosition, Segments[segmentPosition] & ~((uint)1 << bitPosition)), Size);

            return new BitMask(this);
        }

        /// <summary>
        /// Shifts the BitMask to the right by the number of trailing zeros.
        /// </summary>
        /// <returns>A BitMask where the trailing zeros are shifted out to the right.</returns>
        public BitMask ShiftOutLeastSignificantZeros()
        {
            var leastSignificantOnePosition = GetLeastSignificantOnePosition();
            var mask = new BitMask(this);
            if (leastSignificantOnePosition == 0) return mask;

            return mask >> leastSignificantOnePosition - 1;
        }
        /// <summary>
        /// Sets the segment on the given index to the segment given as an argument.
        /// </summary>
        /// /// <param name="index">The index of the Segment to set.</param>
        /// /// <param name="segment">The segment that the BitMask's segment on the given index will be set to.</param>
        /// <returns>A BitMask where the trailing zeros are shifted out to the right.</returns>
        public BitMask SetSegment(int index, uint segment)
        {
            if (index >= SegmentCount) return new BitMask(this);

            return FromImmutableArray(Segments.SetItem(index, segment));
        }

        #endregion

        #region Operators

        public static bool operator ==(BitMask left, BitMask right)
        {
            if (left.SegmentCount != right.SegmentCount) return false;

            for (ushort i = 0; i < left.SegmentCount; i++)
                if (left.Segments[i] != right.Segments[i]) return false;

            return true;
        }

        public static bool operator >(BitMask left, BitMask right)
        {
            for (ushort i = 1; i <= left.SegmentCount; i++)
                if (left.Segments[left.SegmentCount - i] > right.Segments[left.SegmentCount - i]) return true;

            return false;
        }

        public static bool operator <(BitMask left, BitMask right)
        {
            for (ushort i = 1; i <= left.SegmentCount; i++)
                if (left.Segments[left.SegmentCount - i] < right.Segments[left.SegmentCount - i]) return true;

            return false;
        }

        public static bool operator >=(BitMask left, BitMask right) => !(left < right);

        public static bool operator <=(BitMask left, BitMask right) => !(left > right);

        public static bool operator !=(BitMask left, BitMask right) => !(left == right);

        public static BitMask operator +(BitMask left, uint right) => left + new BitMask(right, left.Size);

        public static BitMask operator -(BitMask left, uint right) => left - new BitMask(right, left.Size);

        /// <summary>
        /// Bit-by-bit addition of two masks with "ripple-carry".
        /// </summary>
        /// <param name="left">Left operand BitMask.</param>
        /// <param name="right">Right operand BitMask.</param>
        /// <returns>The sum of the masks with respect to the size of the bigger operand.</returns>
        public static BitMask operator +(BitMask left, BitMask right)
        {
            if (left.SegmentCount == 0 || right.SegmentCount == 0) return left;
            bool carry = false, leftBit, rightBit;
            byte buffer;
            ushort segmentPosition = 0, position = 0;
            var segments = new uint[left.SegmentCount];

            for (ushort i = 0; i < (left.Size > right.Size ? left.Size : right.Size); i++)
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

            return new BitMask(segments);
        }

        /// <summary>
        /// Bit-by-bit subtraction of two masks with "ripple-carry".
        /// </summary>
        /// <param name="left">Left operand BitMask.</param>
        /// <param name="right">Right operand BitMask.</param>
        /// <returns>The difference between the masks with respect to the size of the bigger operand.</returns>
        public static BitMask operator -(BitMask left, BitMask right)
        {
            if (left.SegmentCount == 0 || right.SegmentCount == 0) return left;

            bool carry = false, leftBit, rightBit;
            byte buffer;
            ushort segmentPosition = 0, position = 0;
            var segments = new uint[left.SegmentCount];

            for (ushort i = 0; i < (left.Size > right.Size ? left.Size : right.Size); i++)
            {
                leftBit = (left.Segments[segmentPosition] >> position) % 2 == 1;
                rightBit = i >= right.Size ? false : (right.Segments[segmentPosition] >> position) % 2 == 1;

                buffer = (byte)(2 + (leftBit ? 1 : 0) - (rightBit ? 1 : 0) - (carry ? 1 : 0));

                if (buffer % 2 == 1) segments[segmentPosition] += (uint)(1 << position);
                carry = buffer >> 1 == 0;

                position++;
                if (position >> 5 == 1)
                {
                    position = 0;
                    segmentPosition++;
                }
            }

            return new BitMask(segments);
        }

        public static BitMask operator ++(BitMask mask) => mask + 1;

        public static BitMask operator --(BitMask mask) => mask - 1;

        public static BitMask operator |(BitMask left, BitMask right)
        {
            if (left.SegmentCount != right.SegmentCount) return new BitMask(left.Size);

            var segments = new uint[left.SegmentCount];

            for (ushort i = 0; i < segments.Length; i++)
                segments[i] = left.Segments[i] | right.Segments[i];

            return new BitMask(segments);
        }

        public static BitMask operator &(BitMask left, BitMask right)
        {
            if (left.SegmentCount != right.SegmentCount) return new BitMask(left.Size);

            var segments = new uint[left.SegmentCount];

            for (ushort i = 0; i < segments.Length; i++)
                segments[i] = left.Segments[i] & right.Segments[i];

            return new BitMask(segments);
        }

        public static BitMask operator ^(BitMask left, BitMask right)
        {
            if (left.SegmentCount != right.SegmentCount) return new BitMask(left.Size);

            var segments = new uint[left.SegmentCount];

            for (ushort i = 0; i < segments.Length; i++)
                segments[i] = left.Segments[i] ^ right.Segments[i];

            return new BitMask(segments);
        }

        public static BitMask operator ~(BitMask input)
        {
            var segments = new uint[input.SegmentCount];

            for (ushort i = 0; i < segments.Length; i++)
                segments[i] = ~input.Segments[i];

            return new BitMask(segments);
        }

        /// <summary>
        /// Bit-shifting of a BitMask to the right by an integer. Shifts left if negative value is given.
        /// </summary>
        /// <param name="left">Left operand BitMask to shift.</param>
        /// <param name="right">Right operand int tells how many bits to shift by.</param>
        /// <returns>BitMask of size of left BitMask, shifted left by number of bits given in the right integer.</returns>
        public static BitMask operator >>(BitMask left, int right)
        {
            if (right < 0) return left << -right;
            //if (right > left.Size) return new BitMask(left.Size);

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

            return new BitMask(segments);
        }

        /// <summary>
        /// Bit-shifting of a BitMask to the left by an integer. Shifts right if negative value is given.
        /// </summary>
        /// <param name="left">Left operand BitMask.</param>
        /// <param name="right">Right operand int tells how many bits to shift by.</param>
        /// <returns>BitMask of size of left BitMask, shifted right by number of bits given in the right integer.</returns>
        public static BitMask operator <<(BitMask left, int right)
        {
            if (right < 0) return left >> -right;
            //if (right > left.Size) return new BitMask(left.Size);

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

            return new BitMask(segments);
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Finds the most significant 1-bit.
        /// </summary>
        /// <returns>Returns the position (not index!) of the most significant 1-bit
        /// or zero if there is none.</returns>
        public ushort GetMostSignificantOnePosition()
        {
            ushort position = 0;
            uint currentSegment;

            // ushort can't be checked against with ">= 0", because that's always true.
            for (ushort i = 1; i <= SegmentCount; i++)
            {
                currentSegment = Segments[SegmentCount - i];
                while (currentSegment != 0)
                {
                    currentSegment >>= 1;
                    position++;
                    if (currentSegment == 0) return (ushort)((SegmentCount - i) * 32 + position);
                }
            }

            return 0;
        }

        public BitMask GetTwosComplement()
        {
            var mask = new BitMask(this);

            return (((~mask + 1) << (SegmentCount * 32 - Size)) >> (SegmentCount * 32 - Size));
        }

        public ushort LengthOfRunOfBits(ushort startingPosition)
        {
            ushort length = 1;
            var mask = new BitMask(this) << (SegmentCount * 32 - startingPosition);
            bool startingBit = mask.Segments[0] >> 31 > 0;
            mask <<= 1;
            for (var i = 0; i < startingPosition; i++) 
            {
                if (mask.Segments[0] >> 31 > 0 != startingBit) return length;
                mask <<= 1;
                length++;
            }
            return length;
        }

        /// <summary>
        /// Finds the least significant 1-bit.
        /// </summary>
        /// <returns>Returns the position (not index!) of the least significant 1-bit
        /// or zero if there is none.</returns>
        public ushort GetLeastSignificantOnePosition()
        {
            ushort position = 1;
            uint currentSegment;

            for (ushort i = 0; i < SegmentCount; i++)
            {
                currentSegment = Segments[i];
                if (currentSegment == 0) position += 32;
                else
                {
                    while (currentSegment % 2 == 0)
                    {
                        position++;
                        currentSegment >>= 1;
                    }
                    if (currentSegment % 2 == 1) return position;
                }
            }

            return 0;
        }

        // Array indexer is not supported by Hastlayer yet.
        //public uint this[int i] => Segments[i];

        public uint GetLowest32Bits() => Segments[0];

        #endregion

        #region Overrides

        public override bool Equals(object obj) => this == (BitMask)obj;

        public override int GetHashCode()
        {
            var segmentHashCodes = new int[SegmentCount];
            for (int i = 0; i < SegmentCount; i++) segmentHashCodes[i] = Segments[i].GetHashCode();

            return segmentHashCodes.GetHashCode();
        }

        public override string ToString() => string.Join(", ", Segments);

        #endregion
    }
}
