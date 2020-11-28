using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;

namespace BlakeHashTests
{
    public class RunsAlg : IPValueAlg
    {
        // ошибка в описании теста (2-6): (2) написано, что НЕ запускать тест, если 
        // сумма БОЛЬШЕ, чем значение в частотном тесте
        // далее пишут пример, где показывают, что сумма меньше, и они говорят, что тест запускать не надо
        public double getPValue(BitArray hash)
        {
            var hashLen = hash.Length;
            double pi = 0;

            foreach (bool v in hash)
                if (v)
                    pi++;

            pi /= hashLen;

            if (pi - 0.5 > 2 / Math.Sqrt(10))
                return 0;

            var vObs = 1;

            for (int i = 0; i < hash.Length - 1; i++)
                if (hash[i] ^ hash[i + 1])
                    vObs++;

            var pValue = Special.Erfc(Math.Abs(vObs - 2 * hashLen * pi * (1 - pi)) /
                (2 * Math.Sqrt(2 * hashLen) * pi * (1 - pi)));

            return pValue;
        }
    }
}
