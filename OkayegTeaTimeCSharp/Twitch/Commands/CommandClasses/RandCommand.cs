﻿using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses
{
    public class RandCommand : Command
    {
        public RandCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendRandomMessage(ChatMessage));
        }
    }
}