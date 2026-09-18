// See https://aka.ms/new-console-template for more information
using System.Text;
using Source.Encryptor;
using Source.Utills;

var encoder = new AlbertEncryptor();

var str = encoder.Encrypt("hello, dude","denis", 2);

System.Console.WriteLine(str);

