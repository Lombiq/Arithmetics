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
			(posit1+posit1).ShouldBe(new Posit32_3(0.03125));
		}
		
	}
}
