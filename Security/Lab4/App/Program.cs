using Source;
using Source.FileSystem;

var lfsr = new LFSR(
);

var cipher = new GammaCipher();

string message = "Секрет";        
int[] msgBits = cipher.TextToBits(message);

var gamma = lfsr.GenerateGamma(msgBits.Length);
var cipherBits = cipher.Xor(msgBits, gamma);
string cipherText = cipher.BitsToText(cipherBits);
FileSaver.SaveCipherText(cipherText);

Console.WriteLine("Сообщение:  " + message);
Console.WriteLine("Биты:       " + string.Join("", msgBits));
Console.WriteLine("Гамма:      " + string.Join("", gamma));
Console.WriteLine("Шифр (биты):" + string.Join("", cipherBits));
Console.WriteLine("Шифр (текст):" + cipherText);


var decrypted = cipher.Decrypt(cipherBits, gamma);

