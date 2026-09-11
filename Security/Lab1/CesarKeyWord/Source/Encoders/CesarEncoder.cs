namespace Source.Encoders
{
    public static class CesarEncoder 
    {
        private static readonly char[] Alphabet = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

        private const int key = 6;
        public static string Encrypt(string input)
        { 
            var result = new char[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                var J = Array.IndexOf(Alphabet, input[i]);

                if (J == -1)
                {
                    result[i] = input[i]; continue;
                }

                int I = (J + key) % Alphabet.Length;

                result[i] = Alphabet[I];
            }
            return new string(result);
        }

        public static string Decrypt(string input)
        {
            var result = new char[input.Length];
            var decryptKey = Alphabet.Length - key;
            for(int i =0; i < input.Length; i++)
            {
                var J = Array.IndexOf(Alphabet, input[i]);

                if (J == -1)
                {
                    result[i] = input[i];
                    continue;
                }

                int I = (J + decryptKey) % Alphabet.Length;

                result[i] = Alphabet[I];
            }
            return new string(result);
        }
    }
}
