using System;
using System.Collections;
using System.Collections.Generic;

namespace BlakeHashTests
{
    /// <summary>
    /// Класс для проверки псевдослучайных последовательностей
    /// Использует тест рангов бинарных матриц
    /// </summary>
    public class BinaryMatrixRankAlg : IPValueAlg
    {
        private int _rows;
        private int _columns;
        private double[] _consts = new double[]
        {
            0.2888,
            0.5776,
            0.1336
        };

        /// <param name="rows">Количество строк матрицы</param>
        /// <param name="columns">Количество столбцов матрицы</param>
        public BinaryMatrixRankAlg(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
        }

        /// <summary>
        /// Метод, считающий p-value на основе рангов бинарных матриц
        /// </summary>
        /// <param name="hash">Псевдослучайная последовательность</param>
        /// <returns>p-value</returns>
        public double getPValue(BitArray hash)
        {
            var matrixSize = _rows * _columns;
            var matrixNumber = hash.Length / (matrixSize);
            var matrixs = new List<int[,]>();

            // построение списка матриц из последовательности
            for (int i = 0; i < matrixNumber; i++)
            {
                var matrix = new int[_rows, _columns];

                for (int j = 0; j < _rows; j++)
                {
                    for (int k = 0; k < _columns; k++)
                    {
                        matrix[j, k] = hash[i * matrixSize + j * _rows + k] ? 1 : 0;
                    }
                }

                matrixs.Add(matrix);
            }

            var numberOfMatrixWithMRank = 0;
            var numberOfMatrixWithM_1Rank = 0;

            for (int i = 0; i < matrixNumber; i++)
            {
                var rank = Rank(matrixs[i], _rows, _columns);

                if (rank == _rows)
                    numberOfMatrixWithMRank++;
                else if (rank == _rows - 1)
                    numberOfMatrixWithM_1Rank++;
            }

            var numberOfOthersMatrix = matrixNumber - numberOfMatrixWithMRank - numberOfMatrixWithM_1Rank;

            // распределение суммы квадратов k = 3 независимых стандартных нормальных случайных величин
            var x2 = Math.Pow(numberOfMatrixWithMRank - _consts[0] * matrixNumber, 2) / (_consts[0] * matrixNumber) +
                Math.Pow(numberOfMatrixWithM_1Rank - _consts[1] * matrixNumber, 2) / (_consts[1] * matrixNumber) +
                Math.Pow(numberOfOthersMatrix - _consts[2] * matrixNumber, 2) / (_consts[2] * matrixNumber);
            var pValue = Math.Exp(-x2 / 2);

            return pValue;
        }

        /// <summary>
        /// Метод, считающий ранг матрицы
        /// </summary>
        /// <param name="matrix">Матрица</param>
        /// <param name="n">Количество строк</param>
        /// <param name="m">Количество столбцов</param>
        /// <returns>Ранг матрицы</returns>
        /// https://e-maxx.ru/algo/matrix_rank
        private int Rank(int[,] matrix, int n, int m)
        {
            const double EPS = 1E-9;
            int rank = Math.Max(n, m);
            var line_used = new bool[n];

            for (int i = 0; i < m; ++i)
            {
                int j;

                for (j = 0; j < n; ++j)
                    if (!line_used[j] && Math.Abs(matrix[j,i]) > EPS)
                        break;

                if (j == n)
                    --rank;
                else
                {
                    line_used[j] = true;

                    for (int p = i + 1; p < m; ++p)
                        matrix[j, p] /= matrix[j, i];

                    for (int k = 0; k < n; ++k)
                        if (k != j && Math.Abs(matrix[k, i]) > EPS)
                            for (int p = i + 1; p < m; ++p)
                                matrix[k, p] -= matrix[j, p] * matrix[k, i];
                }
            }

            return rank;
        }
    }
}
