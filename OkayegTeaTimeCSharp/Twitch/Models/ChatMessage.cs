﻿using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Models;

public class ChatMessage : IChatMessage
{
    public string DisplayName { get; }

    public string[] LowerSplit { get; }

    public string Message { get; }

    public string[] Split { get; }

    public string Username { get; }

    public ChatMessage(TwitchLibMessage twitchLibMessage)
    {
        DisplayName = twitchLibMessage.DisplayName;
        Message = GetMessage(twitchLibMessage).RemoveChatterinoChar().TrimAll();
        LowerSplit = GetLowerSplit();
        Split = GetSplit();
        Username = twitchLibMessage.Username;
    }

    private string GetMessage(TwitchLibMessage twitchLibMessage)
    {
        string message = twitchLibMessage.RawIrcMessage.Match(@"(WHISPER|PRIVMSG)\s#?\w+\s:.+$");
        return message.ReplacePattern(@"^(WHISPER|PRIVMSG)\s#?\w+\s:", "");
    }

    private string[] GetSplit()
    {
        return Message.Split();
    }

    private string[] GetLowerSplit()
    {
        return Message.ToLower().Split();
    }
}
