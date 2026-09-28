
using System.Text;

namespace Source.FileSystem
{
    public static class FileSaver
    {
        public static void SaveGamma(int[] gamma,string report)
        {
            var sb = new StringBuilder();
            var path = FilePaths.PathGamma();
            for (int i = 0; i < gamma.Length; i++)
            {
                sb.Append(gamma[i]);
            }
             sb.Append(Environment.NewLine);
            File.WriteAllText(path, report + Environment.NewLine + sb.ToString());
        }
        public static void SaveReg5(int[] reg)
        {
            var sb = new StringBuilder();
            var path = FilePaths.PathRegister5();
            for (int i = 0; i < reg.Length; i++)
            {
                sb.Append(reg[i]);
            }

            sb.Append(Environment.NewLine);

            File.WriteAllText(path, sb.ToString());
        }
        public static void SaveReg17(int[] reg)
        {
            var sb = new StringBuilder();
            var path = FilePaths.PathRegister17();
            for (int i = 0; i < reg.Length; i++)
            {
                sb.Append(reg[i]);
            }

            sb.Append(Environment.NewLine);

            File.WriteAllText(path, sb.ToString());
        }

        public static void SaveCipherBits(string bits)
        {
            var path = FilePaths.PathCipherBits();

           
            File.WriteAllText(path, bits.ToString());

        }
        public static void SaveCipherText(string? cipherText)
        {
            var path = FilePaths.PathCipherText();

            var sb = new StringBuilder();

            sb.Append(cipherText);

            sb.Append(Environment.NewLine);

            File.WriteAllText(path, sb.ToString());

        }
        public static void SaveDecrypted(string decrypted)
        {
            var path = FilePaths.PathToDecrypted();

            File.WriteAllText(path, decrypted);
        }
    }
}