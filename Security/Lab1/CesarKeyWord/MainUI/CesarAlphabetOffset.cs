using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Source.Commons;
using Source.Encoders;

namespace MainUI
{
    public static class CesarAlphabetOffset
    {
        private static CesarEncoder _encoder = new CesarEncoder();
        public static (string alphabet, string alphabetOffset) ShowAlphabet(int k)
        {
            var primaryAlphabet = FileLoader.LoadAlphabet();

            var alphabetEncrypted = _encoder.Encrypt(primaryAlphabet, k);

            var withOffset = string.Empty;
            
            foreach (var num in alphabetEncrypted)
            {
                withOffset = num;
            }
            return (primaryAlphabet, withOffset);
        }
    }
}