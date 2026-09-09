// See https://aka.ms/new-console-template for more information
using System.Drawing;
using Source;
using Source.Model;

var startState = new State
{
    Balls =
    [
        new() { Color = AppColors.Purple }, // 0
        new() { Color = AppColors.Cyan },   // 1
        new() { Color = AppColors.Orange }, // 2
        new() { Color = AppColors.Purple }, // 3
        new() { Color = AppColors.Cyan },   // 4
        new() { Color = AppColors.Orange }, // 5
        new() { Color = AppColors.Purple }, // 6
        new() { Color = AppColors.Cyan },   // 7
        new() { Color = AppColors.Orange }, // 8
        new() { Color = AppColors.Purple }, // 9
        new() { Color = AppColors.Cyan },   // 10
        new() { Color = AppColors.Orange }, // 11
    ]
};
var generator = new Dfs(startState, startState);

var targetState = startState;

targetState = generator.Rotate(targetState, "Red", 1);
targetState = generator.Rotate(targetState, "Yellow", 1);
targetState = generator.Rotate(targetState, "White", -1);
var startCounts = startState.Balls
.GroupBy(b => b.Color)
.ToDictionary(g => g.Key,
 g => g.Count());

var targetCounts = targetState.Balls
.GroupBy(b => b.Color)
.ToDictionary(g => g.Key,
 g => g.Count());

bool ok = startCounts.Count == targetCounts.Count &&
          startCounts.All(kvp => targetCounts.ContainsKey(kvp.Key) && targetCounts[kvp.Key] == kvp.Value);

if (!ok)
{
    throw new InvalidDataException("Start и Target не совместимы! [Наборы цветов не совпадают]");
}
var dfs = new Dfs(startState, targetState);

var movablePositions = GameGeometry.Rings
    .SelectMany(r => r.Positions)
    .ToHashSet();

for (int i = 0; i < startState.Balls.Length; i++)
{
    if (!movablePositions.Contains(i))
    {
        if (startState.Balls[i].Color != targetState.Balls[i].Color)
        {
            throw new InvalidOperationException(
                $"Позиция {i} никогда не участвует в вращении, " +
                $"но цвета отличаются: " +
                $"{startState.Balls[i].Color} -> {targetState.Balls[i].Color}");
        }
    }
}

var solution = dfs.FindWay();

if (solution.Count == 0)
{
    System.Console.WriteLine("Решений не найдено!");
}

else
{
    System.Console.WriteLine($"Получен путь из {solution.Count} шагов");
    var c = 1;
    foreach(var step in solution)
    {
        var ringName = step.ringName;
        var rotate = step.direction == 1 ? "право" : "лево";
        System.Console.WriteLine($"[{c++}] повернуть {ringName} на {rotate}");
    }
  
}