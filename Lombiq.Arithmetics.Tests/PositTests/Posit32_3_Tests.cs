using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit32_3Tests
	{
		[Test]
		public void Posit32_3_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_3(0).PositBits, 0);
			Assert.AreEqual((int)new Posit32_3(1), 1);
			Assert.AreEqual((int)new Posit32_3(-1), -1);
			Assert.AreEqual((int)new Posit32_3(3), 3);
			Assert.AreEqual((int)new Posit32_3(-3), -3);
			Assert.AreEqual((int)new Posit32_3(8), 8);
			Assert.AreEqual((int)new Posit32_3(-16), -16);
			Assert.AreEqual((int)new Posit32_3(1024), 1024);
			Assert.AreEqual((int)new Posit32_3(-1024), -1024);

						Assert.AreEqual((int)new Posit32_3(int.MaxValue), 2147483647);
			Assert.AreEqual((int)new Posit32_3(int.MinValue), -2147483648);
			Assert.AreEqual((int)new Posit32_3(100), 100);

									
		}

		[Test]
		public void Posit32_3_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_3((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit32_3((float)1.0), 1);
			Assert.AreEqual((float)new Posit32_3((float)2.0), 2);
			Assert.AreEqual((float)new Posit32_3((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit32_3((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit32_3((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit32_3((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit32_3((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit32_3((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit32_3((float)6), 6);
			Assert.AreEqual((float)new Posit32_3((float)-6), -6);
			Assert.AreEqual((float)new Posit32_3((float) 1.76684706477838E+72),float.NaN);
			Assert.AreEqual((float)new Posit32_3((float) -1.76684706477838E+72),float.NaN);
			   			
			}
		[Test]
		public void Posit32_3_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit32_3(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit32_3(1.0), 1.0);
			Assert.AreEqual((double)new Posit32_3(2.0), 2);
			Assert.AreEqual((double)new Posit32_3(0.5), 0.5);
			Assert.AreEqual((double)new Posit32_3(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit32_3(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit32_3(1.5), 1.5);
			Assert.AreEqual((double)new Posit32_3(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit32_3(-1.5), -1.5);
			Assert.AreEqual((double)new Posit32_3(6), 6);
			Assert.AreEqual((double)new Posit32_3(-6), -6);
			Assert.AreEqual((float)(double)new Posit32_3( 1.76684706477838E+72),(float)1.76684706477838E+72);
			Assert.AreEqual((float)(double)new Posit32_3( -1.76684706477838E+72),(float)-1.76684706477838E+72);
							
		}
		
		[Test]
		public void Posit32_3_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit32_3(1);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_3(1000));
		}

		[Test]
		public void Posit32_3_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit32_3(-500);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_3(499));
		}

		[Test]
		public void Posit32_3_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit32_3(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit32_3(0.03125));
			(posit1-posit2).ShouldBe(new Posit32_3(-0.015625));
			(new Posit32_3(1) - new Posit32_3(0.1)).ShouldBe(new Posit32_3(0.9));
			
						   (new Posit32_3(10.015625) - new Posit32_3(0.015625)).ShouldBe(new Posit32_3(10));
			   (new Posit32_3(127.5) + new Posit32_3(127.5)).ShouldBe(new Posit32_3(255));
			   (new Posit32_3(-16.625) + new Posit32_3(21.875)).ShouldBe(new Posit32_3(-16.625 + 21.875));
									   (new Posit32_3(0.00001) + new Posit32_3(100)).ShouldBe(new Posit32_3(100.00001));
			  					
		}	

		[Test]
		public void Posit32_3_MultiplicationIsCorrect()
		{
			var posit1 = new Posit32_3(1);
			 (posit1 * new Posit32_3(0.015625)).ShouldBe(new Posit32_3(0.015625));
			 (posit1 * new Posit32_3(256)).ShouldBe(new Posit32_3(256));
			 (-posit1 * new Posit32_3(3)).ShouldBe(new Posit32_3(-3));
			 (new Posit32_3(2) * new Posit32_3(0.015625)).ShouldBe(new Posit32_3(0.03125));
			 (new Posit32_3(4) * new Posit32_3(16)).ShouldBe(new Posit32_3(64));
			 (new Posit32_3(-3) * new Posit32_3(-4)).ShouldBe(new Posit32_3(12));
			
						   (new Posit32_3(127.5) * new Posit32_3(2)).ShouldBe(new Posit32_3(255));
			   (new Posit32_3(-16.625) * new Posit32_3(-4)).ShouldBe(new Posit32_3(66.5));
									   	(new Posit32_3(100) * new Posit32_3(0.9)).ShouldBe(new Posit32_3(90));
			   	(new Posit32_3(-0.95) * new Posit32_3(-10000)).ShouldBe(new Posit32_3(9500));
				(new Posit32_3(-0.995) * new Posit32_3(100000)).ShouldBe(new Posit32_3(-99500));
			  					
		}	
		[Test]
		public void Posit32_3_DivisionIsCorrect()
		{
			 var posit1 = new Posit32_3(1);
			 (new Posit32_3(0.015625) / posit1).ShouldBe(new Posit32_3(0.015625));
			 (new Posit32_3(256) / posit1).ShouldBe(new Posit32_3(256));
			 (new Posit32_3(3) / -posit1).ShouldBe(new Posit32_3(-3));
			 (new Posit32_3(0.03125) / new Posit32_3(2)).ShouldBe(new Posit32_3(0.015625));
			 (new Posit32_3(64) / new Posit32_3(16)).ShouldBe(new Posit32_3(4));
			 (new Posit32_3(12) / new Posit32_3(-4)).ShouldBe(new Posit32_3(-3));
			
						 (new Posit32_3(252) / new Posit32_3(2)).ShouldBe(new Posit32_3(126));
			 (new Posit32_3(66.5) / new Posit32_3(-4)).ShouldBe(new Posit32_3(-16.625));
									 (new Posit32_3(90) / new Posit32_3(0.9)).ShouldBe(new Posit32_3(100));
			 (new Posit32_3(9500)  / new Posit32_3(-10000)).ShouldBe(new Posit32_3(-0.95));
			 (new Posit32_3(-80900) / new Posit32_3(100000)).ShouldBe(new Posit32_3(-0.809));
		 	  
		 }										
	}
}
