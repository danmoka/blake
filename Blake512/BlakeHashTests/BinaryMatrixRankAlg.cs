using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BlakeHashTests
{
    public class BinaryMatrixRankAlg : IPValueAlg
    {
        private int _m;
        private int _q;

        public BinaryMatrixRankAlg(int m, int q)
        {
            _m = m;
            _q = q;
        }
        public double getPValue(BitArray hash)
        {
            var blocksLen = _m * _q;
            var blocksCount = hash.Length / (blocksLen);
            var matrixs = new List<int[,]>();

            for (int i = 0; i < blocksCount; i++)
            {
                var matrix = new int[_m, _q];

                for (int j = 0; j < _m; j++)
                {
                    for (int k = 0; k < _q; k++)
                    {
                        matrix[j, k] = hash[i * blocksLen + j * _m + k] ? 1 : 0;
                    }
                }

                matrixs.Add(matrix);
            }

            var mRankCount = 0;
            var m_1RankCount = 0;

            for (int i = 0; i < blocksCount; i++)
            {
                var rank = Rank(matrixs[i], _m, _q);

                if (rank == _m)
                    mRankCount++;
                else if (rank == _m - 1)
                    m_1RankCount++;
            }

            var otherRanksCount = blocksCount - mRankCount - m_1RankCount;

            var x2 = Math.Pow(mRankCount - 0.2888 * blocksCount, 2) / (0.2888 * blocksCount) +
                Math.Pow(m_1RankCount - 0.5776 * blocksCount, 2) / (0.5776 * blocksCount) +
                Math.Pow(otherRanksCount - 0.1336 * blocksCount, 2) / (0.1336 * blocksCount);

            var pValue = Math.Exp(-x2 / 2);

            return pValue;
        }

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
