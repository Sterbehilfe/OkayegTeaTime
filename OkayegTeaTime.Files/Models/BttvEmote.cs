#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public class BttvEmote
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("code")]
    public string Name { get; set; }

    [JsonPropertyName("imageType")]
    public string ImageType { get; set; }

    [JsonPropertyName("user")]
    public BttvUser User { get; set; }

#nullable enable
    public override bool Equals(object? obj)
    {
        return obj is BttvEmote emote && emote.Id == Id;
    }

    public override string ToString()
    {
        return Name;
    }
}
