using NUnit.Framework;
using Shouldly;
using System.Diagnostics;
using System.Globalization;

namespace Lombiq.Arithmetics.Tests
{
	[TestFixture]
	class Posit16_1Tests
	{
		[Test]
		public void Posit16_1_IntConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_1(0).PositBits, 0);
			Assert.AreEqual((int)new Posit16_1(1), 1);
			Assert.AreEqual((int)new Posit16_1(-1), -1);
			Assert.AreEqual((int)new Posit16_1(3), 3);
			Assert.AreEqual((int)new Posit16_1(-3), -3);
			Assert.AreEqual((int)new Posit16_1(8), 8);
			Assert.AreEqual((int)new Posit16_1(-16), -16);
			Assert.AreEqual((int)new Posit16_1(1024), 1024);
			Assert.AreEqual((int)new Posit16_1(-1024), -1024);

						Assert.AreEqual((int)new Posit16_1(int.MaxValue), 268435456);
			Assert.AreEqual((int)new Posit16_1(int.MinValue), -268435456);
									
		}

		[Test]
		public void Posit16_1_FloatConversionIsCorrect()
		{
			Assert.AreEqual(new Posit16_1((float)0.0).PositBits, 0);
			
			Assert.AreEqual((float)new Posit16_1((float)1.0), 1);
			Assert.AreEqual((float)new Posit16_1((float)2.0), 2);
			Assert.AreEqual((float)new Posit16_1((float)0.5), 0.5);
			Assert.AreEqual((float)new Posit16_1((float)0.0625), 0.0625);

			Assert.AreEqual((float)new Posit16_1((float)0.09375), 0.09375);
			Assert.AreEqual((float)new Posit16_1((float)1.5), 1.5);
			Assert.AreEqual((float)new Posit16_1((float)-0.09375),- 0.09375);
			Assert.AreEqual((float)new Posit16_1((float)-1.5), -1.5);
			Assert.AreEqual((float)new Posit16_1((float)6), 6);
			Assert.AreEqual((float)new Posit16_1((float)-6), -6);
						Assert.AreEqual((float)new Posit16_1((float) 268435456),(float)268435456);
			Assert.AreEqual((float)new Posit16_1((float) -268435456),(float)-268435456);
							
			}
		[Test]
		public void Posit16_1_DoubleConversionIsCorrect()
		{
			Assert.AreEqual((double)new Posit16_1(0.0).PositBits, 0);
			
			Assert.AreEqual((double)new Posit16_1(1.0), 1.0);
			Assert.AreEqual((double)new Posit16_1(2.0), 2);
			Assert.AreEqual((double)new Posit16_1(0.5), 0.5);
			Assert.AreEqual((double)new Posit16_1(0.0625), 0.0625);

			Assert.AreEqual((double)new Posit16_1(0.09375), 0.09375);
			Assert.AreEqual((double)new Posit16_1(1.5), 1.5);
			Assert.AreEqual((double)new Posit16_1(-0.09375),-0.09375);
			Assert.AreEqual((double)new Posit16_1(-1.5), -1.5);
			Assert.AreEqual((double)new Posit16_1(6), 6);
			Assert.AreEqual((double)new Posit16_1(-6), -6);
			Assert.AreEqual((float)(double)new Posit16_1( 268435456),(float)268435456);
			Assert.AreEqual((float)(double)new Posit16_1( -268435456),(float)-268435456);
							
		}
		
		[Test]
		public void Posit16_1_AdditionIsCorrectForPositives()
		{
			var posit1 = new Posit16_1(1);

			for (var i = 1; i < 256; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_1(256));
		}

		[Test]
		public void Posit16_1_AdditionIsCorrectForNegatives()
		{
			var posit1 = new Posit16_1(-128);

			for (var i = 1; i < 256; i++)
			{
				posit1 += 1;
			}
			((uint)posit1).ShouldBe((uint)new Posit16_1(127));
		}

		[Test]
		public void Posit16_1_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit16_1(0.015625);
			(posit1+posit1).ShouldBe(new Posit16_1(0.03125));
		}
		
	}
}
