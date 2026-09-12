namespace Source
{
    using Source.Encoders;
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class MainEncryptor(CesarEncoder cesarEncoder, MagicSquareEncoder maSquareEncoder)
    {
          public string Encrypt()
        {
            var filePath =Path.Combine(FilePath.PathInputs(), "input.txt");

            var word = File.ReadAllText(filePath);

            var firstStage = cesarEncoder.Encrypt(word);

            var secondStage = maSquareEncoder.Encrypt(firstStage);

            return secondStage;
        }

        public string Decrypt()
        {
            var filePath = Path.Combine(FilePath.PathResults(), "Encrypted.txt");

            var word = File.ReadAllText(filePath);

            var secondStage = maSquareEncoder.Decrypt(word);

            var firstStage = cesarEncoder.Decrypt(word);

            return firstStage;
        }
    }
}
