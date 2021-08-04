﻿using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class Chatterino7Command
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            twitchBot.Send(chatMessage.Channel, $"Website: 7tv.app || Releases: github.com/SevenTV/chatterino7/releases");
        }
    }
}
