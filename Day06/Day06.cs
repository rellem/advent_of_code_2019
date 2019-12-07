using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Day06
{
    public class Tests
    {
        [Test]
        public void SolvePart1_InputRellem()
        {
            var expectedAnswer = "300598";
            var actualAnswer = Solver.SolvePart1(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart2_InputRellem()
        {
            var expectedAnswer = "520";
            var actualAnswer = Solver.SolvePart2(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            var orbits2orbited = new Dictionary<string, string>();
            var lines = input.Trim().Split("\n");
            foreach (var line in lines)
            {
                var splitLine = line.Split(")");
                var orbited = splitLine[0];
                var orbits = splitLine[1];
                orbits2orbited.Add(orbits, orbited);
            }

            return checksum(orbits2orbited).ToString();
        }

        public static string SolvePart2(string input)
        {
            var orbits2orbited = new Dictionary<string, string>();
            var lines = input.Trim().Split("\n");
            foreach (var line in lines)
            {
                var splitLine = line.Split(")");
                var orbited = splitLine[0];
                var orbits = splitLine[1];
                orbits2orbited.Add(orbits, orbited);
            }

            return getShortestPath("YOU", "SAN", orbits2orbited).ToString();
        }

        private static int getShortestPath(string planet1, string planet2, Dictionary<string, string> orbits2orbited)
        {
            var path1 = getAllOrbited(planet1, orbits2orbited);
            var path2 = getAllOrbited(planet2, orbits2orbited);
            var i = 0;
            foreach (var x in path1)
            {
                var j = 0;
                foreach (var y in path2)
                {
                    if (x == y)
                    {
                        return i + j;
                    }
                    j++;
                }
                i++;
            }
            return 0;
        }

        private static int checksum(Dictionary<string, string> orbits2orbited)
        {

            var totalCount = 0;
            foreach (var x in orbits2orbited)
            {
                totalCount += count(x.Key, orbits2orbited);
            }
            return totalCount;
        }

        private static int count(string planet, Dictionary<string, string> orbits2orbited)
        {
            if (orbits2orbited.ContainsKey(planet))
            {
                return 1 + count(orbits2orbited[planet], orbits2orbited);
            }
            return 0;
        }

        private static List<string> getAllOrbited(string planet, Dictionary<string, string> orbits2orbited)
        {
            var list = new List<string>();
            if (orbits2orbited.ContainsKey(planet))
            {
                list.Add(orbits2orbited[planet]);
                list.AddRange(getAllOrbited(orbits2orbited[planet], orbits2orbited));
                return list;
            }
            return list;
        }
    }
}