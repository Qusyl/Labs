

namespace Source.FileSystem
{
    public static class Saver
    {
        public static void SaveKeyY(int y)
        {
            var path = "/home/denis/Documents/Labs/Security/Lab6/Source/OpenKeys/y.txt";

            File.WriteAllText(path, y.ToString());
        }
    }
}