using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit16_0Tests
	{
		[Test]
		public void Posit16_0_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_0(0).PositBits, 0);
			Assert.AreEqual((int)new Posit16_0(1), 1);
			Assert.AreEqual((int)new Posit16_0(-1), -1);
			Assert.AreEqual((int)new Posit16_0(3), 3);
			Assert.AreEqual((int)new Posit16_0(-3), -3);
			Assert.AreEqual((int)new Posit16_0(8), 8);
			Assert.AreEqual((int)new Posit16_0(-16), -16);
			Assert.AreEqual((int)new Posit16_0(1024), 1024);
			Assert.AreEqual((int)new Posit16_0(-1024), -1024);

			Assert.AreEqual((int)new Posit16_0(int.MaxValue), 16384);
			Assert.AreEqual((int)new Posit16_0(int.MinValue), -16384);
			Assert.AreEqual((int)new Posit16_0(100), 100);						
		}

		[Test]
		public void Posit16_0_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_0((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit16_0((float)1.0), 1);
			Assert.AreEqual((float)new Posit16_0((float)2.0), 2);
			Assert.AreEqual((float)new Posit16_0((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit16_0((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit16_0((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit16_0((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit16_0((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit16_0((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit16_0((float)6), 6);
			Assert.AreEqual((float)new Posit16_0((float)-6), -6);
			Assert.AreEqual((float)new Posit16_0((float) 16384),(float)16384);
			Assert.AreEqual((float)new Posit16_0((float) -16384),(float)-16384);			
			}

		[Test]
		public void Posit16_0_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit16_0(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit16_0(1.0), 1.0);
			Assert.AreEqual((double)new Posit16_0(2.0), 2);
			Assert.AreEqual((double)new Posit16_0(0.5), 0.5);
			Assert.AreEqual((double)new Posit16_0(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit16_0(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit16_0(1.5), 1.5);
			Assert.AreEqual((double)new Posit16_0(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit16_0(-1.5), -1.5);
			Assert.AreEqual((double)new Posit16_0(6), 6);
			Assert.AreEqual((double)new Posit16_0(-6), -6);
			Assert.AreEqual((float)(double)new Posit16_0( 16384),(float)16384);
			Assert.AreEqual((float)(double)new Posit16_0( -16384),(float)-16384);			
		}
		
		[Test]
		public void Posit16_0_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit16_0(1);

			for (var i = 1; i < 128; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_0(128));
		}

		[Test]
		public void Posit16_0_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit16_0(-64);

			for (var i = 1; i < 128; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_0(63));
		}

		[Test]
		public void Posit16_0_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit16_0(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit16_0(0.03125));
			(posit1-posit2).ShouldBe(new Posit16_0(-0.015625));
			(new Posit16_0(1) - new Posit16_0(0.1)).ShouldBe(new Posit16_0(0.9));
			
			(new Posit16_0(10.015625) - new Posit16_0(0.015625)).ShouldBe(new Posit16_0(10));
			(new Posit16_0(127.5) + new Posit16_0(127.5)).ShouldBe(new Posit16_0(255));
			(new Posit16_0(-16.625) + new Posit16_0(21.875)).ShouldBe(new Posit16_0(-16.625 + 21.875));
			  					
		}	

		[Test]
		public void Posit16_0_MultiplicationIsCorrect()
		{
			 var posit1 = new Posit16_0(1);
			 (posit1 * new Posit16_0(0.015625)).ShouldBe(new Posit16_0(0.015625));
			 (posit1 * new Posit16_0(256)).ShouldBe(new Posit16_0(256));
			 (-posit1 * new Posit16_0(3)).ShouldBe(new Posit16_0(-3));
			 (new Posit16_0(2) * new Posit16_0(0.015625)).ShouldBe(new Posit16_0(0.03125));
			 (new Posit16_0(4) * new Posit16_0(16)).ShouldBe(new Posit16_0(64));
			 (new Posit16_0(-3) * new Posit16_0(-4)).ShouldBe(new Posit16_0(12));
			
			 (new Posit16_0(127.5) * new Posit16_0(2)).ShouldBe(new Posit16_0(255));
			 (new Posit16_0(-16.625) * new Posit16_0(-4)).ShouldBe(new Posit16_0(66.5));		  					
		}	

		[Test]
		public void Posit16_0_DivisionIsCorrect()
		{
			 var posit1 = new Posit16_0(1);
			 (posit1 / new Posit16_0(0)).ShouldBe(new Posit16_0(Posit16_0.NaNBitMask, true));
			 (new Posit16_0(0.015625) / posit1).ShouldBe(new Posit16_0(0.015625));
			 (new Posit16_0(256) / posit1).ShouldBe(new Posit16_0(256));
			 (new Posit16_0(3) / -posit1).ShouldBe(new Posit16_0(-3));
			 (new Posit16_0(0.03125) / new Posit16_0(2)).ShouldBe(new Posit16_0(0.015625));
			 (new Posit16_0(64) / new Posit16_0(16)).ShouldBe(new Posit16_0(4));
			 (new Posit16_0(12) / new Posit16_0(-4)).ShouldBe(new Posit16_0(-3));
			
			 (new Posit16_0(252) / new Posit16_0(2)).ShouldBe(new Posit16_0(126));
			 (new Posit16_0(66.5) / new Posit16_0(-4)).ShouldBe(new Posit16_0(-16.625));
			   
		 }	

		[Test]
		public void Posit16_0_SqrtIsCorrect()
		{
             var a = 257;
             System.Console.WriteLine((byte)a);
			 var posit1 = new Posit16_0(1);
			 Posit16_0.Sqrt(posit1).ShouldBe(posit1);
			 Posit16_0.Sqrt(-posit1).ShouldBe(new Posit16_0(Posit16_0.NaNBitMask, true));
	 
			 (Posit16_0.Sqrt(new Posit16_0(4))).ShouldBe(new Posit16_0(2));
			 (Posit16_0.Sqrt(new Posit16_0(64))).ShouldBe(new Posit16_0(8));
			 (Posit16_0.Sqrt(new Posit16_0(0.25))).ShouldBe(new Posit16_0(0.5));
			 
			 (Posit16_0.Sqrt(new Posit16_0(100))).ShouldBe(new Posit16_0(10));
			 (Posit16_0.Sqrt(new Posit16_0(144))).ShouldBe(new Posit16_0(12));
			 (Posit16_0.Sqrt(new Posit16_0(896))).ShouldBe(new Posit16_0(29.9332590942));
						 
			 		}
		
		[Test]
		public void Posit16_0_FusedSumIsCorrect()
		{
			System.Console.WriteLine("Posit16_0 " +  Posit16_0.QuireSize + " fs: "+  Posit16_0.QuireFractionSize);
			var positArray = new Posit16_0[257];
			positArray[0] = new Posit16_0(-64);
			for(var i=1; i <= 256; i++) positArray[i] = new Posit16_0(0.5);          
			
			Assert.AreEqual(Posit16_0.FusedSum(positArray).PositBits, new Posit16_0(64).PositBits);

			positArray[2] = new Posit16_0(Posit16_0.NaNBitMask, true);
			Assert.AreEqual(Posit16_0.FusedSum(positArray).PositBits, positArray[2].PositBits);

			var positArray2 = new Posit16_0[1281];
			positArray2[0] = new Posit16_0(0);
			for(var i=1; i <= 1280; i++) positArray2[i] = new Posit16_0(0.1);
			Assert.AreEqual(Posit16_0.FusedSum(positArray2).PositBits, new Posit16_0(128).PositBits);
		}
	}
}
