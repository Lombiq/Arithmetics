using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit16_3Tests
	{
		[Test]
		public void Posit16_3_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_3(0).PositBits, 0);
			Assert.AreEqual((int)new Posit16_3(1), 1);
			Assert.AreEqual((int)new Posit16_3(-1), -1);
			Assert.AreEqual((int)new Posit16_3(3), 3);
			Assert.AreEqual((int)new Posit16_3(-3), -3);
			Assert.AreEqual((int)new Posit16_3(8), 8);
			Assert.AreEqual((int)new Posit16_3(-16), -16);
			Assert.AreEqual((int)new Posit16_3(1024), 1024);
			Assert.AreEqual((int)new Posit16_3(-1024), -1024);

						Assert.AreEqual((int)new Posit16_3(int.MaxValue), 2147483647);
			Assert.AreEqual((int)new Posit16_3(int.MinValue), -2147483648);
			Assert.AreEqual((int)new Posit16_3(100), 100);

									
		}

		[Test]
		public void Posit16_3_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_3((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit16_3((float)1.0), 1);
			Assert.AreEqual((float)new Posit16_3((float)2.0), 2);
			Assert.AreEqual((float)new Posit16_3((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit16_3((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit16_3((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit16_3((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit16_3((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit16_3((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit16_3((float)6), 6);
			Assert.AreEqual((float)new Posit16_3((float)-6), -6);
						Assert.AreEqual((float)new Posit16_3((float) 5.19229685853483E+33),(float)5.19229685853483E+33);
			Assert.AreEqual((float)new Posit16_3((float) -5.19229685853483E+33),(float)-5.19229685853483E+33);
							
			}
		[Test]
		public void Posit16_3_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit16_3(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit16_3(1.0), 1.0);
			Assert.AreEqual((double)new Posit16_3(2.0), 2);
			Assert.AreEqual((double)new Posit16_3(0.5), 0.5);
			Assert.AreEqual((double)new Posit16_3(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit16_3(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit16_3(1.5), 1.5);
			Assert.AreEqual((double)new Posit16_3(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit16_3(-1.5), -1.5);
			Assert.AreEqual((double)new Posit16_3(6), 6);
			Assert.AreEqual((double)new Posit16_3(-6), -6);
			Assert.AreEqual((float)(double)new Posit16_3( 5.19229685853483E+33),(float)5.19229685853483E+33);
			Assert.AreEqual((float)(double)new Posit16_3( -5.19229685853483E+33),(float)-5.19229685853483E+33);
							
		}
		
		[Test]
		public void Posit16_3_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit16_3(1);

			for (var i = 1; i < 512; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_3(512));
		}

		[Test]
		public void Posit16_3_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit16_3(-256);

			for (var i = 1; i < 512; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_3(255));
		}

		[Test]
		public void Posit16_3_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit16_3(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit16_3(0.03125));
			(posit1-posit2).ShouldBe(new Posit16_3(-0.015625));
			(new Posit16_3(1) - new Posit16_3(0.1)).ShouldBe(new Posit16_3(0.9));
			
						   (new Posit16_3(10.015625) - new Posit16_3(0.015625)).ShouldBe(new Posit16_3(10));
			   (new Posit16_3(127.5) + new Posit16_3(127.5)).ShouldBe(new Posit16_3(255));
			   (new Posit16_3(-16.625) + new Posit16_3(21.875)).ShouldBe(new Posit16_3(-16.625 + 21.875));
						  					
		}	

		[Test]
		public void Posit16_3_MultiplicationIsCorrect()
		{
			var posit1 = new Posit16_3(1);
			 (posit1 * new Posit16_3(0.015625)).ShouldBe(new Posit16_3(0.015625));
			 (posit1 * new Posit16_3(256)).ShouldBe(new Posit16_3(256));
			 (-posit1 * new Posit16_3(3)).ShouldBe(new Posit16_3(-3));
			 (new Posit16_3(2) * new Posit16_3(0.015625)).ShouldBe(new Posit16_3(0.03125));
			 (new Posit16_3(4) * new Posit16_3(16)).ShouldBe(new Posit16_3(64));
			 (new Posit16_3(-3) * new Posit16_3(-4)).ShouldBe(new Posit16_3(12));
			
						   (new Posit16_3(127.5) * new Posit16_3(2)).ShouldBe(new Posit16_3(255));
			   (new Posit16_3(-16.625) * new Posit16_3(-4)).ShouldBe(new Posit16_3(66.5));
						  					
		}	
		[Test]
		public void Posit16_3_DivisionIsCorrect()
		{
			 var posit1 = new Posit16_3(1);
			 (new Posit16_3(0.015625) / posit1).ShouldBe(new Posit16_3(0.015625));
			 (new Posit16_3(256) / posit1).ShouldBe(new Posit16_3(256));
			 (new Posit16_3(3) / -posit1).ShouldBe(new Posit16_3(-3));
			 (new Posit16_3(0.03125) / new Posit16_3(2)).ShouldBe(new Posit16_3(0.015625));
			 (new Posit16_3(64) / new Posit16_3(16)).ShouldBe(new Posit16_3(4));
			 (new Posit16_3(12) / new Posit16_3(-4)).ShouldBe(new Posit16_3(-3));
			
						 (new Posit16_3(252) / new Posit16_3(2)).ShouldBe(new Posit16_3(126));
			 (new Posit16_3(66.5) / new Posit16_3(-4)).ShouldBe(new Posit16_3(-16.625));
						  
		 }										
	}
}
