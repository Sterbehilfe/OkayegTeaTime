﻿using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.Ffz;

public class FfzBotBadges
{
    [JsonPropertyName("2")]
    public List<string> Users { get; set; }
}
