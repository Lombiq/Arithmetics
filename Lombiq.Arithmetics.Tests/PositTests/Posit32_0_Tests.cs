using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit32_0Tests
	{
		[Test]
		public void Posit32_0_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_0(0).PositBits, 0);
			Assert.AreEqual((int)new Posit32_0(1), 1);
			Assert.AreEqual((int)new Posit32_0(-1), -1);
			Assert.AreEqual((int)new Posit32_0(3), 3);
			Assert.AreEqual((int)new Posit32_0(-3), -3);
			Assert.AreEqual((int)new Posit32_0(8), 8);
			Assert.AreEqual((int)new Posit32_0(-16), -16);
			Assert.AreEqual((int)new Posit32_0(1024), 1024);
			Assert.AreEqual((int)new Posit32_0(-1024), -1024);

						Assert.AreEqual((int)new Posit32_0(int.MaxValue), 1073741824);
			Assert.AreEqual((int)new Posit32_0(int.MinValue), -1073741824);
									
		}

		[Test]
		public void Posit32_0_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit32_0((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit32_0((float)1.0), 1);
			Assert.AreEqual((float)new Posit32_0((float)2.0), 2);
			Assert.AreEqual((float)new Posit32_0((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit32_0((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit32_0((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit32_0((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit32_0((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit32_0((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit32_0((float)6), 6);
			Assert.AreEqual((float)new Posit32_0((float)-6), -6);
						Assert.AreEqual((float)new Posit32_0((float) 1073741824),(float)1073741824);
			Assert.AreEqual((float)new Posit32_0((float) -1073741824),(float)-1073741824);
							
			}
		[Test]
		public void Posit32_0_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit32_0(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit32_0(1.0), 1.0);
			Assert.AreEqual((double)new Posit32_0(2.0), 2);
			Assert.AreEqual((double)new Posit32_0(0.5), 0.5);
			Assert.AreEqual((double)new Posit32_0(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit32_0(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit32_0(1.5), 1.5);
			Assert.AreEqual((double)new Posit32_0(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit32_0(-1.5), -1.5);
			Assert.AreEqual((double)new Posit32_0(6), 6);
			Assert.AreEqual((double)new Posit32_0(-6), -6);
			Assert.AreEqual((float)(double)new Posit32_0( 1073741824),(float)1073741824);
			Assert.AreEqual((float)(double)new Posit32_0( -1073741824),(float)-1073741824);
							
		}
		
		[Test]
		public void Posit32_0_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit32_0(1);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_0(1000));
		}

		[Test]
		public void Posit32_0_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit32_0(-500);

			for (var i = 1; i < 1000; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit32_0(499));
		}
		
	}
}
