using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit16_0Tests
	{
		[Test]
		public void Posit16_0_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_0(0).PositBits, 0);
			Assert.AreEqual((int)new Posit16_0(1), 1);
			Assert.AreEqual((int)new Posit16_0(-1), -1);
			Assert.AreEqual((int)new Posit16_0(3), 3);
			Assert.AreEqual((int)new Posit16_0(-3), -3);
			Assert.AreEqual((int)new Posit16_0(8), 8);
			Assert.AreEqual((int)new Posit16_0(-16), -16);
			Assert.AreEqual((int)new Posit16_0(1024), 1024);
			Assert.AreEqual((int)new Posit16_0(-1024), -1024);

						Assert.AreEqual((int)new Posit16_0(int.MaxValue), 16384);
			Assert.AreEqual((int)new Posit16_0(int.MinValue), -16384);
									
		}

		[Test]
		public void Posit16_0_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_0((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit16_0((float)1.0), 1);
			Assert.AreEqual((float)new Posit16_0((float)2.0), 2);
			Assert.AreEqual((float)new Posit16_0((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit16_0((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit16_0((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit16_0((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit16_0((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit16_0((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit16_0((float)6), 6);
			Assert.AreEqual((float)new Posit16_0((float)-6), -6);
						Assert.AreEqual((float)new Posit16_0((float) 16384),(float)16384);
			Assert.AreEqual((float)new Posit16_0((float) -16384),(float)-16384);
							
			}
		[Test]
		public void Posit16_0_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit16_0(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit16_0(1.0), 1.0);
			Assert.AreEqual((double)new Posit16_0(2.0), 2);
			Assert.AreEqual((double)new Posit16_0(0.5), 0.5);
			Assert.AreEqual((double)new Posit16_0(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit16_0(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit16_0(1.5), 1.5);
			Assert.AreEqual((double)new Posit16_0(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit16_0(-1.5), -1.5);
			Assert.AreEqual((double)new Posit16_0(6), 6);
			Assert.AreEqual((double)new Posit16_0(-6), -6);
			Assert.AreEqual((float)(double)new Posit16_0( 16384),(float)16384);
			Assert.AreEqual((float)(double)new Posit16_0( -16384),(float)-16384);
							
		}
		
		[Test]
		public void Posit16_0_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit16_0(1);

			for (var i = 1; i < 128; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_0(128));
		}

		[Test]
		public void Posit16_0_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit16_0(-64);

			for (var i = 1; i < 128; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_0(63));
		}
		
	}
}
