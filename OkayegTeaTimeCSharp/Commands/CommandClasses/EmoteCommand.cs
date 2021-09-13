﻿using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class EmoteCommand : Command
    {
        public EmoteCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\sffz(\s((\d+)|(\w+(\s\d+)?)))?")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendFFZEmotes(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\sbttv(\s((\d+)|(\w+(\s\d+)?)))?")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendBTTVEmotes(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s7tv(\s((\d+)|(\w+(\s\d+)?)))?")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.Send7TVEmotes(ChatMessage));
            }
        }
    }
}
