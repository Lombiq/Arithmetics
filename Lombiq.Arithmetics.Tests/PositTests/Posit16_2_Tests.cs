using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit16_2Tests
	{
		[Test]
		public void Posit16_2_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_2(0).PositBits, 0);
			Assert.AreEqual((int)new Posit16_2(1), 1);
			Assert.AreEqual((int)new Posit16_2(-1), -1);
			Assert.AreEqual((int)new Posit16_2(3), 3);
			Assert.AreEqual((int)new Posit16_2(-3), -3);
			Assert.AreEqual((int)new Posit16_2(8), 8);
			Assert.AreEqual((int)new Posit16_2(-16), -16);
			Assert.AreEqual((int)new Posit16_2(1024), 1024);
			Assert.AreEqual((int)new Posit16_2(-1024), -1024);

						Assert.AreEqual((int)new Posit16_2(int.MaxValue), 2147483647);
			Assert.AreEqual((int)new Posit16_2(int.MinValue), -2147483648);
									
		}

		[Test]
		public void Posit16_2_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_2((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit16_2((float)1.0), 1);
			Assert.AreEqual((float)new Posit16_2((float)2.0), 2);
			Assert.AreEqual((float)new Posit16_2((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit16_2((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit16_2((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit16_2((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit16_2((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit16_2((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit16_2((float)6), 6);
			Assert.AreEqual((float)new Posit16_2((float)-6), -6);
						Assert.AreEqual((float)new Posit16_2((float) 7.20575940379279E+16),(float)7.20575940379279E+16);
			Assert.AreEqual((float)new Posit16_2((float) -7.20575940379279E+16),(float)-7.20575940379279E+16);
							
			}
		[Test]
		public void Posit16_2_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit16_2(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit16_2(1.0), 1.0);
			Assert.AreEqual((double)new Posit16_2(2.0), 2);
			Assert.AreEqual((double)new Posit16_2(0.5), 0.5);
			Assert.AreEqual((double)new Posit16_2(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit16_2(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit16_2(1.5), 1.5);
			Assert.AreEqual((double)new Posit16_2(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit16_2(-1.5), -1.5);
			Assert.AreEqual((double)new Posit16_2(6), 6);
			Assert.AreEqual((double)new Posit16_2(-6), -6);
			Assert.AreEqual((float)(double)new Posit16_2( 7.20575940379279E+16),(float)7.20575940379279E+16);
			Assert.AreEqual((float)(double)new Posit16_2( -7.20575940379279E+16),(float)-7.20575940379279E+16);
							
		}
		
		[Test]
		public void Posit16_2_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit16_2(1);

			for (var i = 1; i < 512; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_2(512));
		}

		[Test]
		public void Posit16_2_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit16_2(-256);

			for (var i = 1; i < 512; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_2(255));
		}
		
	}
}
