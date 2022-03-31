﻿using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.Bttv;

public class BttvChannelEmote
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("code")]
    public string Name { get; set; }

    [JsonPropertyName("imageType")]
    public string ImageType { get; set; }

    [JsonPropertyName("userId")]
    public string UserId { get; set; }

#nullable enable
    public override bool Equals(object? obj)
    {
        return obj is BttvChannelEmote emote && emote.Name == Name;
    }

    public override string ToString()
    {
        return Name;
    }
}
