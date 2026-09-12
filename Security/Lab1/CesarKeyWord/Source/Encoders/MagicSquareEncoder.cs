using System;
using System.Collections.Generic;
using System.Text;

namespace Source.Encoders
{
    public class MagicSquareEncoder
    {
        private bool IsSquare(int a) => Math.Sqrt(a) * Math.Sqrt(a) == a;
        private  string EncryptBlock(string block)
        {
            var buffer = block.ToCharArray();

            var square = LoadSquare();

            var len = square.Length;

            if (!IsSquare(len))
            {
                throw new InvalidOperationException("Квадрат не правильной формы");
            }

            if (block.Length < square.Length)
            {
                for (int i = block.Length - 1; i < len; i++)
                {
                    buffer[i] = '0';
                }
            }
            var route = new int[len];

            for (int i = 0; i < len; i++)
            {
                route[square[i] - 1] = i;
            }
            var result = new char[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = buffer[route[i]];
            }
            return new string(result);
        }

        private  string DecryptBlock(string block, int[] square)
        {
            var len = square.Length;
           
            if (!IsSquare(len))
            {
                throw new InvalidOperationException("Квадрат не правильной формы");
            }

            var route = new int[len];

            for (int i = 0; i < len; i++)
            {
                route[square[i] - 1] = i;
            }
            var result = new char[len];
            for (int i = 0; i < len; i++)
            {
                result[route[i]] = block[i];
            }
            return new string(result);
        }
        public  string Encrypt(string word)
        {
            var square = LoadSquare();
            var len = square.Length;
            while (word.Length % len != 0)
            {
                word += '0';
            }

            StringBuilder sb = new StringBuilder();

            for (int start = 0; start < word.Length; start += len)
            {
                string block = EncryptBlock(word.Substring(start, len));
                sb.Append(block);
            }

            return sb.ToString();
        }
        public  string Decrypt(string word)
        {
            var square = LoadSquare();

            var len = square.Length;

            StringBuilder sb = new StringBuilder();

            for (int start = 0; start < word.Length; start += len)
            {
                var block = DecryptBlock(word.Substring(start, len), square);
                sb.Append(block);
            }
            return sb.ToString();
        }

        private  int[] LoadSquare()
        {
            var directory = FilePath.PathSecrets();

            var path = Path.Combine(directory, "s_key.txt");

            return File
                .ReadAllText(path)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(num =>int.Parse(num))
                .ToArray();
        }

    }
}
