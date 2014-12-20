using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace test
{
    class MyMath
    {
        static int[] pow2Mem;
        public MyMath()
        {
            pow2Mem = new int[100];
            for (int i = 0; i < pow2Mem.Length; i++)
                pow2Mem[i] = -1;
        }
        public int Pow(int a, int b)
        {
            if (pow2Mem[b] > 0) return pow2Mem[b];
            int n = 1;
            for (int i = 0; i < b; i++)
            {
                n *= a;
            }
            pow2Mem[b] = n;
            return n;
        }
    }
}
