using Accord.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BlakeHashTests
{
    public class FrequencyBlockAlg : IPValueAlg
    {
        private int _blockLen;

        public FrequencyBlockAlg(int blockLen)
        {
            _blockLen = blockLen;
        }

        public double getPValue(BitArray hash)
        {
            var blocksCount = hash.Length / _blockLen;
            var piOfBlocks = new double[blocksCount];

            for(int i = 0; i < blocksCount; i++)
            {
                for(int j = 0; j < _blockLen; j++)
                {
                    if (hash[i * _blockLen + j])
                        piOfBlocks[i]++;
                }

                piOfBlocks[i] /= _blockLen;
            }

            var x2 = 0.0;

            foreach (var pi in piOfBlocks)
                x2 += Math.Pow(pi - 0.5, 2);

            x2 *= 4 * _blockLen;
            var pValue = 1 - Gamma.LowerIncomplete((double) blocksCount / 2, x2 / 2);

            return pValue;
        }
    }
}
