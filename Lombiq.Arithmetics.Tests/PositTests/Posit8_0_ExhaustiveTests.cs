using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Lombiq.Arithmetics.Tests.PositTests
{

    [TestFixture]
    class Posit8_0_ExhaustiveTests
    {
        [Test]
        public void AllPosit8_0_AdditionsAreCorrect()
        {
            string[] inputLines = System.IO.File.ReadAllLines("C:\\SoftPosit\\SoftPosit\\build\\Linux-x86_64-GCC\\posit8_0List.txt");
            string[] resultLines = System.IO.File.ReadAllLines("C:\\SoftPosit\\SoftPosit\\build\\Linux-x86_64-GCC\\Posit8_0_Addition.txt");

            List<Posit8_0> positList = new List<Posit8_0>();

            foreach (var line in inputLines)
            {
                positList.Add(new Posit8_0(double.Parse(line, System.Globalization.CultureInfo.InvariantCulture)));
            }

            var i = 0;
            var wrong = 0;
            foreach (var leftPosit in positList)
            {
                foreach (var rightPosit in positList)
                {
                     Assert.AreEqual((double)(leftPosit + rightPosit), Double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture), leftPosit +" +  " + rightPosit + " equals " + (leftPosit+rightPosit) );
                    if ((double)(leftPosit + rightPosit) != Double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture))
                    {
                        Console.WriteLine(leftPosit + " +  " + rightPosit + " equals " + (leftPosit + rightPosit) + "But should be " + Double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture));
                        wrong++;
                    }
                    i++;
                }
            }

            Console.WriteLine("wrong: " + wrong);

        }

        [Test]
        public void AllPosit8_0_MultiplicationsAreCorrect()
        {
            string[] inputLines = System.IO.File.ReadAllLines("C:\\SoftPosit\\SoftPosit\\build\\Linux-x86_64-GCC\\posit8_0List.txt");
            string[] resultLines = System.IO.File.ReadAllLines("C:\\SoftPosit\\SoftPosit\\build\\Linux-x86_64-GCC\\Posit8_0_Multiplication.txt");

            List<Posit8_0> positList = new List<Posit8_0>();

            foreach (var line in inputLines)
            {
                positList.Add(new Posit8_0(double.Parse(line, System.Globalization.CultureInfo.InvariantCulture)));
            }

            var i = 0;
            //var wrong = 0;
            foreach (var leftPosit in positList)
            {
                foreach (var rightPosit in positList)
                {
                    Assert.AreEqual((double)(leftPosit * rightPosit), Double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture), leftPosit + " *  " + rightPosit + " equals " + (leftPosit * rightPosit));
                    //if ((double)(leftPosit * rightPosit) != Double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture))
                    //{
                    //    //Console.WriteLine(leftPosit + " *  " + rightPosit + " equals " + (leftPosit * rightPosit) + "But should be " + Double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture));
                    //    wrong++;
                    //}
                    i++;
                }
            }

            //Console.WriteLine("wrong: " + wrong);

        }

        [Test]
        public void AllPosit8_0_DivisionsAreCorrect()
        {
            string[] inputLines = System.IO.File.ReadAllLines("C:\\SoftPosit\\SoftPosit\\build\\Linux-x86_64-GCC\\posit8_0List.txt");
            string[] resultLines = System.IO.File.ReadAllLines("C:\\SoftPosit\\SoftPosit\\build\\Linux-x86_64-GCC\\Posit8_0_Division.txt");

            List<Posit8_0> positList = new List<Posit8_0>();

            foreach (var line in inputLines)
            {
                positList.Add(new Posit8_0(double.Parse(line, System.Globalization.CultureInfo.InvariantCulture)));

            }
            Console.WriteLine(positList.Count);

            var i = 0;
            var wrong = 0;
            foreach (var leftPosit in positList)
            {
                foreach (var rightPosit in positList)
                {
                    // Assert.AreEqual((double)(leftPosit / rightPosit), Double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture), leftPosit + " /  " + rightPosit + " equals " + (leftPosit / rightPosit));
                    if ((double)(leftPosit / rightPosit) != Double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture))
                    {
                        Console.WriteLine(leftPosit + " /  " + rightPosit + " equals " + (leftPosit / rightPosit) + "But should be " + Double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture));
                        if (leftPosit != new Posit8_0(0) && rightPosit != new Posit8_0(0)) wrong++;
                    }
                    i++;
                }
            }

            Console.WriteLine("wrong: " + wrong);

        }

        [Test]
        public void AllPosit8_0_SqrtsAreCorrect()
        {
            string[] inputLines = System.IO.File.ReadAllLines("C:\\SoftPosit\\SoftPosit\\build\\Linux-x86_64-GCC\\posit8_0List.txt");
            string[] resultLines = System.IO.File.ReadAllLines("C:\\SoftPosit\\SoftPosit\\build\\Linux-x86_64-GCC\\Posit8_0_Sqrt.txt");

            List<Posit8_0> positList = new List<Posit8_0>();

            foreach (var line in inputLines)
            {
                positList.Add(new Posit8_0(double.Parse(line, System.Globalization.CultureInfo.InvariantCulture)));

            }
            Console.WriteLine(positList.Count);

            var i = 0;
            var wrong = 0;
            var biggerByOne = 0;
            var smallerByOne = 0;

            foreach (var leftPosit in positList)
            {

                double doubleResult = Double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture);
                Posit8_0 positResult = Posit8_0.Sqrt(leftPosit);
                // Assert.AreEqual((double)(Posit8_0.Sqrt(leftPosit)), Double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture), "Sqrt(" + leftPosit + ") equals " + Posit8_0.Sqrt(leftPosit));
                if ((double)positResult != doubleResult)
                {
                    //Console.WriteLine(leftPosit + " Sqrt:  " + positResult + "But should be " + doubleResult);
                    if (leftPosit.IsPositive())
                    {
                        wrong++;
                        Console.WriteLine(leftPosit + " Sqrt:  " + positResult + "But should be " + doubleResult);
                        Console.WriteLine(positResult.PositBits - new Posit8_0(doubleResult).PositBits);
                        if(positResult.PositBits - new Posit8_0(doubleResult).PositBits != -1)
                        {
                            Console.WriteLine("SF: " + leftPosit.CalculateScaleFactor());
                        }
                        else Console.WriteLine("OB1SF: " + leftPosit.CalculateScaleFactor());




                    }
                    if ((double)(new Posit8_0((byte)(positResult.PositBits + 1), true)) == doubleResult)
                    {
                        biggerByOne++;
                        //Console.WriteLine("original: " + positResult + " biggerByOne: " + (new Posit8_0((byte)(positResult.PositBits + 1), true)));

                    }
                    if ((double)(new Posit8_0((byte)(positResult.PositBits - 1), true)) == doubleResult)
                    {
                        smallerByOne++;
                        //Console.WriteLine("original: " + positResult + " smallerByOne: " + (new Posit8_0((byte)(positResult.PositBits - 1), true)));

                    }

                }
                i++;

            }

            Console.WriteLine("wrong: " + wrong);
            Console.WriteLine("biggerByOne: " + biggerByOne);
            Console.WriteLine("smallerByOne: " + smallerByOne);

        }


        [Test]
        public void DebuggingAdditionTestCases()
        {
            var leftPosit = new Posit8_0(0.046875);
            var rightPosit = new Posit8_0(-2);
            var resultPosit = new Posit8_0(-1.9375);

            var addedPosit = leftPosit + rightPosit;
            Console.WriteLine(addedPosit.PositBits);
            Console.WriteLine(new Posit8_0((double)addedPosit).PositBits);
            Console.WriteLine(Posit8_0.AssemblePositBitsWithRounding(true, 0, (byte)0b11111100));
            Console.WriteLine(0b10100010);

            Assert.AreEqual((addedPosit), resultPosit, leftPosit + " +  " + rightPosit + " equals " + (leftPosit + rightPosit));

        }


        [Test]
        public void DebuggingMultiplicationTestCases()
        {
            var leftPosit = new Posit8_0(0.015625);
            var rightPosit = new Posit8_0(1.5);
            var resultPosit = new Posit8_0(0.03125);

            var multipliedPosit = leftPosit * rightPosit;
            Console.WriteLine(multipliedPosit.PositBits);
            Console.WriteLine(new Posit8_0((double)multipliedPosit).PositBits);
            //Console.WriteLine(Posit8_0.AssemblePositBitsWithRounding(true, 0, (byte)0b11111100));
            //Console.WriteLine(0b10100010);

            Assert.AreEqual((double)(multipliedPosit), (double)resultPosit, leftPosit + " *  " + rightPosit + " equals " + (leftPosit + rightPosit));

        }

        [Test]
        public void DebuggingDivisionTestCases()
        {
            double d = Double.Parse("Infinity", System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine(d);
        }

    }
}
