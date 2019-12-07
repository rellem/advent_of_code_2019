using System.IO;
using NUnit.Framework;

namespace Day01
{
    public class Tests
    {
        [Test]
        public void SolvePart1_InputRellem()
        {
            var expectedAnswer = "3394106";
            var actualAnswer = Solver.SolvePart1(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart2_InputRellem()
        {
            var expectedAnswer = "5088280";
            var actualAnswer = Solver.SolvePart2(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            var totalFuel = 0;
            var lines = input.Trim().Split("\n");
            foreach (var line in lines)
            {
                var mass = int.Parse(line);
                var fuel = mass / 3 - 2;
                totalFuel += fuel;
            }
            return totalFuel.ToString();
        }

        public static string SolvePart2(string input)
        {
            var totalFuel = 0;
            var lines = input.Trim().Split("\n");
            foreach (var line in lines)
            {
                var mass = int.Parse(line);
                var fuel = mass / 3 - 2;
                totalFuel += fuel;
                var remainingMass = fuel;
                while (true)
                {
                    mass = fuel;
                    fuel = mass / 3 - 2;
                    if (fuel <= 0)
                    {
                        break;
                    }
                    totalFuel += fuel;
                }
            }
            return totalFuel.ToString();
        }
    }
}