


namespace Source
{
    using Source.Common;
    using Source.Commons;
    using Source.Model;
    public class GameEngine
    {
        public GameContext Context { get; init; }

        private Dfs _graph { get; init; }

public GameEngine()

        {
    // DataLoader.Init();
    (GameGeometry geometry, UserInput inputs) = DataLoader.LoadData();

    Context = new GameContext(geometry, inputs);

    _graph = new Dfs(Context);
}      public List<(string ringName, int direction)> Play()
{
    
    var result = _graph.FindWay();

    _graph.PrintSolution(result);

    return result;
}

      

    }
}
