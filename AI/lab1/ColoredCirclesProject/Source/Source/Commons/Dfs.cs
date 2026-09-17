using System.Drawing;
using System.Reflection.Metadata;
using Source.Model;

namespace Source.Common
{
   
    public class Dfs
    {
        private HashSet<string> _visited { get; set; } = new();
        private  int MaxDepth = 10;

        private bool isFoundTheWay = false;
        private readonly GameContext _context;
        private Stack<(State currState, List<(string, int)> move)> _possibleWays { get; set; } = new();
    
        public Dfs(GameContext context)
        {
           
            _context = context;

            var startVisit = ColorToString(_context.InputData.StartState);
            
            _visited.Add(startVisit);

            _possibleWays.Push((_context.InputData.StartState, new()));

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
        private void ExtendDeepth()
        {
            if (!isFoundTheWay)
            {
                MaxDepth += 5;
            }
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
                _visited.Clear();
                _possibleWays.Clear();
                _visited.Add(ColorToString(_context.InputData.StartState));
                _possibleWays.Push((_context.InputData.StartState, new()));

                var result = FindWayWithMaxDepth(startDepth);
                if (result != null)
                {
                    return result;
                }
                startDepth += 1;
            }

            return [];
        }

        private List<(string ringName, int direction)>? FindWayWithMaxDepth(int limit)
        {
            while (_possibleWays.Count > 0)
            {
              
                var way = _possibleWays.Pop();

                if (IsTarget(way.currState))
                {
                    return way.move;
                }
                 if (way.move.Count >= limit)
                {
                  
                    continue;
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
            return null;
        }
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
  
  
  
