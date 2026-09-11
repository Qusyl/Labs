using System;
using System.Collections.Generic;
using System.Text;

namespace Source.Encoders
{
    public static class MagicSquareEncoder
    {
        private static readonly int[] MagicSquare = [8, 1, 6, 3, 5, 7, 4, 9, 2];

        private static string EncryptBlock(string block)
        {
            var buffer = block.ToCharArray();

            if (block.Length < 9)
            {
                for (int i = block.Length - 1; i < 9; i++)
                {
                    buffer[i] = '0';
                }
            }
            var route = new int[9];

            for (int i = 0; i < 9; i++)
            {
                route[MagicSquare[i] - 1] = i;
            }
            var result = new char[9];
            for (int i = 0; i < 9; i++)
            {
                result[i] = buffer[route[i]];
            }
            return new string(result);
        }

        private static string DecryptBlock(string block)
        {
            if (block.Length < 9)
            {
                throw new InvalidOperationException("шифр должен быть не менее 9 символов!");
            }

            var route = new int[9];

            for (int i = 0; i < 9; i++)
            {
                route[MagicSquare[i] - 1] = i;
            }
            var result = new char[9];
            for (int i = 0; i < 9; i++)
            {
                result[route[i]] = block[i];
            }
            return new string(result);
        }
        public static string Encrypt(string word)
        {
            while(word.Length % 9 != 0)
            {
                word += '0';
            }

            StringBuilder sb = new StringBuilder();

            for (int start = 0; start < word.Length; start += 9)
            {
                string block = EncryptBlock(word.Substring(start, 9));
                sb.Append(block);
            }

            return sb.ToString();
        }
        public static string Decrypt(string word)
        {
            if(word.Length < 9)
            {
                throw new InvalidOperationException("Шифр должен быть не менее 9 символов");
            }

            StringBuilder sb = new StringBuilder();

            for (int start = 0; start < word.Length; start += 9)
            {
                var block = DecryptBlock(word.Substring(start, 9));
                sb.Append(block);
            }
            return sb.ToString();
        }

    }
}
