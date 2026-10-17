using Source.FileSystem;
using Step = Source.AesStep.AesStep;

namespace Source;

public class Aes
{
    private readonly byte[] _sbox;

    public Aes() { _sbox = FileLoader.LoadSBox(); }

    public List<Step> Encrypt(byte[] text, byte[] key)
    {
        var steps = new List<Step>();
        byte[][] w = ExpandKey(key);

      
        byte[,] state = PrepareState(text);
        steps.Add(new Step
        {
            Name = "Входной блок",
            Description = "16 байт блока записываются в матрицу 4×4 по столбцам.",
            Explanation =
                "AES работает с блоком из 16 байт как с матрицей 4×4, которая называется State.\n\n" +
                "Байты записываются по столбцам: байты 0–3 образуют первый столбец, 4–7 второй и т.д.\n\n" +
                "Дальше в этом раунде над матрицей выполняются 4 операции, а перед ними начальное сложение с ключом (раунд 0).",
            State = state,
            AfterLabel = "Матрица State",
            Trace = TraceInput(text)
        });

        
        byte[,] k0 = RoundKey(w, 0);
        byte[,] next = AddRoundKey(state, k0);
        steps.Add(new Step
        {
            Name = "Раунд 0: AddRoundKey",
            Description = "Начальное сложение блока с исходным ключом.",
            Explanation =
                "Каждый байт State складывается по XOR с соответствующим байтом ключа.\n\n" +
                "XOR работает побитово: 0⊕0=0, 1⊕1=0, 0⊕1=1, 1⊕0=1.\n\n" +
                "Это единственная операция AES, где участвует ключ. Остальные шаги только перемешивают данные.",
            Before = state, BeforeLabel = "State",
            Operator = "⊕", Operand = k0, OperandLabel = "Ключ раунда 0",
            State = next, AfterLabel = "Результат",
            Trace = TraceXor(state, k0, next)
        });
        state = next;

      
        byte[,] k1 = RoundKey(w, 1);
        steps.Add(new Step
        {
            Name = "Раунд 1: ключ раунда",
            Description = "Из исходного ключа вычисляется ключ первого раунда.",
            Explanation =
                "Ключ делится на 4 слова (по 4 байта), каждое слово это столбец матрицы ключа.\n\n" +
                "Новые слова считаются так:\n" +
                "• w4 = w0 ⊕ g(w3), где g это RotWord → SubWord → XOR с Rcon\n" +
                "• w5 = w1 ⊕ w4\n• w6 = w2 ⊕ w5\n• w7 = w3 ⊕ w6\n\n" +
                "RotWord: циклический сдвиг слова на байт влево.\n" +
                "SubWord: замена каждого байта по S-Box.\n" +
                "Rcon: раундовая константа (для раунда 1 это 01 00 00 00).",
            Before = k0, BeforeLabel = "Ключ раунда 0",
            State = k1, AfterLabel = "Ключ раунда 1",
            Trace = TraceKeyExpansion(w)
        });

        
        next = SubBytes(state);
        steps.Add(new Step
        {
            Name = "Раунд 1: SubBytes",
            Description = "Каждый байт заменяется по таблице S-Box.",
            Explanation =
                "Старшие 4 бита байта выбирают строку S-Box, младшие 4 бита столбец. " +
                "Байт на пересечении и есть результат.\n\n" +
                "Эта операция нелинейная: именно она защищает AES от линейного и дифференциального криптоанализа.\n\n" +
                "Подсвечены ячейки, значение которых изменилось.",
            Before = state, BeforeLabel = "До SubBytes",
            State = next, AfterLabel = "После SubBytes",
            Trace = TraceSubBytes(state, next)
        });
        state = next;

      
        next = ShiftRows(state);
        steps.Add(new Step
        {
            Name = "Раунд 1: ShiftRows",
            Description = "Строки циклически сдвигаются влево на 0, 1, 2 и 3 позиции.",
            Explanation =
                "Строка 0 не двигается, строка 1 сдвигается на 1 байт влево, строка 2 на 2, строка 3 на 3. " +
                "Байты, вышедшие за левый край, возвращаются справа.\n\n" +
                "Зачем: после SubBytes байты в столбце никак не связаны с соседними столбцами. " +
                "ShiftRows разбрасывает байты каждого столбца по разным столбцам, " +
                "и на следующем шаге они смешиваются.",
            Before = state, BeforeLabel = "До ShiftRows",
            State = next, AfterLabel = "После ShiftRows",
            Trace = TraceShiftRows(state, next)
        });
        state = next;

        
        next = MixColumns(state, out var mixTrace);
        steps.Add(new Step
        {
            Name = "Раунд 1: MixColumns",
            Description = "Каждый столбец умножается на фиксированную матрицу в поле GF(2⁸).",
            Explanation =
                "Столбец (a0..a3) умножается на матрицу:\n" +
                "02 03 01 01\n01 02 03 01\n01 01 02 03\n03 01 01 02\n\n" +
                "Правила умножения в GF(2⁸):\n" +
                "• 01·x = x\n" +
                "• 02·x = сдвиг x влево на 1 бит; если старший бит был 1, ещё XOR с 1B\n" +
                "• 03·x = (02·x) ⊕ x\n" +
                "• сложение это XOR\n\n" +
                "В итоге каждый байт нового столбца зависит от всех четырёх байтов старого.",
            Before = state, BeforeLabel = "До MixColumns",
            State = next, AfterLabel = "После MixColumns",
            Trace = mixTrace
        });
        state = next;

    
        next = AddRoundKey(state, k1);
        var trace = TraceXor(state, k1, next);
        trace.Add("");
        trace.Add("Это результат первого раунда AES-128.");
        trace.Add("В полном AES дальше идут ещё 9 раундов (последний без MixColumns).");
        steps.Add(new Step
        {
            Name = "Раунд 1: AddRoundKey",
            Description = "Сложение с ключом раунда 1. Конец раунда.",
            Explanation =
                "Последняя операция раунда: XOR State с ключом раунда 1, который мы получили на шаге расширения ключа.\n\n" +
                "Результат это выход первого раунда.",
            Before = state, BeforeLabel = "State",
            Operator = "⊕", Operand = k1, OperandLabel = "Ключ раунда 1",
            State = next, AfterLabel = "Итог раунда 1",
            Trace = trace
        });

        return steps;
    }

   

