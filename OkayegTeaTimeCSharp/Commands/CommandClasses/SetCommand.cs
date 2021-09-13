﻿using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class SetCommand : Command
    {
        public SetCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\sprefix\s\S+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetPrefix(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\semote\s\S+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetEmoteInFront(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetSongRequestState(ChatMessage));
            }
        }
    }
}
