namespace Source.Encoders
{
    public class CesarEncoder 
    {
        private int _key = 0;
        public  string Encrypt(string path, int? enteredKey = default)
        {
            var input = ReadText("Input.txt");

            if (!enteredKey.HasValue)
            {
                LoadKey();
            }

            var alphabet = LoadAlphabet()
                .ToArray();

            var result = new char[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                var J = Array.IndexOf(alphabet, input[i]);

                if (J == -1)
                {
                    result[i] = input[i]; continue;
                }

                int I = (J + _key) % alphabet.Length;

                result[i] = alphabet[I];
            }
            return new string(result);
        }

        public string Decrypt(string input, int? enteredKey = default)
        {
            var result = new char[input.Length];

            var alphabet = LoadAlphabet()
                .ToArray() ;

            if (!enteredKey.HasValue)
            {
                LoadKey();
            }
            var decryptKey = alphabet.Length - _key;
            for(int i =0; i < input.Length; i++)
            {
                var J = Array.IndexOf(alphabet, input[i]);

                if (J == -1)
                {
                    result[i] = input[i];
                    continue;
                }

                int I = (J + decryptKey) % alphabet.Length;

                result[i] = alphabet[I];
            }
            return new string(result);
        }
        
        private string ReadText(string fileName)
        {
            var directory = FilePath.PathInputs();

            string textForEncryptPath = Path.Combine(directory,"Input.txt");

            if (!File.Exists(textForEncryptPath))
            {
                throw new InvalidOperationException($"Файл {fileName} не найден по пути {textForEncryptPath}!");
            }
               
            return File.ReadAllText(textForEncryptPath);
        }
        private string LoadAlphabet()
        {
            var directory = FilePath.PathSecrets();

            var alphabetPath = Path.Combine(directory, "Alphabet.txt");

            if (!File.Exists(alphabetPath))
            {
                throw new InvalidOperationException($"Файла с алфаитом не существует по пути {alphabetPath}");
            }

            return File.ReadAllText(alphabetPath);

        }
       
        private void LoadKey()
        {
            var keyPath = Path.Combine(FilePath.PathSecrets(), "c_key.txt");

            if (!File.Exists(keyPath))
            {
                throw new InvalidOperationException($"Ключ не найден! : [{keyPath}]");
            }

            _key = int.Parse(File.ReadAllText(keyPath));
        }
    }
}
