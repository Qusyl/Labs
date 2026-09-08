namespace Source.Model
{
    public static class GameGeometry
    {
        public static Dictionary<string, int[]> Geometry { get; } = new Dictionary<string, int[]>
        {
            {AppColors.Red.ToString(), new[] {3, 0, 1, 2}},
            {AppColors.Yellow.ToString(), new[] {2, 1, 6, 7} },
            {AppColors.White.ToString(), new[] {1, 4, 5, 6 } }
        };
    }
}