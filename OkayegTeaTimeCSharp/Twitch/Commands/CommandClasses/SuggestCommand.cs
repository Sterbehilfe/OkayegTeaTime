﻿using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses
{
    public class SuggestCommand : Command
    {
        public SuggestCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s\S{3,}")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSuggestionNoted(ChatMessage));
            }
        }
    }
}
