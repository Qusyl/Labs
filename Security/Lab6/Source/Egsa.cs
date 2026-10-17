using System.Numerics;
using System.Text;
using Source.FileSystem;

namespace Source;

public class Egsa
{
    private readonly int x;

    private int k;


    public Egsa()
    {
        x = Loader.LoadKey();
    }

    public string Sign(string message)
    {
        var dataBytes = Encoding.UTF8.GetBytes(message);

        int p = Loader.LoadP();

        int g = Loader.LoadG();

        int m = CalculateM(dataBytes, p);

        GenerateK(p);

        int a = CalculateA(g, p);

        int b = CalculateB(m, a, p);

        string sign = $"{a} {b}";

        return sign;
    }

    public bool Verify(string message,string[] sign)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);

        int p = Loader.LoadP();

        int g = Loader.LoadG();

        var a = int.Parse(sign[0]);

        var b = int.Parse(sign[1]);

        if (a < 1 || a >= p || b < 0 || b >= p - 1)
        {

            return false;
        }

        int y = CalculateY(p, g);

        int m = CalculateM(messageBytes, p);

        var firstStatement = (int)(BigInteger.ModPow(y, a, p) * BigInteger.ModPow(a, b, p) % p);

        var secondStatement = (int)BigInteger.ModPow(g, m, p);

        return firstStatement == secondStatement;
    }

    private int CalculateM(byte[] data, int p)
    {
        var mod = p - 1;
        var hash = System.Security.Cryptography.SHA256.HashData(data);

        var hashBig = new BigInteger(hash, isUnsigned: true, isBigEndian: false);

        var mBig = hashBig % mod;

        int m = (int)mBig;

        if (m == 0) m = 1;

        return m;
    }

    private int CalculateA(int g, int p)
{
    return (int)BigInteger.ModPow(g, k, p);
}

    private int CalculateB(int m, int a, int p)
    {
        var mod = p - 1;
        var reversK = ModularInverse(k, mod);
        int value = (m - x * a) * reversK;
       return (value % mod + mod) % mod;
    }

    private int CalculateY(int p, int g)
{
    return (int)BigInteger.ModPow(g, x, p);
}
    private void GenerateK(int p)
{
    int mod = p - 1;

    do
    {
        k = Random.Shared.Next(2, p - 1);
    }
    while (GreatestCommonDivisor(k, mod) != 1);
}

private int GreatestCommonDivisor(int a, int b)
{
    while (b != 0)
    {
        (a, b) = (b, a % b);
    }

    return a;
}

    

    private int ModularInverse(int  a, int mod)
    {
        int t = 0;

        int newT = 1;

        int r = mod;

        int newR = a;

        while (newR != 0)
        {
            int quotient = r / newR;

            (t, newT) = (newT, t - quotient * newT);
            (r, newR) = (newR, r - quotient * newR);

        }

        if (r != 1)
        {
            throw new ArgumentException("Обратного аргумента не существует");
        }

        return (t % mod + mod) % mod;
    }

}
