﻿using System.Text.RegularExpressions;
using HLE.Emojis;
using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Handlers;
using OkayegTeaTimeCSharp.Messages.Enums;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Messages;

public class MessageHandler : Handler
{
    public CommandHandler CommandHandler { get; }

    private const string _pajaAlertUsername = "pajbot";
    private static readonly Regex _pajaAlertPattern = new($@"^pajaS\s+{Emoji.RotatingLight}\s+ALERT$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private const string _pajaAlertChannel = "pajlada";
    private const string _pajaAlertEmote = "pajaStare";
    private const string _pajaAlertMessage = $"/me {_pajaAlertEmote} {Emoji.RotatingLight} OBACHT";

    public MessageHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
        CommandHandler = new(twitchBot);
    }

    public override void Handle(ITwitchChatMessage chatMessage)
    {
        if (!chatMessage.UserTags.Contains(UserTag.Special))
        {
            DatabaseController.AddUser(chatMessage.Username);

            DatabaseController.AddMessage(chatMessage);

            DatabaseController.CheckIfAFK(TwitchBot, chatMessage);

            DatabaseController.CheckForReminder(TwitchBot, chatMessage);

            CommandHandler.Handle(chatMessage);

            DatabaseController.CheckForNukes(TwitchBot, chatMessage);

            HandleSpecificMessages(chatMessage);
        }
    }

    private void HandleSpecificMessages(ITwitchChatMessage chatMessage)
    {
        CheckForSpotifyUri(chatMessage);
    }

    public void CheckForPajaAlert(ChatMessage chatMessage)
    {
        if (chatMessage.Username == _pajaAlertUsername && _pajaAlertPattern.IsMatch(chatMessage.Message))
        {
            TwitchBot.TwitchClient.SendMessage(_pajaAlertChannel, _pajaAlertMessage);
        }
    }

    private void CheckForSpotifyUri(ITwitchChatMessage chatMessage)
    {
        if (chatMessage.Channel.Name == Settings.SecretOfflineChat)
        {
            string uri = BotActions.SendDetectedSpotifyURI(chatMessage);
            if (!string.IsNullOrEmpty(uri))
            {
                TwitchBot.Send(Settings.SecretOfflineChat, uri);
            }
        }
    }
}
