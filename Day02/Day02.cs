using System;
using System.IO;
using NUnit.Framework;

namespace Day02
{
    public class Tests
    {
        [Test]
        public void SolvePart1_InputRellem()
        {
            var expectedAnswer = "5482655";
            var actualAnswer = Solver.SolvePart1(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }

        [Test]
        public void SolvePart2_InputRellem()
        {
            var expectedAnswer = "4967";
            var actualAnswer = Solver.SolvePart2(File.ReadAllText("input-rellem"));
            Assert.AreEqual(expectedAnswer, actualAnswer);
        }
    }

    public class Solver
    {
        public static string SolvePart1(string input)
        {
            var stringOps = input.Trim().Split(",");
            var ops = new Int32[stringOps.Length];
            var i = 0;
            foreach (var stringOp in stringOps)
            {
                ops[i] = Int32.Parse(stringOp);
                i++;
            }

            ops[1] = 12;
            ops[2] = 2;

            var pos = 0;
            var done = false;
            while (true)
            {
                var op = ops[pos];
                switch (op)
                {
                    case 1:
                        ops[ops[pos + 3]] = ops[ops[pos + 1]] + ops[ops[pos + 2]];
                        break;
                    case 2:
                        ops[ops[pos + 3]] = ops[ops[pos + 1]] * ops[ops[pos + 2]];
                        break;
                    case 99:
                        done = true;
                        break;
                    default:
                        throw new Exception("Unexpected opcode: " + op);
                }
                if (done)
                {
                    break;
                }
                pos += 4;
            }

            return ops[0].ToString();
        }

        public static string SolvePart2(string input)
        {
            var stringOps = input.Trim().Split(",");
            var ops = new Int32[stringOps.Length];
            var i = 0;
            foreach (var stringOp in stringOps)
            {
                ops[i] = Int32.Parse(stringOp);
                i++;
            }

            int? result = null;
            for (var noun = 0; noun <= 50; noun++)
            {
                for (var verb = 0; verb <= 99; verb++)
                {
                    var memory = (int[])ops.Clone();
                    memory[1] = noun;
                    memory[2] = verb;
                    var output = execute(memory);
                    if (output == 19690720)
                    {
                        result = 100 * noun + verb;
                    }
                }
            }

            return result.ToString();
        }

        private static int execute(int[] memory)
        {

            var ip = 0;
            var done = false;
            while (true)
            {
                var op = memory[ip];
                switch (op)
                {
                    case 1:
                        memory[memory[ip + 3]] = memory[memory[ip + 1]] + memory[memory[ip + 2]];
                        ip += 4;
                        break;
                    case 2:
                        memory[memory[ip + 3]] = memory[memory[ip + 1]] * memory[memory[ip + 2]];
                        ip += 4;
                        break;
                    case 99:
                        done = true;
                        break;
                    default:
                        //return 987;
                        throw new Exception("Unexpected opcode: " + op);
                }
                if (done)
                {
                    break;
                }
            }

            return memory[0];
        }
    }
}