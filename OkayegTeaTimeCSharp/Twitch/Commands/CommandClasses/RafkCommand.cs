﻿using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses
{
    public class RafkCommand : Command
    {
        public RafkCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendResumingAfkStatus(ChatMessage));
        }
    }
}