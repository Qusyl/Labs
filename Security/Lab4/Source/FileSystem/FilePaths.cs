using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Source.FileSystem
{
    public static class FilePaths
    {
        public static string PathVector()
        {
            var directory = PathDirectory();

            var path = Path.Combine(directory, "Secrets", "vector.txt");
            return path;
        }

        public static string PathToAlphabet()
        {
            var directory = PathDirectory();

            var path = Path.Combine(directory, "Secrets", "alphabet64.txt");

            return path;
        }

        public static string PathTap5()
        {
            var directory = PathDirectory();

            var path = Path.Combine(directory, "Secrets", "taps.txt");

            return path;
        }
        public static string PathTap17()
        {
            var directory = PathDirectory();

            var path = Path.Combine(directory, "Secrets", "taps.txt");

            return path;
        }

        public static string PathToDecrypted()
        {
            var directory = PathDirectory();

            var path = Path.Combine(directory, "Outputs", "decrypted.txt");

            return path;
        }
        public static string PathCipherText()
        {
            var directory = PathDirectory();

            var path = Path.Combine(directory, "Outputs", "cipher.txt");

            return path;
        }
        
        public static string PathCipherBits()
        {
            var directory = PathDirectory();

            var path = Path.Combine(directory, "Outputs", "cipher_bits.txt");

            return path;
        }
        public static string PathGamma()
        {
            var directory = PathDirectory();
            var path = Path.Combine(directory, "Secrets", "gamma.txt");
            return path;
        }

        public static string PathRegister5()
        {
            var directory = PathDirectory();
            var path = Path.Combine(directory, "Secrets", "registers", "reg_5.txt");
            return path;
        }
         public static string PathRegister17()
        {
            var directory = PathDirectory();
            var path = Path.Combine(directory, "Secrets", "registers", "reg_17.txt");
            return path;
        }
        private static string PathDirectory()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());

            do
            {
                dir = dir?.Parent;
            } while (dir != null && dir.EnumerateFiles("*.sln").Any());
            
            return dir?.FullName ?? throw new DirectoryNotFoundException("Not found slnx directory");
        }
    }  
}