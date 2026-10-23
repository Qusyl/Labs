using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Source.FileSystem;

namespace Source;

public class Egsa
{
    private readonly int x;
    private int k;

    public StepLog SignLog { get; } = new();
    public StepLog VerifyLog { get; } = new();

    public Egsa()
    {
        x = Loader.LoadKey();
    }

    public string Sign(string message)
    {
        var log = SignLog;
        log.Clear();

        log.Add("Подписание", $"сообщение: \"{message}\"");

        int p = Loader.LoadP();
        int g = Loader.LoadG();
        log.Add("Шаг 1. Параметры", $"p = {p}, g = {g}");

        log.Add("Шаг 2. Закрытый ключ", $"x = {x}");

        int y = CalculateY(p, g);
        log.Add("Шаг 3. Открытый ключ", $"y = g^x mod p = {g}^{x} mod {p} = {y}");

        int m = CalculateM(Encoding.UTF8.GetBytes(message), p, log, "Шаг 4. Хеш сообщения");

        GenerateK(p, log, "Шаг 5. Случайное k");

        int a = CalculateA(g, p);
        log.Add("Шаг 6. Вычисление a", $"a = g^k mod p = {g}^{k} mod {p} = {a}");

        int kInv = ModularInverse(k, p - 1);
        log.Add("Шаг 7. Обратный к k", $"k⁻¹ mod (p-1) = {k}⁻¹ mod {p - 1} = {kInv}");

        int b = CalculateB(m, a, kInv, p);
        log.Add("Шаг 8. Вычисление b",
            $"b = (m - x·a)·k⁻¹ mod (p-1) = ({m} - {x}·{a})·{kInv} mod {p - 1} = {b}");

        log.Add("Итог", $"подпись (a, b) = ({a}, {b})");

        return $"{a} {b}";
    }

    public bool Verify(string message, string[] sign)
    {
        var log = VerifyLog;
        log.Clear();

        log.Add("Проверка подписи", $"сообщение: \"{message}\", подпись: ({sign[0]}, {sign[1]})");

        int p = Loader.LoadP();
        int g = Loader.LoadG();
        int y = Loader.LoadY();
        log.Add("Шаг 1. Параметры и открытый ключ", $"p = {p}, g = {g}, y = {y}");

        int a = int.Parse(sign[0]);
        int b = int.Parse(sign[1]);

        bool rangeOk = a >= 1 && a < p && b >= 0 && b < p - 1;
        log.Add("Шаг 2. Проверка диапазонов",
            $"1 ≤ a < {p} и 0 ≤ b < {p - 1}: a = {a}, b = {b} → {(rangeOk ? "выполнено" : "НЕ выполнено")}");

        if (!rangeOk)
        {
            log.Add("Итог", "✗ подпись отклонена: параметры вне допустимого диапазона");
            return false;
        }

        int m = CalculateM(Encoding.UTF8.GetBytes(message), p, log, "Шаг 3. Хеш сообщения");

        int left = (int)(BigInteger.ModPow(y, a, p) * BigInteger.ModPow(a, b, p) % p);
        log.Add("Шаг 4. Левая часть", $"y^a · a^b mod p = {y}^{a} · {a}^{b} mod {p} = {left}");

        int right = (int)BigInteger.ModPow(g, m, p);
        log.Add("Шаг 5. Правая часть", $"g^m mod p = {g}^{m} mod {p} = {right}");

        bool ok = left == right;
        log.Add("Итог", ok
            ? $"✓ {left} = {right}: подпись корректна"
            : $"✗ {left} ≠ {right}: подпись недействительна или сообщение изменено");

        return ok;
    }

    private int CalculateM(byte[] data, int p, StepLog log, string title)
    {
        var hash = SHA256.HashData(data);
        byte[] lastBytes = { hash[30], hash[31] };
        int shortHash = (hash[30] << 8) | hash[31];

        int m = shortHash % (p - 1);

        log.Add(title,
        $"SHA-256 = {Convert.ToHexString(hash)}; " +
        $"последние 2 байта = {Convert.ToHexString(lastBytes)} ({shortHash}); " +
        $"m = {shortHash} mod ({p} - 1) = {m}");
        return m;
    }

    private int CalculateA(int g, int p) => (int)BigInteger.ModPow(g, k, p);

    private int CalculateB(int m, int a, int kInv, int p)
    {
        long mod = p - 1;
        long diff = ((m - (long)x * a) % mod + mod) % mod;
        return (int)(diff * kInv % mod);
    }

    private int CalculateY(int p, int g)
    {
        int y = (int)BigInteger.ModPow(g, x, p);
        Saver.SaveKeyY(y);
        return y;
    }

    private void GenerateK(int p, StepLog log, string title)
    {
        int mod = p - 1;
        int attempts = 0;

        do
        {
            k = Random.Shared.Next(2, p - 1);
            attempts++;
        }
        while (GreatestCommonDivisor(k, mod) != 1);

        log.Add(title, $"k = {k}, НОД(k, {mod}) = 1 (попыток: {attempts})");
    }

    private int GreatestCommonDivisor(int a, int b)
    {
        while (b != 0)
            (a, b) = (b, a % b);

        return a;
    }

    private int ModularInverse(int a, int mod)
    {
        long t = 0, newT = 1;
        long r = mod, newR = a;

        while (newR != 0)
        {
            long q = r / newR;
            (t, newT) = (newT, t - q * newT);
            (r, newR) = (newR, r - q * newR);
        }

        if (r != 1)
            throw new ArgumentException("Обратного элемента не существует");

        return (int)((t % mod + mod) % mod);
    }
}