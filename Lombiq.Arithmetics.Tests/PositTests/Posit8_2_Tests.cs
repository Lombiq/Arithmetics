using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit8_2Tests
	{
		[Test]
		public void Posit8_2_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit8_2(0).PositBits, 0);
			Assert.AreEqual((int)new Posit8_2(1), 1);
			Assert.AreEqual((int)new Posit8_2(-1), -1);
			Assert.AreEqual((int)new Posit8_2(3), 3);
			Assert.AreEqual((int)new Posit8_2(-3), -3);
			Assert.AreEqual((int)new Posit8_2(8), 8);
			Assert.AreEqual((int)new Posit8_2(-16), -16);
			Assert.AreEqual((int)new Posit8_2(1024), 1024);
			Assert.AreEqual((int)new Posit8_2(-1024), -1024);

									
		}

		[Test]
		public void Posit8_2_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit8_2((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit8_2((float)1.0), 1);
			Assert.AreEqual((float)new Posit8_2((float)2.0), 2);
			Assert.AreEqual((float)new Posit8_2((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit8_2((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit8_2((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit8_2((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit8_2((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit8_2((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit8_2((float)6), 6);
			Assert.AreEqual((float)new Posit8_2((float)-6), -6);
						Assert.AreEqual((float)new Posit8_2((float) 16777216),(float)16777216);
			Assert.AreEqual((float)new Posit8_2((float) -16777216),(float)-16777216);
							
			}
		[Test]
		public void Posit8_2_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit8_2(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit8_2(1.0), 1.0);
			Assert.AreEqual((double)new Posit8_2(2.0), 2);
			Assert.AreEqual((double)new Posit8_2(0.5), 0.5);
			Assert.AreEqual((double)new Posit8_2(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit8_2(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit8_2(1.5), 1.5);
			Assert.AreEqual((double)new Posit8_2(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit8_2(-1.5), -1.5);
			Assert.AreEqual((double)new Posit8_2(6), 6);
			Assert.AreEqual((double)new Posit8_2(-6), -6);
			Assert.AreEqual((float)(double)new Posit8_2( 16777216),(float)16777216);
			Assert.AreEqual((float)(double)new Posit8_2( -16777216),(float)-16777216);
							
		}
		
		[Test]
		public void Posit8_2_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit8_2(1);

			for (var i = 1; i < 8; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit8_2(8));
		}

		[Test]
		public void Posit8_2_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit8_2(-4);

			for (var i = 1; i < 8; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit8_2(3));
		}

		[Test]
		public void Posit8_2_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit8_2(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit8_2(0.03125));
			(posit1-posit2).ShouldBe(new Posit8_2(-0.015625));
			(new Posit8_2(1) - new Posit8_2(0.1)).ShouldBe(new Posit8_2(0.9));
			
						  					
		}	

		[Test]
		public void Posit8_2_MultiplicationIsCorrect()
		{
			var posit1 = new Posit8_2(1);
			 (posit1 * new Posit8_2(0.015625)).ShouldBe(new Posit8_2(0.015625));
			 (posit1 * new Posit8_2(256)).ShouldBe(new Posit8_2(256));
			 (-posit1 * new Posit8_2(3)).ShouldBe(new Posit8_2(-3));
			 (new Posit8_2(2) * new Posit8_2(0.015625)).ShouldBe(new Posit8_2(0.03125));
			 (new Posit8_2(4) * new Posit8_2(16)).ShouldBe(new Posit8_2(64));
			 (new Posit8_2(-3) * new Posit8_2(-4)).ShouldBe(new Posit8_2(12));
			
						  					
		}	
		[Test]
		public void Posit8_2_DivisionIsCorrect()
		{
			 var posit1 = new Posit8_2(1);
			 (new Posit8_2(0.015625) / posit1).ShouldBe(new Posit8_2(0.015625));
			 (new Posit8_2(256) / posit1).ShouldBe(new Posit8_2(256));
			 (new Posit8_2(3) / -posit1).ShouldBe(new Posit8_2(-3));
			 (new Posit8_2(0.03125) / new Posit8_2(2)).ShouldBe(new Posit8_2(0.015625));
			 (new Posit8_2(64) / new Posit8_2(16)).ShouldBe(new Posit8_2(4));
			 (new Posit8_2(12) / new Posit8_2(-4)).ShouldBe(new Posit8_2(-3));
			
						  
		 }										
	}
}
