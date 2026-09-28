using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Source.FileSystem
{
    public static class FileLoader
    {
        public static int[] LoadReg5()
        {
            var path = FilePaths.PathVector();

            var content = File.ReadAllLines(path);
            
            var c = content[0].Select(ch => ch - '0').ToArray();
        
            return c;
        }

        public static int[] LoadReg17()
        {
            var path = FilePaths.PathVector();

            var content = File.ReadAllLines(path);

            var c = content[1].Select(ch => ch - '0').ToArray();

            return c;
        }
        public static string LoadAlphabet()
        {
            var path = FilePaths.PathToAlphabet();

            return File.ReadAllText(path);
        }
    }
}