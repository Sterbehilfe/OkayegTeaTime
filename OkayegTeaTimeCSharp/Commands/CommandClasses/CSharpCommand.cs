﻿using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class CSharpCommand : Command
    {
        public CSharpCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixHelper.GetPrefix(ChatMessage.Channel), @"\s.+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendCompilerResult(ChatMessage));
            }
        }
    }
}
