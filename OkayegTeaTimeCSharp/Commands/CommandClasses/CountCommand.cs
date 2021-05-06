﻿using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class CountCommand
    {
        public const CommandType Type = CommandType.Count;

        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\se(mote)?\s\S+")))
            {
                twitchBot.SendLoggedEmoteCount(chatMessage, chatMessage.GetSplit()[2]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s#\w+")))
            {
                twitchBot.SendLoggedMessagesChannelCount(chatMessage, chatMessage.GetLowerSplit()[1]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s\w+")))
            {
                twitchBot.SendLoggedMessagesUserCount(chatMessage, chatMessage.GetLowerSplit()[1]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s-users")))
            {
                twitchBot.SendLoggedDistinctUsersCount(chatMessage);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"$")))
            {
                twitchBot.SendLoggedMessagesCount(chatMessage);
            }
        }
    }
}