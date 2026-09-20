namespace Program
{
    public class Program
    {
        public static long F(int n, int k)
        {
            if (k < 0 || k > n)
            {
                return 0;
            } 
            if(k > n - k)
            {
                k = n - k;
            }
            long enumer = 1;
            long denomer = 1;
            for(int i = 1; i <= k; i++)
            {
                enumer *= (n - k + i);
                denomer *= i;
            }
            return enumer / denomer;
        }
        public static void Main(string[] args)
        {
            Console.Write("n,k = ");
            string[] input = Console.ReadLine()!.Split(',');
            (int n, int k) = (int.Parse(input[0]), int.Parse(input[1]));
            long result = F(n, k);
            Console.WriteLine(result);
        }
    }
}