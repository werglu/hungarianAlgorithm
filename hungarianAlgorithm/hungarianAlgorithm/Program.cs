using System;
using System.IO;

namespace hungarianAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            int k = 0;
            int n = 0; //liczba studni
            Point[] wells;
            Point[] houses;

            Console.WriteLine("Podaj nazwę pliku z katalogu 'In'");
            var inFile = Console.ReadLine();
            if (!inFile.Contains(".txt"))
            {
                inFile += ".txt";
            }

            using (var reader = new StreamReader(@"..\..\..\In\" + inFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var splittedLine = line.Split(' ');
                    k = int.Parse(splittedLine[0]);
                    n = int.Parse(splittedLine[1]);
                    wells = new Point[n];
                    houses = new Point[n * k];

                    for (int i = 0; i < n; i++)
                    {
                        line = reader.ReadLine();
                        splittedLine = line.Split(' ');
                        wells[i] = new Point(double.Parse(splittedLine[1]), double.Parse(splittedLine[2]));
                    }

                    for (int i = 0; i < k * n; i++)
                    {
                        line = reader.ReadLine();
                        splittedLine = line.Split(' ');
                        houses[i] = new Point(double.Parse(splittedLine[1]), double.Parse(splittedLine[2]));
                    }
                }
            }
        }
    }
}
