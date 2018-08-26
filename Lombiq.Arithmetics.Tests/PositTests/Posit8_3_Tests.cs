using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit8_3Tests
	{
		[Test]
		public void Posit8_3_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit8_3(0).PositBits, 0);
			Assert.AreEqual((int)new Posit8_3(1), 1);
			Assert.AreEqual((int)new Posit8_3(-1), -1);
			Assert.AreEqual((int)new Posit8_3(3), 3);
			Assert.AreEqual((int)new Posit8_3(-3), -3);
			Assert.AreEqual((int)new Posit8_3(8), 8);
			Assert.AreEqual((int)new Posit8_3(-16), -16);
			Assert.AreEqual((int)new Posit8_3(1024), 1024);
			Assert.AreEqual((int)new Posit8_3(-1024), -1024);

									
		}

		[Test]
		public void Posit8_3_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit8_3((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit8_3((float)1.0), 1);
			Assert.AreEqual((float)new Posit8_3((float)2.0), 2);
			Assert.AreEqual((float)new Posit8_3((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit8_3((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit8_3((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit8_3((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit8_3((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit8_3((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit8_3((float)6), 6);
			Assert.AreEqual((float)new Posit8_3((float)-6), -6);
						Assert.AreEqual((float)new Posit8_3((float) 281474976710656),(float)281474976710656);
			Assert.AreEqual((float)new Posit8_3((float) -281474976710656),(float)-281474976710656);
							
			}
		[Test]
		public void Posit8_3_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit8_3(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit8_3(1.0), 1.0);
			Assert.AreEqual((double)new Posit8_3(2.0), 2);
			Assert.AreEqual((double)new Posit8_3(0.5), 0.5);
			Assert.AreEqual((double)new Posit8_3(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit8_3(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit8_3(1.5), 1.5);
			Assert.AreEqual((double)new Posit8_3(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit8_3(-1.5), -1.5);
			Assert.AreEqual((double)new Posit8_3(6), 6);
			Assert.AreEqual((double)new Posit8_3(-6), -6);
			Assert.AreEqual((float)(double)new Posit8_3( 281474976710656),(float)281474976710656);
			Assert.AreEqual((float)(double)new Posit8_3( -281474976710656),(float)-281474976710656);
							
		}
		
		[Test]
		public void Posit8_3_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit8_3(1);

			for (var i = 1; i < 8; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit8_3(8));
		}

		[Test]
		public void Posit8_3_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit8_3(-4);

			for (var i = 1; i < 8; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit8_3(3));
		}

		[Test]
		public void Posit8_3_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit8_3(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit8_3(0.03125));
			(posit1-posit2).ShouldBe(new Posit8_3(-0.015625));
			(new Posit8_3(1) - new Posit8_3(0.1)).ShouldBe(new Posit8_3(0.9));
			
						  					
		}	

		[Test]
		public void Posit8_3_MultiplicationIsCorrect()
		{
			var posit1 = new Posit8_3(1);
			 (posit1 * new Posit8_3(0.015625)).ShouldBe(new Posit8_3(0.015625));
			 (posit1 * new Posit8_3(256)).ShouldBe(new Posit8_3(256));
			 (-posit1 * new Posit8_3(3)).ShouldBe(new Posit8_3(-3));
			 (new Posit8_3(2) * new Posit8_3(0.015625)).ShouldBe(new Posit8_3(0.03125));
			 (new Posit8_3(4) * new Posit8_3(16)).ShouldBe(new Posit8_3(64));
			 (new Posit8_3(-3) * new Posit8_3(-4)).ShouldBe(new Posit8_3(12));
			
						  					
		}	
		[Test]
		public void Posit8_3_DivisionIsCorrect()
		{
			 var posit1 = new Posit8_3(1);
			 (new Posit8_3(0.015625) / posit1).ShouldBe(new Posit8_3(0.015625));
			 (new Posit8_3(256) / posit1).ShouldBe(new Posit8_3(256));
			 (new Posit8_3(3) / -posit1).ShouldBe(new Posit8_3(-3));
			 (new Posit8_3(0.03125) / new Posit8_3(2)).ShouldBe(new Posit8_3(0.015625));
			 (new Posit8_3(64) / new Posit8_3(16)).ShouldBe(new Posit8_3(4));
			 (new Posit8_3(12) / new Posit8_3(-4)).ShouldBe(new Posit8_3(-3));
			
						  
		 }										
	}
}
