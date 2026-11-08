using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Source.Commons
{
    public static class AnaliticSaver
    {
        public static void SaveAnalitic(SearchStatistics statistic)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(statistic, options);
            var path = Path.Combine("statistic", $"{Guid.NewGuid()}");
            File.WriteAllText(path, json);
        }
    }
}