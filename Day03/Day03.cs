using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Day03
{
    public class Tests
    {
        [Test]
        public void SolvePart1_InputRellem()
        {
            var expectedAnswer = "258";
            var actualAnswer = Solver.SolvePart1(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart2_InputRellem()
        {
            var expectedAnswer = "12304";
            var actualAnswer = Solver.SolvePart2(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            var lines = input.Trim().Split("\n");
            var wire1 = lines[0].Split(",");
            var wire2 = lines[1].Split(",");
            var grid1 = new Dictionary<int, Dictionary<int, int>>();
            runWire(grid1, wire1);
            var grid2 = new Dictionary<int, Dictionary<int, int>>();
            runWire(grid2, wire2);
            var minDistance = Int32.MaxValue;
            foreach (var kvX in grid1)
            {
                foreach (var kvY in grid1[kvX.Key])
                {
                    if (grid2.ContainsKey(kvX.Key))
                    {

                        if (grid2[kvX.Key].ContainsKey(kvY.Key))
                        {
                            var distance = (int)(Math.Abs(kvX.Key) + Math.Abs(kvY.Key));
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                            }
                        }
                    }
                }
            }
            return minDistance + "";
        }

        public static string SolvePart2(string input)
        {
            var lines = input.Trim().Split("\n");
            var wire1 = lines[0].Split(",");
            var wire2 = lines[1].Split(",");
            var grid1 = new Dictionary<int, Dictionary<int, int>>();
            runWire(grid1, wire1);
            var grid2 = new Dictionary<int, Dictionary<int, int>>();
            runWire(grid2, wire2);
            var minSteps = Int32.MaxValue;
            foreach (var kvX in grid1)
            {
                foreach (var kvY in grid1[kvX.Key])
                {
                    if (grid2.ContainsKey(kvX.Key))
                    {

                        if (grid2[kvX.Key].ContainsKey(kvY.Key))
                        {
                            var steps = grid1[kvX.Key][kvY.Key] + grid2[kvX.Key][kvY.Key];
                            if (steps < minSteps)
                            {
                                minSteps = steps;
                            }
                        }
                    }
                }
            }
            return minSteps + "";
        }

        static void runWire(Dictionary<int, Dictionary<int, int>> grid, string[] wire)
        {
            var posX = 0;
            var posY = 0;
            var stepsTaken = 0;
            foreach (var ins in wire)
            {
                var dir = ins.Substring(0, 1);
                var steps = Int32.Parse(ins.Substring(1));
                for (var i = 1; i <= steps; i++)
                {
                    switch (dir)
                    {
                        case "R":
                            posX++;
                            break;
                        case "U":
                            posY++;
                            break;
                        case "L":
                            posX--;
                            break;
                        case "D":
                            posY--;
                            break;
                        default:
                            throw new Exception("Unexpected dir: " + dir);
                    }
                    stepsTaken++;
                    grid.TryAdd(posX, new Dictionary<int, int>());
                    grid[posX].TryAdd(posY, stepsTaken);
                }
            }
        }
    }
}