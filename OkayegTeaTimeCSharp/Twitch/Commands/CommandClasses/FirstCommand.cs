﻿using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class FirstCommand : Command
{
    public FirstCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+\s#?\w+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirstUserChannel(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s#\w+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirstChannel(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirstUser(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix)))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirst(ChatMessage));
        }
    }
}
