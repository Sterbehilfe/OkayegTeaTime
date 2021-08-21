﻿using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses
{
    public class CheckCommand : Command
    {
        public CheckCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixHelper.GetPrefix(ChatMessage.Channel), @"\safk\s\w+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckAfk(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixHelper.GetPrefix(ChatMessage.Channel), @"\sreminder\s\d+(\s|$)")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckReminder(ChatMessage));
            }
        }
    }
}