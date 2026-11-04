namespace Source
{
    using Source.Analyse;
    using Source.Common;
    using Source.Commons;
    using Source.Model;

    public class GameEngine
    {
        public GameContext Context { get; init; }

        private Bfs _graph { get; init; }

        public GameEngine()
        {
            (GameGeometry geometry, UserInput inputs) = DataLoader.LoadData();

            Context = new GameContext(geometry, inputs);

            _graph = new Bfs(Context);
        }

        public List<(string ringName, int direction)> Play()
        {
            return _graph.FindWay();
        }
        public AnalyseReport GetReport()
        {
            // return new AnalyseReport(_graph.Iterations, _graph.MaxOpenSize, _graph.MaxTotalSize, _graph.CurrentOpenSize);
            return new AnalyseReport(0, 0, 0,0);
        }
        public State ApplyMove(
            State state,
            string ringName,
            int direction)
        {
            return _graph.Rotate(
                state,
                ringName,
                direction);
        }
    }
}