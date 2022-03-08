﻿using System.Text.RegularExpressions;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SuggestCommand : Command
{
    public SuggestCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S{3,}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string suggestion = ChatMessage.Message[(ChatMessage.LowerSplit[0].Length + 1)..];
            DbController.AddSugestion(ChatMessage.Username, ChatMessage.Channel.Name, suggestion);
            Response = $"{ChatMessage.Username}, your suggestion has been noted";
        }
    }
}
