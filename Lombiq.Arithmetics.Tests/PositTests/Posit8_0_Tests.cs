using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit8_0Tests
	{
		[Test]
		public void Posit8_0_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit8_0(0).PositBits, 0);
			Assert.AreEqual((int)new Posit8_0(1), 1);
			Assert.AreEqual((int)new Posit8_0(-1), -1);
			Assert.AreEqual((int)new Posit8_0(3), 3);
			Assert.AreEqual((int)new Posit8_0(-3), -3);
			Assert.AreEqual((int)new Posit8_0(8), 8);
			Assert.AreEqual((int)new Posit8_0(-16), -16);
			Assert.AreEqual((int)new Posit8_0(1024), 64);
			Assert.AreEqual((int)new Posit8_0(-1024), -64);

									
		}

		[Test]
		public void Posit8_0_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit8_0((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit8_0((float)1.0), 1);
			Assert.AreEqual((float)new Posit8_0((float)2.0), 2);
			Assert.AreEqual((float)new Posit8_0((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit8_0((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit8_0((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit8_0((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit8_0((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit8_0((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit8_0((float)6), 6);
			Assert.AreEqual((float)new Posit8_0((float)-6), -6);
			Assert.AreEqual((float)new Posit8_0((float) 64),(float)64);
			Assert.AreEqual((float)new Posit8_0((float) -64),(float)-64);			
			}

		[Test]
		public void Posit8_0_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit8_0(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit8_0(1.0), 1.0);
			Assert.AreEqual((double)new Posit8_0(2.0), 2);
			Assert.AreEqual((double)new Posit8_0(0.5), 0.5);
			Assert.AreEqual((double)new Posit8_0(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit8_0(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit8_0(1.5), 1.5);
			Assert.AreEqual((double)new Posit8_0(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit8_0(-1.5), -1.5);
			Assert.AreEqual((double)new Posit8_0(6), 6);
			Assert.AreEqual((double)new Posit8_0(-6), -6);
			Assert.AreEqual((float)(double)new Posit8_0( 64),(float)64);
			Assert.AreEqual((float)(double)new Posit8_0( -64),(float)-64);			
		}
		
		[Test]
		public void Posit8_0_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit8_0(1);

			for (var i = 1; i < 8; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit8_0(8));
		}

		[Test]
		public void Posit8_0_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit8_0(-4);

			for (var i = 1; i < 8; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit8_0(3));
		}

		[Test]
		public void Posit8_0_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit8_0(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit8_0(0.03125));
			(posit1-posit2).ShouldBe(new Posit8_0(-0.015625));
			(new Posit8_0(1) - new Posit8_0(0.1)).ShouldBe(new Posit8_0(0.9));
			
			  					
		}	

		[Test]
		public void Posit8_0_MultiplicationIsCorrect()
		{
			 var posit1 = new Posit8_0(1);
			 (posit1 * new Posit8_0(0.015625)).ShouldBe(new Posit8_0(0.015625));
			 (posit1 * new Posit8_0(256)).ShouldBe(new Posit8_0(256));
			 (-posit1 * new Posit8_0(3)).ShouldBe(new Posit8_0(-3));
			 (new Posit8_0(2) * new Posit8_0(0.015625)).ShouldBe(new Posit8_0(0.03125));
			 (new Posit8_0(4) * new Posit8_0(16)).ShouldBe(new Posit8_0(64));
			 (new Posit8_0(-3) * new Posit8_0(-4)).ShouldBe(new Posit8_0(12));
			
				  					
		}	

		[Test]
		public void Posit8_0_DivisionIsCorrect()
		{
			 var posit1 = new Posit8_0(1);
			 (posit1 / new Posit8_0(0)).ShouldBe(new Posit8_0(Posit8_0.NaNBitMask, true));
			 (new Posit8_0(0.015625) / posit1).ShouldBe(new Posit8_0(0.015625));
			 (new Posit8_0(256) / posit1).ShouldBe(new Posit8_0(256));
			 (new Posit8_0(3) / -posit1).ShouldBe(new Posit8_0(-3));
			 (new Posit8_0(0.03125) / new Posit8_0(2)).ShouldBe(new Posit8_0(0.015625));
			 (new Posit8_0(64) / new Posit8_0(16)).ShouldBe(new Posit8_0(4));
			 (new Posit8_0(12) / new Posit8_0(-4)).ShouldBe(new Posit8_0(-3));
			
			    
		 }	

		[Test]
		public void Posit8_0_SqrtIsCorrect()
		{
			 var posit1 = new Posit8_0(1);
			 Posit8_0.Sqrt(posit1).ShouldBe(posit1);
			 Posit8_0.Sqrt(-posit1).ShouldBe(new Posit8_0(Posit8_0.NaNBitMask, true));
	 
			 (Posit8_0.Sqrt(new Posit8_0(4))).ShouldBe(new Posit8_0(2));
			 (Posit8_0.Sqrt(new Posit8_0(64))).ShouldBe(new Posit8_0(8));
			 (Posit8_0.Sqrt(new Posit8_0(0.25))).ShouldBe(new Posit8_0(0.5));
			 
			 			 
			 		}
		
		[Test]
		public void Posit8_0_FusedSumIsCorrect()
		{
			System.Console.WriteLine("Posit8_0 " +  Posit8_0.QuireSize + " fs: "+  Posit8_0.QuireFractionSize);
			var positArray = new Posit8_0[257];
			positArray[0] = new Posit8_0(-64);
			for(var i=1; i <= 256; i++) positArray[i] = new Posit8_0(0.5);          
			
			Assert.AreEqual(Posit8_0.FusedSum(positArray).PositBits, new Posit8_0(64).PositBits);

			positArray[2] = new Posit8_0(Posit8_0.NaNBitMask, true);
			Assert.AreEqual(Posit8_0.FusedSum(positArray).PositBits, positArray[2].PositBits);

			var positArray2 = new Posit8_0[1281];
			positArray2[0] = new Posit8_0(0);
			for(var i=1; i <= 1280; i++) positArray2[i] = new Posit8_0(0.1);
			Assert.AreEqual(Posit8_0.FusedSum(positArray2).PositBits, new Posit8_0(128).PositBits);
		}
	}
}
