using System;
using System.Collections.Generic;
using System.Text;

namespace Source.Commons
{
    public static class FilePath
    {
        public static string PathSecrets()
        {
            var directory = FoundDirectoryRoot();
            Console.WriteLine($"Корень {directory}");
            return Path.Combine(directory,"Source", "Secrets");
        }

        private static string FoundDirectoryRoot()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null && !dir.EnumerateFiles("*.slnx").Any())
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new InvalidOperationException("Project not founds");
        }
        public static string PathInputs()
        {
            var directory = FoundDirectoryRoot();

            return Path.Combine(directory, "CesarKeyWord", "Inputs");
        }

        public static string PathResults()
        {
            var directory = FoundDirectoryRoot();

            return Path.Combine(directory, "CesarKeyWord", "Results");
        }
        public static string PathTemp()
        {
            var directory = FoundDirectoryRoot();

            return Path.Combine(directory, "Source", "Commons", "Temp", "temp.txt");
        }
    }
}
