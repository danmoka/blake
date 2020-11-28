using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BlakeHashTests
{
    interface IPValueAlg
    {
        double getPValue(BitArray hash);
    }
}
