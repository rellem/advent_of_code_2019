using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NUnit.Framework;

namespace Day09
{
    public class Tests
    {
        [Test]
        public void SolvePart1_InputRellem()
        {
            var expectedAnswer = "3512778005";
            var actualAnswer = Solver.SolvePart1(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart1_Example1()
        {
            var expectedAnswer = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            var actualAnswer = Solver.SolvePart1("109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99");
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart1_Example2()
        {
            var expectedAnswer = "1219070632396864";
            var actualAnswer = Solver.SolvePart1("1102,34915192,34915192,7,4,7,99,0");
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart1_Example3()
        {
            var expectedAnswer = "1125899906842624";
            var actualAnswer = Solver.SolvePart1("104,1125899906842624,99");
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart2_InputRellem()
        {
            var expectedAnswer = "35920";
            var actualAnswer = Solver.SolvePart2(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            var memory = ParseInput(input);
            var machineInput = new BufferBlock<long>();
            Trace.Assert(machineInput.Post(1));
            var output = new BufferBlock<long>();
            Execute(memory, machineInput, output).GetAwaiter().GetResult();
            Trace.Assert(output.TryReceiveAll(out IList<long> outputList));

            return string.Join(",", outputList);
        }

        public static string SolvePart2(string input)
        {
            var memory = ParseInput(input);
            var machineInput = new BufferBlock<long>();
            Trace.Assert(machineInput.Post(2));
            var output = new BufferBlock<long>();
            Execute(memory, machineInput, output).GetAwaiter().GetResult();
            Trace.Assert(output.TryReceiveAll(out IList<long> outputList));

            return string.Join(",", outputList);
        }

        private static class Instructions
        {
            public const long Add = 1;
            public const long Multiplication = 2;
            public const long ReadInput = 3;
            public const long WriteOutput = 4;
            public const long JumpIfTrue = 5;
            public const long JumpIfFalse = 6;
            public const long LessThan = 7;
            public const long EqualTo = 8;
            public const long AdjustRelativeBase = 9;
            public const long Halt = 99;
        }

        private enum ParameterMode
        {
            Position,
            Immediate,
            Relative
        }

        private static long[] ParseInput(string input)
        {
            var stringOps = input.Trim().Split(",");
            var ops = new long[100000];
            var i = 0;
            foreach (var stringOp in stringOps)
            {
                ops[i] = long.Parse(stringOp);
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

        private static async Task Execute(long[] memory, BufferBlock<long> input, BufferBlock<long> output)
        {
            var ip = 0L;
            var rb = 0L;
            var done = false;
            while (!done)
            {
                var opAndMode = memory[ip];
                var op = opAndMode % 100;
                var positionMode1 = ((opAndMode / 100) % 10) switch
                {
                    0 => ParameterMode.Position,
                    1 => ParameterMode.Immediate,
                    2 => ParameterMode.Relative,
                    _ => throw new Exception("Unexpected")
                };
                var positionMode2 = ((opAndMode / 1000) % 10) switch
                {
                    0 => ParameterMode.Position,
                    1 => ParameterMode.Immediate,
                    2 => ParameterMode.Relative,
                    _ => throw new Exception("Unexpected")
                };
                var positionMode3 = ((opAndMode / 10000) % 10) switch
                {
                    0 => ParameterMode.Position,
                    1 => ParameterMode.Immediate,
                    2 => ParameterMode.Relative,
                    _ => throw new Exception("Unexpected")
                };
                var off1 = positionMode1 == ParameterMode.Relative ? rb : 0;
                //var off2 = positionMode2 == ParameterMode.Relative ? rb : 0;
                var off3 = positionMode3 == ParameterMode.Relative ? rb : 0;
                switch (op)
                {
                    case Instructions.Add:
                        memory[memory[ip + 3] + off3] = ReadParam(memory, ip + 1, rb, positionMode1) + ReadParam(memory, ip + 2, rb, positionMode2);
                        ip += 4;
                        break;
                    case Instructions.Multiplication:
                        memory[memory[ip + 3] + off3] = ReadParam(memory, ip + 1, rb, positionMode1) * ReadParam(memory, ip + 2, rb, positionMode2);
                        ip += 4;
                        break;
                    case Instructions.ReadInput:
                        //Console.WriteLine($"Read input");
                        memory[memory[ip + 1] + off1] = await input.ReceiveAsync();
                        ip += 2;
                        break;
                    case Instructions.WriteOutput:
                        var paramValue = ReadParam(memory, ip + 1, rb, positionMode1);
                        //Console.WriteLine($"Write output: {paramValue}");
                        Trace.Assert(output.Post(paramValue));
                        ip += 2;
                        break;
                    case Instructions.JumpIfTrue:
                        if (ReadParam(memory, ip + 1, rb, positionMode1) > 0)
                        {
                            ip = ReadParam(memory, ip + 2, rb, positionMode2);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;
                    case Instructions.JumpIfFalse:
                        if (ReadParam(memory, ip + 1, rb, positionMode1) == 0)
                        {
                            ip = ReadParam(memory, ip + 2, rb, positionMode2);
                        }
                        else
                        {
                            ip += 3;
                        }
                        break;
                    case Instructions.LessThan:
                        memory[memory[ip + 3] + off3] = ReadParam(memory, ip + 1, rb, positionMode1) < ReadParam(memory, ip + 2, rb, positionMode2) ? 1 : 0;
                        ip += 4;
                        break;
                    case Instructions.EqualTo:
                        memory[memory[ip + 3] + off3] = ReadParam(memory, ip + 1, rb, positionMode1) == ReadParam(memory, ip + 2, rb, positionMode2) ? 1 : 0;
                        ip += 4;
                        break;
                    case Instructions.AdjustRelativeBase:
                        rb += ReadParam(memory, ip + 1, rb, positionMode1);
                        ip += 2;
                        break;
                    case Instructions.Halt:
                        done = true;
                        break;
                    default:
                        throw new Exception($"Unexpected opcode: {op}");
                }
            }
            output.Complete();
        }

        private static long ReadParam(long[] memory, long pos, long rb, ParameterMode parameterMode)
        {
            return parameterMode switch
            {
                ParameterMode.Position => memory[memory[pos]],
                ParameterMode.Immediate => memory[pos],
                ParameterMode.Relative => memory[rb + memory[pos]],
                _ => throw new Exception("Bad mode"),
            };
        }
    }
}