using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit16_2Tests
	{
		[Test]
		public void Posit16_2_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_2(0).PositBits, 0);
			Assert.AreEqual((int)new Posit16_2(1), 1);
			Assert.AreEqual((int)new Posit16_2(-1), -1);
			Assert.AreEqual((int)new Posit16_2(3), 3);
			Assert.AreEqual((int)new Posit16_2(-3), -3);
			Assert.AreEqual((int)new Posit16_2(8), 8);
			Assert.AreEqual((int)new Posit16_2(-16), -16);
			Assert.AreEqual((int)new Posit16_2(1024), 1024);
			Assert.AreEqual((int)new Posit16_2(-1024), -1024);

			Assert.AreEqual((int)new Posit16_2(int.MaxValue), 2147483647);
			Assert.AreEqual((int)new Posit16_2(int.MinValue), -2147483648);
			Assert.AreEqual((int)new Posit16_2(100), 100);						
		}

		[Test]
		public void Posit16_2_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_2((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit16_2((float)1.0), 1);
			Assert.AreEqual((float)new Posit16_2((float)2.0), 2);
			Assert.AreEqual((float)new Posit16_2((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit16_2((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit16_2((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit16_2((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit16_2((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit16_2((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit16_2((float)6), 6);
			Assert.AreEqual((float)new Posit16_2((float)-6), -6);
			Assert.AreEqual((float)new Posit16_2((float) 7.20575940379279E+16),(float)7.20575940379279E+16);
			Assert.AreEqual((float)new Posit16_2((float) -7.20575940379279E+16),(float)-7.20575940379279E+16);			
			}

		[Test]
		public void Posit16_2_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit16_2(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit16_2(1.0), 1.0);
			Assert.AreEqual((double)new Posit16_2(2.0), 2);
			Assert.AreEqual((double)new Posit16_2(0.5), 0.5);
			Assert.AreEqual((double)new Posit16_2(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit16_2(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit16_2(1.5), 1.5);
			Assert.AreEqual((double)new Posit16_2(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit16_2(-1.5), -1.5);
			Assert.AreEqual((double)new Posit16_2(6), 6);
			Assert.AreEqual((double)new Posit16_2(-6), -6);
			Assert.AreEqual((float)(double)new Posit16_2( 7.20575940379279E+16),(float)7.20575940379279E+16);
			Assert.AreEqual((float)(double)new Posit16_2( -7.20575940379279E+16),(float)-7.20575940379279E+16);			
		}
		
		[Test]
		public void Posit16_2_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit16_2(1);

			for (var i = 1; i < 512; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_2(512));
		}

		[Test]
		public void Posit16_2_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit16_2(-256);

			for (var i = 1; i < 512; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_2(255));
		}

		[Test]
		public void Posit16_2_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit16_2(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit16_2(0.03125));
			(posit1-posit2).ShouldBe(new Posit16_2(-0.015625));
			(new Posit16_2(1) - new Posit16_2(0.1)).ShouldBe(new Posit16_2(0.9));
			
			(new Posit16_2(10.015625) - new Posit16_2(0.015625)).ShouldBe(new Posit16_2(10));
			(new Posit16_2(127.5) + new Posit16_2(127.5)).ShouldBe(new Posit16_2(255));
			(new Posit16_2(-16.625) + new Posit16_2(21.875)).ShouldBe(new Posit16_2(-16.625 + 21.875));
			  					
		}	

		[Test]
		public void Posit16_2_MultiplicationIsCorrect()
		{
			 var posit1 = new Posit16_2(1);
			 (posit1 * new Posit16_2(0.015625)).ShouldBe(new Posit16_2(0.015625));
			 (posit1 * new Posit16_2(256)).ShouldBe(new Posit16_2(256));
			 (-posit1 * new Posit16_2(3)).ShouldBe(new Posit16_2(-3));
			 (new Posit16_2(2) * new Posit16_2(0.015625)).ShouldBe(new Posit16_2(0.03125));
			 (new Posit16_2(4) * new Posit16_2(16)).ShouldBe(new Posit16_2(64));
			 (new Posit16_2(-3) * new Posit16_2(-4)).ShouldBe(new Posit16_2(12));
			
			 (new Posit16_2(127.5) * new Posit16_2(2)).ShouldBe(new Posit16_2(255));
			 (new Posit16_2(-16.625) * new Posit16_2(-4)).ShouldBe(new Posit16_2(66.5));		  					
		}	

		[Test]
		public void Posit16_2_DivisionIsCorrect()
		{
			 var posit1 = new Posit16_2(1);
			 (posit1 / new Posit16_2(0)).ShouldBe(new Posit16_2(Posit16_2.NaNBitMask, true));
			 (new Posit16_2(0.015625) / posit1).ShouldBe(new Posit16_2(0.015625));
			 (new Posit16_2(256) / posit1).ShouldBe(new Posit16_2(256));
			 (new Posit16_2(3) / -posit1).ShouldBe(new Posit16_2(-3));
			 (new Posit16_2(0.03125) / new Posit16_2(2)).ShouldBe(new Posit16_2(0.015625));
			 (new Posit16_2(64) / new Posit16_2(16)).ShouldBe(new Posit16_2(4));
			 (new Posit16_2(12) / new Posit16_2(-4)).ShouldBe(new Posit16_2(-3));
			
			 (new Posit16_2(252) / new Posit16_2(2)).ShouldBe(new Posit16_2(126));
			 (new Posit16_2(66.5) / new Posit16_2(-4)).ShouldBe(new Posit16_2(-16.625));
			   
		 }	

		[Test]
		public void Posit16_2_SqrtIsCorrect()
		{
			 var posit1 = new Posit16_2(1);
			 Posit16_2.Sqrt(posit1).ShouldBe(posit1);
			 Posit16_2.Sqrt(-posit1).ShouldBe(new Posit16_2(Posit16_2.NaNBitMask, true));
	 
			 (Posit16_2.Sqrt(new Posit16_2(4))).ShouldBe(new Posit16_2(2));
			 (Posit16_2.Sqrt(new Posit16_2(64))).ShouldBe(new Posit16_2(8));
			 (Posit16_2.Sqrt(new Posit16_2(0.25))).ShouldBe(new Posit16_2(0.5));
			 
			 (Posit16_2.Sqrt(new Posit16_2(100))).ShouldBe(new Posit16_2(10));
			 (Posit16_2.Sqrt(new Posit16_2(144))).ShouldBe(new Posit16_2(12));
			 (Posit16_2.Sqrt(new Posit16_2(896))).ShouldBe(new Posit16_2(29.9332590942));
						 
			 
		}
		
		[Test]
		public void Posit16_2_FusedSumIsCorrect()
		{

		System.Console.WriteLine("Posit16_2 " +  Posit16_2.QuireSize + " fs: "+  Posit16_2.QuireFractionSize);
			var positArray = new Posit16_2[257];
			positArray[0] = new Posit16_2(-64);
			for(var i=1;i<=256;i++) positArray[i] = new Posit16_2(0.5);          
			
			Assert.AreEqual(Posit16_2.FusedSum(positArray).PositBits, new Posit16_2(64).PositBits);

			positArray[2] = new Posit16_2(Posit16_2.NaNBitMask, true);
			Assert.AreEqual(Posit16_2.FusedSum(positArray).PositBits, positArray[2].PositBits);
		}
	}
}
