using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Source.Utills;

namespace Source.Encryptor
{
    public class AlbertEncryptor
    {
        private readonly char[] _externalDisk;

        private char[] _internalDisk;
        private readonly int _basedStep;

        private int manualStep = 1;

        public AlbertEncryptor()
        {
            _externalDisk = InitExternalDisk();
            InitInternalDisk();
    
          
           _basedStep = InitBasedStep();
        }

        public IEnumerable<char[]> Encrypt(string message, int step)
        {
            Clear();
            InitInternalDisk();
            char[] encrypted = new char[message.Length];

            char[] internalDisk = new char[_internalDisk.Length];

            Array.Copy(_internalDisk, internalDisk, _internalDisk.Length);
            internalDisk = ShiftAlphabetEncoding(internalDisk, _basedStep);

            for (int i = 0; i < message.Length; i++)
            {
                var indexOfCharExternalDisk = Array.IndexOf(_externalDisk, message[i]);

                encrypted[i] = internalDisk[indexOfCharExternalDisk];

                Save(internalDisk);

                internalDisk = ShiftAlphabetEncoding(internalDisk, step);

                yield return encrypted.ToArray();
            }
            File.WriteAllText(PathLoader.EncryptedPath(), new string(encrypted));
        }
    
        public char[] GetExternalDisk()
        {
            var exDisk = new char[_externalDisk.Length];

            Array.Copy(_externalDisk, exDisk, _externalDisk.Length);

            return exDisk;
        }
        public IEnumerable<char[]> Decrypt(string message, int step)
        {
            Clear();
            InitInternalDisk();
            
            char[] decrypted = new char[message.Length];

            var internalDisk = new char[_internalDisk.Length];

            Array.Copy(_internalDisk, internalDisk, _internalDisk.Length);

            internalDisk = ShiftAlphabetEncoding(internalDisk, _basedStep);

            for (int i = 0; i < message.Length; i++)
            {
                var indexOfCharInternalDisk = Array.IndexOf(internalDisk, message[i]);
                if (indexOfCharInternalDisk == -1)
                {
                    throw new Exception($"Символ '{message[i]}' не найден в диске");
                }
                decrypted[i] = _externalDisk[indexOfCharInternalDisk];
                Save(internalDisk);
                internalDisk = ShiftAlphabetEncoding(internalDisk, step);
                yield return decrypted.ToArray();
            }
            
            File.WriteAllText(PathLoader.DecryptedPath(), new string(decrypted));
        }

        public char[] GetShiftAlphabet()
        {

            var internalDisk = new char[_internalDisk.Length];

            Array.Copy(_internalDisk, internalDisk, _internalDisk.Length);

            return internalDisk;
        }
        private void Save(char[] snapshot)
        {
            var path = PathLoader.TempPath();
            File.AppendAllText(path, new string(snapshot) + Environment.NewLine);
        }
        private void Clear()
        {
            var path = PathLoader.TempPath();
            File.WriteAllText(path, "");
        }
        private char[] ShiftAlphabetEncoding(char[] current, int step)
        {
            int m = current.Length;

            char[] shifted = new char[m];

            for(int i = 0; i < current.Length; i++)
            {
                shifted[(i + step) % m] = current[i];
            }
            return shifted;

        }
    
        private int InitBasedStep()
        {
            var path = PathLoader.BasedStepPath();

            return int.Parse(File.ReadAllText(path).Replace("\r", "").Replace("\n", ""));
        }

        private char[]? InitExternalDisk()
        {
            var externalDisk = File.ReadAllText(PathLoader.AlphabetPath())
            .Replace("\n", "")
            .Replace("\r", "");

            return externalDisk?.ToArray();
        }



        public  void InitInternalDisk()
        {
            var keyword = File
            .ReadAllText(PathLoader.KeyWordPath())
            .Replace("\n", "")
            .Replace("\r", ""); 
            _internalDisk = new char[_externalDisk.Length];

            var visited = new HashSet<char>();

            var startPos = 0;
            for (int i = 0; i < keyword.Length; i++)
            {
                if (!visited.Contains(keyword[i]))
                {
                    _internalDisk[startPos] = keyword[i];
                    visited.Add(keyword[i]);
                    startPos++;
                }
                
            }
            
            for(int i = 0; i < _externalDisk.Length ; i++)
            {
                if (!visited.Contains(_externalDisk[i]))
                {
                    _internalDisk[startPos] = _externalDisk[i];
                    visited.Add(_externalDisk[i]);
                    startPos++;
                }
                
               
            }

            var path = PathLoader.ShiftedAlphabetPath();

            var forSave = new string(_internalDisk);

            File.WriteAllText(path, forSave);
        }        
    }
}