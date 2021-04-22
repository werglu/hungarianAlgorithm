using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace hungarianAlgorithm
{
    public class Algorithm
    {
        int n; //liczba studni
        int k;
        int kn; //liczba domów
        public Point[] wells;
        public Point[] houses;
        public double[,] distances;
        public int[,] markedZeros; //-1-wykreślone zero, 1-wybrane zero
        int markedZerosCount = 0;

        public Algorithm(int n, int k)
        {
            this.n = n;
            this.k = k;
            kn = k * n;
            wells = new Point[n];
            houses = new Point[kn];
            distances = new double[kn, kn];
            markedZeros = new int[kn, kn];
        }

        public void Solve()
        {
            CreateDistancesMatrix();
            ReduceRows();
            ReduceColumns();
            MarkZeros();

            DisplayMatrix();
        }

        public void DisplayMatrix()
        {
            for (int i = 0; i < kn; i++)
            {
                for (int j=0; j<kn; j++)
                {
                    Console.Write(distances[i, j].ToString() + ' ');
                }

                Console.WriteLine();
            }

        }
        public void MarkZeros()
        {
            int zerosCounter = 0; //liczba zer w danym wierszu/kolumnie
            int zeroIndex = 0; //index znalezionego zera w wierszu/kolumnie

            for (int i=0; i<kn; i++) //wiersze
            {
                zerosCounter = 0;
                zeroIndex = 0;

                for (int j=0; j<kn; j++)
                {
                    if (distances[i, j] == 0 && markedZeros[i, j] == 0)
                    {
                        zerosCounter++;
                        zeroIndex = j;
                    }
                }

                if (zerosCounter == 1)
                {
                    markedZeros[i, zeroIndex] = 1; //wybieramy zero
                    markedZerosCount++;

                    for (int j=0; j<kn; j++)
                    {
                        if (distances[j, zeroIndex] == 0 && markedZeros[j, zeroIndex] == 0)
                        {
                            markedZeros[j, zeroIndex] = -1; //wykreślamy zera w danej kolumnie
                        }
                    }
                }
            }

            for (int i=0; i<kn; i++) //kolumny
            {
                zerosCounter = 0;
                zeroIndex = 0;

                for (int j = 0; j < kn; j++)
                {
                    if (distances[j, i] == 0 && markedZeros[j, i] == 0)
                    {
                        zerosCounter++;
                        zeroIndex = j;
                    }
                }

                if (zerosCounter == 1)
                {
                    markedZeros[zeroIndex, i] = 1; //wybieramy zero
                    markedZerosCount++;

                    for (int j = 0; j < kn; j++)
                    {
                        if (distances[zeroIndex, j] == 0 && markedZeros[zeroIndex, j] == 0)
                        {
                            markedZeros[zeroIndex, j] = -1; //wykreślamy zera w danym wierszu
                        }
                    }
                }
            }
        }

        public void CreateDistancesMatrix()
        {
            int index = 0;

            for (int i=0; i<kn; i++)
            {
                index = 0;

                for (int j=0; j<n; j++)
                {
                    for (int l=0; l<k; l++)
                    {
                        distances[i, index] = GetDistance(houses[i], wells[j]);
                        index++;
                    }
                }
            }
        }

        public void ReduceRows()
        {
            double minValue = Double.MaxValue;

            for (int i=0; i<kn; i++)
            {
                minValue = Double.MaxValue;

                for (int j=0; j<kn; j++)
                {
                    if (distances[i, j] < minValue)
                    {
                        minValue = distances[i, j];
                    }
                }

                for (int j=0; j<kn; j++)
                {
                    distances[i, j] = Math.Round(distances[i, j] - minValue, 2);
                }
            }
        }

        public void ReduceColumns()
        {
            double minValue = Double.MaxValue;

            for (int i = 0; i < kn; i++)
            {
                minValue = Double.MaxValue;

                for (int j = 0; j < kn; j++)
                {
                    if (distances[j, i] < minValue)
                    {
                        minValue = distances[j, i];
                    }
                }

                for (int j = 0; j < kn; j++)
                {
                    distances[j, i] = Math.Round(distances[j, i] - minValue, 2);
                }
            }
        }

        public double GetDistance(Point p1, Point p2)
        {
            return Math.Round(Math.Sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y)), 2);
        }
    }
}
