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
		
	}
}
