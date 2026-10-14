using System.Drawing;
using System.Reflection.Metadata;
using Source.Model;

namespace Source.Common
{

    public class Dfs
    {
        private HashSet<string> _visited { get; set; } = new();
        private HashSet<string> _closed { get; set; } = new();
       
        private int _maxBallsCount;
        private int _iterations = 0;
        private int _maxOpenSize = 0;
        private int _maxTotalSize = 0;
        private bool isFoundTheWay = false;
        private bool _useSecondHeuristic = false;
        private readonly GameContext _context;
        // private Stack<(State state, int depth)> _possibleWays { get; set; } = new();
        private PriorityQueue<(State state, int g), int> _possibleWays { get; set; } = new(); // (s, g), f, где ф - приор
        private Dictionary<string, int> _bestG { get; set; } = new();
        private readonly Dictionary<AppColors, Dictionary<string, int>> _patternDistances = new();

        private Dictionary<string, (string parentKey, string ringName, int direction)> _parent = new();

        public Dfs(GameContext context, bool useSecondHeurist = false)
        {
            _context = context;
            _maxBallsCount = context.Geometry.Rings.First().Positions.Length;
            _useSecondHeuristic = useSecondHeurist;
            foreach(var color in _context.InputData.TargetState.Balls.Select(b => b.Color).Distinct())
            {
                _patternDistances[color] = BuildPatternDistance(color);
            }
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
            Reset();

            State start = _context.InputData.StartState;

            string startKey = ColorToString(start);

            int startG = 0;

            int startH = CalculateHeuristic(start);

            int startF = startG + startH;

            _bestG[startKey] = startG;

            _visited.Add(startKey);

            _possibleWays.Enqueue((start, startG), startF);

            while (_possibleWays.Count > 0)
            {
                _iterations++;

                var currentNode = _possibleWays.Dequeue();

                var currentState = currentNode.state;

                string currentKey = ColorToString(currentState);

                int currentG = currentNode.g;

                _maxOpenSize = Math.Max(
               _maxOpenSize,
               _possibleWays.Count + 1);

                _maxTotalSize = Math.Max(
                _maxTotalSize,
                _possibleWays.Count + _closed.Count + 1);

                if (_bestG.TryGetValue(currentKey, out var bestG) && currentG != bestG)
                {
                    continue; 
                }

                if (IsTarget(currentState))
                {
                    return BuildPath(currentKey);
                }

                _closed.Add(currentKey);

                foreach (var action in GetPossibleActions())
                {
                    State nextState = Rotate(
                            currentState,
                            action.ringName,
                            action.direction
                    );

                    string nextKey = ColorToString(nextState);

                    int newG = currentG + 1;

                    if (_bestG.TryGetValue(nextKey, out var prevG) && prevG <= newG)
                    {
                        continue;
                    }

                    _bestG[nextKey] = newG;
                    _visited.Add(nextKey);
                    _parent[nextKey] = (currentKey, action.ringName, action.direction);

                    int newH = CalculateHeuristic(nextState);

                    int newF = newH + newG;

                    _possibleWays.Enqueue((nextState, newG), newF);

                    _maxOpenSize = Math.Max(
                        _maxOpenSize,
                        _possibleWays.Count);

                    _maxTotalSize = Math.Max(
                        _maxTotalSize,
                        _possibleWays.Count + _closed.Count);
                }


            }
            return [];

            }

        private void Reset()
    {
        _visited.Clear();
        _closed.Clear();
        _possibleWays.Clear();
        _bestG.Clear();
        _parent.Clear();

        _iterations = 0;
        _maxOpenSize = 0;
        _maxTotalSize = 0;

        isFoundTheWay = false;
    }
        private string GetColorPattern(State state, AppColors color)
        {
            return string.Concat(state.Balls.Select(ball => ball.Color == color ? '1' : '0'));
        }

        private Dictionary<string, int> BuildPatternDistance(AppColors color)
        {

            var distance = new Dictionary<string, int>();

            var queue = new Queue<string>();

            string targetPattern = GetColorPattern(_context.InputData.TargetState, color);

            distance[targetPattern] = 0;
            queue.Enqueue(targetPattern);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                int currentDistance = distance[current];

                foreach (var ring in _context.Geometry.Rings)
                {
                    foreach (int direction in new[] { 1, -1 })
                    {
                        string next = RotatePattern(current, ring.Positions, direction);

                        if (distance.ContainsKey(next))
                        {
                            continue;
                        }

                        distance[next] = currentDistance + 1;
                        queue.Enqueue(next);
                    }
                }
            }

            return distance;
        }

        private string RotatePattern(string pattern, int[] pos, int direction)
        {
            var result = pattern.ToCharArray();

            int count = pos.Length;

            for (int i = 0; i < count; i++)
            {
                int sourceIndex = (i - direction + count) % count;

                result[pos[i]] = pattern[pos[sourceIndex]];
            }

            return new string(result);
        }
        
        private int CalculateHeuristic(State state)
        {
            return _useSecondHeuristic ?
            CalculateHeuristic_2(state) :
            CalculateHeuristic_1(state);
        }
        private int CalculateHeuristic_2(State state)
        {
            int maxDistance = 0;

            foreach (var (color, distances) in _patternDistances)
            {
                string pattern = GetColorPattern(state, color);

                if (distances.TryGetValue(pattern, out int distance))
                {
                    maxDistance = Math.Max(distance, maxDistance);
                }
            }

            return maxDistance;
        }

       
        private int CalculateHeuristic_1(State state)
        {
            State target = _context.InputData.TargetState;

            int missplaced = 0;

            for (int i = 0; i < state.Balls.Length; i++)
            {
                if (target.Balls[i].Color != state.Balls[i].Color)
                {
                    missplaced++;
                }
            }

            return (missplaced + _maxBallsCount - 1) / _maxBallsCount; // дабы вверх округлялось число

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
  
  
  
