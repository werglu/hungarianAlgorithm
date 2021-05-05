using System;
using System.IO;

namespace hungarianAlgorithm
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var algorithm = new Algorithm(1, 1);

            if (args.Length == 0)
            {
                Console.Error.WriteLine("Należy podać plik wejściowy");
                Environment.Exit(1);
            }
            if (args.Length == 6) //-k 2 -n 3 -o wygenerowany.txt
            {
                var file = args[5];
                if (!file.EndsWith(".txt"))
                {
                    file += ".txt";
                }

                using (var writer = new StreamWriter(@"..//..//In//" + file))
                {
                    writer.WriteLine($"{args[1]} {args[3]}");
                    for (int i=0; i<int.Parse(args[3]); i++)
                    {
                        Random rnd = new Random();
                        var x = rnd.Next(0, 1000);
                        var y = rnd.Next(0, 1000);
                        writer.WriteLine($"{i+1} {x/10} {y/10}");
                    }
                    for (int i = 0; i < int.Parse(args[3])*int.Parse(args[1]); i++)
                    {
                        Random rnd = new Random();
                        var x = rnd.Next(0, 1000);
                        var y = rnd.Next(0, 1000);
                        writer.WriteLine($"{i + 1} {x / 10} {y / 10}");
                    }
                }

                Console.WriteLine("Plik został wygenerowany");
                Environment.Exit(0);
            }

            var inFile = args[0];
            if (string.IsNullOrEmpty(inFile))
            {
                Console.Error.WriteLine("Pusta nazwa pliku");
                Environment.Exit(1);
            }

            if (!inFile.EndsWith(".txt"))
            {
                inFile += ".txt";
            }

            try
            {
                using (var reader = new StreamReader(@"..//..//In//" + inFile))
                {
                    var lineNumber = 1;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            throw new CorruptedInputFileException(lineNumber, "pusta linia");

                        var splittedLine = line.Split(' ');
                        if (splittedLine.Length != 2)
                            throw new CorruptedInputFileException(lineNumber,
                                "nie można odczytać ilości studni i domów");

                        var k = int.Parse(splittedLine[0]);
                        var n = int.Parse(splittedLine[1]); //liczba studni

                        algorithm = new Algorithm(n, k);

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
                    }

                    var solution = algorithm.Solve();


                    if (File.Exists(@"..//..//Out//" + inFile))
                    {
                        File.Delete(@"..//..//Out//" + inFile);
                    }
                    using (var writer = new StreamWriter(@"..//..//Out//" + inFile))
                    {
                        writer.WriteLine("ROZWIĄZANIE (dom -> studnia)");

                        var cost = 0.0;
                        foreach (var assigment in solution)
                        {
                            writer.WriteLine($"{assigment.HouseId + 1} -> {assigment.WellId + 1}");
                            cost += Algorithm.GetDistance(algorithm.Houses[assigment.HouseId], algorithm.Wells[assigment.WellId]);
                        }
                        writer.WriteLine(cost);
                    }

                    Console.ReadKey();
                }
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine($"Nie znaleziono pliku {inFile}");
                Environment.Exit(1);
            }
            catch (CorruptedInputFileException e)
            {
                Console.Error.WriteLine($"Uszkodzony plik wejściowy na linii {e.LineNumber}: {e.Message}");
                Environment.Exit(1);
            }
        }

        private class CorruptedInputFileException : Exception
        {
            public readonly int LineNumber;

            public CorruptedInputFileException(int lineNumber, string message) : base(message)
            {
                LineNumber = lineNumber;
            }
        }
    }
}