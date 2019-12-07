using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NUnit.Framework;

namespace Day07
{
    public class Tests
    {
        [Test]
        public void SolvePart1_InputRellem()
        {
            var expectedAnswer = "272368";
            var actualAnswer = Solver.SolvePart1(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart2_InputRellem()
        {
            var expectedAnswer = "19741286";
            var actualAnswer = Solver.SolvePart2(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            var maxOutputSignal = 0;
            for (var p0 = 0; p0 < 5; p0++)
            {
                for (var p1 = 0; p1 < 5; p1++)
                {
                    if (p1 == p0) continue;
                    for (var p2 = 0; p2 < 5; p2++)
                    {
                        if (p2 == p1 || p2 == p0) continue;
                        for (var p3 = 0; p3 < 5; p3++)
                        {
                            if (p3 == p2 || p3 == p1 || p3 == p0) continue;
                            for (var p4 = 0; p4 < 5; p4++)
                            {
                                if (p4 == p3 || p4 == p2 || p4 == p1 || p4 == p0) continue;
                                var inputSignal = 0;
                                var phaseSettings = new List<int>() { p0, p1, p2, p3, p4 };
                                foreach (var phaseSetting in phaseSettings)
                                {
                                    var memory = parseInput(input);
                                    var inp = new List<int>() { phaseSetting, inputSignal };
                                    var output = executePart1(memory, inp);
                                    var outputSignal = output[output.Count - 1];
                                    if (outputSignal > maxOutputSignal)
                                    {
                                        maxOutputSignal = outputSignal;
                                    }
                                    inputSignal = outputSignal;
                                }
                            }
                        }
                    }
                }
            }

            return maxOutputSignal.ToString();
        }

        public static string SolvePart2(string input)
        {
            BufferBlock<int> buffer = new BufferBlock<int>();
            var maxOutputSignal = 0;
            for (var p0 = 5; p0 < 10; p0++)
            {
                for (var p1 = 5; p1 < 10; p1++)
                {
                    if (p1 == p0) continue;
                    for (var p2 = 5; p2 < 10; p2++)
                    {
                        if (p2 == p1 || p2 == p0) continue;
                        for (var p3 = 5; p3 < 10; p3++)
                        {
                            if (p3 == p2 || p3 == p1 || p3 == p0) continue;
                            for (var p4 = 5; p4 < 10; p4++)
                            {
                                if (p4 == p3 || p4 == p2 || p4 == p1 || p4 == p0) continue;
                                var phaseSettings = new List<int>() { p0, p1, p2, p3, p4 };

                                var e2a = new BufferBlock<int>();
                                e2a.Post(p0);
                                e2a.Post(0);

                                var a2b = new BufferBlock<int>();
                                a2b.Post(p1);

                                var b2c = new BufferBlock<int>();
                                b2c.Post(p2);

                                var c2d = new BufferBlock<int>();
                                c2d.Post(p3);

                                var d2e = new BufferBlock<int>();
                                d2e.Post(p4);

                                var tasks = new Task[5];
                                tasks[0] = executePart2(parseInput(input), e2a, a2b);
                                tasks[1] = executePart2(parseInput(input), a2b, b2c);
                                tasks[2] = executePart2(parseInput(input), b2c, c2d);
                                tasks[3] = executePart2(parseInput(input), c2d, d2e);
                                tasks[4] = executePart2(parseInput(input), d2e, e2a);

                                Task.WaitAll(tasks);

                                var outputSignal = e2a.Receive();

                                if (outputSignal > maxOutputSignal)
                                {
                                    maxOutputSignal = outputSignal;
                                }
                            }
                        }
                    }
                }
            }

            return maxOutputSignal.ToString();
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

        private static List<int> executePart1(int[] memory, List<int> input)
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

        private static async Task<bool> executePart2(int[] memory, BufferBlock<int> input, BufferBlock<int> output)
        {
            //var inputEnumerator = input.GetEnumerator();
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
                        memory[memory[ip + 1]] = await input.ReceiveAsync();
                        ip += 2;
                        break;
                    case 4:
                        var paramValue = readParam(memory, ip + 1, isImmediateMode1);
                        output.Post(paramValue);
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

            return true;
        }

        private static int readParam(int[] memory, int pos, bool isImmediateMode)
        {
            return isImmediateMode ? memory[pos] : memory[memory[pos]];
        }
    }
}