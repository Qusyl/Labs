namespace Source.Model
{
    using System.Text.Json.Serialization;
    public class UserInput
    {
        [JsonPropertyName("Start")]
        public State StartState { get; set; }

        [JsonPropertyName("Target")]
        public State TargetState { get; set; }

    }
}
