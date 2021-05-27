using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;



namespace hungarianAlgorithm
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
           // Stopwatch sw = new Stopwatch();

            var inputDir = Util.GetEnv("INPUT_DIR", "../../In");
            var outputDir = Util.GetEnv("OUTPUT_DIR", "../../Out");

            var options = new ProgramOptions(args);

            if (string.IsNullOrEmpty(options.FileName))
            {
                Console.Error.WriteLine("Należy podać plik wejściowy");
                Environment.Exit(1);
            }

            if (options.Generate)
            {
                using (var writer = new StreamWriter($"{inputDir}/{options.FileName}"))
                {
                    Util.GenerateProblem(writer, options.K, options.N);
                }

                Console.Error.WriteLine("Plik został wygenerowany");
                Environment.Exit(0);
            }

            try
            {
                using (var reader = new StreamReader($"{inputDir}/{options.FileName}"))
                {
                    var algorithm = Util.ParseProblem(reader);
                    if (options.Verbose)
                        algorithm.SetVerbose();
                    //sw.Start();
                    var solution = algorithm.Solve();
                    //sw.Stop();
                    Console.WriteLine("Wyniki zostały zapisane do pliku");
                   // Console.WriteLine("Elapsed={0}", sw.Elapsed);

                    using (var writer = new StreamWriter($"{outputDir}/{options.FileName}"))
                    {
                        var assignments = solution as Assignment[] ?? solution.ToArray();
                        Util.WriteSolution(writer, assignments);
                        writer.WriteLine(algorithm.GetCost(assignments));
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine($"Nie znaleziono pliku {options.FileName}");
                Environment.Exit(1);
            }
            catch (Util.CorruptedInputFileException e)
            {
                Console.Error.WriteLine($"Uszkodzony plik wejściowy na linii {e.LineNumber}: {e.Message}");
                Environment.Exit(1);
            }
        }
    }
}