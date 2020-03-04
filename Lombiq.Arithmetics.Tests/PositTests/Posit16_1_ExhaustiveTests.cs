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
            filePath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath), "PositTests");
            positListLines = File.ReadAllLines(Path.Combine(filePath, "Posit16_1List.txt"));
        }

        [Fact]
        public void AllPosit16_1_SqrtsAreCorrect()
        {
            string[] resultLines = File.ReadAllLines(Path.Combine(filePath, "Posit16_1_Sqrt.txt"));

            List<Posit16_1> positList = new List<Posit16_1>();

            foreach (var line in positListLines)
            {
                positList.Add(new Posit16_1(double.Parse(line, System.Globalization.CultureInfo.InvariantCulture)));
            }

            var i = 0;
            double correctResult;
            foreach (var leftPosit in positList)
            {
                correctResult = double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture);
                if (double.IsInfinity(correctResult)) correctResult = double.NaN;
                Assert.AreEqual((double)(Posit16_1.Sqrt(leftPosit)), correctResult, "Sqrt(" + leftPosit + ") equals " + Posit16_1.Sqrt(leftPosit));
                i++;
            }
        }
    }
}
