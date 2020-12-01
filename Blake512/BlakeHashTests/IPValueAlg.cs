using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BlakeHashTests
{
    interface IPValueAlg
    {
        /// <summary>
        /// Метод, возвращающий p-value по данной бинарной последовательности
        /// p-value - вероятность того, что идеальный генератор сгенерировал последовательность менее случайную, чем исследуемый
        /// </summary>
        /// <param name="hash">Бинарная последовательность</param>
        /// <returns>p-value</returns>
        double getPValue(BitArray hash);
    }
}
