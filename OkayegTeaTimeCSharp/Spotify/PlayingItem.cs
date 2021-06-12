﻿namespace OkayegTeaTimeCSharp.Spotify
{
    public abstract class PlayingItem
    {
        public string Title { get; protected set; }

        public string Artist { get; protected set; }

        public string URI { get; protected set; }

        public string Message { get; protected set; }
    }
}