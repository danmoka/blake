using System;
using System.Collections;
using Accord.Math;

namespace BlakeHashTests
{
    /// <summary>
    /// Класс для проверки псевдослучайных последовательностей
    /// Использует частотный тест
    /// </summary>
    public class FrequencyAlg : IPValueAlg
    {
        private double _rootOf2 = 1.41421356237;

        /// <summary>
        /// Метод, считающий p-value на основе частоты встречаемости 1 и 0
        /// </summary>
        /// <param name="hash">Псевдослучайная последовательность</param>
        /// <returns>p-value</returns>
        public double getPValue(BitArray hash)
        {
            // сумма 1 и -1
            double sum = 0;

            foreach (bool v in hash)
                sum += v ? 1 : -1;

            var absoluteSum = Math.Abs(sum) / Math.Sqrt(hash.Length);
            var pValue = Special.Erfc(absoluteSum / _rootOf2); // erfc - Complementary Error Function (дополнительная функция ошибок)

            return pValue;
        }
    }
}
