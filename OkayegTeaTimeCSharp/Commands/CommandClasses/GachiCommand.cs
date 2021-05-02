﻿using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Commands.CommandEnums;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class GachiCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias, CommandType type)
        {
            twitchBot.SendRandomGachi(chatMessage);
        }
    }
}
