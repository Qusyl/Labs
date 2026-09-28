using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Source.BinConverter
{
    public static class DecimalToBin
    {
        public static int[] Convert(int num)
        {
            string binary = "";

            while (num > 0)
            {
                binary = (num % 2) + binary;
                num /= 2;
            }

            var c = binary.Select(ch => ch - '0').ToArray();

            return c;
        }
    }
}