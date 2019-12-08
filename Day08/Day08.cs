namespace Day07
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

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

    public class SpaceImageFormat
    {
        public const char Black = '0';
        public const char White = '1';
        public const char Transparent = '2';
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            const int width = 25;
            const int height = 6;
            var layers = GetLayers(input, width, height);
            int? min0 = null;
            int? result = null;
            foreach (var layer in layers)
            {
                var num0 = layer.Where(x => x == SpaceImageFormat.Black).Count();
                if (min0 == null || num0 < min0)
                {
                    min0 = num0;
                    var num1 = layer.Where(x => x == SpaceImageFormat.White).Count();
                    var num2 = layer.Where(x => x == SpaceImageFormat.Transparent).Count();
                    result = num1 * num2;
                }
            }

            return result!.Value.ToString();
        }

        public static string SolvePart2(string input)
        {
            const int width = 25;
            const int height = 6;
            var layers = GetLayers(input, width, height);
            var image = (char[])layers[0].Clone();
            foreach (var layer in layers)
            {
                for (var i = 0; i < layer.Length; i++)
                {
                    if (image[i] == SpaceImageFormat.Transparent)
                    {
                        image[i] = layer[i];
                    }
                }
            }

            var imageFilename = $"Day08-Part2-{DateTime.Now.Ticks}.bmp";
            SaveAsBitmap(image, width, height, imageFilename);

            return Ocr(image);
        }

        private static void SaveAsBitmap(char[] image, int width, int height, string filename)
        {
            var bitmap = new Bitmap(width, height);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixel = image[(y * width) + x];
                    var color = pixel switch
                    {
                        SpaceImageFormat.Black => Color.Black,
                        SpaceImageFormat.White => Color.White,
                        SpaceImageFormat.Transparent => Color.Red,
                        _ => throw new Exception("Unexpected color: " + pixel),
                    };
                    bitmap.SetPixel(x, y, color);
                }
            }

            bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private static string Ocr(char[] image)
        {
            var imageString = string.Join(string.Empty, image);
            return imageString switch
            {
                "111101110000110011001110000010100100001010010100100010011100000101001011100010001001000010111101001010000100101001010010100101111011100011001001011100" => "ZBJAB",
                _ => throw new Exception("Unable to OCR: " + imageString),
            };
        }

        private static List<char[]> GetLayers(string input, int width, int height)
        {
            var chars = input.Trim().ToCharArray();
            var pixelsPerLayer = width * height;
            var numLayers = chars.Length / pixelsPerLayer;
            Debug.Assert((chars.Length % pixelsPerLayer) == 0, "Unexpected");
            var layers = new List<char[]>();
            for (var layerIndex = 0; layerIndex < numLayers; layerIndex++)
            {
                layers.Add(input.Substring(layerIndex * pixelsPerLayer, pixelsPerLayer).ToCharArray());
            }

            return layers;
        }
    }
}
