using Source.Model;
using System.Text.Json.Serialization;

namespace Source.Model
{
    public class State
    {
        [JsonPropertyName("Balls")]
        public ColoredBall[] Balls { get; set; }

    }
}