using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Source.AlphabetBuilder
{
    public static class AlphabetStringBuilder
    {
        public static string Build(char[] alphabet)
        {
            var sb = new StringBuilder();
            int k = 6;
                for (int i = 0; i < alphabet.Length; i++)
                {
                    sb.Append($"{alphabet[i]} ");
                    if (i == k)
                    {
                    sb.Append(Environment.NewLine);
                        k += 7;
                    }
                }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
    }
}