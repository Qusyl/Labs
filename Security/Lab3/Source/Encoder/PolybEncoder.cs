using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Source.FileSystem;

namespace Source.Encoder
{
    public class PolybEncoder
    {
        private char[] _aplhabet;
        public PolybEncoder()
        {
            _aplhabet = FileLoader.LoadTable();
        }

        public string Encrypt(string message)
        {
            char[] result = new char[message.Length];
            for (int i = 0; i < message.Length; i++)
            {
                int index = Array.IndexOf(_aplhabet, message[i]);
                if (index == -1)
                {
                    result[i] = message[i];
                    continue;
                }

                int newIndex = ModEncrypt(index);

                result[i] = _aplhabet[newIndex];
            }
            var resultString = new string(result);

            FileSaver.SaveEncrypted(resultString);
            

            return resultString;
        }

        public string Decrypt(string message, char[]? alphabet = default)
        {
            char[] result = new char[message.Length];

            if(alphabet is not null)
            {
               var res =  DecryptWithManualAlphabet(alphabet, message);
                FileSaver.SaveDecrypted(res);
                return res;
            }

            for (int i = 0; i < message.Length; i++)
            {
                int index = Array.IndexOf(_aplhabet, message[i]);
                if (index == -1)
                {
                    result[i] = message[i];
                    continue;
                }

                int newIndex = ModDecrypt(index, _aplhabet.Length);

                result[i] = _aplhabet[newIndex];
            }

            var resultString = new string(result);

            FileSaver.SaveDecrypted(resultString);

            return new string(resultString);
        }

        private string DecryptWithManualAlphabet(char[]? alphabet, string message)
        {
            char[] result = new char[message.Length];

            for (int i = 0; i < message.Length; i++)
            {
                int index = Array.IndexOf(alphabet, message[i]);
                if (index == -1)
                {
                    result[i] = message[i];
                    continue;
                }

                int newIndex = ModDecrypt(index, alphabet.Length);

                result[i] = alphabet[newIndex];
            }

            var resultString = new string(result);

            FileSaver.SaveDecrypted(resultString);

            return new string(resultString);
        }

        private int ModEncrypt(int index)
        {
            int m = _aplhabet.Length;
            return (index + 7) % m;
        }
        
        private int ModDecrypt(int index, int m)
        {
            return ((index - 7) + m) % m;
        }
    }
}