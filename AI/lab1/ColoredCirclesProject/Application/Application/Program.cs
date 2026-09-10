// See https://aka.ms/new-console-template for more information
using Source;

namespace Program
{
    using Source;
    using Source.Common;
    using Source.Model;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public static class Game
    {
        public static void Main()
        {
           
            var gameEngine = new GameEngine();

            gameEngine.Play();
        } 
    }
}
