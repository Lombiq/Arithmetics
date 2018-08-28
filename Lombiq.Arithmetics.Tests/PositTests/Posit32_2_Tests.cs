using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit32_2Tests
	{
		[Test]
		public void Posit32_2_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_2(0).PositBits, 0);
			Assert.AreEqual((int)new Posit32_2(1), 1);
			Assert.AreEqual((int)new Posit32_2(-1), -1);
			Assert.AreEqual((int)new Posit32_2(3), 3);
			Assert.AreEqual((int)new Posit32_2(-3), -3);
			Assert.AreEqual((int)new Posit32_2(8), 8);
			Assert.AreEqual((int)new Posit32_2(-16), -16);
			Assert.AreEqual((int)new Posit32_2(1024), 1024);
			Assert.AreEqual((int)new Posit32_2(-1024), -1024);

			Assert.AreEqual((int)new Posit32_2(int.MaxValue), 2147483647);
			Assert.AreEqual((int)new Posit32_2(int.MinValue), -2147483648);
			Assert.AreEqual((int)new Posit32_2(100), 100);						
		}

		[Test]
		public void Posit32_2_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_2((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit32_2((float)1.0), 1);
			Assert.AreEqual((float)new Posit32_2((float)2.0), 2);
			Assert.AreEqual((float)new Posit32_2((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit32_2((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit32_2((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit32_2((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit32_2((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit32_2((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit32_2((float)6), 6);
			Assert.AreEqual((float)new Posit32_2((float)-6), -6);
			Assert.AreEqual((float)new Posit32_2((float) 1.32922799578492E+36),(float)1.32922799578492E+36);
			Assert.AreEqual((float)new Posit32_2((float) -1.32922799578492E+36),(float)-1.32922799578492E+36);			
			}

		[Test]
		public void Posit32_2_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit32_2(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit32_2(1.0), 1.0);
			Assert.AreEqual((double)new Posit32_2(2.0), 2);
			Assert.AreEqual((double)new Posit32_2(0.5), 0.5);
			Assert.AreEqual((double)new Posit32_2(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit32_2(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit32_2(1.5), 1.5);
			Assert.AreEqual((double)new Posit32_2(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit32_2(-1.5), -1.5);
			Assert.AreEqual((double)new Posit32_2(6), 6);
			Assert.AreEqual((double)new Posit32_2(-6), -6);
			Assert.AreEqual((float)(double)new Posit32_2( 1.32922799578492E+36),(float)1.32922799578492E+36);
			Assert.AreEqual((float)(double)new Posit32_2( -1.32922799578492E+36),(float)-1.32922799578492E+36);			
		}
		
		[Test]
		public void Posit32_2_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit32_2(1);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_2(1000));
		}

		[Test]
		public void Posit32_2_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit32_2(-500);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_2(499));
		}

		[Test]
		public void Posit32_2_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit32_2(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit32_2(0.03125));
			(posit1-posit2).ShouldBe(new Posit32_2(-0.015625));
			(new Posit32_2(1) - new Posit32_2(0.1)).ShouldBe(new Posit32_2(0.9));
			
			(new Posit32_2(10.015625) - new Posit32_2(0.015625)).ShouldBe(new Posit32_2(10));
		    (new Posit32_2(127.5) + new Posit32_2(127.5)).ShouldBe(new Posit32_2(255));
			(new Posit32_2(-16.625) + new Posit32_2(21.875)).ShouldBe(new Posit32_2(-16.625 + 21.875));
			(new Posit32_2(0.00001) + new Posit32_2(100)).ShouldBe(new Posit32_2(100.00001));  					
		}	

		[Test]
		public void Posit32_2_MultiplicationIsCorrect()
		{
			 var posit1 = new Posit32_2(1);
			 (posit1 * new Posit32_2(0.015625)).ShouldBe(new Posit32_2(0.015625));
			 (posit1 * new Posit32_2(256)).ShouldBe(new Posit32_2(256));
			 (-posit1 * new Posit32_2(3)).ShouldBe(new Posit32_2(-3));
			 (new Posit32_2(2) * new Posit32_2(0.015625)).ShouldBe(new Posit32_2(0.03125));
			 (new Posit32_2(4) * new Posit32_2(16)).ShouldBe(new Posit32_2(64));
			 (new Posit32_2(-3) * new Posit32_2(-4)).ShouldBe(new Posit32_2(12));
			
			 (new Posit32_2(127.5) * new Posit32_2(2)).ShouldBe(new Posit32_2(255));
			 (new Posit32_2(-16.625) * new Posit32_2(-4)).ShouldBe(new Posit32_2(66.5));		(new Posit32_2(100) * new Posit32_2(0.9)).ShouldBe(new Posit32_2(90));
			 (new Posit32_2(-0.95) * new Posit32_2(-10000)).ShouldBe(new Posit32_2(9500));
			 (new Posit32_2(-0.995) * new Posit32_2(100000)).ShouldBe(new Posit32_2(-99500));  					
		}	

		[Test]
		public void Posit32_2_DivisionIsCorrect()
		{
			 var posit1 = new Posit32_2(1);
			 (posit1 / new Posit32_2(0)).ShouldBe(new Posit32_2(Posit32_2.NaNBitMask, true));
			 (new Posit32_2(0.015625) / posit1).ShouldBe(new Posit32_2(0.015625));
			 (new Posit32_2(256) / posit1).ShouldBe(new Posit32_2(256));
			 (new Posit32_2(3) / -posit1).ShouldBe(new Posit32_2(-3));
			 (new Posit32_2(0.03125) / new Posit32_2(2)).ShouldBe(new Posit32_2(0.015625));
			 (new Posit32_2(64) / new Posit32_2(16)).ShouldBe(new Posit32_2(4));
			 (new Posit32_2(12) / new Posit32_2(-4)).ShouldBe(new Posit32_2(-3));
			
			 (new Posit32_2(252) / new Posit32_2(2)).ShouldBe(new Posit32_2(126));
			 (new Posit32_2(66.5) / new Posit32_2(-4)).ShouldBe(new Posit32_2(-16.625));
			 (new Posit32_2(90) / new Posit32_2(0.9)).ShouldBe(new Posit32_2(100));
			 (new Posit32_2(9200)  / new Posit32_2(-10000)).ShouldBe(new Posit32_2(-0.92));
			 (new Posit32_2(-80800) / new Posit32_2(1000)).ShouldBe(new Posit32_2(-80.80));  
		 }										
	}
}
