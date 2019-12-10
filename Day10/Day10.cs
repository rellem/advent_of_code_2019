using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NUnit.Framework;

namespace Day10
{
    public class Tests
    {
        [Test]
        public void SolvePart1_InputRellem()
        {
            var expectedAnswer = "326";
            var actualAnswer = Solver.SolvePart1(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart1_Example1()
        {
            var expectedAnswer = "8";
            var actualAnswer = Solver.SolvePart1(@"
.#..#
.....
#####
....#
...##");
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart1_Example2()
        {
            var expectedAnswer = "33";
            var actualAnswer = Solver.SolvePart1(@"
......#.#.
#..#.#....
..#######.
.#.#.###..
.#..#.....
..#....#.#
#..#....#.
.##.#..###
##...#..#.
.#....####");
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart1_Example3()
        {
            var expectedAnswer = "210";
            var actualAnswer = Solver.SolvePart1(@"
.#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##");
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart2_InputRellem()
        {
            var expectedAnswer = "?";
            var actualAnswer = Solver.SolvePart2(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }
    public class Tests2
    {
        [Test]
        public void GetIntermediatePoints_TestSamePoint()
        {
            var expectedAnswer = new List<Point> { };
            var actualAnswer = Solver.GetIntermediatePoints(new Point() { X = 0, Y = 0 }, new Point() { X = 0, Y = 0 });
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void GetIntermediatePoints_TestOneToTheRight()
        {
            var expectedAnswer = new List<Point> { };
            var actualAnswer = Solver.GetIntermediatePoints(new Point() { X = 0, Y = 0 }, new Point() { X = 1, Y = 0 });
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void GetIntermediatePoints_TestTwoToTheRight()
        {
            var expectedAnswer = new List<Point>
            {
                new Point() { X = 1, Y = 0 }
            };
            var actualAnswer = Solver.GetIntermediatePoints(new Point() { X = 0, Y = 0 }, new Point() { X = 2, Y = 0 });
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void GetIntermediatePoints_TestA()
        {
            var expectedAnswer = new List<Point>
            {
                new Point() { X = 3, Y = 1 },
                new Point() { X = 6, Y = 2 },
            };
            var actualAnswer = Solver.GetIntermediatePoints(new Point() { X = 0, Y = 0 }, new Point() { X = 9, Y = 3 });
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void GetIntermediatePoints_TestAReversed()
        {
            var expectedAnswer = new List<Point>
            {
                new Point() { X = 6, Y = 2 },
                new Point() { X = 3, Y = 1 },
            };
            var actualAnswer = Solver.GetIntermediatePoints(new Point() { X = 9, Y = 3 }, new Point() { X = 0, Y = 0 });
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }


    public struct Point
    {
        public int X;
        public int Y;
        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            var map = new Dictionary<Point, int>();
            var x = 0;
            var y = 0;
            foreach (var line in input.Trim().Split("\n"))
            {
                foreach (var chr in line.ToCharArray())
                {
                    if (chr == '#')
                    {
                        map.Add(new Point { X = x, Y = y }, 0);
                    }
                    x++;
                }
                y++;
            }

            foreach (var p1 in map.Keys.ToList())
            {
                foreach (var p2 in map.Keys.ToList())
                {
                    if (p1.Equals(p2)) { continue; }
                    if (CanSeeEachother(p1, p2, map))
                    {
                        map[p1]++;
                    }
                }
            }

            var maxVisible = -1;
            foreach (var kv in map)
            {
                if (kv.Value > maxVisible)
                {
                    maxVisible = kv.Value;
                }
            }

            return maxVisible.ToString();
        }

        public static string SolvePart2(string input)
        {
            return "TODO";
        }

        public static bool CanSeeEachother(Point p1, Point p2, Dictionary<Point, int> map)
        {
            foreach (var intermediatePoint in GetIntermediatePoints(p1, p2))
            {
                if (map.ContainsKey(intermediatePoint))
                {
                    return false;
                }
            }
            return true;
        }

        public static List<Point> GetIntermediatePoints(Point p1, Point p2)
        {
            int diffX = Math.Abs(p1.X - p2.X);
            int diffY = Math.Abs(p1.Y - p2.Y);
            int numIntermediateSteps = Math.Max(diffX, diffY) - 1;
            var intermediatePoints = new List<Point>();
            for (var intermediateStep = 1; intermediateStep <= numIntermediateSteps; intermediateStep++)
            {
                var modX = ((p2.X - p1.X) * intermediateStep) % (numIntermediateSteps + 1);
                var modY = ((p2.Y - p1.Y) * intermediateStep) % (numIntermediateSteps + 1);
                if (modX == 0 && modY == 0)
                {
                    int currX = p1.X + ((p2.X - p1.X) * intermediateStep) / (numIntermediateSteps + 1);
                    int currY = p1.Y + ((p2.Y - p1.Y) * intermediateStep) / (numIntermediateSteps + 1);
                    intermediatePoints.Add(new Point() { X = currX, Y = currY });
                }
            }
            return intermediatePoints;
        }

    }
}