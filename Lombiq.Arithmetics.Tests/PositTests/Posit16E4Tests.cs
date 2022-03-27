using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
    public class Posit16E4Tests
    {
        [Fact]
        public void Posit16E4_IntConversionIsCorrect()
        {
            Assert.AreEqual(new Posit16E4(0).PositBits, 0);
            Assert.AreEqual((int)new Posit16E4(1), 1);
            Assert.AreEqual((int)new Posit16E4(-1), -1);
            Assert.AreEqual((int)new Posit16E4(3), 3);
            Assert.AreEqual((int)new Posit16E4(-3), -3);
            Assert.AreEqual((int)new Posit16E4(8), 8);
            Assert.AreEqual((int)new Posit16E4(-16), -16);
            Assert.AreEqual((int)new Posit16E4(1024), 1024);
            
            Assert.AreEqual((int)new Posit16E4(-1024), -1024);

            Assert.AreEqual((int)new Posit16E4(int.MaxValue), 2147483647);
            Assert.AreEqual((int)new Posit16E4(int.MinValue), -2147483648);
            Assert.AreEqual((int)new Posit16E4(100), 100);						
        }

        [Fact]
        public void Posit16E4_FloatConversionIsCorrect()
        {
            Assert.AreEqual(new Posit16E4((float)0.0).PositBits, 0);
            
            Assert.AreEqual((float)new Posit16E4((float)1.0), 1);
            Assert.AreEqual((float)new Posit16E4((float)2.0), 2);
            Assert.AreEqual((float)new Posit16E4((float)0.5), 0.5);
            Assert.AreEqual((float)new Posit16E4((float)0.0625), 0.0625);

            Assert.AreEqual((float)new Posit16E4((float)0.09375), 0.09375);
            Assert.AreEqual((float)new Posit16E4((float)1.5), 1.5);
            Assert.AreEqual((float)new Posit16E4((float)-0.09375),- 0.09375);
            Assert.AreEqual((float)new Posit16E4((float)-1.5), -1.5);
            Assert.AreEqual((float)new Posit16E4((float)6), 6);
            Assert.AreEqual((float)new Posit16E4((float)-6), -6);
            Assert.AreEqual((float)new Posit16E4((float) 2.69599466671506E+67),float.NaN);
            Assert.AreEqual((float)new Posit16E4((float) -2.69599466671506E+67),float.NaN);
               			
            }

        [Fact]
        public void Posit16E4_DoubleConversionIsCorrect()
        {
            Assert.AreEqual((double)new Posit16E4(0.0).PositBits, 0);
            
            Assert.AreEqual(new Posit16E4(double.Epsilon), new Posit16E4(Posit16E4.MinPositiveValueBitMask, true));

            Assert.AreEqual((double)new Posit16E4(1.0), 1.0);
            Assert.AreEqual((double)new Posit16E4(2.0), 2);
            Assert.AreEqual((double)new Posit16E4(0.5), 0.5);
            Assert.AreEqual((double)new Posit16E4(0.0625), 0.0625);

            Assert.AreEqual((double)new Posit16E4(0.09375), 0.09375);
            Assert.AreEqual((double)new Posit16E4(1.5), 1.5);
            Assert.AreEqual((double)new Posit16E4(-0.09375),-0.09375);
            Assert.AreEqual((double)new Posit16E4(-1.5), -1.5);
            Assert.AreEqual((double)new Posit16E4(6), 6);
            Assert.AreEqual((double)new Posit16E4(-6), -6);
            Assert.AreEqual((float)(double)new Posit16E4( 2.69599466671506E+67),(float)2.69599466671506E+67);
            Assert.AreEqual((float)(double)new Posit16E4( -2.69599466671506E+67),(float)-2.69599466671506E+67);			
        }
        
        [Fact]
        public void Posit16E4_AdditionIsCorrectForPositives()
        {
            var posit1 = new Posit16E4(1);

            for (var i = 1; i < 512; i++)
            {
                posit1 += 1;
            }
            Assert.AreEqual(((uint)posit1), (uint)new Posit16E4(512));
        }

        [Fact]
        public void Posit16E4_AdditionIsCorrectForNegatives()
        {
            var posit1 = new Posit16E4(-256);

            for (var i = 1; i < 512; i++)
            {
                posit1 += 1;
            }
            Assert.AreEqual(((uint)posit1), (uint)new Posit16E4(255));
        }

        [Fact]
        public void Posit16E4_AdditionIsCorrectForReals()
        {
            var posit1 = new Posit16E4(0.015625);
            var posit2 = posit1 + posit1;
            Assert.AreEqual(posit2, new Posit16E4(0.03125));
            Assert.AreEqual((posit1-posit2), new Posit16E4(-0.015625));
            Assert.AreEqual((new Posit16E4(1) - new Posit16E4(0.1)), new Posit16E4(0.9));
            
            Assert.AreEqual((new Posit16E4(10.015625) - new Posit16E4(0.015625)), new Posit16E4(10));
            Assert.AreEqual((new Posit16E4(127.5) + new Posit16E4(127.5)), new Posit16E4(255));
            Assert.AreEqual((new Posit16E4(-16.625) + new Posit16E4(21.875)), new Posit16E4(-16.625 + 21.875));
              					
        }	

        [Fact]
        public void Posit16E4_MultiplicationIsCorrect()
        {
             var posit1 = new Posit16E4(1);
             Assert.AreEqual((posit1 * new Posit16E4(0.015625)), new Posit16E4(0.015625));
             Assert.AreEqual((posit1 * new Posit16E4(256)), new Posit16E4(256));
             Assert.AreEqual((-posit1 * new Posit16E4(3)), new Posit16E4(-3));
             Assert.AreEqual((new Posit16E4(2) * new Posit16E4(0.015625)), new Posit16E4(0.03125));
             Assert.AreEqual((new Posit16E4(4) * new Posit16E4(16)), new Posit16E4(64));
             Assert.AreEqual((new Posit16E4(-3) * new Posit16E4(-4)), new Posit16E4(12));
            
        	 Assert.AreEqual((new Posit16E4(127.5) * new Posit16E4(2)), new Posit16E4(255));
             Assert.AreEqual((new Posit16E4(-16.625) * new Posit16E4(-4)), new Posit16E4(66.5));          					
        }	

        [Fact]
        public void Posit16E4_DivisionIsCorrect()
        {
             var posit1 = new Posit16E4(1);
             Assert.AreEqual((posit1 / new Posit16E4(0)), new Posit16E4(Posit16E4.NaNBitMask, true));
             Assert.AreEqual((new Posit16E4(0.015625) / posit1), new Posit16E4(0.015625));
             Assert.AreEqual((new Posit16E4(256) / posit1), new Posit16E4(256));
             Assert.AreEqual((new Posit16E4(3) / -posit1), new Posit16E4(-3));
             Assert.AreEqual((new Posit16E4(0.03125) / new Posit16E4(2)), new Posit16E4(0.015625));
             Assert.AreEqual((new Posit16E4(64) / new Posit16E4(16)), new Posit16E4(4));
             Assert.AreEqual((new Posit16E4(12) / new Posit16E4(-4)), new Posit16E4(-3));
            
             Assert.AreEqual((new Posit16E4(252) / new Posit16E4(2)), new Posit16E4(126));
             Assert.AreEqual((new Posit16E4(66.5) / new Posit16E4(-4)), new Posit16E4(-16.625));
               
         }	

        [Fact]
        public void Posit16E4_SqrtIsCorrect()
        {
             var posit1 = new Posit16E4(1);
             Assert.AreEqual(Posit16E4.Sqrt(posit1), posit1);
             Assert.AreEqual(Posit16E4.Sqrt(-posit1), new Posit16E4(Posit16E4.NaNBitMask, true));
     
             Assert.AreEqual((Posit16E4.Sqrt(new Posit16E4(4))), new Posit16E4(2));
             Assert.AreEqual((Posit16E4.Sqrt(new Posit16E4(64))), new Posit16E4(8));
             Assert.AreEqual((Posit16E4.Sqrt(new Posit16E4(0.25))), new Posit16E4(0.5));
             
             Assert.AreEqual((Posit16E4.Sqrt(new Posit16E4(100))), new Posit16E4(10));
             Assert.AreEqual((Posit16E4.Sqrt(new Posit16E4(144))), new Posit16E4(12));
             Assert.AreEqual((Posit16E4.Sqrt(new Posit16E4(896))), new Posit16E4(29.9332590942));
                         
                     }
        
        [Fact]
        public void Posit16E4_FusedSumIsCorrect()
        {
            //System.Console.WriteLine("Posit16E4 " +  Posit16E4.QuireSize + " fs: "+  Posit16E4.QuireFractionSize);
            var positArray = new Posit16E4[257];
            positArray[0] = new Posit16E4(-64);
            for(var i=1; i <= 256; i++) positArray[i] = new Posit16E4(0.5);          
            
            Assert.AreEqual(Posit16E4.FusedSum(positArray).PositBits, new Posit16E4(64).PositBits);

            positArray[2] = new Posit16E4(Posit16E4.NaNBitMask, true);
            Assert.AreEqual(Posit16E4.FusedSum(positArray).PositBits, positArray[2].PositBits);

            var positArray2 = new Posit16E4[1281];
            positArray2[0] = new Posit16E4(0);
            for(var i=1; i <= 1280; i++) positArray2[i] = new Posit16E4(0.1);
            Assert.AreEqual(Posit16E4.FusedSum(positArray2).PositBits, new Posit16E4(128).PositBits);
        }

        [Fact]
        public void Posit16E4_FusedDotProductIsCorrect()
        {
            var positArray1 = new Posit16E4[3];
            var positArray2 = new Posit16E4[3];
            positArray1[0] = new Posit16E4(1);
            positArray1[1] = new Posit16E4(2);
            positArray1[2] = new Posit16E4(3);

            positArray2[0] = new Posit16E4(1);
            positArray2[1] = new Posit16E4(2);
            positArray2[2] = new Posit16E4(4);
            Assert.AreEqual(Posit16E4.FusedDotProduct(positArray1, positArray2).PositBits, new Posit16E4(17).PositBits);

            var positArray3 = new Posit16E4[3];
            positArray3[0] = new Posit16E4(-1);
            positArray3[1] = new Posit16E4(2);
            positArray3[2] = new Posit16E4(-100);
            Assert.AreEqual(Posit16E4.FusedDotProduct(positArray1, positArray3), new Posit16E4(-297));

             var positArray4 = new Posit16E4[3];
            positArray4[0] = new Posit16E4(-1);
            positArray4[1] = new Posit16E4(2);
            positArray4[2] = new Posit16E4(Posit16E4.MaxValueBitMask, true);
            Assert.AreEqual(Posit16E4.FusedDotProduct(positArray1, positArray4), new Posit16E4(Posit16E4.MaxValueBitMask, true));
        }

        [Fact]
        public void Posit16E4_ConversionToOtherEnvsIsCorrect()
        {

            Assert.AreEqual((Posit8E0)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit8E0( Posit8E0.NaNBitMask, true));	
            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit8E0 : " + 0.01*i*i*i*1 + " i: " + i );
                Assert.AreEqual(((Posit8E0)new Posit16E4(0.01*i*i*i*1 )), new Posit8E0((double) new Posit16E4(0.01*i*i*i*1)), "Converting value " + (0.01*i*i*i*1) + "to Posit8E0 has failed. ");	
            }

            for(var i = 0; i < 800; i++){
                //System.Console.WriteLine("Posit8E0 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit8E0)new Posit16E4(1.0-(1.0/(i+1))), new Posit8E0((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8E0 has failed. ");	
            }
    
            Assert.AreEqual((Posit8E1)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit8E1( Posit8E1.NaNBitMask, true));	
            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit8E1 : " + 0.01*i*i*i*2 + " i: " + i );
                Assert.AreEqual(((Posit8E1)new Posit16E4(0.01*i*i*i*2 )), new Posit8E1((double) new Posit16E4(0.01*i*i*i*2)), "Converting value " + (0.01*i*i*i*2) + "to Posit8E1 has failed. ");	
            }

            for(var i = 0; i < 800; i++){
                //System.Console.WriteLine("Posit8E1 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit8E1)new Posit16E4(1.0-(1.0/(i+1))), new Posit8E1((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8E1 has failed. ");	
            }
    
            Assert.AreEqual((Posit8E2)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit8E2( Posit8E2.NaNBitMask, true));	
            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit8E2 : " + 0.01*i*i*i*3 + " i: " + i );
                Assert.AreEqual(((Posit8E2)new Posit16E4(0.01*i*i*i*3 )), new Posit8E2((double) new Posit16E4(0.01*i*i*i*3)), "Converting value " + (0.01*i*i*i*3) + "to Posit8E2 has failed. ");	
            }

            for(var i = 0; i < 800; i++){
                //System.Console.WriteLine("Posit8E2 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit8E2)new Posit16E4(1.0-(1.0/(i+1))), new Posit8E2((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8E2 has failed. ");	
            }
    
            Assert.AreEqual((Posit8E3)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit8E3( Posit8E3.NaNBitMask, true));	
            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit8E3 : " + 0.01*i*i*i*4 + " i: " + i );
                Assert.AreEqual(((Posit8E3)new Posit16E4(0.01*i*i*i*4 )), new Posit8E3((double) new Posit16E4(0.01*i*i*i*4)), "Converting value " + (0.01*i*i*i*4) + "to Posit8E3 has failed. ");	
            }

            for(var i = 0; i < 800; i++){
                //System.Console.WriteLine("Posit8E3 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit8E3)new Posit16E4(1.0-(1.0/(i+1))), new Posit8E3((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8E3 has failed. ");	
            }
    
            Assert.AreEqual((Posit8E4)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit8E4( Posit8E4.NaNBitMask, true));	
            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit8E4 : " + 0.01*i*i*i*5 + " i: " + i );
                Assert.AreEqual(((Posit8E4)new Posit16E4(0.01*i*i*i*5 )), new Posit8E4((double) new Posit16E4(0.01*i*i*i*5)), "Converting value " + (0.01*i*i*i*5) + "to Posit8E4 has failed. ");	
            }

            for(var i = 0; i < 800; i++){
                //System.Console.WriteLine("Posit8E4 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit8E4)new Posit16E4(1.0-(1.0/(i+1))), new Posit8E4((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8E4 has failed. ");	
            }
    
            Assert.AreEqual((Posit16E0)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit16E0( Posit16E0.NaNBitMask, true));	
            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit16E0 : " + 0.01*i*i*i*1 + " i: " + i );
                Assert.AreEqual(((Posit16E0)new Posit16E4(0.01*i*i*i*1 )), new Posit16E0((double) new Posit16E4(0.01*i*i*i*1)), "Converting value " + (0.01*i*i*i*1) + "to Posit16E0 has failed. ");	
            }

            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit16E0 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit16E0)new Posit16E4(1.0-(1.0/(i+1))), new Posit16E0((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16E0 has failed. ");	
            }
    
            Assert.AreEqual((Posit16E1)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit16E1( Posit16E1.NaNBitMask, true));	
            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit16E1 : " + 0.01*i*i*i*2 + " i: " + i );
                Assert.AreEqual(((Posit16E1)new Posit16E4(0.01*i*i*i*2 )), new Posit16E1((double) new Posit16E4(0.01*i*i*i*2)), "Converting value " + (0.01*i*i*i*2) + "to Posit16E1 has failed. ");	
            }

            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit16E1 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit16E1)new Posit16E4(1.0-(1.0/(i+1))), new Posit16E1((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16E1 has failed. ");	
            }
    
            Assert.AreEqual((Posit16E2)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit16E2( Posit16E2.NaNBitMask, true));	
            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit16E2 : " + 0.01*i*i*i*3 + " i: " + i );
                Assert.AreEqual(((Posit16E2)new Posit16E4(0.01*i*i*i*3 )), new Posit16E2((double) new Posit16E4(0.01*i*i*i*3)), "Converting value " + (0.01*i*i*i*3) + "to Posit16E2 has failed. ");	
            }

            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit16E2 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit16E2)new Posit16E4(1.0-(1.0/(i+1))), new Posit16E2((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16E2 has failed. ");	
            }
    
            Assert.AreEqual((Posit16E3)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit16E3( Posit16E3.NaNBitMask, true));	
            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit16E3 : " + 0.01*i*i*i*4 + " i: " + i );
                Assert.AreEqual(((Posit16E3)new Posit16E4(0.01*i*i*i*4 )), new Posit16E3((double) new Posit16E4(0.01*i*i*i*4)), "Converting value " + (0.01*i*i*i*4) + "to Posit16E3 has failed. ");	
            }

            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit16E3 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit16E3)new Posit16E4(1.0-(1.0/(i+1))), new Posit16E3((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16E3 has failed. ");	
            }
    
            Assert.AreEqual((Posit32E0)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit32E0( Posit32E0.NaNBitMask, true));	
            for(var i = 0; i < 6400; i++){
                //System.Console.WriteLine("Posit32E0 : " + 0.01*i*i*i*1 + " i: " + i );
                Assert.AreEqual(((Posit32E0)new Posit16E4(0.01*i*i*i*1 )), new Posit32E0((double) new Posit16E4(0.01*i*i*i*1)), "Converting value " + (0.01*i*i*i*1) + "to Posit32E0 has failed. ");	
            }

            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit32E0 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit32E0)new Posit16E4(1.0-(1.0/(i+1))), new Posit32E0((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32E0 has failed. ");	
            }
    
            Assert.AreEqual((Posit32E1)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit32E1( Posit32E1.NaNBitMask, true));	
            for(var i = 0; i < 6400; i++){
                //System.Console.WriteLine("Posit32E1 : " + 0.01*i*i*i*2 + " i: " + i );
                Assert.AreEqual(((Posit32E1)new Posit16E4(0.01*i*i*i*2 )), new Posit32E1((double) new Posit16E4(0.01*i*i*i*2)), "Converting value " + (0.01*i*i*i*2) + "to Posit32E1 has failed. ");	
            }

            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit32E1 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit32E1)new Posit16E4(1.0-(1.0/(i+1))), new Posit32E1((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32E1 has failed. ");	
            }
    
            Assert.AreEqual((Posit32E2)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit32E2( Posit32E2.NaNBitMask, true));	
            for(var i = 0; i < 6400; i++){
                //System.Console.WriteLine("Posit32E2 : " + 0.01*i*i*i*3 + " i: " + i );
                Assert.AreEqual(((Posit32E2)new Posit16E4(0.01*i*i*i*3 )), new Posit32E2((double) new Posit16E4(0.01*i*i*i*3)), "Converting value " + (0.01*i*i*i*3) + "to Posit32E2 has failed. ");	
            }

            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit32E2 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit32E2)new Posit16E4(1.0-(1.0/(i+1))), new Posit32E2((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32E2 has failed. ");	
            }
    
            Assert.AreEqual((Posit32E3)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit32E3( Posit32E3.NaNBitMask, true));	
            for(var i = 0; i < 6400; i++){
                //System.Console.WriteLine("Posit32E3 : " + 0.01*i*i*i*4 + " i: " + i );
                Assert.AreEqual(((Posit32E3)new Posit16E4(0.01*i*i*i*4 )), new Posit32E3((double) new Posit16E4(0.01*i*i*i*4)), "Converting value " + (0.01*i*i*i*4) + "to Posit32E3 has failed. ");	
            }

            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit32E3 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit32E3)new Posit16E4(1.0-(1.0/(i+1))), new Posit32E3((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32E3 has failed. ");	
            }
    
            Assert.AreEqual((Posit32E4)new Posit16E4(Posit16E4.NaNBitMask, true), new Posit32E4( Posit32E4.NaNBitMask, true));	
            for(var i = 0; i < 6400; i++){
                //System.Console.WriteLine("Posit32E4 : " + 0.01*i*i*i*5 + " i: " + i );
                Assert.AreEqual(((Posit32E4)new Posit16E4(0.01*i*i*i*5 )), new Posit32E4((double) new Posit16E4(0.01*i*i*i*5)), "Converting value " + (0.01*i*i*i*5) + "to Posit32E4 has failed. ");	
            }

            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit32E4 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit32E4)new Posit16E4(1.0-(1.0/(i+1))), new Posit32E4((double) new Posit16E4( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32E4 has failed. ");	
            }
            }
    }
}
