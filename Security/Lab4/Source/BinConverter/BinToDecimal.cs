using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Source.BinConverter
{
    public static class BinToDecimal
    {
        public static int Convert(int[] num)
        {
            var result = 0;
            for (int i = 0; i < num.Length; i++)
            {
                int twoInExp = (int)Math.Pow(2, i);

                result += twoInExp * num[i];
            }
            return result;
        }
    }
}