using System.Drawing;
using Source.Model;

namespace Source
{
   
    public class Dfs
    {
        private HashSet<string> _visited { get; set; } = new();

        private readonly State _targetState;

        private Stack<(State currState, List<(string, int)> move)> _possibleWays { get; set; } = new();
    
        public Dfs(State startState, State targetState)
        {
            _targetState = targetState;

            var startVisit = ColorToString(startState);
            
            _visited.Add(startVisit);

            _possibleWays.Push((startState, new()));

        }
        private string ColorToString(State state)
        {
            var colors = new List<string>();

            for (int i = 0; i < state.Balls.Length; i++)
            {
                var ballColor = state.Balls[i].Color.ToString();
                colors.Add(ballColor);
            }

            return string.Join("-", colors);
        }
        public List<(string ringName, int direction)> GetPossibleActions()
        {
            var actions = new List<(string, int)>();

            foreach (var ring in GameGeometry.Rings)
            {
                actions.Add((ring.Name, -1));
                actions.Add((ring.Name, 1));
            }
            return actions;
        }
        public bool IsVisited(State state)
        {
            return _visited.Contains(ColorToString(state));
        }
        public List<(string ringName, int direction)> FindWay()
        {
            while (_possibleWays.Count > 0)
            {
                var way = _possibleWays.Pop();

                if (IsTarget(way.currState))
                {
                    return way.move;
                }

                List<(string ringName, int direction)> possibleActions = GetPossibleActions();

                foreach (var action in possibleActions)
                {
                    var stateAfterRotate = Rotate(way.currState, action.ringName, action.direction);

                    if (!IsVisited(stateAfterRotate))
                    {
                        var colorsAfterRotate = ColorToString(stateAfterRotate);

                        _visited.Add(colorsAfterRotate);

                        var newMove = new List<(string, int)>(way.move)
                        {
                            (action.ringName, action.direction)
                        };

                        _possibleWays.Push((stateAfterRotate, newMove));
                    }
                }
            }
            return [];
        }

        private bool IsTarget(State state)
        {
            return ColorToString(state) == ColorToString(_targetState);
        }
        public State Rotate(State current, string ringName, int direction)
        {
            int[] order = GameGeometry.Rings
            .FirstOrDefault(r => r.Name == ringName)!
            .Positions;

            int k = order.Length;

            ColoredBall[] newBallsPosition = (ColoredBall[])current.Balls.Clone();

            var oldColors = new AppColors[k];
           
            for (int i = 0; i < k; i++)
            {
                var currentColor = current.Balls[order[i]].Color;

                oldColors[i] = currentColor;
            }

            for (int i = 0; i < k; i++)
            {
                var a = i - direction;

                int sourceIndex = Mod(a, k);

                newBallsPosition[order[i]] = new ColoredBall
                {
                    Color = oldColors[sourceIndex]
                };
            }
            return new State
            {
                Balls = newBallsPosition
            };
        }

        private int Mod(int a, int m) => ((a % m) + m) % m;
    }
}