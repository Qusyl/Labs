using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Source.FileSystem
{
    public static class FileLoader
    {
        public static char[] LoadTable()
        {

            var path = FilePaths.PathTable();

            var content = File
            .ReadAllText(path)
            .Replace("\r", "")
            .Replace("\n", "");

            return content.ToArray()!;
        }

        public static string LoadEncrypted()
        {
            var path = FilePaths.PathToEncrypted();

            return File.ReadAllText(path);
        }

        public static List<char[]> LoadAlphabets()
        {
            var list = new List<char[]>(20);

            var content = PrepareAlphabets();

            for (int i = 0; i < content.Length; i++)
            {
                var alphabet = new char[content[i].Length];

                for (int j = 0; j < content[i].Length; j++)
                {
                    alphabet[j] = content[i][j];
                }

                list.Add(alphabet);

            }

            foreach (var alph in list)
            {
                int k = 6;
                for (int i = 0; i < alph.Length; i++)
                {
                    System.Console.Write($"{alph[i]} ");
                    if (i == k)
                    {
                        System.Console.WriteLine();
                        k += 7;
                    }
                }
                System.Console.WriteLine();
            }
            return list;
        }
        
        private static string[] PrepareAlphabets()
        {
            var path = FilePaths.PathToAlphabets();

            return  File.ReadAllLines(path);
        
        }
        
    }
}