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
		
	}
}

