using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace Day05
{
    public class Tests
    {
        [Test]
        public void SolvePart1_InputRellem()
        {
            var expectedAnswer = "13547311";
            var actualAnswer = Solver.SolvePart1(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart2_InputRellem()
        {
            var expectedAnswer = "236453";
            var actualAnswer = Solver.SolvePart2(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            var memory = parseInput(input);
            var output = execute(memory, new List<int>() { 1 });
            var result = output[output.Count - 1];
            return result.ToString();
        }

        public static string SolvePart2(string input)
        {
            var memory = parseInput(input);
            var output = execute(memory, new List<int>() { 5 });
            var result = output[output.Count - 1];
            return result.ToString();
        }

        private static int[] parseInput(string input)
        {
            var stringOps = input.Trim().Split(",");
            var ops = new int[stringOps.Length];
            var i = 0;
            foreach (var stringOp in stringOps)
            {
                ops[i] = int.Parse(stringOp);
                i++;
            }
            return ops;
        }

        private static List<int> execute(int[] memory, List<int> input)
        {
            var inputEnumerator = input.GetEnumerator();
            var output = new List<int>();
            var ip = 0;
            var done = false;
            while (!done)
            {
                var opAndMode = memory[ip];
                var op = opAndMode % 100;
                var isImmediateMode1 = ((opAndMode / 100) % 10) > 0;
                var isImmediateMode2 = ((opAndMode / 1000) % 10) > 0;
                switch (op)
                {
                    case 1:
                        memory[memory[ip + 3]] = readParam(memory, ip + 1, isImmediateMode1) + readParam(memory, ip + 2, isImmediateMode2);
                        ip += 4;
                        break;
                    case 2:
                        memory[memory[ip + 3]] = readParam(memory, ip + 1, isImmediateMode1) * readParam(memory, ip + 2, isImmediateMode2);
                        ip += 4;
                        break;
                    case 3:
                        Debug.Assert(inputEnumerator.MoveNext());
                        memory[memory[ip + 1]] = inputEnumerator.Current;
                        //Console.WriteLine("read input: " + inputEnumerator.Current);
                        ip += 2;
                        break;
                    case 4:
                        var paramValue = readParam(memory, ip + 1, isImmediateMode1);
                        output.Add(paramValue);
                        //Console.WriteLine("write output: " + paramValue);
                        ip += 2;
                        break;
                    case 5:
                        if (readParam(memory, ip + 1, isImmediateMode1) > 0)
                        {
                            ip = readParam(memory, ip + 2, isImmediateMode2);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;
                    case 6:
                        if (readParam(memory, ip + 1, isImmediateMode1) == 0)
                        {
                            ip = readParam(memory, ip + 2, isImmediateMode2);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;
                    case 7:
                        memory[memory[ip + 3]] = readParam(memory, ip + 1, isImmediateMode1) < readParam(memory, ip + 2, isImmediateMode2) ? 1 : 0;
                        ip += 4;
                        break;
                    case 8:
                        memory[memory[ip + 3]] = readParam(memory, ip + 1, isImmediateMode1) == readParam(memory, ip + 2, isImmediateMode2) ? 1 : 0;
                        ip += 4;
                        break;
                    case 99:
                        done = true;
                        break;
                    default:
                        throw new Exception("Unexpected opcode: " + op);
                }
            }

            inputEnumerator.Dispose();

            return output;
        }

        private static int readParam(int[] memory, int pos, bool isImmediateMode)
        {
            return isImmediateMode ? memory[pos] : memory[memory[pos]];
        }
    }
}