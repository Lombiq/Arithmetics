﻿
using System;
using System.Runtime.CompilerServices;

namespace Lombiq.Arithmetics
{
	public readonly struct Posit8_1 : IComparable, IConvertible, IFormattable, IEquatable<Posit8_1>, IComparable<Posit8_1>
	{
		public byte PositBits { get; }

		#region Posit structure

		public const byte MaximumExponentSize = 1;

		public const byte Size = 8;

		public const uint Useed = 1 << (1 << MaximumExponentSize);

		public const byte FirstRegimeBitIndex = Size - 2;

		public const byte FirstRegimeBitPosition = Size - 1;

		public const byte SizeMinusFixedBits = Size - 2 - MaximumExponentSize;

		public const ushort QuireSize =  128;

		public const short QuireFractionSize = (4*Size-8)*( 1 << MaximumExponentSize)/2;

		#endregion

		#region Posit Masks

		public const byte SignBitMask = ( byte )1 << Size - 1;
		
		public const  byte  FirstRegimeBitBitMask = ( byte )1 << Size - 2;

		public const  byte  EmptyBitMask = 0;

		public const  byte  MaxValueBitMask = byte.MaxValue - SignBitMask;
		
		public const  byte  MinPositiveValueBitMask = 1;

		public const  byte  NaNBitMask = SignBitMask;

		public const uint Float32ExponentMask = 0x_7f80_0000;

		public const uint Float32FractionMask = 0x_007f_ffff;

		public const uint Float32HiddenBitMask = 0x_0080_0000;

		public const uint Float32SignBitMask = 0x_8000_0000;

		public const ulong Double64FractionMask = 0x000F_FFFF_FFFF_FFFF;

		public const ulong Double64ExponentMask = 0x7FF0_0000_0000_0000;

		public const ulong Double64HiddenBitMask = 0x0010_0000_0000_0000;

		#endregion

		#region Posit constructors

		public Posit8_1(byte bits, bool fromBitMask) =>
			PositBits = fromBitMask ? bits : new Posit8_1(bits).PositBits;

		public Posit8_1(Quire q)
		{
			PositBits = NaNBitMask;
			var sign = false;
			var positionOfMostSigniFicantOne = QuireSize-1;
			var firstSegment = (ulong)(q >> (QuireSize - 64));
			if (firstSegment >= 0x8000000000000000)
			{
				q = ~q;
				q += 1;
				sign = true;
			}
			firstSegment = (ulong)(q >> (QuireSize - 64));
			while (firstSegment < 0x8000000000000000 && positionOfMostSigniFicantOne > 0)
			{
				q <<= 1;
				positionOfMostSigniFicantOne -= 1;
				firstSegment = (ulong)(q >> (QuireSize - 64));
			}

			var scaleFactor = positionOfMostSigniFicantOne - QuireFractionSize;
			if (positionOfMostSigniFicantOne == 0)
			{
				PositBits = 0;
				return;
			}
						var resultRegimeKValue = scaleFactor / (1 << MaximumExponentSize);
			var resultExponentBits = (byte) (scaleFactor % (1 << MaximumExponentSize));
			if (resultExponentBits < 0)
			{
				resultRegimeKValue -= 1;
				resultExponentBits += 1 << MaximumExponentSize;
			}
			

			PositBits = AssemblePositBitsWithRounding(sign, resultRegimeKValue,  resultExponentBits, (byte )(q >> QuireSize - Size));
		}

		public  Posit8_1(bool sign, short scaleFactor, byte fraction)
		{
					var resultRegimeKValue = scaleFactor / (1 << MaximumExponentSize);
			var resultExponentBits =  (scaleFactor % (1 << MaximumExponentSize));
			if (resultExponentBits < 0)
			{
				resultRegimeKValue -= 1;
				resultExponentBits += 1 << MaximumExponentSize;
			}
			PositBits = AssemblePositBitsWithRounding(sign, resultRegimeKValue, (byte)resultExponentBits, fraction);
					}

		public Posit8_1(uint value)
		{
			
			if (value == 0) {
				PositBits = (byte)value;
				return;
			}
						var exponentValue = (byte)(PositHelper.GetMostSignificantOnePosition(value) - 1);

			byte kValue = 0;
			while (exponentValue >= 1 << MaximumExponentSize && kValue < Size - 1)
			{
				exponentValue -= 1 << MaximumExponentSize;
				kValue++;
			}
						if (kValue > (Size - 2))
			{
				kValue = (Size - 2);
				 exponentValue = 0;
			}
			while (value >byte.MaxValue) value >>= 1;			

			PositBits = AssemblePositBitsWithRounding(false, kValue, exponentValue, (byte)value);
		}

		public  Posit8_1(int value)
		{
			PositBits = value >= 0 ? new Posit8_1((uint)value).PositBits : GetTwosComplement(new Posit8_1((uint)-value).PositBits);
		}

		public Posit8_1(ulong value)
		{
			
			if (value == 0) {
				PositBits = (byte)value;
				return;
			}
						var exponentValue = (byte)(PositHelper.GetMostSignificantOnePosition(value) - 1);

			byte kValue = 0;
			while (exponentValue >= 1 << MaximumExponentSize && kValue < Size - 1)
			{
				exponentValue -= 1 << MaximumExponentSize;
				kValue++;
			}
						if (kValue > (Size - 2))
			{
				kValue = (Size - 2);
				 exponentValue = 0;
			}
			while (value >byte.MaxValue) value >>= 1;
			
			PositBits = AssemblePositBitsWithRounding(false, kValue, exponentValue, (byte)value);
		}

		public  Posit8_1(long value)
		{
			PositBits = value >= 0 ? new Posit8_1((ulong)value).PositBits : GetTwosComplement(new Posit8_1((ulong)-value).PositBits);
		}

		public  Posit8_1(float floatBits)
		{
			PositBits = NaNBitMask;
			if (float.IsInfinity(floatBits) || float.IsNaN(floatBits))
			{
				return;
			}
			if (floatBits == 0)
			{
				PositBits = 0;
				return;
			}

			uint uintRepresentation;
			unsafe
			{
				uint* floatPointer = (uint*)&floatBits;
				uintRepresentation = *floatPointer;
			}

			var signBit = (uintRepresentation & Float32SignBitMask) != 0;
			int scaleFactor = (int)((uintRepresentation << 1) >> 24) - 127;
			var fractionBits = uintRepresentation & Float32FractionMask;

			// Adding the hidden bit if it is one.
			if (scaleFactor != -127) fractionBits += Float32HiddenBitMask;
			else scaleFactor += 1;
						fractionBits >>= 24 - Size;
									var regimeKValue = scaleFactor / (1 << MaximumExponentSize);

			if (scaleFactor < 0) regimeKValue = regimeKValue - 1;

			var exponentValue = (byte)(scaleFactor - regimeKValue * (1 << MaximumExponentSize));
			if (exponentValue == 1 << MaximumExponentSize)
			{
				regimeKValue += 1;
				exponentValue = 0;
			}

			if (regimeKValue < -(Size - 1))
			{
				regimeKValue = -(Size - 1);
				exponentValue = 0;
			}
			if (regimeKValue > (Size - 2))
			{
				regimeKValue = (Size - 2);
				exponentValue = 0;
			}
						PositBits = AssemblePositBitsWithRounding(signBit, regimeKValue,  exponentValue, (byte)fractionBits);
		}

		public Posit8_1(double doubleBits)
		{
			PositBits = NaNBitMask;
			if (double.IsInfinity(doubleBits) || double.IsNaN(doubleBits))
			{
				return;
			}
			if (doubleBits == 0)
			{
				PositBits = 0;
				return;
			}

			ulong ulongRepresentation;
			unsafe
			{
				ulong* doublePointer = (ulong*)&doubleBits;
				ulongRepresentation = *doublePointer;
			}


			var signBit = (ulongRepresentation & ((ulong)Float32SignBitMask << 32)) != 0;
			int scaleFactor = (int)((ulongRepresentation << 1) >> 53) - 1023;
			var fractionBits =((ulongRepresentation & Double64FractionMask) >> 53 - Size);

			// Adding the hidden bit if it is one.
			if (scaleFactor != -1023) fractionBits += (Double64HiddenBitMask >> 53 - Size);
			else scaleFactor += 1;
						var regimeKValue = scaleFactor / (1 << MaximumExponentSize);
			if (scaleFactor < 0) regimeKValue = regimeKValue - 1;

			var exponentValue = (byte)(scaleFactor - regimeKValue * (1 << MaximumExponentSize));
			if (exponentValue == 1 << MaximumExponentSize)
			{
				regimeKValue += 1;
				exponentValue = 0;
			}

			if (regimeKValue < -(Size - 1))
			{
				regimeKValue = -(Size - 1);
				exponentValue = 0;
			}
			if (regimeKValue > (Size - 2))
			{
				regimeKValue = (Size - 2);
				exponentValue = 0;
			}
						PositBits = AssemblePositBitsWithRounding(signBit, regimeKValue,  exponentValue, (byte)fractionBits);
		}

		#endregion

		#region Posit numeric states

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsPositive() => (PositBits & SignBitMask) == EmptyBitMask;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsNaN() => PositBits == NaNBitMask;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsZero() => PositBits == EmptyBitMask;

		#endregion

		#region Methods to handle parts of the Posit 

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte  EncodeRegimeBits(int regimeKValue)
		{
			byte  regimeBits;
			if (regimeKValue > 0)
			{
				regimeBits = (byte)((1 << regimeKValue + 1) - 1);
				regimeBits <<= Size - PositHelper.GetMostSignificantOnePosition(regimeBits) - 1;
			}
			else regimeBits =(byte)(FirstRegimeBitBitMask >> -regimeKValue);

			return regimeBits;
		}

		private byte  AssemblePositBits(bool signBit, int regimeKValue, byte  exponentBits, byte  fractionBits)
		{
			// Calculating the regime. 
			var wholePosit = EncodeRegimeBits(regimeKValue);

			// Attaching the exponent
			var regimeLength = PositHelper.LengthOfRunOfBits(wholePosit, FirstRegimeBitPosition);


			wholePosit += (byte)(exponentBits << SizeMinusFixedBits - regimeLength);

			var fractionMostSignificantOneIndex = PositHelper.GetMostSignificantOnePosition(fractionBits) - 1;

			// Hiding the hidden bit. (It is always one.) 
			fractionBits = PositHelper.SetZero(fractionBits, (ushort)fractionMostSignificantOneIndex);

			wholePosit += (byte)(fractionBits << SizeMinusFixedBits - fractionMostSignificantOneIndex - regimeLength);

			return signBit ? GetTwosComplement(wholePosit) : wholePosit;
		}
			public static byte  AssemblePositBitsWithRoundingFrom32Bits(bool signBit, int regimeKValue, uint exponentBits, uint fractionBits)
		{
			
			if (regimeKValue >= Size-2)
			{
				return signBit? (byte)(SignBitMask+1) : MaxValueBitMask;
			}
			if (regimeKValue <= -(Size-2))
			{
				return signBit?  byte.MaxValue : MinPositiveValueBitMask;
			}

			// Calculating the regime. 
			var wholePosit = EncodeRegimeBits(regimeKValue);

			// Attaching the exponent.
			var regimeLength = PositHelper.LengthOfRunOfBits(wholePosit, FirstRegimeBitPosition);
								
			var exponentShiftedLeftBy = (sbyte)SizeMinusFixedBits - regimeLength;
			wholePosit += exponentShiftedLeftBy >= 0 ? (byte) (exponentBits << exponentShiftedLeftBy) : (byte)(exponentBits >> -exponentShiftedLeftBy);

			// Calculating rounding.
			var fractionMostSignificantOneIndex = PositHelper.GetMostSignificantOnePosition(fractionBits) - 1;

			// Hiding the hidden bit. (It is always one.) 
			fractionBits = PositHelper.SetZero(fractionBits, (ushort)fractionMostSignificantOneIndex);

			if (exponentShiftedLeftBy < 0)
			{
			  
				if (exponentBits == 1)
				{
					if (fractionBits > 0)
					{
						wholePosit += 1;
					}
					else wholePosit += (byte)(wholePosit & 1);
				}else  return signBit ? GetTwosComplement(wholePosit) : wholePosit;      



			/*	if (exponentBits < SignBitMask) return signBit ? GetTwosComplement(wholePosit) : wholePosit;

				if (exponentBits == SignBitMask)
				{
					if (fractionBits > 0)
					{
						wholePosit += 1;
					}
					else wholePosit += (byte)(wholePosit & 1);
				}
				else wholePosit += 1; */
			
			}
											

			var fractionShiftedLeftBy = SizeMinusFixedBits - (fractionMostSignificantOneIndex) - (regimeLength);
			// Attaching the fraction.
			wholePosit += fractionShiftedLeftBy >= 0 ? (byte)(fractionBits << fractionShiftedLeftBy) : (byte)(fractionBits >> -fractionShiftedLeftBy);
			// Calculating rounding.
			if (fractionShiftedLeftBy < 0)
			{
				if (Size + fractionShiftedLeftBy >= 0) fractionBits <<= Size + fractionShiftedLeftBy;
				else fractionBits >>= -(Size - fractionShiftedLeftBy);
				//return !signBit ? wholePosit : GetTwosComplement(wholePosit);
				if (fractionBits >= SignBitMask)
				{
					if (fractionBits == SignBitMask)
					{
						wholePosit += (byte)(wholePosit & 1);
					}
					else wholePosit += 1;
				}
			}

			return signBit ? GetTwosComplement(wholePosit) : wholePosit;
	}	
	
		public static byte  AssemblePositBitsWithRounding(bool signBit, int regimeKValue,byte exponentBits ,  byte fractionBits)
		{
			
			if (regimeKValue >= Size-2)
			{
				return signBit? (byte)(SignBitMask+1) : MaxValueBitMask;
			}
			if (regimeKValue <= -(Size-2))
			{
				return signBit?  byte.MaxValue : MinPositiveValueBitMask;
			}

			// Calculating the regime. 
			var wholePosit = EncodeRegimeBits(regimeKValue);

			// Attaching the exponent.
			var regimeLength = PositHelper.LengthOfRunOfBits(wholePosit, FirstRegimeBitPosition);
								
			var exponentShiftedLeftBy = (sbyte)SizeMinusFixedBits - regimeLength;
			wholePosit += exponentShiftedLeftBy >= 0 ? (byte) (exponentBits << exponentShiftedLeftBy) : (byte)(exponentBits >> -exponentShiftedLeftBy);

			// Calculating rounding.
			var fractionMostSignificantOneIndex = PositHelper.GetMostSignificantOnePosition(fractionBits) - 1;

			// Hiding the hidden bit. (It is always one.) 
			fractionBits = PositHelper.SetZero(fractionBits, (ushort)fractionMostSignificantOneIndex);

			if (exponentShiftedLeftBy < 0)
			{
			  
				if (exponentBits == 1)
				{
					if (fractionBits > 0)
					{
						wholePosit += 1;
					}
					else wholePosit += (byte)(wholePosit & 1);
				}else  return signBit ? GetTwosComplement(wholePosit) : wholePosit;      



			/*	if (exponentBits < SignBitMask) return signBit ? GetTwosComplement(wholePosit) : wholePosit;

				if (exponentBits == SignBitMask)
				{
					if (fractionBits > 0)
					{
						wholePosit += 1;
					}
					else wholePosit += (byte)(wholePosit & 1);
				}
				else wholePosit += 1; */
			
			}
											

			var fractionShiftedLeftBy = SizeMinusFixedBits - (fractionMostSignificantOneIndex) - (regimeLength);
			// Attaching the fraction.
			wholePosit += fractionShiftedLeftBy >= 0 ? (byte)(fractionBits << fractionShiftedLeftBy) : (byte)(fractionBits >> -fractionShiftedLeftBy);
			// Calculating rounding.
			if (fractionShiftedLeftBy < 0)
			{
				if (Size + fractionShiftedLeftBy >= 0) fractionBits <<= Size + fractionShiftedLeftBy;
				else fractionBits >>= -(Size - fractionShiftedLeftBy);
				//return !signBit ? wholePosit : GetTwosComplement(wholePosit);
				if (fractionBits >= SignBitMask)
				{
					if (fractionBits == SignBitMask)
					{
						wholePosit += (byte)(wholePosit & 1);
					}
					else wholePosit += 1;
				}
			}

			return signBit ? GetTwosComplement(wholePosit) : wholePosit;
	}	

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sbyte GetRegimeKValue()
		{
			var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
			var lengthOfRunOfBits = PositHelper.LengthOfRunOfBits(bits, FirstRegimeBitPosition);

			return (bits & FirstRegimeBitBitMask) == EmptyBitMask
				? (sbyte)-lengthOfRunOfBits
				: (sbyte)(lengthOfRunOfBits - 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public sbyte GetRegimeKValueWithoutSignCheck(byte lengthOfRunOfBits)
		{
			return (PositBits & FirstRegimeBitBitMask) == EmptyBitMask
				? (sbyte)-lengthOfRunOfBits
				: (sbyte)(lengthOfRunOfBits - 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public short CalculateScaleFactor()
		{
			var regimeKvalue = GetRegimeKValue();
			//return (int)((GetRegimeKValue() == 0) ? 1 + GetExponentValue() : (GetRegimeKValue() * (1 << MaximumExponentSize) + GetExponentValue()));
			return (regimeKvalue == -FirstRegimeBitPosition) ? (short)0 : (short)(regimeKvalue * (1 << MaximumExponentSize) + GetExponentValue());
		}

	
				 
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte ExponentSize()
		{
			var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
			var lengthOfRunOfBits = PositHelper.LengthOfRunOfBits(bits, FirstRegimeBitPosition);
			byte result;
			if (lengthOfRunOfBits + 2 <= Size)
			{
				result = Size - (lengthOfRunOfBits + 2) > MaximumExponentSize
					 ? MaximumExponentSize : (byte)(Size - (lengthOfRunOfBits + 2));
			}
			else result = (byte)(Size - lengthOfRunOfBits - 1);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte ExponentSizeWithoutSignCheck()
		{
			var lengthOfRunOfBits = PositHelper.LengthOfRunOfBits(PositBits, FirstRegimeBitPosition);
			return Size - (lengthOfRunOfBits + 2) > MaximumExponentSize
				? MaximumExponentSize : (byte)(Size - (lengthOfRunOfBits + 2));
		}		  

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint GetExponentValue()
		{
			var exponentMask = IsPositive() ? PositBits : GetTwosComplement(PositBits);
			var exponentSize = ExponentSize();
			exponentMask = (byte)((byte)((exponentMask >> (int)FractionSize())
							<< (Size - exponentSize))
							>> (Size - MaximumExponentSize));
			return exponentSize == 0 ? (byte)0 : exponentMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint GetExponentValueWithoutSignCheck()
		{
			return (byte)((byte)((PositBits >> (int)FractionSizeWithoutSignCheck())
							<< (Size - ExponentSize()))
							>> (Size - MaximumExponentSize));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint GetExponentValueWithoutSignCheck(uint fractionSize)
		{
			return (byte)((byte)((PositBits >> (int)fractionSize)
							<< (Size - ExponentSize()))
							>> (Size - MaximumExponentSize));
		}
		 
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint FractionSize()
		{
			var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
			var fractionSize = Size - (PositHelper.LengthOfRunOfBits(bits, FirstRegimeBitPosition) + 2 + MaximumExponentSize);
			return fractionSize > 0 ? (uint)fractionSize : 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint FractionSizeWithoutSignCheck()
		{
			var fractionSize = Size - (PositHelper.LengthOfRunOfBits(PositBits, FirstRegimeBitPosition) + 2 + MaximumExponentSize);
			return fractionSize > 0 ? (uint)fractionSize : 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint FractionSizeWithoutSignCheck(byte lengthOfRunOfBits)
		{
			var fractionSize = Size - (lengthOfRunOfBits + 2 + MaximumExponentSize);
			return fractionSize > 0 ? (uint)fractionSize : 0;
		}

		#endregion

		#region Helper methods for operations and conversions

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint Fraction()
		{
			var fractionSize = FractionSize();
			var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
			return (byte)((byte)(bits << (int)(Size - fractionSize))
						  >> (int)(Size - fractionSize));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte FractionWithHiddenBit()
		{
			var fractionSize = FractionSize();
			var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
			var result = (byte)((byte)(bits << (int)(Size - fractionSize))
						 >> (int)(Size - fractionSize));
			return fractionSize == 0 ? (byte)1 : PositHelper.SetOne(result, (ushort)fractionSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte FractionWithHiddenBit(uint fractionSize)
		{
			var bits = IsPositive() ? PositBits : GetTwosComplement(PositBits);
			var result = (byte)((byte)(bits << (int)(Size - fractionSize))
						 >> (int)(Size - fractionSize));
			return PositHelper.SetOne(result, (ushort)fractionSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte FractionWithHiddenBitWithoutSignCheck()
		{
			var fractionSizeWithoutSignCheck = FractionSizeWithoutSignCheck();
			var result = (byte)((byte)(PositBits << (int)(Size - fractionSizeWithoutSignCheck))
						 >> (int)(Size - fractionSizeWithoutSignCheck));
			return PositHelper.SetOne(result, (ushort)fractionSizeWithoutSignCheck);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte FractionWithHiddenBitWithoutSignCheck(uint fractionSize)
		{
			var numberOfNonFractionBits = (int)(Size - fractionSize);
			var result = (byte)((byte)(PositBits << numberOfNonFractionBits)
						 >> numberOfNonFractionBits);
			return PositHelper.SetOne(result, (ushort)fractionSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short CalculateScaleFactor(sbyte regimeKValue , uint exponentValue , byte maximumExponentSize) =>
			(short)(regimeKValue * (1 << maximumExponentSize)  + exponentValue);

		public static Quire MultiplyIntoQuire(Posit8_1 left, Posit8_1 right)
		{

			if (left.IsZero() || right.IsZero()) return new Quire((ushort)QuireSize);
			if (left.IsNaN() || right.IsNaN()) return new Quire(1, (ushort)QuireSize) << (QuireSize - 1);
			var leftIsPositive = left.IsPositive();
			var rightIsPositive = right.IsPositive();
			var resultSignBit = leftIsPositive != rightIsPositive;

			left = Abs(left);
			right = Abs(right);
			var leftFractionSize = left.FractionSizeWithoutSignCheck();
			var rightFractionSize = right.FractionSizeWithoutSignCheck();

			var longResultFractionBits = (ushort)(left.FractionWithHiddenBitWithoutSignCheck() *
				(ulong)right.FractionWithHiddenBitWithoutSignCheck());
			var fractionSizeChange = PositHelper.GetMostSignificantOnePosition(longResultFractionBits) - (leftFractionSize + rightFractionSize + 1);
			var scaleFactor =
				CalculateScaleFactor(left.GetRegimeKValue() , left.GetExponentValue(), MaximumExponentSize) +
				CalculateScaleFactor(right.GetRegimeKValue(), right.GetExponentValue(), MaximumExponentSize);

			scaleFactor += (int)fractionSizeChange;

			var quireArray = new ulong[QuireSize / 64];
			quireArray[0] = longResultFractionBits;
			var resultQuire = new Quire(quireArray);
			resultQuire <<= (QuireFractionSize - PositHelper.GetMostSignificantOnePosition(longResultFractionBits) + 1 + scaleFactor);

			return !resultSignBit ? resultQuire : (~resultQuire) + 1;
		}

		#endregion

		#region Bit level Helper Methods		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Posit8_1 Abs(Posit8_1 input)
		{
			var signBit = input.PositBits >> Size - 1;
			var maskOfSignBits = 0 - signBit;
			return new Posit8_1((byte)((input.PositBits ^ maskOfSignBits) + signBit), true);
		}		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte GetTwosComplement(byte bits) => (byte)(~bits + 1);

		#endregion

		#region Algebraic functions

		public static Posit8_1 Sqrt(Posit8_1 number)
		{
			if (number.IsNaN() || number.IsZero()) return number;
			if (!number.IsPositive()) return new Posit8_1(NaNBitMask, true);

			var inputScaleFactor = number.CalculateScaleFactor(); 
			var inputFractionWithHiddenBit = number.FractionWithHiddenBitWithoutSignCheck();			

			byte resultFractionBits = 0; 
			byte startingEstimate = 0; 
			byte temporaryEstimate; 
			byte estimateMaskingBit = (SignBitMask >> 2); 


			if ((inputScaleFactor & 1) != 0) // if the scaleFactor is odd, shift the number to make it even
			{
				inputScaleFactor -= 1;
				inputFractionWithHiddenBit += inputFractionWithHiddenBit;
				estimateMaskingBit <<= 1;
			}
			inputScaleFactor >>= 1;
			inputFractionWithHiddenBit <<= Size-2 - PositHelper.GetMostSignificantOnePosition(inputFractionWithHiddenBit);
			
			while (estimateMaskingBit != 0)
			{
				temporaryEstimate =(byte) (startingEstimate + estimateMaskingBit);
				if (temporaryEstimate <= inputFractionWithHiddenBit)
				{
					startingEstimate = (byte)(temporaryEstimate + estimateMaskingBit);
					inputFractionWithHiddenBit -= temporaryEstimate;
					resultFractionBits += estimateMaskingBit;
				}

				inputFractionWithHiddenBit += inputFractionWithHiddenBit;
				estimateMaskingBit >>= 1;
			}
			
				
			var resultRegimeKValue = inputScaleFactor / (1 << MaximumExponentSize);
			var resultExponentBits = (inputScaleFactor % (1 << MaximumExponentSize));
			if (resultExponentBits < 0)
			{
				resultRegimeKValue -= 1;
				resultExponentBits += 1 << MaximumExponentSize;
			}
			
			return new Posit8_1(AssemblePositBitsWithRounding(false, resultRegimeKValue,  (byte) resultExponentBits,  resultFractionBits), true);
		}

		#endregion

		#region Fused operations

		public static Posit8_1 FusedSum(Posit8_1[] posits)
		{
			var resultQuire = new Quire((ushort)QuireSize);

			for (var i = 0; i < posits.Length; i++)
			{
				if (posits[i].IsNaN()) return posits[i];
				resultQuire += (Quire)posits[i];
			}

			return new Posit8_1(resultQuire);
		}

		public static Quire FusedSum(Posit8_1[] posits, Quire startingValue)
		{
			var quireNaNMask = new Quire(1, (ushort)QuireSize) << (QuireSize - 1);

			if (startingValue == quireNaNMask) return quireNaNMask;
			for (var i = 0; i < posits.Length; i++)
			{
				if (posits[i].IsNaN()) return quireNaNMask;
				startingValue += (Quire)posits[i];
			}

			return startingValue;
		}

		public static Posit8_1 FusedDotProduct(Posit8_1[] positArray1, Posit8_1[] positArray2)
		{
			if (positArray1.Length != positArray2.Length) return new Posit8_1(NaNBitMask, true);

			var resultQuire = new Quire((ushort)QuireSize);

			for (var i = 0; i < positArray1.Length; i++)
			{
				if (positArray1[i].IsNaN()) return positArray1[i];
				if (positArray2[i].IsNaN()) return positArray2[i];
				resultQuire += MultiplyIntoQuire(positArray1[i], positArray2[i]);

			}

			return new Posit8_1(resultQuire);
		}

		public static Posit8_1 FusedMultiplyAdd(Posit8_1 a, Posit8_1 b, Posit8_1 c)
		{
			var positArray1 = new Posit8_1[2];
			var positArray2 = new Posit8_1[2];

			positArray1[0] = a;
			positArray1[1] = new Posit8_1(1);
			positArray2[0] = b;
			positArray2[1] = c;

			return FusedDotProduct(positArray1, positArray2);
		}

		public static Posit8_1 FusedAddMultiply(Posit8_1 a, Posit8_1 b, Posit8_1 c)
		{
			var positArray1 = new Posit8_1[2];
			var positArray2 = new Posit8_1[2];

			positArray1[0] = a;
			positArray1[1] = b;
			positArray2[0] = c;
			positArray2[1] = c;

			return FusedDotProduct(positArray1, positArray2);
		}

		public static Posit8_1 FusedMultiplyMultiplySubtract(Posit8_1 a, Posit8_1 b, Posit8_1 c, Posit8_1 d)
		{
			var positArray1 = new Posit8_1[2];
			var positArray2 = new Posit8_1[2];

			positArray1[0] = a;
			positArray1[1] = -c;
			positArray2[0] = b;
			positArray2[1] = d;

			return FusedDotProduct(positArray1, positArray2);
		}
		#endregion

		#region Arithmetic Operators       

		public static Posit8_1 operator +(Posit8_1 left, Posit8_1 right)
		{
			// Handling special cases first.
			if (left.IsNaN()) return left;
			if (right.IsNaN()) return right;
			if (left.IsZero()) return right;
			if (right.IsZero()) return left;

			var leftSignBit = left.PositBits >> Size - 1;
			var leftMaskOfSignBits = 0 - leftSignBit;
			var leftAbsoluteValue = new Posit8_1((byte)((left.PositBits ^ leftMaskOfSignBits) + leftSignBit), true);

			var rightSignBit = right.PositBits >> Size - 1;
			var rightMaskOfSignBits = 0 - rightSignBit;
			var rightAbsoluteValue = new Posit8_1((byte)((right.PositBits ^ rightMaskOfSignBits) + rightSignBit), true);

			var leftLengthOfRunOfBits = PositHelper.LengthOfRunOfBits(leftAbsoluteValue.PositBits, FirstRegimeBitPosition);
			var rightLengthOfRunOfBits = PositHelper.LengthOfRunOfBits(rightAbsoluteValue.PositBits, FirstRegimeBitPosition);

			var leftFractionSize = leftAbsoluteValue.FractionSizeWithoutSignCheck(leftLengthOfRunOfBits);
			var rightFractionSize = rightAbsoluteValue.FractionSizeWithoutSignCheck(rightLengthOfRunOfBits);

			var signBitsMatch = leftSignBit == rightSignBit;
			sbyte leftRegimeKValue = leftAbsoluteValue.GetRegimeKValueWithoutSignCheck(leftLengthOfRunOfBits);
			sbyte rightRegimeKValue = rightAbsoluteValue.GetRegimeKValueWithoutSignCheck(rightLengthOfRunOfBits);
			               
			uint rightExponentValue = rightAbsoluteValue.GetExponentValueWithoutSignCheck(rightFractionSize);
			uint leftExponentValue = leftAbsoluteValue.GetExponentValueWithoutSignCheck(leftFractionSize);
			
			var resultSignBit = leftAbsoluteValue > rightAbsoluteValue ? leftSignBit == 1 : rightSignBit == 1;
			byte resultFractionBits = 0;

			var leftScaleFactor = CalculateScaleFactor(leftRegimeKValue, leftExponentValue, MaximumExponentSize);
			var rightScaleFactor = CalculateScaleFactor(rightRegimeKValue, rightExponentValue, MaximumExponentSize);

			var scaleFactorDifference = leftScaleFactor - rightScaleFactor;

			var scaleFactor =
				scaleFactorDifference >= 0
					? leftScaleFactor
					: rightScaleFactor;

			var leftFraction = leftAbsoluteValue.FractionWithHiddenBitWithoutSignCheck(leftFractionSize);
			var rightFraction = rightAbsoluteValue.FractionWithHiddenBitWithoutSignCheck(rightFractionSize);

			if (scaleFactorDifference == 0)
			{
				if (signBitsMatch)
				{
					resultFractionBits += (byte)(leftFraction + rightFraction);
				}
				else
				{
					if (leftFraction >= rightFraction)
					{
						resultFractionBits += (byte)(leftFraction - rightFraction);
					}
					else
					{
						resultFractionBits +=(byte) (rightFraction - leftFraction);
					}
				}

				scaleFactor += (short)(PositHelper.GetMostSignificantOnePosition(resultFractionBits) -
							  leftFractionSize - 1);
			}
			else if (scaleFactorDifference > 0) // The scale factor of the left Posit is bigger.
			{
				var fractionSizeDifference = (int)(leftFractionSize - rightFractionSize);
				resultFractionBits += leftFraction;
				var biggerPositMovedToLeft = (int)(FirstRegimeBitPosition - leftFractionSize - 1);
				resultFractionBits <<= biggerPositMovedToLeft;
				var smallerPositMovedToLeft = biggerPositMovedToLeft - scaleFactorDifference + fractionSizeDifference;

				if (signBitsMatch)
				{
					if (smallerPositMovedToLeft >= 0)
					{
						resultFractionBits +=(byte)(rightFraction << smallerPositMovedToLeft);
					}
					else resultFractionBits +=(byte) (rightFraction >> -smallerPositMovedToLeft);
				}
				else resultFractionBits -= smallerPositMovedToLeft >= 0
						? (byte)(rightFraction << smallerPositMovedToLeft)
						: (byte)(rightFraction >> -smallerPositMovedToLeft);

				scaleFactor += (short)(PositHelper.GetMostSignificantOnePosition(resultFractionBits) - FirstRegimeBitPosition);
			}
			else // The scale factor of the right Posit is bigger.
			{
				var fractionSizeDifference = (int)(rightFractionSize - leftFractionSize);
				resultFractionBits += rightFraction;
				var biggerPositMovedToLeft = (int)(FirstRegimeBitPosition - rightFractionSize - 1);
				resultFractionBits <<= biggerPositMovedToLeft;

				if (signBitsMatch)
				{
					if (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference >= 0)
					{
						resultFractionBits += (byte)(leftFraction << (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference));
					}
					else resultFractionBits += (byte)(leftFraction >> -(biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference));

				}
				else if (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference >= 0)
				{
					resultFractionBits -=(byte)(leftFraction << (biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference));
				}
				else resultFractionBits -=(byte)(leftFraction >> -(biggerPositMovedToLeft + scaleFactorDifference + fractionSizeDifference));

				scaleFactor += (short)(PositHelper.GetMostSignificantOnePosition(resultFractionBits) - FirstRegimeBitPosition);
			}
			if (resultFractionBits == 0) return new Posit8_1(0, true);

			var resultRegimeKValue = scaleFactor / (1 << MaximumExponentSize);
			var resultExponentBits = (scaleFactor % (1 << MaximumExponentSize));
			if (resultExponentBits < 0)
			{
				resultRegimeKValue -= 1;
				resultExponentBits += (1 << MaximumExponentSize);
			}

			return new Posit8_1(AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, (byte) resultExponentBits, resultFractionBits), true);
		}

		public static Posit8_1 operator +(Posit8_1 left, int right) => left + new Posit8_1(right);

		public static Posit8_1 operator -(Posit8_1 left, Posit8_1 right) => left + -right;

		public static Posit8_1 operator -(Posit8_1 left, int right) => left - new Posit8_1(right);

		public static Posit8_1 operator -(Posit8_1 x)
		{
			if (x.IsNaN() || x.IsZero()) return new Posit8_1(x.PositBits, true);
			return new Posit8_1(GetTwosComplement(x.PositBits), true);
		}

		public static bool operator ==(Posit8_1 left, Posit8_1 right) => left.PositBits == right.PositBits;

		public static bool operator >(Posit8_1 left, Posit8_1 right)
		{
			if (left.IsPositive() != right.IsPositive()) return left.IsPositive();
			return left.IsPositive() ? left.PositBits > right.PositBits : !(left.PositBits > right.PositBits);
		}

		public static bool operator <(Posit8_1 left, Posit8_1 right) => !(left.PositBits > right.PositBits);

		public static bool operator !=(Posit8_1 left, Posit8_1 right) => !(left == right);

		public static Posit8_1 operator *(Posit8_1 left, int right) => left * new Posit8_1(right);

		public static Posit8_1 operator *(Posit8_1 left, Posit8_1 right)
		{
			if (left.IsZero() || right.IsZero()) return new Posit8_1(0);
			var leftIsPositive = left.IsPositive();
			var rightIsPositive = right.IsPositive();
			var resultSignBit = leftIsPositive != rightIsPositive;

			left = Abs(left);
			right = Abs(right);
			var leftFractionSize = left.FractionSizeWithoutSignCheck();
			var rightFractionSize = right.FractionSizeWithoutSignCheck();

			var longResultFractionBits = (ushort)(left.FractionWithHiddenBitWithoutSignCheck() *
				(ushort)right.FractionWithHiddenBitWithoutSignCheck());
			var fractionSizeChange = PositHelper.GetMostSignificantOnePosition(longResultFractionBits) - (leftFractionSize + rightFractionSize + 1);
			var fractionBitsShiftedBy = (int)(leftFractionSize + 1 + rightFractionSize + 1 - Size);
			var resultFractionBits = (byte)(longResultFractionBits >> (fractionBitsShiftedBy > 0 ? fractionBitsShiftedBy : 0));
			var scaleFactor =
				CalculateScaleFactor(left.GetRegimeKValue(), left.GetExponentValue(), MaximumExponentSize) +
				CalculateScaleFactor(right.GetRegimeKValue(), right.GetExponentValue(), MaximumExponentSize);

			scaleFactor += (int)fractionSizeChange;

			var resultRegimeKValue = scaleFactor / (1 << MaximumExponentSize);
			var resultExponentBits = (scaleFactor % (1 << MaximumExponentSize));
			if (resultExponentBits < 0)
			{
				resultRegimeKValue -= 1;
				resultExponentBits += (1 << MaximumExponentSize);
			}
			return new Posit8_1(AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, (byte) resultExponentBits, resultFractionBits), true);
		}

		public static Posit8_1 operator /(Posit8_1 left, int right) => left / new Posit8_1(right);

		public static Posit8_1 operator /(Posit8_1 left, Posit8_1 right)
		{
			if (left.IsZero()) return new Posit8_1(0);
			if (right.IsZero()) return new Posit8_1(NaNBitMask, true);
			var leftIsPositive = left.IsPositive();
			var rightIsPositive = right.IsPositive();
			var resultSignBit = leftIsPositive != rightIsPositive;

			left = Abs(left);
			right = Abs(right);
			var leftFractionSize = left.FractionSizeWithoutSignCheck();
			var rightFractionSize = right.FractionSizeWithoutSignCheck();

			var longResultFractionBits = (ushort)(((ushort)(left.FractionWithHiddenBitWithoutSignCheck()) << (int)(15 - leftFractionSize)) /
				(right.FractionWithHiddenBitWithoutSignCheck() << (int)(7 - rightFractionSize)));
			var fractionSizeChange = PositHelper.GetMostSignificantOnePosition(longResultFractionBits) - (9);

			var scaleFactor =
				CalculateScaleFactor(left.GetRegimeKValue(), left.GetExponentValue(), MaximumExponentSize) -
				CalculateScaleFactor(right.GetRegimeKValue(), right.GetExponentValue(), MaximumExponentSize);
			scaleFactor += fractionSizeChange;

			var resultRegimeKValue = scaleFactor / (1 << MaximumExponentSize);
			var resultExponentBits = (scaleFactor % (1 << MaximumExponentSize));
			if (resultExponentBits < 0)
			{
				resultRegimeKValue -= 1;
				resultExponentBits += (1 << MaximumExponentSize);
			}

			var resultFractionBits = (byte)(longResultFractionBits >> (resultRegimeKValue > 0 ? resultRegimeKValue + 1 : -resultRegimeKValue + 1));

			return new Posit8_1(AssemblePositBitsWithRounding(resultSignBit, resultRegimeKValue, (byte) resultExponentBits, resultFractionBits), true);
		}

		#endregion

		#region Conversion Operators 
	
		public static explicit operator int(Posit8_1 x)
		{
			uint result;
			if (x.PositBits == 0) return 0;

			var scaleFactor = x.GetRegimeKValue() * (1 << MaximumExponentSize) + x.GetExponentValue();

			if (scaleFactor + 1 <= 31) // The posit fits into the range
			{
				var mostSignificantOnePosition = PositHelper.GetMostSignificantOnePosition(x.FractionWithHiddenBit());

				if (scaleFactor - mostSignificantOnePosition + 1 >= 0)
				{
					result = (uint)(x.FractionWithHiddenBit() <<
						(int)(scaleFactor - mostSignificantOnePosition + 1));
				}
				else
				{
					result = (uint)(x.FractionWithHiddenBit() >>
							   -(int)(scaleFactor - mostSignificantOnePosition + 1));
				}
			}
			else return (x.IsPositive()) ? int.MaxValue : int.MinValue;

			return x.IsPositive() ? (int)result : (int)-result;
		}

		public static explicit operator float(Posit8_1 x)
		{
			if (x.IsNaN()) return float.NaN;
			if (x.IsZero()) return 0F;

			var floatBits = x.IsPositive() ? 0: Float32SignBitMask;
			float floatRepresentation;
			var scaleFactor = x.GetRegimeKValue() * (1 << MaximumExponentSize) + x.GetExponentValue();

			if (scaleFactor > 127) return x.IsPositive() ? float.MaxValue : float.MinValue;
			if (scaleFactor < -127) return x.IsPositive() ? float.Epsilon : -float.Epsilon;

			var fraction = x.Fraction();

			if (scaleFactor == -127)
			{
				fraction >>= 1;
				fraction += (Float32HiddenBitMask >> 1);
			}

			floatBits += (uint)((scaleFactor + 127) << 23);

			if (x.FractionSize() <= 23)
			{
				fraction <<= (int)(23 - x.FractionSize());
			}
			else
			{
				fraction >>= (int)-(23 - x.FractionSize());
			}

			floatBits += (fraction << (32 - PositHelper.GetMostSignificantOnePosition(fraction) - 1)) >> (32 - PositHelper.GetMostSignificantOnePosition(fraction) - 1);

			unsafe
			{
				float* floatPointer = (float*)&floatBits;
				floatRepresentation = *floatPointer;
			}

			return floatRepresentation;
		}

		public static explicit operator double(Posit8_1 x)
		{
			if (x.IsNaN()) return double.NaN;
			if (x.IsZero()) return 0D;

			ulong doubleBits = x.IsPositive() ? EmptyBitMask : ((ulong)SignBitMask) << 64-Size;
			double doubleRepresentation;
			long scaleFactor = x.GetRegimeKValue() * (1 << MaximumExponentSize) + x.GetExponentValue();

			var fraction = x.Fraction();
			var longFraction = (ulong) fraction;

			doubleBits += (ulong)((scaleFactor + 1023) << 52);

			longFraction <<= (int)(52 - x.FractionSize());
			doubleBits += (longFraction << (64 - PositHelper.GetMostSignificantOnePosition(longFraction) - 1)) >> (64 - PositHelper.GetMostSignificantOnePosition(longFraction) - 1);

			unsafe
			{
				double* doublePointer = (double*)&doubleBits;
				doubleRepresentation = *doublePointer;
			}
			return doubleRepresentation;
		}

		public static explicit operator Quire(Posit8_1 x)
		{
			if (x.IsNaN()) return new Quire(1, QuireSize) << QuireSize-1;
			if (x.IsZero()) return new Quire(0, QuireSize);
			var quireArray = new ulong[QuireSize / 64];
			quireArray[0] = x.FractionWithHiddenBit();
			var resultQuire = new Quire(quireArray);
			resultQuire <<= (int)(QuireFractionSize - x.FractionSize() + x.CalculateScaleFactor());
			return x.IsPositive() ? resultQuire : (~resultQuire) + 1;
		}

		#endregion

		#region Conversions to other Posit envs

		public static explicit operator Posit8_0(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit8_0(Posit8_0.NaNBitMask, true);
			if (x.IsZero()) return new Posit8_0(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit8_0(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 8)
								 ? (byte)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 8)) 
								 : (byte)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit8_2(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit8_2(Posit8_2.NaNBitMask, true);
			if (x.IsZero()) return new Posit8_2(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit8_2(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 8)
								 ? (byte)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 8)) 
								 : (byte)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit8_3(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit8_3(Posit8_3.NaNBitMask, true);
			if (x.IsZero()) return new Posit8_3(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit8_3(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 8)
								 ? (byte)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 8)) 
								 : (byte)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit8_4(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit8_4(Posit8_4.NaNBitMask, true);
			if (x.IsZero()) return new Posit8_4(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit8_4(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 8)
								 ? (byte)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 8)) 
								 : (byte)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit16_0(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit16_0(Posit16_0.NaNBitMask, true);
			if (x.IsZero()) return new Posit16_0(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit16_0(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 16)
								 ? (ushort)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 16)) 
								 : (ushort)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit16_1(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit16_1(Posit16_1.NaNBitMask, true);
			if (x.IsZero()) return new Posit16_1(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit16_1(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 16)
								 ? (ushort)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 16)) 
								 : (ushort)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit16_2(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit16_2(Posit16_2.NaNBitMask, true);
			if (x.IsZero()) return new Posit16_2(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit16_2(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 16)
								 ? (ushort)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 16)) 
								 : (ushort)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit16_3(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit16_3(Posit16_3.NaNBitMask, true);
			if (x.IsZero()) return new Posit16_3(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit16_3(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 16)
								 ? (ushort)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 16)) 
								 : (ushort)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit16_4(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit16_4(Posit16_4.NaNBitMask, true);
			if (x.IsZero()) return new Posit16_4(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit16_4(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 16)
								 ? (ushort)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 16)) 
								 : (ushort)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit32_0(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit32_0(Posit32_0.NaNBitMask, true);
			if (x.IsZero()) return new Posit32_0(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit32_0(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 32)
								 ? (uint)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 32)) 
								 : (uint)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit32_1(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit32_1(Posit32_1.NaNBitMask, true);
			if (x.IsZero()) return new Posit32_1(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit32_1(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 32)
								 ? (uint)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 32)) 
								 : (uint)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit32_2(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit32_2(Posit32_2.NaNBitMask, true);
			if (x.IsZero()) return new Posit32_2(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit32_2(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 32)
								 ? (uint)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 32)) 
								 : (uint)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit32_3(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit32_3(Posit32_3.NaNBitMask, true);
			if (x.IsZero()) return new Posit32_3(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit32_3(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 32)
								 ? (uint)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 32)) 
								 : (uint)x.FractionWithHiddenBit());
		}

		public static explicit operator Posit32_4(Posit8_1 x)
		{
			if (x.IsNaN()) return new Posit32_4(Posit32_4.NaNBitMask, true);
			if (x.IsZero()) return new Posit32_4(0, true);

			var fractionSizeWithHiddenBit = x.FractionSize() + 1;
			return new Posit32_4(!x.IsPositive(),
								 x.CalculateScaleFactor(),
								 (fractionSizeWithHiddenBit > 32)
								 ? (uint)(x.FractionWithHiddenBit() >> (int)(fractionSizeWithHiddenBit - 32)) 
								 : (uint)x.FractionWithHiddenBit());
		}

		#endregion

		#region Support methods

		public int CompareTo(Object value)
		{
			switch (value)
			{
				case null:
					return 1;
				case Posit8_1  posit:
					return CompareTo(posit);
				default: throw new ArgumentException("Argument must be an other posit.");
			}            
		}

		public int CompareTo(Posit8_1  value)
		{
			if (this < value) return -1;
			if (this > value) return 1;
			if (this == value) return 0;

			// At least one of the values is NaN.
			if (IsNaN()) return (value.IsNaN() ? 0 : -1);
			else return 1;
		}

		// The value of every 32-bit posit can be exactly represented by a double, so using the double's ToString() and
		// Parse() methods will make code generation more consistent.
		public override string ToString() => ((double)this).ToString();

		public string ToString(string format, IFormatProvider formatProvider) => ((double)this).ToString(format, formatProvider);

		public string ToString(IFormatProvider provider) => ((double)this).ToString(provider);

		public override int GetHashCode() => (int)PositBits;

		public Posit8_1  Parse(string number) => new Posit8_1(Double.Parse(number));

		public bool TryParse(string number, out Posit8_1  positResult)
		{
			var returnValue = Double.TryParse(number, out double result);
			positResult = new Posit8_1 (result);
			return returnValue;
		}

		public bool Equals(Posit8_1  other) => (this == other);

		public override bool Equals(object obj) => (obj is Posit8_1  posit) ? Equals(posit) : false;
		
		public TypeCode GetTypeCode() => TypeCode.Object;

		public bool ToBoolean(IFormatProvider provider) => !IsZero();

		public char ToChar(IFormatProvider provider) => throw new InvalidCastException();

		public sbyte ToSByte(IFormatProvider provider) => (sbyte)(int)this;

		public byte ToByte(IFormatProvider provider) => (byte)(uint)this;

		public short ToInt16(IFormatProvider provider) => (short)(int)this;

		public ushort ToUInt16(IFormatProvider provider) => (ushort)(uint)this;

		public int ToInt32(IFormatProvider provider) => (int)this;

		public uint ToUInt32(IFormatProvider provider) => (uint)this;

		public long ToInt64(IFormatProvider provider) => (long)this;

		public ulong ToUInt64(IFormatProvider provider) => (ulong)this;

		public float ToSingle(IFormatProvider provider) => (float)this;

		public double ToDouble(IFormatProvider provider) => (double)this;

		public decimal ToDecimal(IFormatProvider provider) => throw new InvalidCastException();

		public DateTime ToDateTime(IFormatProvider provider) => throw new InvalidCastException();

		public object ToType(Type conversionType, IFormatProvider provider) => throw new InvalidCastException();

		#endregion

	}
}

