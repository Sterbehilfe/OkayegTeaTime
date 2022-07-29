using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public class OwmTemperature
{
    [JsonPropertyName("day")]
    public double Day { get; set; }

    [JsonPropertyName("min")]
    public double Min { get; set; }

    [JsonPropertyName("max")]
    public double Max { get; set; }

    [JsonPropertyName("night")]
    public double Night { get; set; }

    [JsonPropertyName("eve")]
    public double Evening { get; set; }

    [JsonPropertyName("morn")]
    public double Morning { get; set; }
}