    private byte[,] PrepareState(byte[] text)
    {
        var s = new byte[4, 4];
        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
                s[r, c] = text[c * 4 + r];
        return s;
    }

    private byte[,] AddRoundKey(byte[,] s, byte[,] k)
    {
        var res = new byte[4, 4];
        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
                res[r, c] = (byte)(s[r, c] ^ k[r, c]);
        return res;
    }

    private byte[,] SubBytes(byte[,] s)
    {
        var res = new byte[4, 4];
        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
                res[r, c] = _sbox[s[r, c]];
        return res;
    }

    private byte[,] ShiftRows(byte[,] s)
    {
        var res = new byte[4, 4];
        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
                res[r, c] = s[r, (c + r) % 4];
        return res;
    }

    private static readonly byte[,] MixMatrix =
    {
        { 2, 3, 1, 1 },
        { 1, 2, 3, 1 },
        { 1, 1, 2, 3 },
        { 3, 1, 1, 2 }
    };

    private byte[,] MixColumns(byte[,] s, out List<string> trace)
    {
        var res = new byte[4, 4];
        trace = [];

        for (int c = 0; c < 4; c++)
        {
            var a = new byte[4];
            for (int r = 0; r < 4; r++) a[r] = s[r, c];

            trace.Add($"Столбец {c}: {Hex(a)}");

            for (int r = 0; r < 4; r++)
            {
                var products = new byte[4];
                for (int k = 0; k < 4; k++)
                    products[k] = GaloisMultiply(a[k], MixMatrix[r, k]);

                byte result = (byte)(products[0] ^ products[1] ^ products[2] ^ products[3]);
                res[r, c] = result;

                string formula = string.Join(" ⊕ ",
                    Enumerable.Range(0, 4).Select(k => $"{MixMatrix[r, k]:X2}·{a[k]:X2}"));
                string values = string.Join(" ⊕ ", products.Select(p => p.ToString("X2")));

                trace.Add($"  b{r} = {formula}");
                trace.Add($"     = {values} = {result:X2}");
            }
            trace.Add("");
        }
        return res;
    }

    private byte GaloisMultiply(byte a, byte b) //множ поля галоу
    {
        byte p = 0;
        for (int i = 0; i < 8; i++)
        {
            if ((b & 1) == 1) p ^= a;
            bool hi = (a & 0x80) != 0;
            a <<= 1;
            if (hi) a ^= 0x1B;
            b >>= 1;
        }
        return p;
    }



