namespace Program
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("|a| |b| |c| |~b| |~bVA| |F|");
            for (int i = 0; i <= 7; i++)
            {
                int a = (i >> 2) & 1;
                int b = (i >> 1) & 1;
                int c = i & 1;
                int notB = b ^ 1;
                int notbOrA = notB | a;
                int F = notbOrA & c;
                Console.WriteLine($"{a}   {b}   {c}    {notB}     {notbOrA}     {F}");
            }
        }
    }
}