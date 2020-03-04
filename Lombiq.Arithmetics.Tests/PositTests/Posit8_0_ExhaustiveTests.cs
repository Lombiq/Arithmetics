using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;

using Assert = Lombiq.Arithmetics.Tests.CompatibilityAssert;

namespace Lombiq.Arithmetics.Tests
{

    public class Posit8_0_ExhaustiveTests
    {
        private string[] positListLines;
        private string filePath;


        public Posit8_0_ExhaustiveTests()
        {
            filePath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath), "PositTests");
            positListLines = File.ReadAllLines(Path.Combine(filePath, "Posit8_0List.txt"));
        }


        [Fact]
        public void AllPosit8_0_AdditionsAreCorrect()
        {
            string[] resultLines = File.ReadAllLines(Path.Combine(filePath, "Posit8_0_Addition.txt"));

            List<Posit8_0> positList = new List<Posit8_0>();

            foreach (var line in positListLines)
            {
                positList.Add(new Posit8_0(double.Parse(line, System.Globalization.CultureInfo.InvariantCulture)));
            }

            var i = 0;
            foreach (var leftPosit in positList)
            {
                foreach (var rightPosit in positList)
                {
                    Assert.AreEqual((double)(leftPosit + rightPosit), double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture), leftPosit + " +  " + rightPosit + " equals " + (leftPosit + rightPosit));
                    i++;
                }
            }
        }

        [Fact]
        public void AllPosit8_0_MultiplicationsAreCorrect()
        {
            string[] resultLines = File.ReadAllLines(Path.Combine(filePath, "Posit8_0_Multiplication.txt"));
            List<Posit8_0> positList = new List<Posit8_0>();

            foreach (var line in positListLines)
            {
                positList.Add(new Posit8_0(double.Parse(line, System.Globalization.CultureInfo.InvariantCulture)));
            }

            var i = 0;
            foreach (var leftPosit in positList)
            {
                foreach (var rightPosit in positList)
                {
                    Assert.AreEqual((double)(leftPosit * rightPosit), double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture), leftPosit + " *  " + rightPosit + " equals " + (leftPosit * rightPosit));
                    i++;
                }
            }
        }

        [Fact]
        public void AllPosit8_0_DivisionsAreCorrect()
        {
            string[] resultLines = File.ReadAllLines(Path.Combine(filePath, "Posit8_0_Division.txt"));

            List<Posit8_0> positList = new List<Posit8_0>();

            foreach (var line in positListLines)
            {
                positList.Add(new Posit8_0(double.Parse(line, System.Globalization.CultureInfo.InvariantCulture)));

            }

            var i = 0;
            double correctResult;
            foreach (var leftPosit in positList)
            {
                foreach (var rightPosit in positList)
                {
                    correctResult = double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture);
                    if (double.IsInfinity(correctResult)) correctResult = double.NaN;
                    Assert.AreEqual((double)(leftPosit / rightPosit), correctResult, leftPosit + " /  " + rightPosit + " equals " + (leftPosit / rightPosit));
                    i++;
                }
            }

        }

        [Fact]
        public void AllPosit8_0_SqrtsAreCorrect()
        {
            string[] resultLines = File.ReadAllLines(Path.Combine(filePath, "Posit8_0_Sqrt.txt"));

            List<Posit8_0> positList = new List<Posit8_0>();

            foreach (var line in positListLines)
            {
                positList.Add(new Posit8_0(double.Parse(line, System.Globalization.CultureInfo.InvariantCulture)));
            }

            var i = 0;
            double correctResult;
            foreach (var leftPosit in positList)
            {
                correctResult = double.Parse(resultLines[i], System.Globalization.CultureInfo.InvariantCulture);
                if (double.IsInfinity(correctResult)) correctResult = double.NaN;
                Assert.AreEqual((double)(Posit8_0.Sqrt(leftPosit)), correctResult, "Sqrt(" + leftPosit + ") equals " + Posit8_0.Sqrt(leftPosit));
                i++;
            }
        }

        [Fact]
        public void DebuggingAdditionTestCases()
        {
            var leftPosit = new Posit8_0(0.046875);
            var rightPosit = new Posit8_0(-2);
            var resultPosit = new Posit8_0(-1.9375);

            var addedPosit = leftPosit + rightPosit;

            Assert.AreEqual((addedPosit), resultPosit, leftPosit + " +  " + rightPosit + " equals " + (leftPosit + rightPosit));

        }

        [Fact]
        public void DebuggingMultiplicationTestCases()
        {
            var leftPosit = new Posit8_0(0.015625);
            var rightPosit = new Posit8_0(1.5);
            var resultPosit = new Posit8_0(0.03125);

            var multipliedPosit = leftPosit * rightPosit;

            Assert.AreEqual((double)(multipliedPosit), (double)resultPosit, leftPosit + " *  " + rightPosit + " equals " + (leftPosit + rightPosit));

        }

        [Fact]
        public void DebuggingDivisionTestCases()
        {
            double d = double.Parse("Infinity", System.Globalization.CultureInfo.InvariantCulture);
        }

    }
}
