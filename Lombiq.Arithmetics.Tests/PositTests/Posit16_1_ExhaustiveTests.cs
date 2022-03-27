using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;
using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;


namespace Lombiq.Arithmetics.Tests
{

    public class Posit16_1_ExhaustiveTests
    {
        private string[] positListLines;
        private string filePath;


        public Posit16_1_ExhaustiveTests()
        {
            filePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath) + "\\PositTests";
            positListLines = File.ReadAllLines(filePath + "\\Posit16_1List.txt");
        }

        [Fact]
        public void AllPosit16_1_SqrtsAreCorrect()
        {
            string[] resultLines = System.IO.File.ReadAllLines(filePath + "\\Posit16_1_Sqrt.txt");

            List<Posit16E1> positList = new List<Posit16E1>();

            foreach (var line in positListLines)
            {
                positList.Add(new Posit16E1(double.Parse(line, System.Globalization.CultureInfo.InvariantCulture)));
            }

            var i = 0;
            double correctResult;
            foreach (var leftPosit in positList)
            {
                correctResult = double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture);
                if (double.IsInfinity(correctResult)) correctResult = double.NaN;
                Assert.AreEqual((double)(Posit16E1.Sqrt(leftPosit)), correctResult, "Sqrt(" + leftPosit + ") equals " + Posit16E1.Sqrt(leftPosit));
                i++;
            }
        }
    }
}
