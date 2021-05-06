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
        private readonly int[,] _backtrack;
        private readonly System.Collections.BitArray _markedColumns;
        private readonly System.Collections.BitArray _markedRows;
        private int _markedZerosCount;
        private bool _verbose = false;

        public Algorithm(int n, int k)
        {
            _n = n;
            _k = k;
            _kn = k * n;
            Wells = new Point[n];
            Houses = new Point[_kn];
            _distances = new double[_kn, _kn];
            _markedZeros = new int[_kn, _kn];
            _backtrack = new int[_kn, _kn];
            _markedColumns = new System.Collections.BitArray(_kn);
            _markedRows = new System.Collections.BitArray(_kn);
        }

        public IEnumerable<Assignment> Solve()
        {
            CreateDistancesMatrix();
            ReduceRows();
            ReduceColumns();
            MarkZeros();
            if (_verbose)
                Console.Write($"{_markedZerosCount}/{_kn}\r");
            while (_markedZerosCount != _kn)
            {
                ReduceCost();
                MarkZeros();
                if (_verbose)
                    Console.Write($"{_markedZerosCount}/{_kn}\r");
            }

            return FindSolution();
        }

        private void DisplayMatrix()
        {
            Console.WriteLine();
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

            if (_markedZerosCount == _kn)
                return;
            
            for (;;)
            {
                MarkColumnsAndRows();
                
                // Sprawdzamy czy jest jakaś wolna kolumna, która nie jest oznaczona
                var btCol = -1;
                var btRow = -1;
                
                for (var col = 0; col < _kn; ++col)
                {
                    if (!_markedColumns[col])
                        continue;
                    for (var row = 0; row < _kn; ++row)
                    {
                        if (!_markedRows[row])
                            continue;
                        if (_distances[row, col] != 0)
                            continue;
                        if (_backtrack[row, col] == -1)
                            continue;

                        if (_markedZeros[row, col] == 1)
                        {
                            btCol = -1;
                            break;
                        }
                        
                        btCol = col;
                        btRow = row;
                    }

                    if (btCol != -1)
                        break;
                }

                if (btCol == -1)
                    return;

                var colChange = true;

                for(;;)
                {
                    var bt = _backtrack[btRow, btCol];
                    
                    if (colChange)
                    {
                        _markedZeros[btRow, btCol] = 1;
                        ++_markedZerosCount;
                        btCol = bt;
                        colChange = false;
                    }
                    else
                    {
                        _markedZeros[btRow, btCol] = -1;
                        --_markedZerosCount;
                        btRow = bt;
                        colChange = true;
                    }
                    
                    if (bt == -1)
                        break;
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
                for (var col = 0; col < _kn; ++col)
                {
                    _backtrack[row, col] = -1;
                }
            }
            
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

            var colBacktrack = new int[_kn];
            for (var i = 0; i < _kn; ++i)
                colBacktrack[i] = -1;

            var newMarkedRows = true;
            while (newMarkedRows)
            {
                newMarkedRows = false;
                for (var col = 0; col < _kn; ++col)
                {
                    if (_markedColumns[col])
                        continue;

                    var hasZeros = false;
                    var backtrackRow = -1;
                    for (var row = 0; row < _kn; ++row)
                    {
                        if (!_markedRows[row])
                            continue;
                        if (_distances[row, col] != 0)
                            continue;

                        _backtrack[row, col] = colBacktrack[row];
                        backtrackRow = row;
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

                        colBacktrack[row] = col;
                        _backtrack[row, col] = backtrackRow;
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

        private IEnumerable<Assignment> FindSolution()
        {
            var result = new Assignment[_kn];
            var index = 0;
            for (var row = 0; row < _kn; ++row)
            {
                for (var col = 0; col < _kn; ++col)
                {
                    if (_markedZeros[row, col] != 1)
                        continue;

                    result[index] = new Assignment(col / _k, row);
                    ++index;
                    break;
                }
            }

            return result;
        }

        public void SetVerbose()
        {
            _verbose = true;
        }

        public double GetCost(IEnumerable<Assignment> assignments)
        {
            return assignments.Sum(assigment => GetDistance(Houses[assigment.HouseId], Wells[assigment.WellId]));
        }

        private static double GetDistance(Point p1, Point p2)
        {
            return Math.Round(Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y)), 2);
        }
    }
}