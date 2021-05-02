﻿using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Commands.CommandEnums;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class GachiCommand
    {
        public const CommandType Type = CommandType.Gachi;

        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            twitchBot.SendRandomGachi(chatMessage);
        }
    }
}
