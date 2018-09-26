using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit16_1Tests
	{
		[Test]
		public void Posit16_1_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_1(0).PositBits, 0);
			Assert.AreEqual((int)new Posit16_1(1), 1);
			Assert.AreEqual((int)new Posit16_1(-1), -1);
			Assert.AreEqual((int)new Posit16_1(3), 3);
			Assert.AreEqual((int)new Posit16_1(-3), -3);
			Assert.AreEqual((int)new Posit16_1(8), 8);
			Assert.AreEqual((int)new Posit16_1(-16), -16);
			Assert.AreEqual((int)new Posit16_1(1024), 1024);
			Assert.AreEqual((int)new Posit16_1(-1024), -1024);

			Assert.AreEqual((int)new Posit16_1(int.MaxValue), 268435456);
			Assert.AreEqual((int)new Posit16_1(int.MinValue), -268435456);
			Assert.AreEqual((int)new Posit16_1(100), 100);						
		}

		[Test]
		public void Posit16_1_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_1((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit16_1((float)1.0), 1);
			Assert.AreEqual((float)new Posit16_1((float)2.0), 2);
			Assert.AreEqual((float)new Posit16_1((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit16_1((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit16_1((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit16_1((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit16_1((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit16_1((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit16_1((float)6), 6);
			Assert.AreEqual((float)new Posit16_1((float)-6), -6);
			Assert.AreEqual((float)new Posit16_1((float) 268435456),(float)268435456);
			Assert.AreEqual((float)new Posit16_1((float) -268435456),(float)-268435456);			
			}

		[Test]
		public void Posit16_1_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit16_1(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit16_1(1.0), 1.0);
			Assert.AreEqual((double)new Posit16_1(2.0), 2);
			Assert.AreEqual((double)new Posit16_1(0.5), 0.5);
			Assert.AreEqual((double)new Posit16_1(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit16_1(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit16_1(1.5), 1.5);
			Assert.AreEqual((double)new Posit16_1(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit16_1(-1.5), -1.5);
			Assert.AreEqual((double)new Posit16_1(6), 6);
			Assert.AreEqual((double)new Posit16_1(-6), -6);
			Assert.AreEqual((float)(double)new Posit16_1( 268435456),(float)268435456);
			Assert.AreEqual((float)(double)new Posit16_1( -268435456),(float)-268435456);			
		}
		
		[Test]
		public void Posit16_1_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit16_1(1);

			for (var i = 1; i < 256; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_1(256));
		}

		[Test]
		public void Posit16_1_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit16_1(-128);

			for (var i = 1; i < 256; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_1(127));
		}

		[Test]
		public void Posit16_1_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit16_1(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit16_1(0.03125));
			(posit1-posit2).ShouldBe(new Posit16_1(-0.015625));
			(new Posit16_1(1) - new Posit16_1(0.1)).ShouldBe(new Posit16_1(0.9));
			
			(new Posit16_1(10.015625) - new Posit16_1(0.015625)).ShouldBe(new Posit16_1(10));
			(new Posit16_1(127.5) + new Posit16_1(127.5)).ShouldBe(new Posit16_1(255));
			(new Posit16_1(-16.625) + new Posit16_1(21.875)).ShouldBe(new Posit16_1(-16.625 + 21.875));
			  					
		}	

		[Test]
		public void Posit16_1_MultiplicationIsCorrect()
		{
			 var posit1 = new Posit16_1(1);
			 (posit1 * new Posit16_1(0.015625)).ShouldBe(new Posit16_1(0.015625));
			 (posit1 * new Posit16_1(256)).ShouldBe(new Posit16_1(256));
			 (-posit1 * new Posit16_1(3)).ShouldBe(new Posit16_1(-3));
			 (new Posit16_1(2) * new Posit16_1(0.015625)).ShouldBe(new Posit16_1(0.03125));
			 (new Posit16_1(4) * new Posit16_1(16)).ShouldBe(new Posit16_1(64));
			 (new Posit16_1(-3) * new Posit16_1(-4)).ShouldBe(new Posit16_1(12));
			
			 (new Posit16_1(127.5) * new Posit16_1(2)).ShouldBe(new Posit16_1(255));
			 (new Posit16_1(-16.625) * new Posit16_1(-4)).ShouldBe(new Posit16_1(66.5));		  					
		}	

		[Test]
		public void Posit16_1_DivisionIsCorrect()
		{
			 var posit1 = new Posit16_1(1);
			 (posit1 / new Posit16_1(0)).ShouldBe(new Posit16_1(Posit16_1.NaNBitMask, true));
			 (new Posit16_1(0.015625) / posit1).ShouldBe(new Posit16_1(0.015625));
			 (new Posit16_1(256) / posit1).ShouldBe(new Posit16_1(256));
			 (new Posit16_1(3) / -posit1).ShouldBe(new Posit16_1(-3));
			 (new Posit16_1(0.03125) / new Posit16_1(2)).ShouldBe(new Posit16_1(0.015625));
			 (new Posit16_1(64) / new Posit16_1(16)).ShouldBe(new Posit16_1(4));
			 (new Posit16_1(12) / new Posit16_1(-4)).ShouldBe(new Posit16_1(-3));
			
			 (new Posit16_1(252) / new Posit16_1(2)).ShouldBe(new Posit16_1(126));
			 (new Posit16_1(66.5) / new Posit16_1(-4)).ShouldBe(new Posit16_1(-16.625));
			   
		 }	

		[Test]
		public void Posit16_1_SqrtIsCorrect()
		{
			 var posit1 = new Posit16_1(1);
			 Posit16_1.Sqrt(posit1).ShouldBe(posit1);
			 Posit16_1.Sqrt(-posit1).ShouldBe(new Posit16_1(Posit16_1.NaNBitMask, true));
	 
			 (Posit16_1.Sqrt(new Posit16_1(4))).ShouldBe(new Posit16_1(2));
			 (Posit16_1.Sqrt(new Posit16_1(64))).ShouldBe(new Posit16_1(8));
			 (Posit16_1.Sqrt(new Posit16_1(0.25))).ShouldBe(new Posit16_1(0.5));
			 
			 (Posit16_1.Sqrt(new Posit16_1(100))).ShouldBe(new Posit16_1(10));
			 (Posit16_1.Sqrt(new Posit16_1(144))).ShouldBe(new Posit16_1(12));
			 (Posit16_1.Sqrt(new Posit16_1(896))).ShouldBe(new Posit16_1(29.9332590942));
						 
			 		}
		
		[Test]
		public void Posit16_1_FusedSumIsCorrect()
		{
			//System.Console.WriteLine("Posit16_1 " +  Posit16_1.QuireSize + " fs: "+  Posit16_1.QuireFractionSize);
			var positArray = new Posit16_1[257];
			positArray[0] = new Posit16_1(-64);
			for(var i=1; i <= 256; i++) positArray[i] = new Posit16_1(0.5);          
			
			Assert.AreEqual(Posit16_1.FusedSum(positArray).PositBits, new Posit16_1(64).PositBits);

			positArray[2] = new Posit16_1(Posit16_1.NaNBitMask, true);
			Assert.AreEqual(Posit16_1.FusedSum(positArray).PositBits, positArray[2].PositBits);

			var positArray2 = new Posit16_1[1281];
			positArray2[0] = new Posit16_1(0);
			for(var i=1; i <= 1280; i++) positArray2[i] = new Posit16_1(0.1);
			Assert.AreEqual(Posit16_1.FusedSum(positArray2).PositBits, new Posit16_1(128).PositBits);
		}

		[Test]
		public void Posit16_1_FusedDotProductIsCorrect()
		{
			var positArray1 = new Posit16_1[3];
			var positArray2 = new Posit16_1[3];
			positArray1[0] = new Posit16_1(1);
			positArray1[1] = new Posit16_1(2);
			positArray1[2] = new Posit16_1(3);

			positArray2[0] = new Posit16_1(1);
			positArray2[1] = new Posit16_1(2);
			positArray2[2] = new Posit16_1(4);
			Assert.AreEqual(Posit16_1.FusedDotProduct(positArray1, positArray2).PositBits, new Posit16_1(17).PositBits);

			var positArray3 = new Posit16_1[3];
			positArray3[0] = new Posit16_1(-1);
			positArray3[1] = new Posit16_1(2);
			positArray3[2] = new Posit16_1(-100);
			Assert.AreEqual(Posit16_1.FusedDotProduct(positArray1, positArray3), new Posit16_1(-297));

			 var positArray4 = new Posit16_1[3];
			positArray4[0] = new Posit16_1(-1);
			positArray4[1] = new Posit16_1(2);
			positArray4[2] = new Posit16_1(Posit16_1.MaxValueBitMask, true);
			Assert.AreEqual(Posit16_1.FusedDotProduct(positArray1, positArray4), new Posit16_1(Posit16_1.MaxValueBitMask, true));
		}
	}
}
