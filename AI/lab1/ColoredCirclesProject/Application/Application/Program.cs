// See https://aka.ms/new-console-template for more information
using Source;

namespace Program
{

    using Source;


    public static class Game
    {

        public static void Main()
        {
            var gameEngine = new GameEngine();

            var result = gameEngine.Play();

            Console.WriteLine($"Число шагов: {result.Count}");
        }
    }
}
