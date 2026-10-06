using Source.Encoder;
using Source.FileSystem;

namespace Program
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var table = FileLoader.LoadTable();
            var encryptor = new PolybEncoder();
            int k = 6;
            for (int i = 0; i < table.Length; i++)
            {

                System.Console.Write($"{table[i]} ");
                if (i == k)
                {
                    System.Console.WriteLine();
                    k += 7;
                }

            }
            System.Console.WriteLine();

            System.Console.WriteLine("Шифрование");
            System.Console.WriteLine($"Слово: лй.еэву");
            string encrypted = encryptor.Encrypt("лй.еэву");
            System.Console.WriteLine(encrypted);

            System.Console.WriteLine("Расшифровывание");
            string decrypted = encryptor.Decrypt(encrypted);
            System.Console.WriteLine("Расшифрованное слово: ");
            System.Console.WriteLine(decrypted);

        }
    }
}