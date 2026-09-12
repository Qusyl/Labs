using System;
using System.Collections.Generic;
using System.Text;

namespace Source
{
    public static class FileSaver
    {
        public static void SaveEncrypted(string result)
        {
            var path = FilePath.PathResults();

            var resultsPath = Path.Combine(path, "Encrypted.txt");

            File.WriteAllText(resultsPath, result);
        }
        public static void SaveDecrypted(string result)
        {
            var path = FilePath.PathResults();

            var resultsPath = Path.Combine(path, "Decrypted.txt");

            File.WriteAllText(resultsPath, result);
        }
    }
}
