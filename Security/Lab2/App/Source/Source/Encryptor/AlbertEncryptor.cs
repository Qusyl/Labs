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

        public AlbertEncryptor()
        {
           _externalDisk = InitExternalDisk();
          
           _basedStep = InitBasedStep();
        }

        public string Encrypt(string message,string keyWord, int step)
        {
            InitInternalDisk(keyWord);
            char[] encrypted = new char[message.Length];

            char[] internalDisk = new char[_internalDisk.Length];
            
            Array.Copy(_internalDisk, internalDisk, _internalDisk.Length);
            internalDisk = ShiftAlphabetEncoding(internalDisk, _basedStep);

            for (int i = 0; i < message.Length; i++)
            {
                var indexOfCharExternalDisk = Array.IndexOf(_externalDisk, message[i]);
                
                encrypted[i] = internalDisk[indexOfCharExternalDisk];
                
                internalDisk = ShiftAlphabetEncoding(internalDisk, step);

            }

            return new string(encrypted);
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
     
        public string Decrypt(string message)
        {
            return null;
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

        public  void InitInternalDisk(string keyWord)
        {

            _internalDisk = new char[_externalDisk.Length];
            var visited = new HashSet<char>();

            var startPos = 0;
            for (int i = 0; i < keyWord.Length; i++)
            {
                if (!visited.Contains(keyWord[i]))
                {
                    _internalDisk[startPos] = keyWord[i];
                    visited.Add(keyWord[i]);
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