using Accord.Math;
using System;
using System.Collections;

namespace BlakeHashTests
{
    /// <summary>
    /// Класс для проверки псевдослучайных последовательностей
    /// Использует частотный блочный тест
    /// </summary>
    public class FrequencyBlockAlg : IPValueAlg
    {
        private int _blockLength;

        /// <param name="blockLen">Размер блока</param>
        public FrequencyBlockAlg(int blockLen)
        {
            _blockLength = blockLen;
        }

        /// <summary>
        /// Метод, считающий p-value на основе пропорций единиц в блоках
        /// </summary>
        /// <param name="hash">Псевдослучайная последовательность</param>
        /// <returns>p-value</returns>
        public double getPValue(BitArray hash)
        {
            var blocksCount = hash.Length / _blockLength;
            var proportions = new double[blocksCount];

            for(int i = 0; i < blocksCount; i++)
            {
                for(int j = 0; j < _blockLength; j++)
                {
                    if (hash[i * _blockLength + j])
                        proportions[i]++;
                }

                proportions[i] /= _blockLength;
            }

            // мера того, насколько хорошо наблюдаемая доля единиц в блоке 
            // соответствует ожидаемой пропорции (1/2)
            var x2 = 0.0;

            foreach (var pi in proportions)
                x2 += Math.Pow(pi - 0.5, 2);

            x2 *= 4 * _blockLength;
            var pValue = 1 - Gamma.LowerIncomplete((double) blocksCount / 2, x2 / 2); // верхняя неполная гамма функция

            return pValue;
        }
    }
}
