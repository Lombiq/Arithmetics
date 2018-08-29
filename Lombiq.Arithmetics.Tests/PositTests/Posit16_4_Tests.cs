using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit16_4Tests
	{
		[Test]
		public void Posit16_4_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_4(0).PositBits, 0);
			Assert.AreEqual((int)new Posit16_4(1), 1);
			Assert.AreEqual((int)new Posit16_4(-1), -1);
			Assert.AreEqual((int)new Posit16_4(3), 3);
			Assert.AreEqual((int)new Posit16_4(-3), -3);
			Assert.AreEqual((int)new Posit16_4(8), 8);
			Assert.AreEqual((int)new Posit16_4(-16), -16);
			Assert.AreEqual((int)new Posit16_4(1024), 1024);
			Assert.AreEqual((int)new Posit16_4(-1024), -1024);

			Assert.AreEqual((int)new Posit16_4(int.MaxValue), 2147483647);
			Assert.AreEqual((int)new Posit16_4(int.MinValue), -2147483648);
			Assert.AreEqual((int)new Posit16_4(100), 100);						
		}

		[Test]
		public void Posit16_4_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_4((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit16_4((float)1.0), 1);
			Assert.AreEqual((float)new Posit16_4((float)2.0), 2);
			Assert.AreEqual((float)new Posit16_4((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit16_4((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit16_4((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit16_4((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit16_4((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit16_4((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit16_4((float)6), 6);
			Assert.AreEqual((float)new Posit16_4((float)-6), -6);
			Assert.AreEqual((float)new Posit16_4((float) 2.69599466671506E+67),float.NaN);
			Assert.AreEqual((float)new Posit16_4((float) -2.69599466671506E+67),float.NaN);
			   			
			}

		[Test]
		public void Posit16_4_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit16_4(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit16_4(1.0), 1.0);
			Assert.AreEqual((double)new Posit16_4(2.0), 2);
			Assert.AreEqual((double)new Posit16_4(0.5), 0.5);
			Assert.AreEqual((double)new Posit16_4(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit16_4(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit16_4(1.5), 1.5);
			Assert.AreEqual((double)new Posit16_4(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit16_4(-1.5), -1.5);
			Assert.AreEqual((double)new Posit16_4(6), 6);
			Assert.AreEqual((double)new Posit16_4(-6), -6);
			Assert.AreEqual((float)(double)new Posit16_4( 2.69599466671506E+67),(float)2.69599466671506E+67);
			Assert.AreEqual((float)(double)new Posit16_4( -2.69599466671506E+67),(float)-2.69599466671506E+67);			
		}
		
		[Test]
		public void Posit16_4_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit16_4(1);

			for (var i = 1; i < 512; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_4(512));
		}

		[Test]
		public void Posit16_4_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit16_4(-256);

			for (var i = 1; i < 512; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_4(255));
		}

		[Test]
		public void Posit16_4_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit16_4(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit16_4(0.03125));
			(posit1-posit2).ShouldBe(new Posit16_4(-0.015625));
			(new Posit16_4(1) - new Posit16_4(0.1)).ShouldBe(new Posit16_4(0.9));
			
			(new Posit16_4(10.015625) - new Posit16_4(0.015625)).ShouldBe(new Posit16_4(10));
		    (new Posit16_4(127.5) + new Posit16_4(127.5)).ShouldBe(new Posit16_4(255));
			(new Posit16_4(-16.625) + new Posit16_4(21.875)).ShouldBe(new Posit16_4(-16.625 + 21.875));
			  					
		}	

		[Test]
		public void Posit16_4_MultiplicationIsCorrect()
		{
			 var posit1 = new Posit16_4(1);
			 (posit1 * new Posit16_4(0.015625)).ShouldBe(new Posit16_4(0.015625));
			 (posit1 * new Posit16_4(256)).ShouldBe(new Posit16_4(256));
			 (-posit1 * new Posit16_4(3)).ShouldBe(new Posit16_4(-3));
			 (new Posit16_4(2) * new Posit16_4(0.015625)).ShouldBe(new Posit16_4(0.03125));
			 (new Posit16_4(4) * new Posit16_4(16)).ShouldBe(new Posit16_4(64));
			 (new Posit16_4(-3) * new Posit16_4(-4)).ShouldBe(new Posit16_4(12));
			
			 (new Posit16_4(127.5) * new Posit16_4(2)).ShouldBe(new Posit16_4(255));
			 (new Posit16_4(-16.625) * new Posit16_4(-4)).ShouldBe(new Posit16_4(66.5));		  					
		}	

		[Test]
		public void Posit16_4_DivisionIsCorrect()
		{
			 var posit1 = new Posit16_4(1);
			 (posit1 / new Posit16_4(0)).ShouldBe(new Posit16_4(Posit16_4.NaNBitMask, true));
			 (new Posit16_4(0.015625) / posit1).ShouldBe(new Posit16_4(0.015625));
			 (new Posit16_4(256) / posit1).ShouldBe(new Posit16_4(256));
			 (new Posit16_4(3) / -posit1).ShouldBe(new Posit16_4(-3));
			 (new Posit16_4(0.03125) / new Posit16_4(2)).ShouldBe(new Posit16_4(0.015625));
			 (new Posit16_4(64) / new Posit16_4(16)).ShouldBe(new Posit16_4(4));
			 (new Posit16_4(12) / new Posit16_4(-4)).ShouldBe(new Posit16_4(-3));
			
			 (new Posit16_4(252) / new Posit16_4(2)).ShouldBe(new Posit16_4(126));
			 (new Posit16_4(66.5) / new Posit16_4(-4)).ShouldBe(new Posit16_4(-16.625));
			   
		 }	

		[Test]
		public void Posit16_4_SqrtIsCorrect()
		{
			 var posit1 = new Posit16_4(1);
			 Posit16_4.Sqrt(posit1).ShouldBe(posit1);
			 Posit16_4.Sqrt(-posit1).ShouldBe(new Posit16_4(Posit16_4.NaNBitMask, true));
	 
			 (Posit16_4.Sqrt(new Posit16_4(4))).ShouldBe(new Posit16_4(2));
			 (Posit16_4.Sqrt(new Posit16_4(64))).ShouldBe(new Posit16_4(8));
			 (Posit16_4.Sqrt(new Posit16_4(0.25))).ShouldBe(new Posit16_4(0.5));
			 
			 (Posit16_4.Sqrt(new Posit16_4(100))).ShouldBe(new Posit16_4(10));
			 (Posit16_4.Sqrt(new Posit16_4(144))).ShouldBe(new Posit16_4(12));
			 (Posit16_4.Sqrt(new Posit16_4(896))).ShouldBe(new Posit16_4(29.9332590942));
						 
			 
		}
	}
}
