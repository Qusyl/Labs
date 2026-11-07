using System.Text.Json;
using Source.Common;
using Source.Commons;
using Source.Model;

namespace Generator
{
    public class Generator
    {
        public static void Main(string[] args)
        {
            int d = 2;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-d" || args[i] == "--d")
                {
                    if (i + 1 < args.Length)
                    {
                        if (int.TryParse(args[i + 1], out var depth))
                        {
                            d = depth;
                        }
                        else
                        {
                            System.Console.WriteLine("Не удалось определить d");
                            return;
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Значение -d ( --d ) не найдено");
                    }
                }


                FoundRequiredBfsStates(d);


            }
        }
        private static void FoundRequiredBfsStates(int d)
        {
            int resultsFound = 0;
            int needToFind = 10;
            SearchAlgorithm algorithm = SearchAlgorithm.BFS;

            HashSet<string> alreadyFound = new HashSet<string>();
           
            (GameGeometry geometry, UserInput input) data = DataLoader.LoadData();

            if(data.input is null)
            {
                System.Console.WriteLine("INPUT IS NULL");
                return;
            }
            var targetState = data.input.TargetState;
            var startColors = data.input.StartState.Balls;
            if (algorithm is SearchAlgorithm.BFS)
            {
                while (resultsFound < needToFind)
                {
                    var balls = new ColoredBall[startColors.Length];
                    Array.Copy(startColors, balls, startColors.Length);

                    FisherShuffle(balls);

                    var stateKey = string.Join("", balls.Select(c => c.Color));

                    if (!alreadyFound.Add(stateKey))
                    {
                        continue;
                    }

                    var startState = new State
                    {
                        Balls = balls
                    };
                    var usInput = new UserInput
                    {
                        StartState = startState,
                        TargetState = targetState
                    };
                    var context = new GameContext(data.geometry, usInput );

                    Bfs bfs = new Bfs(context);

                    var solutionLen = bfs.FindWay().Count;

                    if(solutionLen == d)
                    {
                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true
                        };
                        var currentState = JsonSerializer.Serialize(startState,options);

                        System.Console.WriteLine(currentState);
                     
                        var foundInput = new UserInput
                        {
                            StartState = startState,
                            TargetState = targetState
                        };
                        var serr = JsonSerializer.Serialize(foundInput,options);
                        File.AppendAllText("result_d_state.json", serr);
                        resultsFound++;
                        System.Console.WriteLine($"Найдено {resultsFound} / {needToFind} состояние");
                        
                    }
                   
                }
            
            }
            
        }
        
        private static void FisherShuffle(ColoredBall[] balls)
        {
            Random random = Random.Shared;
            for (int i = 0; i < balls.Length; i++)
            {
                int j = random.Next(i + 1);
                (balls[i], balls[j]) = (balls[j], balls[i]);
            }
        }

    }
}

