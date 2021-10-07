﻿using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests
{
    public class FfzMainSet
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("_type")]
        public int Type { get; set; }

        [JsonPropertyName("icon")]
        public object Icon { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("css")]
        public object Css { get; set; }

        [JsonPropertyName("emoticons")]
        public List<FfzEmote> Emotes { get; set; }
    }
}
