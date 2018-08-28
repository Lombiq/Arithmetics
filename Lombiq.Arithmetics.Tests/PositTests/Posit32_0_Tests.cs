using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit32_0Tests
	{
		[Test]
		public void Posit32_0_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_0(0).PositBits, 0);
			Assert.AreEqual((int)new Posit32_0(1), 1);
			Assert.AreEqual((int)new Posit32_0(-1), -1);
			Assert.AreEqual((int)new Posit32_0(3), 3);
			Assert.AreEqual((int)new Posit32_0(-3), -3);
			Assert.AreEqual((int)new Posit32_0(8), 8);
			Assert.AreEqual((int)new Posit32_0(-16), -16);
			Assert.AreEqual((int)new Posit32_0(1024), 1024);
			Assert.AreEqual((int)new Posit32_0(-1024), -1024);

			Assert.AreEqual((int)new Posit32_0(int.MaxValue), 1073741824);
			Assert.AreEqual((int)new Posit32_0(int.MinValue), -1073741824);
			Assert.AreEqual((int)new Posit32_0(100), 100);						
		}

		[Test]
		public void Posit32_0_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_0((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit32_0((float)1.0), 1);
			Assert.AreEqual((float)new Posit32_0((float)2.0), 2);
			Assert.AreEqual((float)new Posit32_0((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit32_0((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit32_0((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit32_0((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit32_0((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit32_0((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit32_0((float)6), 6);
			Assert.AreEqual((float)new Posit32_0((float)-6), -6);
			Assert.AreEqual((float)new Posit32_0((float) 1073741824),(float)1073741824);
			Assert.AreEqual((float)new Posit32_0((float) -1073741824),(float)-1073741824);			
			}

		[Test]
		public void Posit32_0_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit32_0(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit32_0(1.0), 1.0);
			Assert.AreEqual((double)new Posit32_0(2.0), 2);
			Assert.AreEqual((double)new Posit32_0(0.5), 0.5);
			Assert.AreEqual((double)new Posit32_0(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit32_0(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit32_0(1.5), 1.5);
			Assert.AreEqual((double)new Posit32_0(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit32_0(-1.5), -1.5);
			Assert.AreEqual((double)new Posit32_0(6), 6);
			Assert.AreEqual((double)new Posit32_0(-6), -6);
			Assert.AreEqual((float)(double)new Posit32_0( 1073741824),(float)1073741824);
			Assert.AreEqual((float)(double)new Posit32_0( -1073741824),(float)-1073741824);			
		}
		
		[Test]
		public void Posit32_0_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit32_0(1);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_0(1000));
		}

		[Test]
		public void Posit32_0_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit32_0(-500);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_0(499));
		}

		[Test]
		public void Posit32_0_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit32_0(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit32_0(0.03125));
			(posit1-posit2).ShouldBe(new Posit32_0(-0.015625));
			(new Posit32_0(1) - new Posit32_0(0.1)).ShouldBe(new Posit32_0(0.9));
			
			(new Posit32_0(10.015625) - new Posit32_0(0.015625)).ShouldBe(new Posit32_0(10));
		    (new Posit32_0(127.5) + new Posit32_0(127.5)).ShouldBe(new Posit32_0(255));
			(new Posit32_0(-16.625) + new Posit32_0(21.875)).ShouldBe(new Posit32_0(-16.625 + 21.875));
			(new Posit32_0(0.00001) + new Posit32_0(100)).ShouldBe(new Posit32_0(100.00001));  					
		}	

		[Test]
		public void Posit32_0_MultiplicationIsCorrect()
		{
			 var posit1 = new Posit32_0(1);
			 (posit1 * new Posit32_0(0.015625)).ShouldBe(new Posit32_0(0.015625));
			 (posit1 * new Posit32_0(256)).ShouldBe(new Posit32_0(256));
			 (-posit1 * new Posit32_0(3)).ShouldBe(new Posit32_0(-3));
			 (new Posit32_0(2) * new Posit32_0(0.015625)).ShouldBe(new Posit32_0(0.03125));
			 (new Posit32_0(4) * new Posit32_0(16)).ShouldBe(new Posit32_0(64));
			 (new Posit32_0(-3) * new Posit32_0(-4)).ShouldBe(new Posit32_0(12));
			
			 (new Posit32_0(127.5) * new Posit32_0(2)).ShouldBe(new Posit32_0(255));
			 (new Posit32_0(-16.625) * new Posit32_0(-4)).ShouldBe(new Posit32_0(66.5));		(new Posit32_0(100) * new Posit32_0(0.9)).ShouldBe(new Posit32_0(90));
			 (new Posit32_0(-0.95) * new Posit32_0(-10000)).ShouldBe(new Posit32_0(9500));
			 (new Posit32_0(-0.995) * new Posit32_0(100000)).ShouldBe(new Posit32_0(-99500));  					
		}	

		[Test]
		public void Posit32_0_DivisionIsCorrect()
		{
			 var posit1 = new Posit32_0(1);
			 (posit1 / new Posit32_0(0)).ShouldBe(new Posit32_0(Posit32_0.NaNBitMask, true));
			 (new Posit32_0(0.015625) / posit1).ShouldBe(new Posit32_0(0.015625));
			 (new Posit32_0(256) / posit1).ShouldBe(new Posit32_0(256));
			 (new Posit32_0(3) / -posit1).ShouldBe(new Posit32_0(-3));
			 (new Posit32_0(0.03125) / new Posit32_0(2)).ShouldBe(new Posit32_0(0.015625));
			 (new Posit32_0(64) / new Posit32_0(16)).ShouldBe(new Posit32_0(4));
			 (new Posit32_0(12) / new Posit32_0(-4)).ShouldBe(new Posit32_0(-3));
			
			 (new Posit32_0(252) / new Posit32_0(2)).ShouldBe(new Posit32_0(126));
			 (new Posit32_0(66.5) / new Posit32_0(-4)).ShouldBe(new Posit32_0(-16.625));
			 (new Posit32_0(90) / new Posit32_0(0.9)).ShouldBe(new Posit32_0(100));
			 (new Posit32_0(9200)  / new Posit32_0(-10000)).ShouldBe(new Posit32_0(-0.92));
			 (new Posit32_0(-80800) / new Posit32_0(1000)).ShouldBe(new Posit32_0(-80.80));  
		 }										
	}
}
