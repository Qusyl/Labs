namespace Source.Encoders
{
    using Source.Commons;
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class MagicSquareEncoder
    {
        public  IEnumerable<string> Encrypt(string word)
        {
            var square = FileLoader.LoadSquare();
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
                yield return sb.ToString();
            }
        }
        public IEnumerable<string> Decrypt(string word)
        {
            var square = Square();

            var len = square.Length;

            var requiredLen = FileLoader.LoadFromTemp();

            StringBuilder sb = new StringBuilder();

            for (int start = 0; start < requiredLen; start += len)
            {
                var block = DecryptBlock(word.Substring(start, len), square);

                sb.Append(block);
                yield return sb.ToString();
            }
            
        }
        private string EncryptBlock(string block)
        {
            var buffer = block.ToCharArray();

            var square = Square();

            var len = square.Length;

            if (!IsSquare(len))
            {
                throw new InvalidOperationException("Квадрат не правильной формы");
            }

            

            if (block.Length < square.Length)
            {
                for (int i = block.Length; i < len; i++)
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
        private bool IsSquare(int a) => Math.Sqrt(a) * Math.Sqrt(a) == a;
     
        private string DecryptBlock(string block, int[] square)
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
        public int[] Square() => FileLoader.LoadSquare();
       

    }
}
