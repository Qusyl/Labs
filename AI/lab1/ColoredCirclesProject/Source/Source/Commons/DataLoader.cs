namespace Source.Commons
{
    using Source.Common;
    using Source.Model;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public static class DataLoader
    {
         public static void Init()
        {
          

    var directory = FindDirectory();

    

    var pathGeometry = Path.Combine(directory, "Data", "Geometry.json");
    var pathInput = Path.Combine(directory, "Data", "Input.json");

  

            var startState = new State
            {
                Balls =
         [
        new() { Color = AppColors.Purple }, // 0
        new() { Color = AppColors.Cyan },   // 1
        new() { Color = AppColors.Red }, // 2
        new() { Color = AppColors.Purple }, // 3
        new() { Color = AppColors.Cyan },   // 4
        new() { Color = AppColors.Red }, // 5
        new() { Color = AppColors.Purple }, // 6
        new() { Color = AppColors.Cyan },   // 7
        new() { Color = AppColors.Red }, // 8
        new() { Color = AppColors.Purple }, // 9
        new() { Color = AppColors.Cyan },   // 10
        new() { Color = AppColors.Red }, // 11
    ]
            };

            var targetState = new State
            {
                Balls =
         [
        new() { Color = AppColors.Purple }, // 0
        new() { Color = AppColors.Purple },   // 1
        new() { Color = AppColors.Purple }, // 2
        new() { Color = AppColors.Purple }, // 3
        new() { Color = AppColors.Cyan },   // 4
        new() { Color = AppColors.Cyan }, // 5
        new() { Color = AppColors.Cyan }, // 6
        new() { Color = AppColors.Cyan },   // 7
        new() { Color = AppColors.Red }, // 8
        new() { Color = AppColors.Red }, // 9
        new() { Color = AppColors.Red },   // 10
        new() { Color = AppColors.Red }, // 11
    ]
            };

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };
            var geometry = new GameGeometry();
            if(geometry.Rings.Length == 0)
            {
                throw new Exception("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            }
            var contentGeometry = JsonSerializer.Serialize(geometry, options);
            var uIn = new UserInput
            {
                TargetState = targetState,
                StartState = startState
            };

            var input = JsonSerializer.Serialize(uIn, options);

            File.WriteAllText(pathInput, input);

            File.WriteAllText(pathGeometry, contentGeometry);

        }
        private static JsonSerializerOptions _jsonOptions { get;  } = new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } };
        public static (GameGeometry Geometry, UserInput InputData) LoadData()
        {
             var input = InitializeUserInput() ?? throw new InvalidDataException("Failed to initiallize user inputs");
            var geometry = InitializeGeometry() ?? throw new InvalidDataException("Failed to load geometry");
            return (geometry, input);
        }
      private static UserInput InitializeUserInput()
{
    var directory = FindDirectory();
    var jsonPath = Path.Combine(directory, "Data", "Input.json");

    if (!File.Exists(jsonPath))
    {
        throw new FileNotFoundException("Input.json not found", jsonPath);
    }

    var jsonContent = File.ReadAllText(jsonPath, Encoding.UTF8);

  

    try
    {
        var userInput = JsonSerializer.Deserialize<UserInput>(
            jsonContent,
            _jsonOptions);

        return userInput
            ?? throw new InvalidDataException("Input.json deserialized to null");
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"JSON EXCEPTION: {ex.Message}");
        throw;
    }
}
    private static GameGeometry InitializeGeometry()
{
   

    var directory = FindDirectory();

   

    var jsonPath = Path.Combine(directory, "Data", "Geometry.json");



    var jsonContent = File.ReadAllText(jsonPath);

  

    try
    {
        var geometry = JsonSerializer.Deserialize<GameGeometry>(
            jsonContent,
            _jsonOptions);

       

        return geometry!;
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"JSON EXCEPTION: {ex.Message}");
        throw;
    }
}

        private static string FindDirectory() =>
          Directory.GetParent(Directory.GetCurrentDirectory())
          .Parent
          .FullName;
    }
}
