using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Source.AlphabetBuilder;
using Source.Encoder;
using Source.FileSystem;

namespace Source
{
    public class CryptoAnalyzator
    {
        private Dictionary<char, double> _tableStat = new Dictionary<char, double>()
        {
            {' ', 0.175 },
            {'о', 0.090 },
            {'е', 0.072 },
            {'ё', 0.072 },
            {'а', 0.062 },
            {'и', 0.062 },
            {'н', 0.053 },
            {'т', 0.053 },
            {'с', 0.045 },
            {'р', 0.040 },
            {'в', 0.038 },
            {'л', 0.035 },
            {'к', 0.028 },
            {'м', 0.026 },
            {'д', 0.025 },
            {'п', 0.023 },
            {'у', 0.021 },
            {'я', 0.018 },
            {'ы', 0.016 },
            {'з', 0.016 },
            {'ь', 0.014 },
            {'ъ', 0.014 },
            {'б', 0.014 },
            {'г', 0.013 },
            {'ч', 0.012 },
            {'й', 0.010 },
            {'х', 0.009 },
            {'ж', 0.007 },
            {'ю', 0.006 },
            {'ш', 0.006 },
            {'ц', 0.004 },
            {'щ', 0.003 },
            {'э', 0.003 },
            {'ф', 0.002 }
        };

        private readonly PolybEncoder _encoder = new PolybEncoder();
        public Dictionary<char, int> _frequencyTable = new Dictionary<char, int>();

        public Dictionary<char, double> _charAppearChance = new Dictionary<char, double>();

        

        private int _textLength = 0;


        public void Analyze()
        {

            var alphabets = FileLoader.LoadAlphabets();
            var message = FileLoader.LoadEncrypted();
            var sb = new StringBuilder();
            Dictionary<double, char[]> bestPair = new Dictionary<double, char[]>()
            {
                { 100000, new char[0]}
            };
            

            foreach (var alphabet in alphabets)
            {
                _frequencyTable.Clear();
                _charAppearChance.Clear();
                
                var decrypted = _encoder.Decrypt(message, alphabet);

                _textLength = decrypted.Length;

                CreateFrequencyTable(decrypted);

                CalculateAppearChance();

                double w = CalculateW();

                bestPair[w] = alphabet;

                sb.Append("=========================\n");

                sb.Append(AlphabetStringBuilder.Build(alphabet));

                MakeSnapshot(sb, w);

                sb.Append(Environment.NewLine);
            }
            sb.Append("-------- Заключение ---------\n");

            var maxWkey = bestPair.Min(p => p.Key);
            var bestAlphabet = bestPair[maxWkey];
            var forDecrypt = FileLoader.LoadEncrypted();
            var messageForDecrypt = _encoder.Decrypt(forDecrypt, bestAlphabet);
            FileSaver.SaveDecryptedWithWW(messageForDecrypt);
            var alphabetString = AlphabetStringBuilder.Build(bestAlphabet);
            sb.Append($"Лучшим алфавитом оказался:\n");
            sb.Append(alphabetString + Environment.NewLine);
            sb.Append($"W = {maxWkey}\n");
            var report = sb.ToString();
            FileSaver.SaveAnalyze(report);
        }
        private void MakeSnapshot(StringBuilder builder, double w)
        {
            builder.Append($"{BuildFrequencyTable()}\n");
            builder.Append($"{BuildChanceTable()}]\n");
            builder.Append($"W = {w}");
        }
        private string BuildFrequencyTable()
        {
            var sb = new StringBuilder();
            sb.Append("Таблица частот:\n");
            foreach ((char ch, int count) in _frequencyTable)
            {
                sb.Append($"[{ch}] : [{count}]\n");
                
            }
            return sb.ToString();
        }
        private string BuildChanceTable()
        {
            var sb = new StringBuilder();
            sb.Append("Таблица вероятности:\n");
            foreach ((char ch, double chance) in _charAppearChance)
            {
                sb.Append($"[{ch}] : [{chance}]\n");
            }
            return sb.ToString();
        }
        
        private void CalculateAppearChance()
        {
            foreach (var ch in _frequencyTable.Keys)
            {
                _charAppearChance[ch] = (double)_frequencyTable[ch] / _textLength;
            }
        }

        private void CreateFrequencyTable(string content)
        {
            for (int i = 0; i < content.Length; i++)
            {
                if (_frequencyTable.ContainsKey(content[i]))
                {
                    _frequencyTable[content[i]]++;
                }
                else
                {
                    _frequencyTable[content[i]] = 1;
                }
            }
        }
        
        private double CalculateW()
        {
            double w = 0;

            foreach ((char ch, double chance) in _charAppearChance)
            {
                if (!_tableStat.ContainsKey(ch))
                {
                    continue;
                }
                var diff = chance - _tableStat[ch];

                w += Math.Pow(diff, 2);
            }

            return w;
        }
    }
}