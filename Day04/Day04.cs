using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Day04
{
    public class Tests
    {
        [Test]
        public void SolvePart1_InputRellem()
        {
            var expectedAnswer = "966";
            var actualAnswer = Solver.SolvePart1(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart2_InputRellem()
        {
            var expectedAnswer = "628";
            var actualAnswer = Solver.SolvePart2(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            var start = int.Parse(input.Split("-")[0]);
            var end = int.Parse(input.Split("-")[1]);
            var numValid = 0;
            for (var password = start; password <= end; password++)
            {
                if (isValidPassword1(password))
                {
                    numValid++;
                }
            }
            return numValid.ToString();
        }

        public static string SolvePart2(string input)
        {
            var start = int.Parse(input.Split("-")[0]);
            var end = int.Parse(input.Split("-")[1]);
            var numValid = 0;
            for (var password = start; password <= end; password++)
            {
                if (isValidPassword2(password))
                {
                    numValid++;
                }
            }
            return numValid.ToString();
        }

        static bool isValidPassword1(int password)
        {
            var foundRepeating = false;
            int? prevDigit = null;
            foreach (var digit in getDigits(password))
            {
                if (digit == prevDigit)
                {
                    foundRepeating = true;
                }
                if (digit < prevDigit)
                {
                    return false;
                }
                prevDigit = digit;
            }
            return foundRepeating;
        }

        static bool isValidPassword2(int password)
        {
            var foundRepeating = false;
            var digits = getDigits(password);
            for (var i = 1; i < digits.Count; i++)
            {
                if (digits[i] < digits[i - 1])
                {
                    return false;
                }
                if (digits[i] == digits[i - 1] && (i - 2 < 0 || digits[i] != digits[i - 2]) && (i + 1 >= digits.Count || digits[i] != digits[i + 1]))
                {
                    foundRepeating = true;
                }
            }
            return foundRepeating;
        }

        static List<int> getDigits(int num)
        {
            var str = num.ToString();
            var digits = new List<int>(str.Length);
            for (var i = 0; i < str.Length; i++)
            {
                digits.Add(int.Parse(str.AsSpan(i, 1)));
            }
            return digits;
        }
    }
}