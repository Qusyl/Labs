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
                .ReadAllText(path)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(num => int.Parse(num))
                .ToArray();
        }

        public static int LoadFromTemp()
        {
            var path = FilePath.PathTemp();
            return int.Parse(File.ReadAllText(path));
        }
    }
}
