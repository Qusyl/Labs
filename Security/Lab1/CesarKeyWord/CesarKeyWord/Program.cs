using Source;
using Source.Encoders;

namespace CesarKeyWord
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cesarEnc = new CesarEncoder();

            var magicEnc = new MagicSquareEncoder();

            var encryptor = new MainEncryptor(cesarEnc, magicEnc);

            var encrypted = encryptor.Encrypt();

            Console.WriteLine($"[MAIN] : зашифровано : {encrypted}");

            File.WriteAllText(Path.Combine(FilePath.PathResults(), "Encrypted.txt"), encrypted);

            var decrypted = encryptor.Decrypt();

            Console.WriteLine($"[MAIN] : расшифровано : {decrypted}");
        }
    }
}