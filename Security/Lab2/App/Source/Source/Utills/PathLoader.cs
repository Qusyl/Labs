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
        private static string GetRootDirectory()
        {
            var path = Directory
            .GetParent(Directory.GetCurrentDirectory())
            .Parent
            .FullName;

            return path;
        }
    }
}