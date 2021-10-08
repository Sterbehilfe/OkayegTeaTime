﻿using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests
{
    public class SevenTvEmote
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("visibility")]
        public int Visibility { get; set; }

        [JsonPropertyName("width")]
        public SevenTvEmoteSize Width { get; set; }

        [JsonPropertyName("height")]
        public SevenTvEmoteSize Height { get; set; }
    }
}
