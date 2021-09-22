﻿using System.Collections.Generic;
using OkayegTeaTimeCSharp.Database;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class EmoteDictionary
    {
        private static Dictionary<string, string> _emotes = new();

        public static void Add(string channel)
        {
            if (!_emotes.ContainsKey(channel))
            {
                _emotes.Add(channel, null);
            }
        }

        public static void FillDictionary()
        {
            _emotes = DataBase.GetEmotesInFront();
        }

        public static string Get(string channel)
        {
            if (_emotes.TryGetValue(channel, out string emote) && !string.IsNullOrEmpty(emote))
            {
                return emote;
            }
            else
            {
                return TwitchConfig.EmoteInFront;
            }
        }

        public static void Update(string channel)
        {
            _emotes[channel] = DataBase.GetEmoteInFront(channel);
        }
    }
}
