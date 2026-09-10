using Source.Common;
using Source.Commons;
using Source.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Source
{
    public class GameEngine
    {
        public GameContext Context { get; init; }

        private Dfs _graph { get; init; }

        public GameEngine()
        {
            try
            {
                (GameGeometry geometry, UserInput inputs) = DataLoader.LoadData();

                Context = new GameContext(geometry, inputs);

                _graph = new Dfs(Context);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[{nameof(GameEngine)}] : {ex.Message}");
            }
        }
        public List<(string ringName, int direction)> Play()
            => _graph.FindWay();

      

    }
}
