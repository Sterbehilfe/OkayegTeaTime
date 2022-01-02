﻿#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests;

public class BttvRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("bots")]
    public List<string> Bots { get; set; }

    [JsonPropertyName("channelEmotes")]
    public List<BttvChannelEmote> ChannelEmotes { get; set; }

    [JsonPropertyName("sharedEmotes")]
    public List<BttvSharedEmote> SharedEmotes { get; set; }
}
