using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Source.FileSystem
{
    public static class Loader
    {
        public static int LoadKey()
        {
            var path = "/home/denis/Documents/Labs/Security/Lab6/Source/Secrets/x.txt";
            var content = File.ReadAllText(path);

            return int.Parse(content);
        }

        public static int LoadP()
        {
            var path = "/home/denis/Documents/Labs/Security/Lab6/Source/OpenKeys/p.txt";
            var content = File.ReadAllText(path);

            return int.Parse(content);
        }
        public static int LoadG()
        {
            var path = "/home/denis/Documents/Labs/Security/Lab6/Source/OpenKeys/g.txt";
            var content = File.ReadAllText(path);

            return int.Parse(content);
        }
        public static int LoadY()
        {
            var path = "/home/denis/Documents/Labs/Security/Lab6/Source/OpenKeys/y.txt";
            var content = File.ReadAllText(path);

            return int.Parse(content);
        }
        public static int LoadA()
        {
            var path = "/home/denis/Documents/Labs/Security/Lab6/Source/OpenKeys/a.txt";
            var content = File.ReadAllText(path);

            return int.Parse(content);
        }
        public static int LoadB()
        {
            var path = "/home/denis/Documents/Labs/Security/Lab6/Source/OpenKeys/b.txt";
            var content = File.ReadAllText(path);

            return int.Parse(content);
        }
    }
}