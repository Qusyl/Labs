using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Source.FileSystem
{
    public static class FilePaths
    {
        public static string PathDirectory()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());

            do
            {
                dir = dir?.Parent;
            } while (dir != null && dir.EnumerateFiles("*.sln").Any());

            System.Console.WriteLine($"!!!SLN!!!! - {dir?.FullName}");

            return dir?.FullName ?? throw new DirectoryNotFoundException("Not found slnx directory");
        }

        public static string PathTable()
        {
            var directory = PathDirectory();

            var path = Path.Combine(directory, "Source", "Secrets", "table.txt");
            System.Console.WriteLine($"Table path = {path}");

            return path;
        }

        public static string PathToEncrypted()
        {
            var directory = PathDirectory();

            var path = Path.Combine(directory, "App", "Result", "encrypted.txt");

            return path;
        }
        public static string PathToDecrypted()
        {
            var directory = PathDirectory();

            var path = Path.Combine(directory, "App", "Result", "decrypted.txt");

            return path;
        }


        public static string PathToWWDecrypted()
        {
            var directory = PathDirectory();
            var path = Path.Combine(directory, "App", "Result", "decrypted_with_w_key.txt");
            return path;
        }

        

        public static string PathToAnalyseResult()
        {
            var directory = PathDirectory();

            var path = Path.Combine(directory, "App", "Result", "analyze.txt");

            return path;
        }

        public static string PathToAlphabets()
        {
            var directory = PathDirectory();
            var path = Path.Combine(directory, "App", "Alphabets", "data.txt");
            return path;
        }
    }
}