using System;
using System.Collections.Generic;
using System.Text;

namespace Source.Commons
{
    public static class FileLoader
    {
        public static int[] LoadSquare()
        {
            var directory = FilePath.PathSecrets();

            var path = Path.Combine(directory, "s_key.txt");

            return File
                .ReadAllText(path, System.Text.Encoding.UTF8)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(num => int.Parse(num))
                .ToArray();
        }

        public static int LoadFromTemp()
        {
            var path = FilePath.PathTemp();
            return int.Parse(File.ReadAllText(path));
        }
        public static string LoadAlphabet()
        {
            var directory = FilePath.PathSecrets();

            var alphabetPath = Path.Combine(directory, "Alphabet.txt");

            if (!File.Exists(alphabetPath))
            {
                throw new InvalidOperationException($"Файла с алфаитом не существует по пути {alphabetPath}");
            }

            return File.ReadAllText(alphabetPath);
        }

         public static int LoadKey()
        {
            var keyPath = Path.Combine(FilePath.PathSecrets(), "c_key.txt");

            if (!File.Exists(keyPath))
            {
                throw new InvalidOperationException($"Ключ не найден! : [{keyPath}]");
            }

            var key = int.Parse(File.ReadAllText(keyPath));

            return key;
        }
    }
}
