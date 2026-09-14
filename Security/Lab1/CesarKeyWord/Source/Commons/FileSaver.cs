using System;
using System.Collections.Generic;
using System.Text;

namespace Source.Commons
{
    public static class FileSaver
    {
        public static void SaveEncrypted(string result)
        {
            var path = FilePath.PathResults();

            var resultsPath = Path.Combine(path, "Encrypted.txt");

            File.WriteAllText(resultsPath, result, System.Text.Encoding.UTF8);
        }
        public static void SaveDecrypted(string result)
        {
            var path = FilePath.PathResults();

            var resultsPath = Path.Combine(path, "Decrypted.txt");

            File.WriteAllText(resultsPath, result, System.Text.Encoding.UTF8);
        }

        public static void SaveTemp(int size)
        {
            var path = FilePath.PathTemp();

            File.WriteAllText(path, size.ToString(), System.Text.Encoding.UTF8);
        }
    }
}
