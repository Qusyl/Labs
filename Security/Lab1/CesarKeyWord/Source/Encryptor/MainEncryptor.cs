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

            var word = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            FileSaver.SaveTemp(word.Length);

            yield return "Шифр Цезаря";
            var firstStage = _cesarEncoder.Encrypt(word, key.HasValue ? key.Value : default);

            var lastFirstStage = string.Empty;

            foreach (var stage in firstStage)
            {
                lastFirstStage = stage;

                yield return stage;
            }
            yield return "Конец шифра Цезаря";
            yield return "Шифр Квадратом ( начало )";
            var secondStage = _maSquareEncoder.Encrypt(lastFirstStage);

            var save = string.Empty;
            foreach(var stage in secondStage)
            {
                save = stage;
                yield return stage;
            }
            yield return "Конец шифра Квадратом";        
            FileSaver.SaveEncrypted(save);
        }

        public IEnumerable<string> Decrypt(int? key = default)
        {
            var filePath = Path.Combine(FilePath.PathResults(), "Encrypted.txt");
            var word = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

            var originalLength = FileLoader.LoadFromTemp();
            yield return "Дешифрация Квадратом";
            var secondStage = _maSquareEncoder.Decrypt(word);
            var lastSecondStage = string.Empty;
            foreach (var stage in secondStage)
            {
                lastSecondStage = stage;
                yield return stage;
            }
            yield return "Конец дешифрации Квадратом";
            lastSecondStage = lastSecondStage.Substring(0, originalLength);
            yield return "Дешифрация Цезаря";
            var firstStage = _cesarEncoder.Decrypt(lastSecondStage, key.HasValue ? key.Value : default);
            var save = string.Empty;
            foreach (var stage in firstStage)
            {
                save = stage;
                yield return stage;
            }

            yield return "Конец дешифрации Цезаря";
            if (save.Length > originalLength)
            {
                save = save.Substring(0, originalLength);
            }

            FileSaver.SaveDecrypted(save);
        }




    }
}
