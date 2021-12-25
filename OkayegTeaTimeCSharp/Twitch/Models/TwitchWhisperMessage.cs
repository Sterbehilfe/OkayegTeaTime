﻿using System.Drawing;
using HLE.Strings;
using OkayegTeaTimeCSharp.Files;
using OkayegTeaTimeCSharp.Files.JsonClasses.Settings;
using OkayegTeaTimeCSharp.Twitch.Messages.Enums;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Models;

public class TwitchWhisperMessage : ITwitchWhisperMessage
{
    public int MessageId { get; }

    public int ThreadId { get; }

    public List<string> Badges { get; }

    public Color Color { get; }

    public string ColorHex { get; }

    public bool IsTurbo { get; }

    public string RawIrcMessage { get; }

    public int UserId { get; }

    public List<UserTag> UserTags { get; }

    public UserType UserType { get; }

    public string DisplayName { get; }

    public string[] LowerSplit { get; }

    public string Message { get; }

    public string[] Split { get; }

    public string Username { get; }

    public TwitchWhisperMessage(WhisperMessage whisperMessage)
    {
        Badges = whisperMessage.Badges.Select(b => b.Key).ToList();
        Color = whisperMessage.Color;
        ColorHex = whisperMessage.ColorHex;
        DisplayName = whisperMessage.DisplayName;
        IsTurbo = whisperMessage.IsTurbo;
        Message = whisperMessage.Message.RemoveChatterinoChar().TrimAll();
        LowerSplit = Message.ToLower().Split();
        MessageId = whisperMessage.MessageId.ToInt();
        RawIrcMessage = whisperMessage.RawIrcMessage;
        Split = Message.Split();
        ThreadId = whisperMessage.ThreadId.ToInt();
        UserId = whisperMessage.UserId.ToInt();
        Username = whisperMessage.Username;
        UserTags = GetUserTags();
        UserType = whisperMessage.UserType;
    }

    private List<UserTag> GetUserTags()
    {
        List<UserTag> result = new() { UserTag.Normal };
        UserLists userLists = JsonController.Settings.UserLists;
        if (userLists.Moderators.Contains(Username))
        {
            result.Add(UserTag.Moderator);
        }
        if (userLists.Owners.Contains(Username))
        {
            result.Add(UserTag.Owner);
        }
        if (userLists.IgnoredUsers.Contains(Username))
        {
            result.Add(UserTag.Special);
        }
        if (userLists.SecretUsers.Contains(Username))
        {
            result.Add(UserTag.Secret);
        }
        return result;
    }
}
