using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NUnit.Framework;

namespace Day07
{
    public class Tests
    {
        [Test]
        public void SolvePart1_InputRellem()
        {
            var expectedAnswer = "2806";
            var actualAnswer = Solver.SolvePart1(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart2_InputRellem()
        {
            var expectedAnswer = "ZBJAB";
            var actualAnswer = Solver.SolvePart2(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            var digits = getDigits(input.Trim());
            var layerSize = 25 * 6;
            var layers = digits.Count / layerSize;
            Console.WriteLine(digits.Count / (layerSize * 1.0));
            var min0 = int.MaxValue;
            var result = -1;
            for (int layer = 0; layer < layers; layer++)
            {
                var num0 = 0;
                var num1 = 0;
                var num2 = 0;
                for (var i = layer * layerSize; i < (layer + 1) * layerSize; i++)
                {
                    var digit = digits[i];
                    if (digit == 0)
                    {
                        num0++;
                    }
                    else if (digit == 1)
                    {
                        num1++;
                    }
                    else if (digit == 2)
                    {
                        num2++;
                    }
                }
                if (num0 < min0)
                {
                    result = num1 * num2;
                    min0 = num0;
                }
            }
            return result.ToString();
        }

        public static string SolvePart2(string input)
        {

            var digits = getDigits(input.Trim());
            var width = 25;
            var height = 6;
            var layerSize = width * height;
            var layers = digits.Count / layerSize;
            var image = new int[layerSize];
            for (var k = 0; k < image.Length; k++)
            {
                image[k] = 2;
            }
            for (int layer = 0; layer < layers; layer++)
            {
                for (var i = layer * layerSize; i < (layer + 1) * layerSize; i++)
                {
                    if (image[i % layerSize] == 2)
                    {
                        image[i % layerSize] = digits[i];
                    }
                }
            }

            var bitmap = new Bitmap(width, height);
            var j = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    bitmap.SetPixel(x, y, image[j] == 0 ? Color.Black : Color.White);
                    j++;
                }
            }
            bitmap.Save("Day08-Part2.bmp");

            // TODO: OCR?
            return string.Join("", image);
        }

        static List<int> getDigits(string str)
        {
            var digits = new List<int>(str.Length);
            for (var i = 0; i < str.Length; i++)
            {
                digits.Add(int.Parse(str.AsSpan(i, 1)));
            }
            return digits;
        }
    }
}