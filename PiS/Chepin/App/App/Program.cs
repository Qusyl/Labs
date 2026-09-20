class Program
{
    static void Main()
    {
        Random rnd = new Random();
        int N, M;
        do
        {
            Console.Write("Введите N (3..10): ");
            N = int.Parse(Console.ReadLine());
        } while (N < 3 || N > 10);
        do
        {
            Console.Write("Введите M (3..10): ");
            M = int.Parse(Console.ReadLine());
        } while (M < 3 || M > 10);
        int[,] A = new int[N, M];
        int[] B = new int[M];
        for (int i = 0; i < N; i++)
            for (int j = 0; j < M; j++)
                A[i, j] = rnd.Next(-5, 16); 
        for (int j = 0; j < M; j++)
            B[j] = rnd.Next(-5, 16);
        Console.WriteLine("\nИсходная матрица A:");
        PrintMatrix(A);
        Console.WriteLine("\nМассив B:");
        for (int j = 0; j < M; j++)
            Console.Write(B[j] + "\t");
        Console.WriteLine();
        for (int i = 0; i < N; i += 2)
            for (int j = 0; j < M; j++)
                A[i, j] = B[j];
        Console.WriteLine("\nВидоизменённая матрица A:");
        PrintMatrix(A);
    }
    static void PrintMatrix(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        for (int i = 0; i < rows; i++)  
        {
            for (int j = 0; j < cols; j++)
                Console.Write(matrix[i, j] + "\t");
            Console.WriteLine();
        }
    }
}