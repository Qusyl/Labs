using Source.Encoders;

namespace CesarKeyWord
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var input = "123456789123045678";
            var encrypted = MagicSquareEncoder.Encrypt(input);
            Console.WriteLine($"Зашифрован: {encrypted}");
            var decrypted = MagicSquareEncoder.Decrypt(encrypted);
            Console.WriteLine($"Расшифрован: {decrypted}");
        }
    }
}