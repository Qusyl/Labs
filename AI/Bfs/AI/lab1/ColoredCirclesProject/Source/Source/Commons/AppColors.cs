using System.Text.Json.Serialization;

namespace Source.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AppColors
    {
        Red,

        Green,

        Blue,

        Purple,

        Yellow,

        Cyan,

        White,
    }
}