    private byte[][] ExpandKey(byte[] key)
    {
        var w = new byte[44][];
        for (int i = 0; i < 4; i++)
            w[i] = [key[i * 4], key[i * 4 + 1], key[i * 4 + 2], key[i * 4 + 3]];

        byte raundConst = 1;
        for (int i = 4; i < 44; i++)
        {
            var prevWord = (byte[])w[i - 1].Clone();
            if (i % 4 == 0)
            {
                prevWord = [_sbox[prevWord[1]], _sbox[prevWord[2]], _sbox[prevWord[3]], _sbox[prevWord[0]]]; 
                prevWord[0] ^= raundConst;
                raundConst = GaloisMultiply(raundConst, 2);
            }
            w[i] = new byte[4];
            for (int j = 0; j < 4; j++)
                w[i][j] = (byte)(w[i - 4][j] ^ prevWord[j]);
        }
        return w;
    }

    private byte[,] RoundKey(byte[][] w, int round)
    {
        var k = new byte[4, 4];
        for (int c = 0; c < 4; c++)
            for (int r = 0; r < 4; r++)
                k[r, c] = w[round * 4 + c][r];
        return k;
    }

  

    private static string Hex(IEnumerable<byte> bytes) =>
        string.Join(" ", bytes.Select(b => b.ToString("X2")));

    private static string Bin(byte b) => Convert.ToString(b, 2).PadLeft(8, '0');

    private List<string> TraceInput(byte[] text)
    {
        var t = new List<string>();
        for (int c = 0; c < 4; c++)
            t.Add($"Байты {c * 4}–{c * 4 + 3}: {Hex(text.Skip(c * 4).Take(4))}  →  столбец {c}");
        return t;
    }

    private List<string> TraceXor(byte[,] s, byte[,] k, byte[,] res)
    {
        var t = new List<string>
        {
            $"Пример для [0,0] в двоичном виде:",
            $"  {Bin(s[0, 0])}",
            $"⊕ {Bin(k[0, 0])}",
            $"= {Bin(res[0, 0])}  ({res[0, 0]:X2})",
            ""
        };
        for (int c = 0; c < 4; c++)
            for (int r = 0; r < 4; r++)
                t.Add($"[{r},{c}]  {s[r, c]:X2} ⊕ {k[r, c]:X2} = {res[r, c]:X2}");
        return t;
    }

    private List<string> TraceSubBytes(byte[,] before, byte[,] after)
    {
        var t = new List<string>();
        for (int c = 0; c < 4; c++)
            for (int r = 0; r < 4; r++)
            {
                byte v = before[r, c];
                t.Add($"[{r},{c}]  {v:X2} → строка {v >> 4:X}, столбец {v & 0xF:X} → {after[r, c]:X2}");
            }
        return t;
    }

    private List<string> TraceShiftRows(byte[,] before, byte[,] after)
    {
        var t = new List<string>();
        for (int r = 0; r < 4; r++)
        {
            var b = Enumerable.Range(0, 4).Select(c => before[r, c]);
            var a = Enumerable.Range(0, 4).Select(c => after[r, c]);
            t.Add($"Строка {r} (сдвиг на {r}):");
            t.Add($"  {Hex(b)}  →  {Hex(a)}");
        }
        return t;
    }

    private List<string> TraceKeyExpansion(byte[][] w)
    {
        byte[] rot = [w[3][1], w[3][2], w[3][3], w[3][0]];
        byte[] sub = rot.Select(b => _sbox[b]).ToArray();
        byte[] g = (byte[])sub.Clone();
        g[0] ^= 0x01;

        var t = new List<string> { "Слова ключа раунда 0:" };
        for (int i = 0; i < 4; i++) t.Add($"  w{i} = {Hex(w[i])}");

        t.Add("");
        t.Add("Вычисляем g(w3):");
        t.Add($"1) RotWord: {Hex(w[3])} → {Hex(rot)}");
        t.Add($"2) SubWord: {Hex(rot)} → {Hex(sub)}");
        t.Add($"3) Rcon[1] = 01 00 00 00:");
        t.Add($"   {Hex(sub)} ⊕ 01 00 00 00 = {Hex(g)}");

        t.Add("");
        t.Add("Новые слова:");
        t.Add($"w4 = w0 ⊕ g = {Hex(w[0])} ⊕ {Hex(g)} = {Hex(w[4])}");
        for (int i = 5; i < 8; i++)
            t.Add($"w{i} = w{i - 4} ⊕ w{i - 1} = {Hex(w[i - 4])} ⊕ {Hex(w[i - 1])} = {Hex(w[i])}");
        return t;
    }
}