﻿using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class NukeCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s\S+\s" + Pattern.TimeSplitPattern + @"\s" + Pattern.TimeSplitPattern + @"(\s|$)")))
            {
                twitchBot.SendCreatedNuke(chatMessage, chatMessage.GetLowerSplit()[1], TimeHelper.ConvertStringToSeconds(new() { chatMessage.GetLowerSplit()[2] }), TimeHelper.ConvertStringToMilliseconds(new() { chatMessage.GetLowerSplit()[3] }));
            }
        }
    }
}