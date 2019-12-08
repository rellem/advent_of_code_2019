using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            foreach (var phaseSettings in GetPermutations(new List<int>() { 0, 1, 2, 3, 4 }, 5))
            {
                var inputSignal = 0;
                foreach (var phaseSetting in phaseSettings)
                {
                    var memory = ParseInput(input);
                    var machineInput = new BufferBlock<int>();
                    Trace.Assert(machineInput.Post(phaseSetting));
                    Trace.Assert(machineInput.Post(inputSignal));
                    var output = new BufferBlock<int>();
                    Execute(memory, machineInput, output).GetAwaiter().GetResult();
                    var outputSignal = output.Receive();
                    if (outputSignal > maxOutputSignal)
                    {
                        maxOutputSignal = outputSignal;
                    }
                    inputSignal = outputSignal;
                }

            }

            return maxOutputSignal.ToString();
        }

        public static string SolvePart2(string input)
        {
            var maxOutputSignal = 0;
            foreach (var phaseSettings in GetPermutations(new List<int>() { 5, 6, 7, 8, 9 }, 5))
            {
                var e2a = new BufferBlock<int>();
                Trace.Assert(e2a.Post(phaseSettings.ElementAt(0)));
                Trace.Assert(e2a.Post(0));

                var a2b = new BufferBlock<int>();
                Trace.Assert(a2b.Post(phaseSettings.ElementAt(1)));

                var b2c = new BufferBlock<int>();
                Trace.Assert(b2c.Post(phaseSettings.ElementAt(2)));

                var c2d = new BufferBlock<int>();
                Trace.Assert(c2d.Post(phaseSettings.ElementAt(3)));

                var d2e = new BufferBlock<int>();
                Trace.Assert(d2e.Post(phaseSettings.ElementAt(4)));

                var tasks = new List<Task>
                {
                    Execute(ParseInput(input), e2a, a2b),
                    Execute(ParseInput(input), a2b, b2c),
                    Execute(ParseInput(input), b2c, c2d),
                    Execute(ParseInput(input), c2d, d2e),
                    Execute(ParseInput(input), d2e, e2a)
                };

                Task.WhenAll(tasks).GetAwaiter().GetResult();

                var outputSignal = e2a.Receive();

                if (outputSignal > maxOutputSignal)
                {
                    maxOutputSignal = outputSignal;
                }
            }

            return maxOutputSignal.ToString();
        }

        private static class Instructions
        {
            public const int Add = 1;
            public const int Multiplication = 2;
            public const int ReadInput = 3;
            public const int WriteOutput = 4;
            public const int JumpIfTrue = 5;
            public const int JumpIfFalse = 6;
            public const int LessThan = 7;
            public const int EqualTo = 8;
            public const int Halt = 99;
        }

        private static int[] ParseInput(string input)
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

        // Taken from: https://stackoverflow.com/a/10629938
        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) { return list.Select(t => new T[] { t }); }
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        private static async Task Execute(int[] memory, BufferBlock<int> input, BufferBlock<int> output)
        {
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
                    case Instructions.Add:
                        memory[memory[ip + 3]] = ReadParam(memory, ip + 1, isImmediateMode1) + ReadParam(memory, ip + 2, isImmediateMode2);
                        ip += 4;
                        break;
                    case Instructions.Multiplication:
                        memory[memory[ip + 3]] = ReadParam(memory, ip + 1, isImmediateMode1) * ReadParam(memory, ip + 2, isImmediateMode2);
                        ip += 4;
                        break;
                    case Instructions.ReadInput:
                        memory[memory[ip + 1]] = await input.ReceiveAsync();
                        ip += 2;
                        break;
                    case Instructions.WriteOutput:
                        var paramValue = ReadParam(memory, ip + 1, isImmediateMode1);
                        Trace.Assert(output.Post(paramValue));
                        ip += 2;
                        break;
                    case Instructions.JumpIfTrue:
                        if (ReadParam(memory, ip + 1, isImmediateMode1) > 0)
                        {
                            ip = ReadParam(memory, ip + 2, isImmediateMode2);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;
                    case Instructions.JumpIfFalse:
                        if (ReadParam(memory, ip + 1, isImmediateMode1) == 0)
                        {
                            ip = ReadParam(memory, ip + 2, isImmediateMode2);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;
                    case Instructions.LessThan:
                        memory[memory[ip + 3]] = ReadParam(memory, ip + 1, isImmediateMode1) < ReadParam(memory, ip + 2, isImmediateMode2) ? 1 : 0;
                        ip += 4;
                        break;
                    case Instructions.EqualTo:
                        memory[memory[ip + 3]] = ReadParam(memory, ip + 1, isImmediateMode1) == ReadParam(memory, ip + 2, isImmediateMode2) ? 1 : 0;
                        ip += 4;
                        break;
                    case Instructions.Halt:
                        done = true;
                        break;
                    default:
                        throw new Exception($"Unexpected opcode: {op}");
                }
            }
        }

        private static int ReadParam(int[] memory, int pos, bool isImmediateMode)
        {
            return isImmediateMode ? memory[pos] : memory[memory[pos]];
        }
    }
}