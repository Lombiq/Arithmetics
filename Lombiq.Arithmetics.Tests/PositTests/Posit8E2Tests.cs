using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{
    public class Posit8E2Tests
    {
        [Fact]
        public void Posit8E2_IntConversionIsCorrect()
        {
            Assert.AreEqual(new Posit8E2(0).PositBits, 0);
            Assert.AreEqual((int)new Posit8E2(1), 1);
            Assert.AreEqual((int)new Posit8E2(-1), -1);
            Assert.AreEqual((int)new Posit8E2(3), 3);
            Assert.AreEqual((int)new Posit8E2(-3), -3);
            Assert.AreEqual((int)new Posit8E2(8), 8);
            Assert.AreEqual((int)new Posit8E2(-16), -16);
            Assert.AreEqual((int)new Posit8E2(1024), 1024);
                        Assert.AreEqual((ulong)new Posit8E2((ulong)16777215), 16777216U);
            
            Assert.AreEqual((int)new Posit8E2(-1024), -1024);

            						
        }

        [Fact]
        public void Posit8E2_FloatConversionIsCorrect()
        {
            Assert.AreEqual(new Posit8E2((float)0.0).PositBits, 0);
            
            Assert.AreEqual((float)new Posit8E2((float)1.0), 1);
            Assert.AreEqual((float)new Posit8E2((float)2.0), 2);
            Assert.AreEqual((float)new Posit8E2((float)0.5), 0.5);
            Assert.AreEqual((float)new Posit8E2((float)0.0625), 0.0625);

            Assert.AreEqual((float)new Posit8E2((float)0.09375), 0.09375);
            Assert.AreEqual((float)new Posit8E2((float)1.5), 1.5);
            Assert.AreEqual((float)new Posit8E2((float)-0.09375),- 0.09375);
            Assert.AreEqual((float)new Posit8E2((float)-1.5), -1.5);
            Assert.AreEqual((float)new Posit8E2((float)6), 6);
            Assert.AreEqual((float)new Posit8E2((float)-6), -6);
            Assert.AreEqual((float)new Posit8E2((float) 16777216),(float)16777216);
            Assert.AreEqual((float)new Posit8E2((float) -16777216),(float)-16777216);			
            }

        [Fact]
        public void Posit8E2_DoubleConversionIsCorrect()
        {
            Assert.AreEqual((double)new Posit8E2(0.0).PositBits, 0);
            
            Assert.AreEqual(new Posit8E2(double.Epsilon), new Posit8E2(Posit8E2.MinPositiveValueBitMask, true));

            Assert.AreEqual((double)new Posit8E2(1.0), 1.0);
            Assert.AreEqual((double)new Posit8E2(2.0), 2);
            Assert.AreEqual((double)new Posit8E2(0.5), 0.5);
            Assert.AreEqual((double)new Posit8E2(0.0625), 0.0625);

            Assert.AreEqual((double)new Posit8E2(0.09375), 0.09375);
            Assert.AreEqual((double)new Posit8E2(1.5), 1.5);
            Assert.AreEqual((double)new Posit8E2(-0.09375),-0.09375);
            Assert.AreEqual((double)new Posit8E2(-1.5), -1.5);
            Assert.AreEqual((double)new Posit8E2(6), 6);
            Assert.AreEqual((double)new Posit8E2(-6), -6);
            Assert.AreEqual((float)(double)new Posit8E2( 16777216),(float)16777216);
            Assert.AreEqual((float)(double)new Posit8E2( -16777216),(float)-16777216);			
        }
        
        [Fact]
        public void Posit8E2_AdditionIsCorrectForPositives()
        {
            var posit1 = new Posit8E2(1);

            for (var i = 1; i < 8; i++)
            {
                posit1 += 1;
            }
            Assert.AreEqual(((uint)posit1), (uint)new Posit8E2(8));
        }

        [Fact]
        public void Posit8E2_AdditionIsCorrectForNegatives()
        {
            var posit1 = new Posit8E2(-4);

            for (var i = 1; i < 8; i++)
            {
                posit1 += 1;
            }
            Assert.AreEqual(((uint)posit1), (uint)new Posit8E2(3));
        }

        [Fact]
        public void Posit8E2_AdditionIsCorrectForReals()
        {
            var posit1 = new Posit8E2(0.015625);
            var posit2 = posit1 + posit1;
            Assert.AreEqual(posit2, new Posit8E2(0.03125));
            Assert.AreEqual((posit1-posit2), new Posit8E2(-0.015625));
            Assert.AreEqual((new Posit8E2(1) - new Posit8E2(0.1)), new Posit8E2(0.9));
            
              					
        }	

        [Fact]
        public void Posit8E2_MultiplicationIsCorrect()
        {
             var posit1 = new Posit8E2(1);
             Assert.AreEqual((posit1 * new Posit8E2(0.015625)), new Posit8E2(0.015625));
             Assert.AreEqual((posit1 * new Posit8E2(256)), new Posit8E2(256));
             Assert.AreEqual((-posit1 * new Posit8E2(3)), new Posit8E2(-3));
             Assert.AreEqual((new Posit8E2(2) * new Posit8E2(0.015625)), new Posit8E2(0.03125));
             Assert.AreEqual((new Posit8E2(4) * new Posit8E2(16)), new Posit8E2(64));
             Assert.AreEqual((new Posit8E2(-3) * new Posit8E2(-4)), new Posit8E2(12));
            
                  					
        }	

        [Fact]
        public void Posit8E2_DivisionIsCorrect()
        {
             var posit1 = new Posit8E2(1);
             Assert.AreEqual((posit1 / new Posit8E2(0)), new Posit8E2(Posit8E2.NaNBitMask, true));
             Assert.AreEqual((new Posit8E2(0.015625) / posit1), new Posit8E2(0.015625));
             Assert.AreEqual((new Posit8E2(256) / posit1), new Posit8E2(256));
             Assert.AreEqual((new Posit8E2(3) / -posit1), new Posit8E2(-3));
             Assert.AreEqual((new Posit8E2(0.03125) / new Posit8E2(2)), new Posit8E2(0.015625));
             Assert.AreEqual((new Posit8E2(64) / new Posit8E2(16)), new Posit8E2(4));
             Assert.AreEqual((new Posit8E2(12) / new Posit8E2(-4)), new Posit8E2(-3));
            
                
         }	

        [Fact]
        public void Posit8E2_SqrtIsCorrect()
        {
             var posit1 = new Posit8E2(1);
             Assert.AreEqual(Posit8E2.Sqrt(posit1), posit1);
             Assert.AreEqual(Posit8E2.Sqrt(-posit1), new Posit8E2(Posit8E2.NaNBitMask, true));
     
             Assert.AreEqual((Posit8E2.Sqrt(new Posit8E2(4))), new Posit8E2(2));
             Assert.AreEqual((Posit8E2.Sqrt(new Posit8E2(64))), new Posit8E2(8));
             Assert.AreEqual((Posit8E2.Sqrt(new Posit8E2(0.25))), new Posit8E2(0.5));
             
                          
                     }
        
        [Fact]
        public void Posit8E2_FusedSumIsCorrect()
        {
            //System.Console.WriteLine("Posit8E2 " +  Posit8E2.QuireSize + " fs: "+  Posit8E2.QuireFractionSize);
            var positArray = new Posit8E2[257];
            positArray[0] = new Posit8E2(-64);
            for(var i=1; i <= 256; i++) positArray[i] = new Posit8E2(0.5);          
            
            Assert.AreEqual(Posit8E2.FusedSum(positArray).PositBits, new Posit8E2(64).PositBits);

            positArray[2] = new Posit8E2(Posit8E2.NaNBitMask, true);
            Assert.AreEqual(Posit8E2.FusedSum(positArray).PositBits, positArray[2].PositBits);

            var positArray2 = new Posit8E2[1281];
            positArray2[0] = new Posit8E2(0);
            for(var i=1; i <= 1280; i++) positArray2[i] = new Posit8E2(0.1);
            Assert.AreEqual(Posit8E2.FusedSum(positArray2).PositBits, new Posit8E2(128).PositBits);
        }

        [Fact]
        public void Posit8E2_FusedDotProductIsCorrect()
        {
            var positArray1 = new Posit8E2[3];
            var positArray2 = new Posit8E2[3];
            positArray1[0] = new Posit8E2(1);
            positArray1[1] = new Posit8E2(2);
            positArray1[2] = new Posit8E2(3);

            positArray2[0] = new Posit8E2(1);
            positArray2[1] = new Posit8E2(2);
            positArray2[2] = new Posit8E2(4);
            Assert.AreEqual(Posit8E2.FusedDotProduct(positArray1, positArray2).PositBits, new Posit8E2(17).PositBits);

            var positArray3 = new Posit8E2[3];
            positArray3[0] = new Posit8E2(-1);
            positArray3[1] = new Posit8E2(2);
            positArray3[2] = new Posit8E2(-100);
            Assert.AreEqual(Posit8E2.FusedDotProduct(positArray1, positArray3), new Posit8E2(-297));

             var positArray4 = new Posit8E2[3];
            positArray4[0] = new Posit8E2(-1);
            positArray4[1] = new Posit8E2(2);
            positArray4[2] = new Posit8E2(Posit8E2.MaxValueBitMask, true);
            Assert.AreEqual(Posit8E2.FusedDotProduct(positArray1, positArray4), new Posit8E2(Posit8E2.MaxValueBitMask, true));
        }

        [Fact]
        public void Posit8E2_ConversionToOtherEnvsIsCorrect()
        {

            Assert.AreEqual((Posit8E0)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit8E0( Posit8E0.NaNBitMask, true));	
            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit8E0 : " + 0.01*i*i*i*1 + " i: " + i );
                Assert.AreEqual(((Posit8E0)new Posit8E2(0.01*i*i*i*1 )), new Posit8E0((double) new Posit8E2(0.01*i*i*i*1)), "Converting value " + (0.01*i*i*i*1) + "to Posit8E0 has failed. ");	
            }

            for(var i = 0; i < 800; i++){
                //System.Console.WriteLine("Posit8E0 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit8E0)new Posit8E2(1.0-(1.0/(i+1))), new Posit8E0((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8E0 has failed. ");	
            }
    
            Assert.AreEqual((Posit8E1)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit8E1( Posit8E1.NaNBitMask, true));	
            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit8E1 : " + 0.01*i*i*i*2 + " i: " + i );
                Assert.AreEqual(((Posit8E1)new Posit8E2(0.01*i*i*i*2 )), new Posit8E1((double) new Posit8E2(0.01*i*i*i*2)), "Converting value " + (0.01*i*i*i*2) + "to Posit8E1 has failed. ");	
            }

            for(var i = 0; i < 800; i++){
                //System.Console.WriteLine("Posit8E1 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit8E1)new Posit8E2(1.0-(1.0/(i+1))), new Posit8E1((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8E1 has failed. ");	
            }
    
            Assert.AreEqual((Posit8E3)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit8E3( Posit8E3.NaNBitMask, true));	
            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit8E3 : " + 0.01*i*i*i*4 + " i: " + i );
                Assert.AreEqual(((Posit8E3)new Posit8E2(0.01*i*i*i*4 )), new Posit8E3((double) new Posit8E2(0.01*i*i*i*4)), "Converting value " + (0.01*i*i*i*4) + "to Posit8E3 has failed. ");	
            }

            for(var i = 0; i < 800; i++){
                //System.Console.WriteLine("Posit8E3 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit8E3)new Posit8E2(1.0-(1.0/(i+1))), new Posit8E3((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8E3 has failed. ");	
            }
    
            Assert.AreEqual((Posit8E4)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit8E4( Posit8E4.NaNBitMask, true));	
            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit8E4 : " + 0.01*i*i*i*5 + " i: " + i );
                Assert.AreEqual(((Posit8E4)new Posit8E2(0.01*i*i*i*5 )), new Posit8E4((double) new Posit8E2(0.01*i*i*i*5)), "Converting value " + (0.01*i*i*i*5) + "to Posit8E4 has failed. ");	
            }

            for(var i = 0; i < 800; i++){
                //System.Console.WriteLine("Posit8E4 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit8E4)new Posit8E2(1.0-(1.0/(i+1))), new Posit8E4((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit8E4 has failed. ");	
            }
    
            Assert.AreEqual((Posit16E0)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit16E0( Posit16E0.NaNBitMask, true));	
            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit16E0 : " + 0.01*i*i*i*1 + " i: " + i );
                Assert.AreEqual(((Posit16E0)new Posit8E2(0.01*i*i*i*1 )), new Posit16E0((double) new Posit8E2(0.01*i*i*i*1)), "Converting value " + (0.01*i*i*i*1) + "to Posit16E0 has failed. ");	
            }

            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit16E0 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit16E0)new Posit8E2(1.0-(1.0/(i+1))), new Posit16E0((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16E0 has failed. ");	
            }
    
            Assert.AreEqual((Posit16E1)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit16E1( Posit16E1.NaNBitMask, true));	
            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit16E1 : " + 0.01*i*i*i*2 + " i: " + i );
                Assert.AreEqual(((Posit16E1)new Posit8E2(0.01*i*i*i*2 )), new Posit16E1((double) new Posit8E2(0.01*i*i*i*2)), "Converting value " + (0.01*i*i*i*2) + "to Posit16E1 has failed. ");	
            }

            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit16E1 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit16E1)new Posit8E2(1.0-(1.0/(i+1))), new Posit16E1((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16E1 has failed. ");	
            }
    
            Assert.AreEqual((Posit16E2)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit16E2( Posit16E2.NaNBitMask, true));	
            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit16E2 : " + 0.01*i*i*i*3 + " i: " + i );
                Assert.AreEqual(((Posit16E2)new Posit8E2(0.01*i*i*i*3 )), new Posit16E2((double) new Posit8E2(0.01*i*i*i*3)), "Converting value " + (0.01*i*i*i*3) + "to Posit16E2 has failed. ");	
            }

            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit16E2 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit16E2)new Posit8E2(1.0-(1.0/(i+1))), new Posit16E2((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16E2 has failed. ");	
            }
    
            Assert.AreEqual((Posit16E3)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit16E3( Posit16E3.NaNBitMask, true));	
            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit16E3 : " + 0.01*i*i*i*4 + " i: " + i );
                Assert.AreEqual(((Posit16E3)new Posit8E2(0.01*i*i*i*4 )), new Posit16E3((double) new Posit8E2(0.01*i*i*i*4)), "Converting value " + (0.01*i*i*i*4) + "to Posit16E3 has failed. ");	
            }

            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit16E3 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit16E3)new Posit8E2(1.0-(1.0/(i+1))), new Posit16E3((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16E3 has failed. ");	
            }
    
            Assert.AreEqual((Posit16E4)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit16E4( Posit16E4.NaNBitMask, true));	
            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit16E4 : " + 0.01*i*i*i*5 + " i: " + i );
                Assert.AreEqual(((Posit16E4)new Posit8E2(0.01*i*i*i*5 )), new Posit16E4((double) new Posit8E2(0.01*i*i*i*5)), "Converting value " + (0.01*i*i*i*5) + "to Posit16E4 has failed. ");	
            }

            for(var i = 0; i < 1600; i++){
                //System.Console.WriteLine("Posit16E4 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit16E4)new Posit8E2(1.0-(1.0/(i+1))), new Posit16E4((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit16E4 has failed. ");	
            }
    
            Assert.AreEqual((Posit32E0)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit32E0( Posit32E0.NaNBitMask, true));	
            for(var i = 0; i < 6400; i++){
                //System.Console.WriteLine("Posit32E0 : " + 0.01*i*i*i*1 + " i: " + i );
                Assert.AreEqual(((Posit32E0)new Posit8E2(0.01*i*i*i*1 )), new Posit32E0((double) new Posit8E2(0.01*i*i*i*1)), "Converting value " + (0.01*i*i*i*1) + "to Posit32E0 has failed. ");	
            }

            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit32E0 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit32E0)new Posit8E2(1.0-(1.0/(i+1))), new Posit32E0((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32E0 has failed. ");	
            }
    
            Assert.AreEqual((Posit32E1)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit32E1( Posit32E1.NaNBitMask, true));	
            for(var i = 0; i < 6400; i++){
                //System.Console.WriteLine("Posit32E1 : " + 0.01*i*i*i*2 + " i: " + i );
                Assert.AreEqual(((Posit32E1)new Posit8E2(0.01*i*i*i*2 )), new Posit32E1((double) new Posit8E2(0.01*i*i*i*2)), "Converting value " + (0.01*i*i*i*2) + "to Posit32E1 has failed. ");	
            }

            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit32E1 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit32E1)new Posit8E2(1.0-(1.0/(i+1))), new Posit32E1((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32E1 has failed. ");	
            }
    
            Assert.AreEqual((Posit32E2)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit32E2( Posit32E2.NaNBitMask, true));	
            for(var i = 0; i < 6400; i++){
                //System.Console.WriteLine("Posit32E2 : " + 0.01*i*i*i*3 + " i: " + i );
                Assert.AreEqual(((Posit32E2)new Posit8E2(0.01*i*i*i*3 )), new Posit32E2((double) new Posit8E2(0.01*i*i*i*3)), "Converting value " + (0.01*i*i*i*3) + "to Posit32E2 has failed. ");	
            }

            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit32E2 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit32E2)new Posit8E2(1.0-(1.0/(i+1))), new Posit32E2((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32E2 has failed. ");	
            }
    
            Assert.AreEqual((Posit32E3)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit32E3( Posit32E3.NaNBitMask, true));	
            for(var i = 0; i < 6400; i++){
                //System.Console.WriteLine("Posit32E3 : " + 0.01*i*i*i*4 + " i: " + i );
                Assert.AreEqual(((Posit32E3)new Posit8E2(0.01*i*i*i*4 )), new Posit32E3((double) new Posit8E2(0.01*i*i*i*4)), "Converting value " + (0.01*i*i*i*4) + "to Posit32E3 has failed. ");	
            }

            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit32E3 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit32E3)new Posit8E2(1.0-(1.0/(i+1))), new Posit32E3((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32E3 has failed. ");	
            }
    
            Assert.AreEqual((Posit32E4)new Posit8E2(Posit8E2.NaNBitMask, true), new Posit32E4( Posit32E4.NaNBitMask, true));	
            for(var i = 0; i < 6400; i++){
                //System.Console.WriteLine("Posit32E4 : " + 0.01*i*i*i*5 + " i: " + i );
                Assert.AreEqual(((Posit32E4)new Posit8E2(0.01*i*i*i*5 )), new Posit32E4((double) new Posit8E2(0.01*i*i*i*5)), "Converting value " + (0.01*i*i*i*5) + "to Posit32E4 has failed. ");	
            }

            for(var i = 0; i < 3200; i++){
                //System.Console.WriteLine("Posit32E4 : " + (1.0-(1.0/(i+1))) + " i: " + i );
                Assert.AreEqual((Posit32E4)new Posit8E2(1.0-(1.0/(i+1))), new Posit32E4((double) new Posit8E2( 1.0-(1.0/(i+1))) ), "Converting value " + (1.0-(1.0/(i+1))) + " to Posit32E4 has failed. ");	
            }
            }
    }
}
