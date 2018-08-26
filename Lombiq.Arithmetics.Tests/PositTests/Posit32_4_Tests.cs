using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit32_4Tests
	{
		[Test]
		public void Posit32_4_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_4(0).PositBits, 0);
			Assert.AreEqual((int)new Posit32_4(1), 1);
			Assert.AreEqual((int)new Posit32_4(-1), -1);
			Assert.AreEqual((int)new Posit32_4(3), 3);
			Assert.AreEqual((int)new Posit32_4(-3), -3);
			Assert.AreEqual((int)new Posit32_4(8), 8);
			Assert.AreEqual((int)new Posit32_4(-16), -16);
			Assert.AreEqual((int)new Posit32_4(1024), 1024);
			Assert.AreEqual((int)new Posit32_4(-1024), -1024);

						Assert.AreEqual((int)new Posit32_4(int.MaxValue), 2147483647);
			Assert.AreEqual((int)new Posit32_4(int.MinValue), -2147483648);
			Assert.AreEqual((int)new Posit32_4(100), 100);

									
		}

		[Test]
		public void Posit32_4_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_4((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit32_4((float)1.0), 1);
			Assert.AreEqual((float)new Posit32_4((float)2.0), 2);
			Assert.AreEqual((float)new Posit32_4((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit32_4((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit32_4((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit32_4((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit32_4((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit32_4((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit32_4((float)6), 6);
			Assert.AreEqual((float)new Posit32_4((float)-6), -6);
			Assert.AreEqual((float)new Posit32_4((float) 3.12174855031599E+144),float.NaN);
			Assert.AreEqual((float)new Posit32_4((float) -3.12174855031599E+144),float.NaN);
			   			
			}
		[Test]
		public void Posit32_4_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit32_4(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit32_4(1.0), 1.0);
			Assert.AreEqual((double)new Posit32_4(2.0), 2);
			Assert.AreEqual((double)new Posit32_4(0.5), 0.5);
			Assert.AreEqual((double)new Posit32_4(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit32_4(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit32_4(1.5), 1.5);
			Assert.AreEqual((double)new Posit32_4(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit32_4(-1.5), -1.5);
			Assert.AreEqual((double)new Posit32_4(6), 6);
			Assert.AreEqual((double)new Posit32_4(-6), -6);
			Assert.AreEqual((float)(double)new Posit32_4( 3.12174855031599E+144),(float)3.12174855031599E+144);
			Assert.AreEqual((float)(double)new Posit32_4( -3.12174855031599E+144),(float)-3.12174855031599E+144);
							
		}
		
		[Test]
		public void Posit32_4_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit32_4(1);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_4(1000));
		}

		[Test]
		public void Posit32_4_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit32_4(-500);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_4(499));
		}

		[Test]
		public void Posit32_4_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit32_4(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit32_4(0.03125));
			(posit1-posit2).ShouldBe(new Posit32_4(-0.015625));
			(new Posit32_4(1) - new Posit32_4(0.1)).ShouldBe(new Posit32_4(0.9));
			
						   (new Posit32_4(10.015625) - new Posit32_4(0.015625)).ShouldBe(new Posit32_4(10));
			   (new Posit32_4(127.5) + new Posit32_4(127.5)).ShouldBe(new Posit32_4(255));
			   (new Posit32_4(-16.625) + new Posit32_4(21.875)).ShouldBe(new Posit32_4(-16.625 + 21.875));
									   (new Posit32_4(0.00001) + new Posit32_4(100)).ShouldBe(new Posit32_4(100.00001));
			  					
		}	

		[Test]
		public void Posit32_4_MultiplicationIsCorrect()
		{
			var posit1 = new Posit32_4(1);
			 (posit1 * new Posit32_4(0.015625)).ShouldBe(new Posit32_4(0.015625));
			 (posit1 * new Posit32_4(256)).ShouldBe(new Posit32_4(256));
			 (-posit1 * new Posit32_4(3)).ShouldBe(new Posit32_4(-3));
			 (new Posit32_4(2) * new Posit32_4(0.015625)).ShouldBe(new Posit32_4(0.03125));
			 (new Posit32_4(4) * new Posit32_4(16)).ShouldBe(new Posit32_4(64));
			 (new Posit32_4(-3) * new Posit32_4(-4)).ShouldBe(new Posit32_4(12));
			
						   (new Posit32_4(127.5) * new Posit32_4(2)).ShouldBe(new Posit32_4(255));
			   (new Posit32_4(-16.625) * new Posit32_4(-4)).ShouldBe(new Posit32_4(66.5));
									   	(new Posit32_4(100) * new Posit32_4(0.9)).ShouldBe(new Posit32_4(90));
			   	(new Posit32_4(-0.95) * new Posit32_4(-10000)).ShouldBe(new Posit32_4(9500));
				(new Posit32_4(-0.995) * new Posit32_4(100000)).ShouldBe(new Posit32_4(-99500));
			  					
		}	
		[Test]
		public void Posit32_4_DivisionIsCorrect()
		{
			 var posit1 = new Posit32_4(1);
			 (new Posit32_4(0.015625) / posit1).ShouldBe(new Posit32_4(0.015625));
			 (new Posit32_4(256) / posit1).ShouldBe(new Posit32_4(256));
			 (new Posit32_4(3) / -posit1).ShouldBe(new Posit32_4(-3));
			 (new Posit32_4(0.03125) / new Posit32_4(2)).ShouldBe(new Posit32_4(0.015625));
			 (new Posit32_4(64) / new Posit32_4(16)).ShouldBe(new Posit32_4(4));
			 (new Posit32_4(12) / new Posit32_4(-4)).ShouldBe(new Posit32_4(-3));
			
						 (new Posit32_4(252) / new Posit32_4(2)).ShouldBe(new Posit32_4(126));
			 (new Posit32_4(66.5) / new Posit32_4(-4)).ShouldBe(new Posit32_4(-16.625));
									 (new Posit32_4(90) / new Posit32_4(0.9)).ShouldBe(new Posit32_4(100));
			 (new Posit32_4(9500)  / new Posit32_4(-10000)).ShouldBe(new Posit32_4(-0.95));
			 (new Posit32_4(-80900) / new Posit32_4(100000)).ShouldBe(new Posit32_4(-0.809));
		 	  
		 }										
	}
}

