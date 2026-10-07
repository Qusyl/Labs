using System.Drawing;
using System.Reflection.Metadata;
using Source.Model;

namespace Source.Common
{

    public class Dfs
    {
        private HashSet<string> _visited { get; set; } = new();
        private HashSet<string> _closed { get; set; } = new();
       
       
        private int _iterations = 0;
        private int _maxOpenSize = 0;
        private int _maxTotalSize = 0;
        private bool isFoundTheWay = false;
        private readonly GameContext _context;
        private Stack<(State state, int depth)> _possibleWays { get; set; } = new();

        private Dictionary<string, (string parentKey, string ringName, int direction)> _parent = new();

        public Dfs(GameContext context)
        {

            _context = context;

            var startKey = ColorToString(_context.InputData.StartState);
            _visited.Add(startKey);
            _possibleWays.Push((_context.InputData.StartState, 1));

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

            foreach (var ring in _context.Geometry.Rings)
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
            var startDepth = 1;
            var maxDepth = 50;
            while (startDepth <= maxDepth)
            {
                Reset();               
                var start = _context.InputData.StartState;
                _visited.Add(ColorToString(start));
                _possibleWays.Push((start, 0));
                var result = FindWayWithMaxDepth(startDepth);
                if (result != null && result.Count > 0)
                {
                    return result;
                }
                startDepth += 1;

            }

            return [];
        }
        private void Reset()
        {
            _visited.Clear();
            _possibleWays.Clear();
            _parent.Clear();
            isFoundTheWay = false;
        }
        private List<(string ringName, int direction)> FindWayWithMaxDepth(int depthLimit)
        {

            while (_possibleWays.Count > 0 && !isFoundTheWay)
            {
                _iterations++;
                _maxOpenSize = Math.Max(_maxOpenSize, _possibleWays.Count);
                _maxTotalSize = Math.Max(_maxTotalSize, _possibleWays.Count + _closed.Count);
                var currentState = _possibleWays.Pop();
                

                var currKey = ColorToString(currentState.state);
                
                if(currentState.depth > depthLimit)
                {
                    continue;
                }

                if (IsTarget(currentState.state))
                {
                    return BuildPath(currKey);
                }
                if(currentState.depth == depthLimit)
                {
                    continue;
                }

                _closed.Add(currKey);

                List<(string ringName, int direction)> possibleActions = GetPossibleActions();

                foreach (var action in possibleActions)
                {
                    var stateAfterRotate = Rotate(currentState.state, action.ringName, action.direction);
                    var keyAfterRotate = ColorToString(stateAfterRotate);

                    if (!_visited.Contains(keyAfterRotate))
                    {
                        _visited.Add(keyAfterRotate);
                        _parent[keyAfterRotate] = (currKey, action.ringName, action.direction);
                        _possibleWays.Push((stateAfterRotate, currentState.depth++));
                    }
                }
            }
            return [];
        }
        private List<(string ringName, int direction)> BuildPath(string targetKey)
        {
            var path = new List<(string ringName, int direction)>();
            var key = targetKey;
            while (_parent.ContainsKey(key))
            {
                var (parentKey, ringName, direction) = _parent[key];
                path.Add((ringName, direction));
                key = parentKey;
            }
            path.Reverse();
            return path;
        }
        // private List<(string ringName, int direction)>? FindWayWithMaxDepth()
        // {
            
        // }
        public State Rotate(State current, string ringName, int direction)
        {
            int[] order = _context.Geometry.Rings
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
        private  int Mod(int a, int m) => ((a % m) + m) % m;
        private bool IsTarget(State state)
        {
            if (ColorToString(state) == ColorToString(_context.InputData.TargetState))
            {
                isFoundTheWay = true;
                return true;
            }
            return false;
        }
        private (char symbol, ConsoleColor color) GetColorSymbol(AppColors color)
{
    return color switch
    {
        AppColors.Purple => ('P', ConsoleColor.Magenta),
        AppColors.Cyan => ('C', ConsoleColor.Cyan),
        AppColors.Red => ('R', ConsoleColor.Red),
        AppColors.Green => ('G', ConsoleColor.Green),
        AppColors.Blue => ('B', ConsoleColor.Blue),
        AppColors.Yellow => ('Y', ConsoleColor.Yellow),
        AppColors.White => ('W', ConsoleColor.White),
        _ => throw new NotSupportedException($"Не известный цвет {color}")
    };
}
        public void PrintState(State state)
        {
            Console.WriteLine();
            Console.WriteLine("       0 1 2 3 4 5 6 7 8 9 10 11");

            foreach (var ring in _context.Geometry.Rings)
            {
                Console.Write($"{ring.Name,-7}");

                for (int position = 0; position < state.Balls.Length; position++)
                {
                    if (ring.Positions.Contains(position))
                    {
                        (char symbol, ConsoleColor color) = GetColorSymbol(state.Balls[position].Color);
                        Console.ForegroundColor = color;
                        Console.Write($" {symbol}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write("· ");
                    }
                }

                Console.WriteLine();
            }


            Console.WriteLine();
        }
public int Iterations => _iterations;
public int MaxOpenSize => _maxOpenSize;
public int CurrentOpenSize => _possibleWays.Count;
public int MaxTotalSize => _maxTotalSize;
public void PrintSolution(List<(string ringName, int direction)> solution)
{
    var currentState = _context.InputData.StartState;

    Console.WriteLine("========== Начало ==========");
    PrintState(currentState);

    for (int i = 0; i < solution.Count; i++)
    {
        var move = solution[i];

        currentState = Rotate(
            currentState,
            move.ringName,
            move.direction);

        Console.WriteLine(
            $"========== Шаг {i + 1}: " +
            $"{move.ringName} " +
            $"{(move.direction == 1 ? "Вправо" : "Влево")} ==========");

        PrintState(currentState);
    }

    Console.WriteLine("========== Цель ==========");
    PrintState(currentState);
}
    }
}  
  
  
  
