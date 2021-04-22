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
        private readonly int _n; //liczba studni
        private readonly int _k;
        private readonly int _kn; //liczba domów
        public readonly Point[] Wells;
        public readonly Point[] Houses;
        private readonly double[,] _distances;
        private readonly int[,] _markedZeros; //-1-wykreślone zero, 1-wybrane zero
        private int _markedZerosCount = 0;

        public Algorithm(int n, int k)
        {
            _n = n;
            _k = k;
            _kn = k * n;
            Wells = new Point[n];
            Houses = new Point[_kn];
            _distances = new double[_kn, _kn];
            _markedZeros = new int[_kn, _kn];
        }

        public void Solve()
        {
            CreateDistancesMatrix();
            ReduceRows();
            ReduceColumns();
            MarkZeros();

            DisplayMatrix();
        }

        private void DisplayMatrix()
        {
            for (var i = 0; i < _kn; i++)
            {
                for (var j = 0; j < _kn; j++)
                {
                    Console.Write($"{_distances[i, j]} ");
                }

                Console.WriteLine();
            }
        }

        private void MarkZeros()
        {
            var zerosCounter = 0; //liczba zer w danym wierszu/kolumnie
            var zeroIndex = 0; //index znalezionego zera w wierszu/kolumnie

            for (var i = 0; i < _kn; i++) //wiersze
            {
                zerosCounter = 0;
                zeroIndex = 0;

                for (var j = 0; j < _kn; j++)
                {
                    if (_distances[i, j] != 0 || _markedZeros[i, j] != 0)
                        continue;

                    zerosCounter++;
                    zeroIndex = j;
                }

                if (zerosCounter != 1)
                    continue;

                _markedZeros[i, zeroIndex] = 1; //wybieramy zero
                _markedZerosCount++;

                for (var j = 0; j < _kn; j++)
                {
                    if (_distances[j, zeroIndex] == 0 && _markedZeros[j, zeroIndex] == 0)
                    {
                        _markedZeros[j, zeroIndex] = -1; //wykreślamy zera w danej kolumnie
                    }
                }
            }

            for (var i = 0; i < _kn; i++) //kolumny
            {
                zerosCounter = 0;
                zeroIndex = 0;

                for (var j = 0; j < _kn; j++)
                {
                    if (_distances[j, i] != 0 || _markedZeros[j, i] != 0)
                        continue;

                    zerosCounter++;
                    zeroIndex = j;
                }

                if (zerosCounter != 1)
                    continue;

                _markedZeros[zeroIndex, i] = 1; //wybieramy zero
                _markedZerosCount++;

                for (var j = 0; j < _kn; j++)
                {
                    if (_distances[zeroIndex, j] == 0 && _markedZeros[zeroIndex, j] == 0)
                    {
                        _markedZeros[zeroIndex, j] = -1; //wykreślamy zera w danym wierszu
                    }
                }
            }
        }

        private void CreateDistancesMatrix()
        {
            for (var i = 0; i < _kn; i++)
            {
                var index = 0;

                for (var j = 0; j < _n; j++)
                {
                    for (var l = 0; l < _k; l++)
                    {
                        _distances[i, index] = GetDistance(Houses[i], Wells[j]);
                        index++;
                    }
                }
            }
        }

        private void ReduceRows()
        {
            for (var i = 0; i < _kn; i++)
            {
                var minValue = double.MaxValue;

                for (var j = 0; j < _kn; j++)
                {
                    if (_distances[i, j] < minValue)
                    {
                        minValue = _distances[i, j];
                    }
                }

                for (var j = 0; j < _kn; j++)
                {
                    _distances[i, j] = Math.Round(_distances[i, j] - minValue, 2);
                }
            }
        }

        private void ReduceColumns()
        {
            for (var i = 0; i < _kn; i++)
            {
                var minValue = double.MaxValue;

                for (var j = 0; j < _kn; j++)
                {
                    if (_distances[j, i] < minValue)
                    {
                        minValue = _distances[j, i];
                    }
                }

                for (var j = 0; j < _kn; j++)
                {
                    _distances[j, i] = Math.Round(_distances[j, i] - minValue, 2);
                }
            }
        }

        private static double GetDistance(Point p1, Point p2)
        {
            return Math.Round(Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y)), 2);
        }
    }
}