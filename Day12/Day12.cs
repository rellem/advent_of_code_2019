using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Day10
{
    public class Tests
    {
        [Test]
        public void SolvePart1_InputRellem()
        {
            var expectedAnswer = "5517";
            var actualAnswer = Solver.SolvePart1(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart2_InputRellem()
        {
            var expectedAnswer = "303070460651184";
            var actualAnswer = Solver.SolvePart2(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            List<Moon> moons = CreateMoons(ParseInput(input));
            for (var step = 1; step <= 1_000; step++)
            {
                moons = ApplyGravity(moons);
                moons = ApplyVelocity(moons);
            }
            return GetTotalEnergyInSystem(moons).ToString();
        }

        public static string SolvePart2(string input)
        {
            List<Moon> moons = CreateMoons(ParseInput(input));
            var seenX = new HashSet<string>();
            var seenY = new HashSet<string>();
            var seenZ = new HashSet<string>();
            Trace.Assert(seenX.Add(GetXPositionAndVelocityString(moons)));
            Trace.Assert(seenY.Add(GetYPositionAndVelocityString(moons)));
            Trace.Assert(seenZ.Add(GetZPositionAndVelocityString(moons)));
            int? stepsX = null;
            int? stepsY = null;
            int? stepsZ = null;
            for (var step = 1; step <= 1_000_000; step++)
            {
                moons = ApplyGravity(moons);
                moons = ApplyVelocity(moons);
                if (stepsX == null && !seenX.Add(GetXPositionAndVelocityString(moons)))
                {
                    stepsX = step;
                }
                if (stepsY == null && !seenY.Add(GetYPositionAndVelocityString(moons)))
                {
                    stepsY = step;
                }
                if (stepsZ == null && !seenZ.Add(GetZPositionAndVelocityString(moons)))
                {
                    stepsZ = step;
                }
                if (stepsX != null && stepsY != null && stepsZ != null)
                {
                    break;
                }
            }
            Console.WriteLine($"{stepsX},{stepsY},{stepsZ}");
            return LeastCommonMultiple(LeastCommonMultiple(stepsX!.Value, stepsY!.Value), stepsZ!.Value).ToString();
        }

        private readonly struct Position
        {
            public readonly int X;
            public readonly int Y;
            public readonly int Z;
            public Position(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }
            public override string ToString()
            {
                return $"{X},{Y},{Z}";
            }
            public int GetPotentialEnergy()
            {
                return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
            }
        }

        private readonly struct Velocity
        {
            public readonly int X;
            public readonly int Y;
            public readonly int Z;
            public Velocity(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }
            public override string ToString()
            {
                return $"{X},{Y},{Z}";
            }
            public int GetKineticEnergy()
            {
                return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
            }
        }

        private readonly struct Moon
        {
            public readonly Position Position;
            public readonly Velocity Velocity;
            public Moon(in Position position, in Velocity velocity)
            {
                Position = position;
                Velocity = velocity;
            }
            public int GetTotalEnergy()
            {
                return Position.GetPotentialEnergy() * Velocity.GetKineticEnergy();
            }
            public string GetXPositionAndVelocityString()
            {
                return $"{Position.X},{Velocity.X}";
            }
            public string GetYPositionAndVelocityString()
            {
                return $"{Position.Y},{Velocity.Y}";
            }
            public string GetZPositionAndVelocityString()
            {
                return $"{Position.Z},{Velocity.Z}";
            }
        }

        private static string GetXPositionAndVelocityString(List<Moon> moons)
        {
            var sb = new StringBuilder();
            foreach (var moon in moons)
            {
                sb = sb.Append(moon.GetXPositionAndVelocityString());
            }
            return sb.ToString();
        }

        private static string GetYPositionAndVelocityString(List<Moon> moons)
        {
            var sb = new StringBuilder();
            foreach (var moon in moons)
            {
                sb = sb.Append(moon.GetYPositionAndVelocityString());
            }
            return sb.ToString();
        }

        private static string GetZPositionAndVelocityString(List<Moon> moons)
        {
            var sb = new StringBuilder();
            foreach (var moon in moons)
            {
                sb = sb.Append(moon.GetZPositionAndVelocityString());
            }
            return sb.ToString();
        }

        private static int GetTotalEnergyInSystem(List<Moon> moons)
        {
            var total = 0;
            foreach (var moon in moons)
            {
                total += moon.GetTotalEnergy();
            }
            return total;
        }

        private static List<Moon> ApplyGravity(List<Moon> moons)
        {
            var changedMoons = new List<Moon>(moons.Count);
            foreach (var moon in moons)
            {
                var diffX = 0;
                var diffY = 0;
                var diffZ = 0;
                foreach (var otherMoon in moons)
                {
                    if (moon.Equals(otherMoon)) { continue; }
                    diffX += Math.Sign(otherMoon.Position.X - moon.Position.X);
                    diffY += Math.Sign(otherMoon.Position.Y - moon.Position.Y);
                    diffZ += Math.Sign(otherMoon.Position.Z - moon.Position.Z);
                }
                var changedMoon = new Moon(
                    position: moon.Position,
                    velocity: new Velocity(
                        x: moon.Velocity.X + diffX,
                        y: moon.Velocity.Y + diffY,
                        z: moon.Velocity.Z + diffZ
                    )
                );
                changedMoons.Add(changedMoon);
            }
            return changedMoons;
        }

        private static List<Moon> ApplyVelocity(List<Moon> moons)
        {
            var changedMoons = new List<Moon>(moons.Count);
            foreach (var moon in moons)
            {
                var changedMoon = new Moon(
                    position: new Position(
                        x: moon.Position.X + moon.Velocity.X,
                        y: moon.Position.Y + moon.Velocity.Y,
                        z: moon.Position.Z + moon.Velocity.Z
                    ),
                    velocity: moon.Velocity
                );
                changedMoons.Add(changedMoon);
            }
            return changedMoons;
        }

        private static List<Position> ParseInput(string input)
        {
            var positions = new List<Position>();
            foreach (var line in input.Trim().Split("\n"))
            {
                Match xMatch = Regex.Match(line, @"x=(-?\d+)");
                Match yMatch = Regex.Match(line, @"y=(-?\d+)");
                Match zMatch = Regex.Match(line, @"z=(-?\d+)");
                var position = new Position(
                    x: int.Parse(xMatch.Groups[1].Value),
                    y: int.Parse(yMatch.Groups[1].Value),
                    z: int.Parse(zMatch.Groups[1].Value)
                );
                positions.Add(position);
            }
            return positions;
        }

        private static List<Moon> CreateMoons(List<Position> positions)
        {
            var moons = new List<Moon>();
            foreach (var position in positions)
            {
                var moon = new Moon(

                    position: position,
                    velocity: new Velocity(x: 0, y: 0, z: 0)
                );
                moons.Add(moon);
            }
            return moons;
        }

        // Taken from: https://stackoverflow.com/a/20824923
        private static long LeastCommonMultiple(long a, long b)
        {
            return (a / GreatestCommonFactor(a, b)) * b;
        }

        // Taken from: https://stackoverflow.com/a/20824923
        private static long GreatestCommonFactor(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
    }
}