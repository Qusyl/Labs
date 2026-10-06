using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Source.FileSystem;

namespace Source
{
    public class GammaCipher
    {


        private readonly string _alphabet;
            

        private Dictionary<char, int> _charToCode;
        private Dictionary<int, char> _codeToChar;

        public GammaCipher()
        {
            _alphabet = FileLoader.LoadAlphabet();
            _charToCode = new Dictionary<char, int>();
            _codeToChar = new Dictionary<int, char>();
            for (int i = 0; i < 64; i++)
            {
                _charToCode[_alphabet[i]] = i;
                _codeToChar[i] = _alphabet[i];
            }
        }


        public int[] TextToBits(string text)
        {
            var bits = new List<int>(text.Length * 6);
            foreach (char c in text)
            {
                if (!_charToCode.TryGetValue(c, out int code))
                {
                    throw new ArgumentException($"Символ '{c}' не входит в 64-символьный алфавит");
                }

                for (int b = 5; b >= 0; b--)
                {
                    bits.Add((code >> b) & 1);
                }

            }

            return bits.ToArray();
        }


        public string BitsToText(int[] bits)
        {
            if (bits.Length % 6 != 0)
                throw new ArgumentException("Длина битовой строки должна быть кратна 6");

            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < bits.Length; i += 6)
            {
                int code = 0;
                for (int j = 0; j < 6; j++)
                {
                    code = (code << 1) | bits[i + j];
                }

                sb.Append(_codeToChar[code]);
            }
            var str = sb.ToString();
         
            return str;
        }

        public string Decrypt(int[] encrypted, int[] gamma)
        {
            var sb = new StringBuilder();
            var decryptedBits = Xor(encrypted, gamma);
            sb.Append("Биты шифра: ");
            System.Console.Write($"Биты шифра: ");
            for (int i = 0; i < decryptedBits.Length; i++)
            {
                sb.Append(decryptedBits[i]);
                System.Console.Write($"{decryptedBits[i]}");
            }
            System.Console.WriteLine();
            sb.Append(Environment.NewLine);
            sb.Append("Текст шифра: ");

            var text = BitsToText(decryptedBits);
            System.Console.WriteLine($"Текст шифра: {text}");
            sb.Append(text);
            FileSaver.SaveDecrypted(sb.ToString());
            return text;
        }

        public int[] Xor(int[] a, int[] b)
        {

            if (a.Length != b.Length) throw new ArgumentException("Длины должны совпадать");
            var path = FilePaths.PathCipherBits();
            var sb = new StringBuilder();

            sb.Append("Биты текста: ");
            for (int i = 0; i < a.Length; i++)
            {
                sb.Append($"{a[i]}");
            }
            sb.Append(Environment.NewLine);
            sb.Append("Биты гаммы: ");
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append($"{b[i]}");
            }
            sb.Append(Environment.NewLine);
            var r = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = a[i] ^ b[i];
            }
            sb.Append("Биты шифра: ");

            for (int i = 0; i < r.Length; i++)
            {
                sb.Append($"{r[i]}");
            }

            sb.Append(Environment.NewLine);

            FileSaver.SaveCipherBits(sb.ToString());

            return r;
        }
    }
}
