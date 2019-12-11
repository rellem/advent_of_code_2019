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
            HashSet<Point> points = ParseInput(input);
            Dictionary<Point, int> pointsWithVisibilityCount = GetPointsWithVisibilityCount(points);
            Point pointWithBestVisibility = GetPointWithBestVisibility(pointsWithVisibilityCount)!.Value;
            return pointsWithVisibilityCount[pointWithBestVisibility].ToString();
        }

        public static string SolvePart2(string input)
        {
            return "TODO";
        }

        public static HashSet<Point> ParseInput(string input)
        {
            var points = new HashSet<Point>();
            var x = 0;
            var y = 0;
            foreach (var line in input.Trim().Split("\n"))
            {
                foreach (var chr in line.ToCharArray())
                {
                    if (chr == '#')
                    {
                        Trace.Assert(points.Add(new Point { X = x, Y = y }));
                    }
                    x++;
                }
                y++;
            }
            return points;
        }

        public static Dictionary<Point, int> GetPointsWithVisibilityCount(HashSet<Point> points)
        {
            var pointsWithVisibilityCount = new Dictionary<Point, int>();
            foreach (var p1 in points)
            {
                pointsWithVisibilityCount.Add(p1, 0);
                foreach (var p2 in points)
                {
                    if (p1.Equals(p2)) { continue; }
                    if (CanSeeEachOther(p1, p2, points))
                    {
                        pointsWithVisibilityCount[p1]++;
                    }
                }
            }
            return pointsWithVisibilityCount;
        }

        public static Point? GetPointWithBestVisibility(Dictionary<Point, int> pointsWithVisibilityCount)
        {
            var maxVisible = -1;
            Point? pointWithBestVisibility = null;
            foreach (var (point, cnt) in pointsWithVisibilityCount)
            {
                if (cnt > maxVisible)
                {
                    maxVisible = cnt;
                    pointWithBestVisibility = point;
                }
            }
            return pointWithBestVisibility;
        }

        public static bool CanSeeEachOther(Point p1, Point p2, HashSet<Point> points)
        {
            foreach (var intermediatePoint in GetIntermediatePoints(p1, p2))
            {
                if (points.Contains(intermediatePoint))
                {
                    return false;
                }
            }
            return true;
        }

        public static List<Point> GetIntermediatePoints(Point p1, Point p2)
        {
            var intermediatePoints = new List<Point>();
            int diffX = p2.X - p1.X;
            int diffY = p2.Y - p1.Y;
            int numIntermediateSteps = Math.Max(Math.Abs(diffX), Math.Abs(diffY)) - 1;
            for (var intermediateStep = 1; intermediateStep <= numIntermediateSteps; intermediateStep++)
            {
                var stepX = Math.DivRem(diffX * intermediateStep, numIntermediateSteps + 1, out int remainderX);
                var stepY = Math.DivRem(diffY * intermediateStep, numIntermediateSteps + 1, out int remainderY);
                if (remainderX == 0 && remainderY == 0)
                {
                    int currX = p1.X + stepX;
                    int currY = p1.Y + stepY;
                    intermediatePoints.Add(new Point() { X = currX, Y = currY });
                }
            }
            return intermediatePoints;
        }
    }
}