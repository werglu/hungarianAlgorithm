using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
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
        private readonly System.Collections.BitArray _markedColumns;
        private readonly System.Collections.BitArray _markedRows;
        private int _markedZerosCount;

        public Algorithm(int n, int k)
        {
            _n = n;
            _k = k;
            _kn = k * n;
            Wells = new Point[n];
            Houses = new Point[_kn];
            _distances = new double[_kn, _kn];
            _markedZeros = new int[_kn, _kn];
            _markedColumns = new System.Collections.BitArray(_kn);
            _markedRows = new System.Collections.BitArray(_kn);
        }

        public IEnumerable<Assigment> Solve()
        {
            CreateDistancesMatrix();
            ReduceRows();
            ReduceColumns();
            DisplayMatrix();
            MarkZeros();
            while (_markedZerosCount != _kn)
            {
                MarkColumnsAndRows();
                ReduceCost();
                MarkZeros();
            }

            return FindSolution();
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
            _markedZerosCount = 0;
            ClearMarkedZeros();

            var marked = true;
            while (marked)
            {
                marked = false;
                for (var col = 0; col < _kn; col++) //kolumny
                {
                    var zerosCounter = 0; //liczba zer w danym wierszu/kolumnie
                    var zeroIndex = 0; //index znalezionego zera w wierszu/kolumnie

                    for (var row = 0; row < _kn; row++)
                    {
                        if (_distances[row, col] != 0 || _markedZeros[row, col] != 0)
                            continue;

                        zerosCounter++;
                        zeroIndex = row;
                    }

                    if (zerosCounter != 1)
                        continue;

                    marked = true;
                    _markedZeros[zeroIndex, col] = 1; //wybieramy zero
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

            for (var row = 0; row < _kn; row++) //wiersze
            {
                var zeroIndex = -1;

                for (var col = 0; col < _kn; col++)
                {
                    if (_distances[row, col] != 0 || _markedZeros[row, col] != 0)
                        continue;

                    zeroIndex = col;
                }

                if (zeroIndex == -1)
                    continue;

                _markedZeros[row, zeroIndex] = 1; //wybieramy zero
                _markedZerosCount++;

                for (var j = 0; j < _kn; j++)
                {
                    if (_distances[j, zeroIndex] == 0 && _markedZeros[j, zeroIndex] == 0)
                    {
                        _markedZeros[j, zeroIndex] = -1; //wykreślamy zera w danej kolumnie
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

        private void MarkColumnsAndRows()
        {
            _markedColumns.SetAll(false);
            _markedRows.SetAll(false);
            for (var row = 0; row < _kn; ++row)
            {
                var noAssignments = true;
                for (var col = 0; col < _kn; ++col)
                {
                    if (_markedZeros[row, col] != 1)
                        continue;

                    noAssignments = false;
                    break;
                }

                if (noAssignments)
                {
                    _markedRows[row] = true;
                }
            }

            var newMarkedRows = true;
            while (newMarkedRows)
            {
                newMarkedRows = false;
                for (var col = 0; col < _kn; ++col)
                {
                    if (_markedColumns[col])
                        continue;

                    var hasZeros = false;
                    for (var row = 0; row < _kn; ++row)
                    {
                        if (!_markedRows[row])
                            continue;
                        if (_distances[row, col] != 0)
                            continue;

                        hasZeros = true;
                        break;
                    }

                    if (!hasZeros)
                        continue;
                    
                    _markedColumns[col] = true;
                    for (var row = 0; row < _kn; ++row)
                    {
                        if (_markedRows[row])
                            continue;
                        if (_markedZeros[row, col] != 1)
                            continue;

                        newMarkedRows = true;
                        _markedRows[row] = true;
                    }
                }
            }
        }

        private void ClearMarkedZeros()
        {
            // Array.Fill z jakiegoś powodu nie działa :/
            for (var row = 0; row < _kn; ++row)
            {
                for (var col = 0; col < _kn; ++col)
                {
                    _markedZeros[row, col] = 0;
                }
            }
        }

        private double FindDelta()
        {
            var min = double.PositiveInfinity;
            for (var row = 0; row < _kn; ++row)
            {
                for (var col = 0; col < _kn; ++col)
                {
                    if (!_markedRows[row] || _markedColumns[col])
                        continue;

                    if (_distances[row, col] < min)
                    {
                        min = _distances[row, col];
                    }
                }
            }

            return min;
        }

        private void ReduceCost()
        {
            var delta = FindDelta();
            for (var row = 0; row < _kn; ++row)
            {
                for (var col = 0; col < _kn; ++col)
                {
                    if (_markedRows[row] && !_markedColumns[col])
                    {
                        _distances[row, col] -= delta;
                    }

                    if (!_markedRows[row] && _markedColumns[col])
                    {
                        _distances[row, col] += delta;
                    }
                }
            }
        }

        private IEnumerable<Assigment> FindSolution()
        {
            var result = new Assigment[_kn];
            var index = 0;
            for (var row = 0; row < _kn; ++row)
            {
                for (var col = 0; col < _kn; ++col)
                {
                    if (_markedZeros[row, col] != 1)
                        continue;

                    result[index] = new Assigment(col / _k, row);
                    ++index;
                    break;
                }
            }

            return result;
        }

        public double GetDistance(Point p1, Point p2)
        {
            return Math.Round(Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y)), 2);
        }
    }
}