using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit8_4Tests
	{
		[Test]
		public void Posit8_4_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit8_4(0).PositBits, 0);
			Assert.AreEqual((int)new Posit8_4(1), 1);
			Assert.AreEqual((int)new Posit8_4(-1), -1);
			Assert.AreEqual((int)new Posit8_4(3), 3);
			Assert.AreEqual((int)new Posit8_4(-3), -3);
			Assert.AreEqual((int)new Posit8_4(8), 8);
			Assert.AreEqual((int)new Posit8_4(-16), -16);
			Assert.AreEqual((int)new Posit8_4(1024), 1024);
			Assert.AreEqual((int)new Posit8_4(-1024), -1024);

									
		}

		[Test]
		public void Posit8_4_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit8_4((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit8_4((float)1.0), 1);
			Assert.AreEqual((float)new Posit8_4((float)2.0), 2);
			Assert.AreEqual((float)new Posit8_4((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit8_4((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit8_4((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit8_4((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit8_4((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit8_4((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit8_4((float)6), 6);
			Assert.AreEqual((float)new Posit8_4((float)-6), -6);
			Assert.AreEqual((float)new Posit8_4((float) 7.92281625142643E+28),(float)7.92281625142643E+28);
			Assert.AreEqual((float)new Posit8_4((float) -7.92281625142643E+28),(float)-7.92281625142643E+28);			
			}

		[Test]
		public void Posit8_4_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit8_4(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit8_4(1.0), 1.0);
			Assert.AreEqual((double)new Posit8_4(2.0), 2);
			Assert.AreEqual((double)new Posit8_4(0.5), 0.5);
			Assert.AreEqual((double)new Posit8_4(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit8_4(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit8_4(1.5), 1.5);
			Assert.AreEqual((double)new Posit8_4(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit8_4(-1.5), -1.5);
			Assert.AreEqual((double)new Posit8_4(6), 6);
			Assert.AreEqual((double)new Posit8_4(-6), -6);
			Assert.AreEqual((float)(double)new Posit8_4( 7.92281625142643E+28),(float)7.92281625142643E+28);
			Assert.AreEqual((float)(double)new Posit8_4( -7.92281625142643E+28),(float)-7.92281625142643E+28);			
		}
		
		[Test]
		public void Posit8_4_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit8_4(1);

			for (var i = 1; i < 4; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit8_4(4));
		}

		[Test]
		public void Posit8_4_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit8_4(-2);

			for (var i = 1; i < 4; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit8_4(1));
		}

		[Test]
		public void Posit8_4_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit8_4(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit8_4(0.03125));
			(posit1-posit2).ShouldBe(new Posit8_4(-0.015625));
			(new Posit8_4(1) - new Posit8_4(0.1)).ShouldBe(new Posit8_4(0.9));
			
			  					
		}	

		[Test]
		public void Posit8_4_MultiplicationIsCorrect()
		{
			 var posit1 = new Posit8_4(1);
			 (posit1 * new Posit8_4(0.015625)).ShouldBe(new Posit8_4(0.015625));
			 (posit1 * new Posit8_4(256)).ShouldBe(new Posit8_4(256));
			 (-posit1 * new Posit8_4(3)).ShouldBe(new Posit8_4(-3));
			 (new Posit8_4(2) * new Posit8_4(0.015625)).ShouldBe(new Posit8_4(0.03125));
			 (new Posit8_4(4) * new Posit8_4(16)).ShouldBe(new Posit8_4(64));
			 (new Posit8_4(-3) * new Posit8_4(-4)).ShouldBe(new Posit8_4(12));
			
				  					
		}	

		[Test]
		public void Posit8_4_DivisionIsCorrect()
		{
			 var posit1 = new Posit8_4(1);
			 (posit1 / new Posit8_4(0)).ShouldBe(new Posit8_4(Posit8_4.NaNBitMask, true));
			 (new Posit8_4(0.015625) / posit1).ShouldBe(new Posit8_4(0.015625));
			 (new Posit8_4(256) / posit1).ShouldBe(new Posit8_4(256));
			 (new Posit8_4(3) / -posit1).ShouldBe(new Posit8_4(-3));
			 (new Posit8_4(0.03125) / new Posit8_4(2)).ShouldBe(new Posit8_4(0.015625));
			 (new Posit8_4(64) / new Posit8_4(16)).ShouldBe(new Posit8_4(4));
			 (new Posit8_4(12) / new Posit8_4(-4)).ShouldBe(new Posit8_4(-3));
			
			    
		 }	

		[Test]
		public void Posit8_4_SqrtIsCorrect()
		{
			 var posit1 = new Posit8_4(1);
			 Posit8_4.Sqrt(posit1).ShouldBe(posit1);
			 Posit8_4.Sqrt(-posit1).ShouldBe(new Posit8_4(Posit8_4.NaNBitMask, true));
	 
			 (Posit8_4.Sqrt(new Posit8_4(4))).ShouldBe(new Posit8_4(2));
			 (Posit8_4.Sqrt(new Posit8_4(64))).ShouldBe(new Posit8_4(8));
			 (Posit8_4.Sqrt(new Posit8_4(0.25))).ShouldBe(new Posit8_4(0.5));
			 
			 			 
			 
		}
		
		[Test]
		public void Posit8_4_FusedSumIsCorrect()
		{

		System.Console.WriteLine("Posit8_4 " +  Posit8_4.QuireSize + " fs: "+  Posit8_4.QuireFractionSize);
			var positArray = new Posit8_4[257];
			positArray[0] = new Posit8_4(-64);
			for(var i=1;i<=256;i++) positArray[i] = new Posit8_4(0.5);          
			
			Assert.AreEqual(Posit8_4.FusedSum(positArray).PositBits, new Posit8_4(64).PositBits);

			positArray[2] = new Posit8_4(Posit8_4.NaNBitMask, true);
			Assert.AreEqual(Posit8_4.FusedSum(positArray).PositBits, positArray[2].PositBits);
		}
	}
}
