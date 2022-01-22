﻿using System.Text.RegularExpressions;
using HLE.Strings;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class JoinCommand : Command
{
    public JoinCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s#?\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (!ChatMessage.IsBotModerator)
            {
                Response += "you are not a moderator of the bot";
                return;
            }

            string channel = ChatMessage.LowerSplit[1];
            string response = TwitchBot.JoinChannel(channel.Remove("#"));
            Response += response;
            return;
        }
    }
}
