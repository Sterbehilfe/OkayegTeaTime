﻿using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class UnsetCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\sprefix")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendUnsetPrefix(chatMessage));
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\sreminder\s\d+")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendUnsetReminder(chatMessage));
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\semote")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendUnsetEmoteInFront(chatMessage));
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\snuke\s\d+")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendUnsetNuke(chatMessage));
            }
        }
    }
}