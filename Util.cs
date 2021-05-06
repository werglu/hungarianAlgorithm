using System;
using System.Collections.Generic;
using System.IO;

namespace hungarianAlgorithm
{
    internal static class Util
    {
        public static string GetEnv(string name, string alternative)
        {
            var env = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrEmpty(env) ? alternative : env;
        }

        public static Algorithm ParseProblem(TextReader reader)
        {
            var lineNumber = 1;
            var line = reader.ReadLine();
            if (string.IsNullOrEmpty(line))
                throw new CorruptedInputFileException(lineNumber, "pusta linia");

            var splittedLine = line.Split(' ');
            if (splittedLine.Length != 2)
                throw new CorruptedInputFileException(lineNumber,
                    "nie można odczytać ilości studni i domów");

            var k = int.Parse(splittedLine[0]);
            var n = int.Parse(splittedLine[1]); //liczba studni

            var algorithm = new Algorithm(n, k);

            for (var i = 0; i < n; i++)
            {
                ++lineNumber;
                line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                    throw new CorruptedInputFileException(lineNumber, "pusta linia");

                splittedLine = line.Split(' ');
                if (splittedLine.Length != 3)
                    throw new CorruptedInputFileException(lineNumber, "nieprawidłowa liczba kolumn");

                algorithm.Wells[i] =
                    new Point(double.Parse(splittedLine[1]), double.Parse(splittedLine[2]));
            }

            for (var i = 0; i < k * n; i++)
            {
                ++lineNumber;
                line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                    throw new CorruptedInputFileException(lineNumber, "pusta linia");

                splittedLine = line.Split(' ');
                if (splittedLine.Length != 3)
                    throw new CorruptedInputFileException(lineNumber, "nieprawidłowa liczba kolumn");

                algorithm.Houses[i] =
                    new Point(double.Parse(splittedLine[1]), double.Parse(splittedLine[2]));
            }

            return algorithm;
        }

        public static void WriteSolution(TextWriter w, IEnumerable<Assignment> assignments)
        {
            foreach (var assigment in assignments)
            {
                w.WriteLine($"{assigment.HouseId + 1} -> {assigment.WellId + 1}");
            }
        }

        public static void GenerateProblem(TextWriter w, int k, int n)
        {
            w.WriteLine($"{k} {n}");
            var rnd = new Random();
            for (var i = 0; i < n; i++)
            {
                var x = rnd.Next(0, 1000);
                var y = rnd.Next(0, 1000);
                w.WriteLine($"{i + 1} {x / 10} {y / 10}");
            }

            for (var i = 0; i < n * k; i++)
            {
                var x = rnd.Next(0, 1000);
                var y = rnd.Next(0, 1000);
                w.WriteLine($"{i + 1} {x / 10} {y / 10}");
            }
        }

        public class CorruptedInputFileException : Exception
        {
            public readonly int LineNumber;

            public CorruptedInputFileException(int lineNumber, string message) : base(message)
            {
                LineNumber = lineNumber;
            }
        }
    }
}