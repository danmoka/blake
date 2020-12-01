using Accord.Math;
using System;
using System.Collections;

namespace BlakeHashTests
{
    /// <summary>
    /// Класс для проверки псевдослучайных последовательностей
    /// Использует тест пересекающихся шаблонов
    /// </summary>
    public class OverlappingTemplateMatchingAlg : IPValueAlg
    {
        private BitArray _template; // шаблон сравнения
        private int _blockSize; // размер блока последовательности

        public OverlappingTemplateMatchingAlg(BitArray template, int blockSize)
        {
            _template = template;
            _blockSize = blockSize;
            // ошибка в примере к тесту 2-18 (In block 2, there are 2 occurrences of 11) - там 1 вхождение
        }

        /// <summary>
        /// Метод, считающий p-value на основе количества совпадений шаблона в блоках
        /// </summary>
        /// <param name="hash">Псевдослучайная последовательность</param>
        /// <returns>p-value</returns>
        public double getPValue(BitArray hash)
        {
            var blocksCount = hash.Length / _blockSize;
            var blocks = new int[blocksCount][];
            // лямбда и эта используются для расчета теоретических вероятностей
            var lambda = (_blockSize - _template.Length + 1) / Math.Pow(2, _template.Length);
            var eta = lambda / 2;
            var k = Convert.ToInt32(2 * lambda); // количество степеней свободы

            var pi = new double[k + 1]; // теоретические вероятности
            var numberOfOccurrencesInBlocks = new int[k + 1];
            var sum = 0.0;

            // просчет теоретических вероятностей
            for (int i = 0; i < k; i++)
            {
                pi[i] = getProbability(i, eta);
                sum += pi[i];
            }

            pi[k] = 1 - sum;

            // разбиение на блоки
            for (int i = 0; i < blocksCount; i++)
            {
                blocks[i] = new int[_blockSize];

                for (int j = 0; j < _blockSize; j++)
                {
                    blocks[i][j] = hash[i * _blockSize + j] ? 1 : 0;
                }
            }

            // поиск шаблонов в блоке
            for (int i = 0; i < blocksCount; i++)
            {
                var numberOfOccurrences = 0;

                for (int j = 0; j < _blockSize - _template.Length + 1; j++)
                {
                    bool match = true;

                    for (int l = 0; l < _template.Length; l++)
                    {
                        if (_template[l] != hash[i * _blockSize + j + l])
                            match = false;
                    }

                    if (match)
                        numberOfOccurrences++;
                }

                if (numberOfOccurrences <= 4)
                    numberOfOccurrencesInBlocks[numberOfOccurrences]++;
                else
                    numberOfOccurrencesInBlocks[k]++;
            }

            // мера того, насколько хорошо количество шаблонов в блоке 
            // соответствует ожидаемой пропорции
            var x2 = 0.0;

            for (int i = 0; i < k + 1; i++)
            {
                x2 += Math.Pow(numberOfOccurrencesInBlocks[i] - blocksCount * pi[i], 2) / (blocksCount * pi[i]);
            }

            var pValue = 1 - Gamma.LowerIncomplete(k / 2.0, x2 / 2.0); // верхняя неполная гамма функция

            return pValue;
        }

        /// <summary>
        /// Считает теоретические вероятности (Magic)
        /// </summary>
        private double getProbability(int u, double eta)
        {
            int l;
            double sum, p;

            if (u == 0)
                p = Math.Exp(-eta);
            else
            {
                sum = 0.0;

                for (l = 1; l <= u; l++)
                    sum += Math.Exp(-eta - u * Math.Log(2) + l * Math.Log(eta) - Gamma.Log(l + 1) + Gamma.Log(u) - Gamma.Log(l) - Gamma.Log(u - l + 1));
                
                p = sum;
            }

            return p;
        }
    }
}
