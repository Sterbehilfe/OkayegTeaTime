﻿using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.API;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class IdCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s\w+")))
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {TwitchAPI.GetChannelID(chatMessage.GetLowerSplit()[1])}");
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel))))
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {TwitchAPI.GetChannelID(chatMessage.Username)}");
            }
        }
    }
}
