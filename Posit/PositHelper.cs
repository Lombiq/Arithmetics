

using System.Runtime.CompilerServices;

namespace Lombiq.Arithmetics
{
	/*This is needed because Hastlayer doesn't support generics yet.*/
	internal static class PositHelper
	{

	#region Bit-level Manipulations
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte SetOne(byte bits, ushort index) =>(byte)( bits | (1 << index));
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort SetOne(ushort bits, ushort index) =>(ushort)( bits | (1 << index));
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint SetOne(uint bits, ushort index) =>(uint)( bits | (1 << index));
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong SetOne(ulong bits, ushort index) =>(ulong)( bits | ((ulong)1 << index));
		
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte SetZero(byte bits, ushort index) => (byte)(bits & ~(1 << index));
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort SetZero(ushort bits, ushort index) => (ushort)(bits & ~(1 << index));
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint SetZero(uint bits, ushort index) => (uint)(bits & ~(1 << index));
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong SetZero(ulong bits, ushort index) => (ulong)(bits & ~((ulong)1 << index));
		
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte LengthOfRunOfBits( byte bits, byte startingPosition)
		{
			byte length = 1;
			bits <<= 8 - startingPosition;
			var startingBit = bits >> (8-1)& 1;
			bits <<= 1;
			for (var i = 0; i < startingPosition; i++)
			{
				if (bits >> (8-1) != startingBit) break;
				bits <<= 1;
				length++;
			}
			return length;
		}
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte LengthOfRunOfBits( ushort bits, byte startingPosition)
		{
			byte length = 1;
			bits <<= 16 - startingPosition;
			var startingBit = bits >> (16-1)& 1;
			bits <<= 1;
			for (var i = 0; i < startingPosition; i++)
			{
				if (bits >> (16-1) != startingBit) break;
				bits <<= 1;
				length++;
			}
			return length;
		}
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte LengthOfRunOfBits( uint bits, byte startingPosition)
		{
			byte length = 1;
			bits <<= 32 - startingPosition;
			var startingBit = bits >> (32-1)& 1;
			bits <<= 1;
			for (var i = 0; i < startingPosition; i++)
			{
				if (bits >> (32-1) != startingBit) break;
				bits <<= 1;
				length++;
			}
			return length;
		}
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte LengthOfRunOfBits( ulong bits, byte startingPosition)
		{
			byte length = 1;
			bits <<= 64 - startingPosition;
			var startingBit = bits >> (64-1)& 1;
			bits <<= 1;
			for (var i = 0; i < startingPosition; i++)
			{
				if (bits >> (64-1) != startingBit) break;
				bits <<= 1;
				length++;
			}
			return length;
		}
		
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte GetMostSignificantOnePosition(byte bits)
		{
			byte position = 0;
			while (bits != 0)
			{
				bits >>= 1;
				position++;
			}
			return position;
		}
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte GetMostSignificantOnePosition(ushort bits)
		{
			byte position = 0;
			while (bits != 0)
			{
				bits >>= 1;
				position++;
			}
			return position;
		}
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte GetMostSignificantOnePosition(uint bits)
		{
			byte position = 0;
			while (bits != 0)
			{
				bits >>= 1;
				position++;
			}
			return position;
		}
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte GetMostSignificantOnePosition(ulong bits)
		{
			byte position = 0;
			while (bits != 0)
			{
				bits >>= 1;
				position++;
			}
			return position;
		}
		
		#endregion

	}
}
 
