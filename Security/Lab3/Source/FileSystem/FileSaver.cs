
namespace Source.FileSystem
{
    public static class FileSaver
    {
        public static void SaveEncrypted(string encrypted)
        {
            var path = FilePaths.PathToEncrypted();

            File.WriteAllText(path, encrypted);
        }


        public static void SaveDecrypted(string decrypted)
        {
            var path = FilePaths.PathToDecrypted();

            File.WriteAllText(path, decrypted);
        }

        public static void SaveAnalyze(string report)
        {
            var path = FilePaths.PathToAnalyseResult();

            File.WriteAllText(path, report);
        }
        public static void SaveDecryptedWithWW(string decrypted)
        {
            var path = FilePaths.PathToWWDecrypted();

            File.WriteAllText(path, decrypted);
        }
    }
}