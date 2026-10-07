using System;
using System.IO;
using System.Linq;

namespace Source.FileSystem
{
    public static class FileLoader
    {
        public static byte[] LoadSBox(string path = "/home/denis/Documents/Labs/Security/Lab5/Source/Secrets/sbox.txt")
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"S-box file not found: {path}");

            string content = File.ReadAllText(path);
            string[] tokens = content.Split(
                new[] { ' ', '\t', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length != 256)
                throw new InvalidDataException($"S-box должен содержать  256 чисел, получено {tokens.Length}");

            byte[] sbox = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                sbox[i] = Convert.ToByte(tokens[i], 16);
            }
            return sbox;
        }
    }
}