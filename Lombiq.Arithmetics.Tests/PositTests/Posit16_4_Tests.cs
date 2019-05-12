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
			Assert.AreEqual((int)new Posit16_4(100), 100);						
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
			
			Assert.AreEqual(new Posit16_4(double.Epsilon), new Posit16_4(Posit16_4.MinPositiveValueBitMask, true));

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

		[Test]
		public void Posit16_4_AdditionIsCorrectForReals()
		{
			var posit1 = new Posit16_4(0.015625);
			var posit2 = posit1 + posit1;
			posit2.ShouldBe(new Posit16_4(0.03125));
			(posit1-posit2).ShouldBe(new Posit16_4(-0.015625));
			(new Posit16_4(1) - new Posit16_4(0.1)).ShouldBe(new Posit16_4(0.9));
			
			(new Posit16_4(10.015625) - new Posit16_4(0.015625)).ShouldBe(new Posit16_4(10));
			(new Posit16_4(127.5) + new Posit16_4(127.5)).ShouldBe(new Posit16_4(255));
			(new Posit16_4(-16.625) + new Posit16_4(21.875)).ShouldBe(new Posit16_4(-16.625 + 21.875));
			  					
		}	

		[Test]
		public void Posit16_4_MultiplicationIsCorrect()
		{
			 var posit1 = new Posit16_4(1);
			 (posit1 * new Posit16_4(0.015625)).ShouldBe(new Posit16_4(0.015625));
			 (posit1 * new Posit16_4(256)).ShouldBe(new Posit16_4(256));
			 (-posit1 * new Posit16_4(3)).ShouldBe(new Posit16_4(-3));
			 (new Posit16_4(2) * new Posit16_4(0.015625)).ShouldBe(new Posit16_4(0.03125));
			 (new Posit16_4(4) * new Posit16_4(16)).ShouldBe(new Posit16_4(64));
			 (new Posit16_4(-3) * new Posit16_4(-4)).ShouldBe(new Posit16_4(12));
			
			 (new Posit16_4(127.5) * new Posit16_4(2)).ShouldBe(new Posit16_4(255));
			 (new Posit16_4(-16.625) * new Posit16_4(-4)).ShouldBe(new Posit16_4(66.5));		  					
		}	

		[Test]
		public void Posit16_4_DivisionIsCorrect()
		{
			 var posit1 = new Posit16_4(1);
			 (posit1 / new Posit16_4(0)).ShouldBe(new Posit16_4(Posit16_4.NaNBitMask, true));
			 (new Posit16_4(0.015625) / posit1).ShouldBe(new Posit16_4(0.015625));
			 (new Posit16_4(256) / posit1).ShouldBe(new Posit16_4(256));
			 (new Posit16_4(3) / -posit1).ShouldBe(new Posit16_4(-3));
			 (new Posit16_4(0.03125) / new Posit16_4(2)).ShouldBe(new Posit16_4(0.015625));
			 (new Posit16_4(64) / new Posit16_4(16)).ShouldBe(new Posit16_4(4));
			 (new Posit16_4(12) / new Posit16_4(-4)).ShouldBe(new Posit16_4(-3));
			
			 (new Posit16_4(252) / new Posit16_4(2)).ShouldBe(new Posit16_4(126));
			 (new Posit16_4(66.5) / new Posit16_4(-4)).ShouldBe(new Posit16_4(-16.625));
			   
		 }	

		[Test]
		public void Posit16_4_SqrtIsCorrect()
		{
			 var posit1 = new Posit16_4(1);
			 Posit16_4.Sqrt(posit1).ShouldBe(posit1);
			 Posit16_4.Sqrt(-posit1).ShouldBe(new Posit16_4(Posit16_4.NaNBitMask, true));
	 
			 (Posit16_4.Sqrt(new Posit16_4(4))).ShouldBe(new Posit16_4(2));
			 (Posit16_4.Sqrt(new Posit16_4(64))).ShouldBe(new Posit16_4(8));
			 (Posit16_4.Sqrt(new Posit16_4(0.25))).ShouldBe(new Posit16_4(0.5));
			 
			 (Posit16_4.Sqrt(new Posit16_4(100))).ShouldBe(new Posit16_4(10));
			 (Posit16_4.Sqrt(new Posit16_4(144))).ShouldBe(new Posit16_4(12));
			 (Posit16_4.Sqrt(new Posit16_4(896))).ShouldBe(new Posit16_4(29.9332590942));
						 
			 		}
		
		[Test]
		public void Posit16_4_FusedSumIsCorrect()
		{
			//System.Console.WriteLine("Posit16_4 " +  Posit16_4.QuireSize + " fs: "+  Posit16_4.QuireFractionSize);
			var positArray = new Posit16_4[257];
			positArray[0] = new Posit16_4(-64);
			for(var i=1; i <= 256; i++) positArray[i] = new Posit16_4(0.5);          
			
			Assert.AreEqual(Posit16_4.FusedSum(positArray).PositBits, new Posit16_4(64).PositBits);

			positArray[2] = new Posit16_4(Posit16_4.NaNBitMask, true);
			Assert.AreEqual(Posit16_4.FusedSum(positArray).PositBits, positArray[2].PositBits);

			var positArray2 = new Posit16_4[1281];
			positArray2[0] = new Posit16_4(0);
			for(var i=1; i <= 1280; i++) positArray2[i] = new Posit16_4(0.1);
			Assert.AreEqual(Posit16_4.FusedSum(positArray2).PositBits, new Posit16_4(128).PositBits);
		}

		[Test]
		public void Posit16_4_FusedDotProductIsCorrect()
		{
			var positArray1 = new Posit16_4[3];
			var positArray2 = new Posit16_4[3];
			positArray1[0] = new Posit16_4(1);
			positArray1[1] = new Posit16_4(2);
			positArray1[2] = new Posit16_4(3);

			positArray2[0] = new Posit16_4(1);
			positArray2[1] = new Posit16_4(2);
			positArray2[2] = new Posit16_4(4);
			Assert.AreEqual(Posit16_4.FusedDotProduct(positArray1, positArray2).PositBits, new Posit16_4(17).PositBits);

			var positArray3 = new Posit16_4[3];
			positArray3[0] = new Posit16_4(-1);
			positArray3[1] = new Posit16_4(2);
			positArray3[2] = new Posit16_4(-100);
			Assert.AreEqual(Posit16_4.FusedDotProduct(positArray1, positArray3), new Posit16_4(-297));

			 var positArray4 = new Posit16_4[3];
			positArray4[0] = new Posit16_4(-1);
			positArray4[1] = new Posit16_4(2);
			positArray4[2] = new Posit16_4(Posit16_4.MaxValueBitMask, true);
			Assert.AreEqual(Posit16_4.FusedDotProduct(positArray1, positArray4), new Posit16_4(Posit16_4.MaxValueBitMask, true));
		}

		[Test]
		public void Posit16_4_ConversionToOtherEnvsIsCorrect()
		{

			Assert.AreEqual((Posit8_0)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit8_0( Posit8_0.NaNBitMask, true));	
			for(var i = 0; i < 1600; i++){
				//System.Console.WriteLine("Posit8_0 : " + 0.01*i*i*i*1 + " i: " + i );
				Assert.AreEqual(((Posit8_0)new Posit16_4(0.01*i*i*i*1 )), new Posit8_0((double) new Posit16_4(0.01*i*i*i*1)), "Converting value " + (0.01*i*i*i*1) + "to Posit8_0 has failed. ");	
			}

			for(var i = 0; i < 800; i++){
				//System.Console.WriteLine("Posit8_0 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit8_0)new Posit16_4(1.0-(1.0/(i+1))), new Posit8_0((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8_0 has failed. ");	
			}
	
			Assert.AreEqual((Posit8_1)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit8_1( Posit8_1.NaNBitMask, true));	
			for(var i = 0; i < 1600; i++){
				//System.Console.WriteLine("Posit8_1 : " + 0.01*i*i*i*2 + " i: " + i );
				Assert.AreEqual(((Posit8_1)new Posit16_4(0.01*i*i*i*2 )), new Posit8_1((double) new Posit16_4(0.01*i*i*i*2)), "Converting value " + (0.01*i*i*i*2) + "to Posit8_1 has failed. ");	
			}

			for(var i = 0; i < 800; i++){
				//System.Console.WriteLine("Posit8_1 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit8_1)new Posit16_4(1.0-(1.0/(i+1))), new Posit8_1((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8_1 has failed. ");	
			}
	
			Assert.AreEqual((Posit8_2)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit8_2( Posit8_2.NaNBitMask, true));	
			for(var i = 0; i < 1600; i++){
				//System.Console.WriteLine("Posit8_2 : " + 0.01*i*i*i*3 + " i: " + i );
				Assert.AreEqual(((Posit8_2)new Posit16_4(0.01*i*i*i*3 )), new Posit8_2((double) new Posit16_4(0.01*i*i*i*3)), "Converting value " + (0.01*i*i*i*3) + "to Posit8_2 has failed. ");	
			}

			for(var i = 0; i < 800; i++){
				//System.Console.WriteLine("Posit8_2 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit8_2)new Posit16_4(1.0-(1.0/(i+1))), new Posit8_2((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8_2 has failed. ");	
			}
	
			Assert.AreEqual((Posit8_3)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit8_3( Posit8_3.NaNBitMask, true));	
			for(var i = 0; i < 1600; i++){
				//System.Console.WriteLine("Posit8_3 : " + 0.01*i*i*i*4 + " i: " + i );
				Assert.AreEqual(((Posit8_3)new Posit16_4(0.01*i*i*i*4 )), new Posit8_3((double) new Posit16_4(0.01*i*i*i*4)), "Converting value " + (0.01*i*i*i*4) + "to Posit8_3 has failed. ");	
			}

			for(var i = 0; i < 800; i++){
				//System.Console.WriteLine("Posit8_3 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit8_3)new Posit16_4(1.0-(1.0/(i+1))), new Posit8_3((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8_3 has failed. ");	
			}
	
			Assert.AreEqual((Posit8_4)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit8_4( Posit8_4.NaNBitMask, true));	
			for(var i = 0; i < 1600; i++){
				//System.Console.WriteLine("Posit8_4 : " + 0.01*i*i*i*5 + " i: " + i );
				Assert.AreEqual(((Posit8_4)new Posit16_4(0.01*i*i*i*5 )), new Posit8_4((double) new Posit16_4(0.01*i*i*i*5)), "Converting value " + (0.01*i*i*i*5) + "to Posit8_4 has failed. ");	
			}

			for(var i = 0; i < 800; i++){
				//System.Console.WriteLine("Posit8_4 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit8_4)new Posit16_4(1.0-(1.0/(i+1))), new Posit8_4((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8_4 has failed. ");	
			}
	
			Assert.AreEqual((Posit16_0)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit16_0( Posit16_0.NaNBitMask, true));	
			for(var i = 0; i < 3200; i++){
				//System.Console.WriteLine("Posit16_0 : " + 0.01*i*i*i*1 + " i: " + i );
				Assert.AreEqual(((Posit16_0)new Posit16_4(0.01*i*i*i*1 )), new Posit16_0((double) new Posit16_4(0.01*i*i*i*1)), "Converting value " + (0.01*i*i*i*1) + "to Posit16_0 has failed. ");	
			}

			for(var i = 0; i < 1600; i++){
				//System.Console.WriteLine("Posit16_0 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit16_0)new Posit16_4(1.0-(1.0/(i+1))), new Posit16_0((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16_0 has failed. ");	
			}
	
			Assert.AreEqual((Posit16_1)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit16_1( Posit16_1.NaNBitMask, true));	
			for(var i = 0; i < 3200; i++){
				//System.Console.WriteLine("Posit16_1 : " + 0.01*i*i*i*2 + " i: " + i );
				Assert.AreEqual(((Posit16_1)new Posit16_4(0.01*i*i*i*2 )), new Posit16_1((double) new Posit16_4(0.01*i*i*i*2)), "Converting value " + (0.01*i*i*i*2) + "to Posit16_1 has failed. ");	
			}

			for(var i = 0; i < 1600; i++){
				//System.Console.WriteLine("Posit16_1 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit16_1)new Posit16_4(1.0-(1.0/(i+1))), new Posit16_1((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16_1 has failed. ");	
			}
	
			Assert.AreEqual((Posit16_2)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit16_2( Posit16_2.NaNBitMask, true));	
			for(var i = 0; i < 3200; i++){
				//System.Console.WriteLine("Posit16_2 : " + 0.01*i*i*i*3 + " i: " + i );
				Assert.AreEqual(((Posit16_2)new Posit16_4(0.01*i*i*i*3 )), new Posit16_2((double) new Posit16_4(0.01*i*i*i*3)), "Converting value " + (0.01*i*i*i*3) + "to Posit16_2 has failed. ");	
			}

			for(var i = 0; i < 1600; i++){
				//System.Console.WriteLine("Posit16_2 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit16_2)new Posit16_4(1.0-(1.0/(i+1))), new Posit16_2((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16_2 has failed. ");	
			}
	
			Assert.AreEqual((Posit16_3)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit16_3( Posit16_3.NaNBitMask, true));	
			for(var i = 0; i < 3200; i++){
				//System.Console.WriteLine("Posit16_3 : " + 0.01*i*i*i*4 + " i: " + i );
				Assert.AreEqual(((Posit16_3)new Posit16_4(0.01*i*i*i*4 )), new Posit16_3((double) new Posit16_4(0.01*i*i*i*4)), "Converting value " + (0.01*i*i*i*4) + "to Posit16_3 has failed. ");	
			}

			for(var i = 0; i < 1600; i++){
				//System.Console.WriteLine("Posit16_3 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit16_3)new Posit16_4(1.0-(1.0/(i+1))), new Posit16_3((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16_3 has failed. ");	
			}
	
			Assert.AreEqual((Posit32_0)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit32_0( Posit32_0.NaNBitMask, true));	
			for(var i = 0; i < 6400; i++){
				//System.Console.WriteLine("Posit32_0 : " + 0.01*i*i*i*1 + " i: " + i );
				Assert.AreEqual(((Posit32_0)new Posit16_4(0.01*i*i*i*1 )), new Posit32_0((double) new Posit16_4(0.01*i*i*i*1)), "Converting value " + (0.01*i*i*i*1) + "to Posit32_0 has failed. ");	
			}

			for(var i = 0; i < 3200; i++){
				//System.Console.WriteLine("Posit32_0 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit32_0)new Posit16_4(1.0-(1.0/(i+1))), new Posit32_0((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32_0 has failed. ");	
			}
	
			Assert.AreEqual((Posit32_1)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit32_1( Posit32_1.NaNBitMask, true));	
			for(var i = 0; i < 6400; i++){
				//System.Console.WriteLine("Posit32_1 : " + 0.01*i*i*i*2 + " i: " + i );
				Assert.AreEqual(((Posit32_1)new Posit16_4(0.01*i*i*i*2 )), new Posit32_1((double) new Posit16_4(0.01*i*i*i*2)), "Converting value " + (0.01*i*i*i*2) + "to Posit32_1 has failed. ");	
			}

			for(var i = 0; i < 3200; i++){
				//System.Console.WriteLine("Posit32_1 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit32_1)new Posit16_4(1.0-(1.0/(i+1))), new Posit32_1((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32_1 has failed. ");	
			}
	
			Assert.AreEqual((Posit32_2)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit32_2( Posit32_2.NaNBitMask, true));	
			for(var i = 0; i < 6400; i++){
				//System.Console.WriteLine("Posit32_2 : " + 0.01*i*i*i*3 + " i: " + i );
				Assert.AreEqual(((Posit32_2)new Posit16_4(0.01*i*i*i*3 )), new Posit32_2((double) new Posit16_4(0.01*i*i*i*3)), "Converting value " + (0.01*i*i*i*3) + "to Posit32_2 has failed. ");	
			}

			for(var i = 0; i < 3200; i++){
				//System.Console.WriteLine("Posit32_2 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit32_2)new Posit16_4(1.0-(1.0/(i+1))), new Posit32_2((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32_2 has failed. ");	
			}
	
			Assert.AreEqual((Posit32_3)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit32_3( Posit32_3.NaNBitMask, true));	
			for(var i = 0; i < 6400; i++){
				//System.Console.WriteLine("Posit32_3 : " + 0.01*i*i*i*4 + " i: " + i );
				Assert.AreEqual(((Posit32_3)new Posit16_4(0.01*i*i*i*4 )), new Posit32_3((double) new Posit16_4(0.01*i*i*i*4)), "Converting value " + (0.01*i*i*i*4) + "to Posit32_3 has failed. ");	
			}

			for(var i = 0; i < 3200; i++){
				//System.Console.WriteLine("Posit32_3 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit32_3)new Posit16_4(1.0-(1.0/(i+1))), new Posit32_3((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32_3 has failed. ");	
			}
	
			Assert.AreEqual((Posit32_4)new Posit16_4(Posit16_4.NaNBitMask, true), new Posit32_4( Posit32_4.NaNBitMask, true));	
			for(var i = 0; i < 6400; i++){
				//System.Console.WriteLine("Posit32_4 : " + 0.01*i*i*i*5 + " i: " + i );
				Assert.AreEqual(((Posit32_4)new Posit16_4(0.01*i*i*i*5 )), new Posit32_4((double) new Posit16_4(0.01*i*i*i*5)), "Converting value " + (0.01*i*i*i*5) + "to Posit32_4 has failed. ");	
			}

			for(var i = 0; i < 3200; i++){
				//System.Console.WriteLine("Posit32_4 : " + (1.0-(1.0/(i+1))) + " i: " + i );
				Assert.AreEqual((Posit32_4)new Posit16_4(1.0-(1.0/(i+1))), new Posit32_4((double) new Posit16_4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32_4 has failed. ");	
			}
			}
	}
}
