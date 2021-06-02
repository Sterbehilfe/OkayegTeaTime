﻿using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class RandCommand
    {
        public const CommandType Type = CommandType.Rand;

        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s\w+\s#?\w+")))
            {
                twitchBot.SendRandomMessage(chatMessage, chatMessage.GetLowerSplit()[1], chatMessage.GetLowerSplit()[2].ReplaceHashtag());
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s\w+")))
            {
                twitchBot.SendRandomMessage(chatMessage, chatMessage.GetLowerSplit()[1]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias)))
            {
                twitchBot.SendRandomMessage(chatMessage);
            }
        }
    }
}