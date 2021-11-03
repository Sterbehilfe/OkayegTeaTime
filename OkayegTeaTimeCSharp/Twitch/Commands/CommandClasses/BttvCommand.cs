﻿using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class BttvCommand : Command
{
    public BttvCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+\s\d+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendBttvEmotes(ChatMessage, ChatMessage.Split[1], ChatMessage.Split[2].ToInt()));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendBttvEmotes(ChatMessage, ChatMessage.Split[1]));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\d+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendBttvEmotes(ChatMessage, count: ChatMessage.Split[1].ToInt()));
        }
        else
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendBttvEmotes(ChatMessage));
        }
    }
}
