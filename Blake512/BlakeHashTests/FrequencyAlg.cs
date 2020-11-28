using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Accord.Math;

namespace BlakeHashTests
{
    public class FrequencyAlg : IPValueAlg
    {
        private double _rootOf2 = 1.41421356237;

        public double getPValue(BitArray hash)
        {
            double sN = 0;

            foreach (bool v in hash)
                sN += v ? 1 : -1;

            var sObs = Math.Abs(sN) / Math.Sqrt(hash.Length);
            var pValue = Special.Erfc(sObs / _rootOf2);

            return pValue;
        }
    }
}
