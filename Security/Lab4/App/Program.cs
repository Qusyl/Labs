using Source;

var lfsr = new LFSR(
);

var cipher = new GammaCipher();

string message = "привет";        
int[] msgBits = cipher.TextToBits(message);

var gamma = lfsr.GenerateGamma(msgBits.Length);
var cipherBits = cipher.Xor(msgBits, gamma);
string cipherText = cipher.BitsToText(cipherBits);

Console.WriteLine("Сообщение:  " + message);
Console.WriteLine("Биты:       " + string.Join("", msgBits));
Console.WriteLine("Гамма:      " + string.Join("", gamma));
Console.WriteLine("Шифр (биты):" + string.Join("", cipherBits));
Console.WriteLine("Шифр (текст):" + cipherText);

// Проверка
var decryptedBits = cipher.Xor(cipherBits, gamma);
Console.WriteLine("Расшифровка: " + cipher.BitsToText(decryptedBits));