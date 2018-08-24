using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit32_1Tests
	{
		[Test]
		public void Posit32_1_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_1(0).PositBits, 0);
			Assert.AreEqual((int)new Posit32_1(1), 1);
			Assert.AreEqual((int)new Posit32_1(-1), -1);
			Assert.AreEqual((int)new Posit32_1(3), 3);
			Assert.AreEqual((int)new Posit32_1(-3), -3);
			Assert.AreEqual((int)new Posit32_1(8), 8);
			Assert.AreEqual((int)new Posit32_1(-16), -16);
			Assert.AreEqual((int)new Posit32_1(1024), 1024);
			Assert.AreEqual((int)new Posit32_1(-1024), -1024);

						Assert.AreEqual((int)new Posit32_1(int.MaxValue), 2147483647);
			Assert.AreEqual((int)new Posit32_1(int.MinValue), -2147483648);
									
		}

		[Test]
		public void Posit32_1_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_1((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit32_1((float)1.0), 1);
			Assert.AreEqual((float)new Posit32_1((float)2.0), 2);
			Assert.AreEqual((float)new Posit32_1((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit32_1((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit32_1((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit32_1((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit32_1((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit32_1((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit32_1((float)6), 6);
			Assert.AreEqual((float)new Posit32_1((float)-6), -6);
						Assert.AreEqual((float)new Posit32_1((float) 1.15292150460685E+18),(float)1.15292150460685E+18);
			Assert.AreEqual((float)new Posit32_1((float) -1.15292150460685E+18),(float)-1.15292150460685E+18);
							
			}
		[Test]
		public void Posit32_1_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit32_1(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit32_1(1.0), 1.0);
			Assert.AreEqual((double)new Posit32_1(2.0), 2);
			Assert.AreEqual((double)new Posit32_1(0.5), 0.5);
			Assert.AreEqual((double)new Posit32_1(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit32_1(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit32_1(1.5), 1.5);
			Assert.AreEqual((double)new Posit32_1(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit32_1(-1.5), -1.5);
			Assert.AreEqual((double)new Posit32_1(6), 6);
			Assert.AreEqual((double)new Posit32_1(-6), -6);
			Assert.AreEqual((float)(double)new Posit32_1( 1.15292150460685E+18),(float)1.15292150460685E+18);
			Assert.AreEqual((float)(double)new Posit32_1( -1.15292150460685E+18),(float)-1.15292150460685E+18);
							
		}
		
		[Test]
		public void Posit32_1_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit32_1(1);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_1(1000));
		}

		[Test]
		public void Posit32_1_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit32_1(-500);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_1(499));
		}

		[Test]
		public void Posit32_1_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit32_1(0.015625);
			(posit1+posit1).ShouldBe(new Posit32_1(0.03125));
		}
		
	}
}
