namespace Source.Commons
{
    using Source.Model;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public static class DataLoader
    {
        private static JsonSerializerOptions _jsonOptions { get;  } = new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, PropertyNameCaseInsensitive = true };
        public static (GameGeometry Geometry, UserInput InputData) LoadData()
        {
            var geometry = InitializeGeometry() ?? throw new InvalidDataException("Failed to load geometry");

            var input = InitializeUserInput() ?? throw new InvalidDataException("Failed to initiallize user inputs");

            return (geometry, input);
        }
        private static UserInput InitializeUserInput()
        {
            var directory = FindDirectory();

            var jsonPath = Path.Combine(directory, "Data", "Geometry.json");

            var jsonContent = File.ReadAllText(jsonPath, Encoding.UTF8);
            if (jsonContent.StartsWith("\uFEFF"))
                jsonContent = jsonContent.Substring(1);
            var userInput = JsonSerializer.Deserialize<UserInput>(jsonContent, _jsonOptions);

            return userInput ?? throw new InvalidDataException("Failed to load user's input");
        }
        private static GameGeometry InitializeGeometry()
        {
            var directory = FindDirectory();
         
            var jsonPath = Path.Combine(directory, "Data", "Geometry.json");
           
            var jsonContent = File.ReadAllText(jsonPath, Encoding.UTF8);
            if (jsonContent.StartsWith("\uFEFF"))
                jsonContent = jsonContent.Substring(1);

            try
            {
                var geometry = JsonSerializer.Deserialize<GameGeometry>(jsonContent, _jsonOptions);
                return geometry ?? throw new InvalidDataException("Failed to load game geometry");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Ошибка парсинга JSON: {ex.Message}");
            
                throw;
            }
        }

        private static string FindDirectory() =>
          Directory.GetParent(Directory.GetCurrentDirectory())
          .Parent
          .Parent
          .Parent
          .Parent
          .FullName;
    }
}
