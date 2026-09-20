using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Source.Utills
{
    public static class PathLoader
    {
        public static string AlphabetPath()
        {
            var directory = GetRootDirectory();
           

            var path = Path.Combine(directory,
             "Source",
             "Source",
             "Secrets",
             "alphabet.txt");
        System.Console.WriteLine(path);
            return path;
        }
        public static string KeyWordPath()
        {
            var directory = GetRootDirectory();

            var path = Path.Combine(directory, "Source", "Source", "Secrets", "keyword.txt");

            return path;
        }
        public static string ShiftedAlphabetPath()
        {
            var directory = GetRootDirectory();

            var path = Path.Combine(directory,
             "Source",
             "Source",
             "Secrets",
             "shifted_alphabet.txt");

            return path;
        }

        public static string BasedStepPath()
        {
            var directory = GetRootDirectory();

            var path = Path.Combine(directory,
             "Source",
             "Source",
             "Secrets",
             "based_step.txt");

            return path;
        }
        public static string EncryptedPath()
        {
            var directory = GetRootDirectory();

            var path = Path.Combine(
                directory,
                "Application",
                "App",
                "Result",
                "Encrypted.txt"
                 );

            return path;
        }
        public static string DecryptedPath()
        {
            var directory = GetRootDirectory();

            var path = Path.Combine(
                directory,
                "Application",
                "App",
                "Result",
                "Decrypted.txt"
                 );

            return path;
        }
        public static string TempPath()
        {
            var directory = GetRootDirectory();

            var path = Path.Combine(directory, "Source", "Source", "Utills", "middleware.txt");

            return path;
        }
        private static string GetRootDirectory()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (dir != null && !dir.EnumerateFiles("*.slnx").Any())
            {
                dir = dir.Parent;
            }

            return dir?.FullName ?? throw new FileNotFoundException("sln not found");
        }
        
    }
}