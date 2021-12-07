﻿using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class SongRequestCommand : Command
{
    public SongRequestCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSongAddedToQueue(ChatMessage));
        }
    }
}
