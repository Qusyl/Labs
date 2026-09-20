namespace Source.Encryptor
{
    using Source.Commons;
    using Source.Encoders;
   
    using System.Collections.Generic;
    
    public class MainEncryptor()
    {
        private CesarEncoder _cesarEncoder = new CesarEncoder();
        private MagicSquareEncoder _maSquareEncoder = new MagicSquareEncoder();
        public  IEnumerable<string> Encrypt(int? key = default)
        {
            var filePath =Path.Combine(FilePath.PathInputs(), "Input.txt");

            var word = File.ReadAllText(filePath);
            FileSaver.SaveTemp(word.Length);

            var firstStage = _cesarEncoder.Encrypt(word, key.HasValue ? key.Value : default);

            var lastFirstStage = string.Empty;

            foreach(var stage in firstStage)
            {
                lastFirstStage = stage;

                yield return stage;
            }

            var secondStage = _maSquareEncoder.Encrypt(lastFirstStage);

            var save = string.Empty;
            foreach(var stage in secondStage)
            {
                save = stage;
                yield return stage;
            }

            FileSaver.SaveEncrypted(save);
        }

        public IEnumerable<string> Decrypt(int? key = default)
        {
            var filePath = Path.Combine(FilePath.PathResults(), "Encrypted.txt");
            var word = File.ReadAllText(filePath);

            var originalLength = FileLoader.LoadFromTemp();

            var secondStage = _maSquareEncoder.Decrypt(word);
            var lastSecondStage = string.Empty;
            foreach (var stage in secondStage)
            {
                lastSecondStage = stage;
                yield return stage;
            }
            lastSecondStage = lastSecondStage.Substring(0, originalLength);
            var firstStage = _cesarEncoder.Decrypt(lastSecondStage, key.HasValue ? key.Value : default);
            var save = string.Empty;
            foreach (var stage in firstStage)
            {
                save = stage;
                yield return stage;
            }

            
            if (save.Length > originalLength)
            {
                save = save.Substring(0, originalLength);
            }

            FileSaver.SaveDecrypted(save);
        }




    }
}
