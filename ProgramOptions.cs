using System.Collections.Generic;

namespace hungarianAlgorithm
{
    public class ProgramOptions
    {
        public readonly bool Generate;
        public readonly string FileName;
        public readonly int N = 2;
        public readonly int K = 2;
        public readonly bool Verbose = false;

        public ProgramOptions(IReadOnlyList<string> args)
        {
            var token = 0;
            while (token != args.Count)
            {
                switch (args[token])
                {
                    case "-n":
                        ++token;
                        N = int.Parse(args[token]);
                        Generate = true;
                        break;
                    case "-k":
                        ++token;
                        K = int.Parse(args[token]);
                        Generate = true;
                        break;
                    case "-o":
                        ++token;
                        FileName = args[token];
                        if (!FileName.EndsWith(".txt"))
                            FileName += ".txt";
                        break;
                    case "-v":
                        Verbose = true;
                        break;
                    default:
                        FileName = args[token];
                        if (!FileName.EndsWith(".txt"))
                            FileName += ".txt";
                        break;
                }

                ++token;
            }
        }
    }
}