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
			(posit1+posit1).ShouldBe(new Posit32_2(0.03125));
		}
		
	}
}
