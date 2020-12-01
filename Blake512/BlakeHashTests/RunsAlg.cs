using System;
using System.Collections;
using Accord.Math;

namespace BlakeHashTests
{
    /// <summary>
    /// Класс для проверки псевдослучайных последовательностей
    /// Использует тест "дырок"
    /// </summary>
    public class RunsAlg : IPValueAlg
    {
        // ошибка в описании теста (2-6): (2) написано, что НЕ запускать тест, если 
        // сумма БОЛЬШЕ, чем значение в частотном тесте
        // далее пишут пример, где показывают, что сумма меньше, и они говорят, что тест запускать не надо

        /// <summary>
        /// Метод, считающий p-value на основе количества непрерывных последовательностей одинаковых бит
        /// </summary>
        /// <param name="hash">Псевдослучайная последовательность</param>
        /// <returns>p-value</returns>
        public double getPValue(BitArray hash)
        {
            var hashLength = hash.Length;
            double onesProportion = 0; // доля единиц

            foreach (bool v in hash)
                if (v)
                    onesProportion++;

            onesProportion /= hashLength;

            if (onesProportion - 0.5 > 2 / Math.Sqrt(10))
                return 0;

            var numberOfHoles = 1;

            for (int i = 0; i < hash.Length - 1; i++)
                if (hash[i] ^ hash[i + 1]) // если биты не совпадают, то
                    numberOfHoles++;

            // erfc - Complementary Error Function (дополнительная функция ошибок)
            var pValue = Special.Erfc(Math.Abs(numberOfHoles - 2 * hashLength * onesProportion * (1 - onesProportion)) /
                (2 * Math.Sqrt(2 * hashLength) * onesProportion * (1 - onesProportion)));

            return pValue;
        }
    }
}
