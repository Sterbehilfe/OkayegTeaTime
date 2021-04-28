using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class Data
    {
        [JsonProperty("commandLists")]
        public CommandLists CommandLists { get; set; }

        [JsonProperty("userLists")]
        public UserLists UserLists { get; set; }

        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }

        [JsonProperty("discord")]
        public Discord Discord { get; set; }
    }
}
