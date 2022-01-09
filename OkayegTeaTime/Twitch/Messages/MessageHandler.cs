﻿using System.Text.RegularExpressions;
using HLE.Emojis;
using HLE.Strings;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Handlers;
using OkayegTeaTime.Twitch.Models;
using TwitchLib = TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Messages;

public class MessageHandler : Handler
{
    public CommandHandler CommandHandler { get; }

    private const int _pajaAlertUserId = 82008718;
    private const int _pajaChannelId = 11148817;
    private static readonly Regex _pajaAlertPattern = new($@"^\s*pajaS\s+{Emoji.RotatingLight}\s+ALERT\s*$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private const string _pajaAlertChannel = "pajlada";
    private const string _pajaAlertEmote = "pajaStare";
    private const string _pajaAlertMessage = $"/me {_pajaAlertEmote} {Emoji.RotatingLight} OBACHT";

    private static readonly Regex _forgottenPrefixPattern = new($@"^@?{AppSettings.Twitch.Username},?\s(pre|suf)fix", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public MessageHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
        CommandHandler = new(twitchBot);
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        if (!chatMessage.IsIgnoredUser)
        {
            DbController.AddUser(chatMessage.Username);

            DbController.CheckIfAfk(TwitchBot, chatMessage);

            DbController.CheckForReminder(TwitchBot, chatMessage);

            CommandHandler.Handle(chatMessage);

            HandleSpecificMessages(chatMessage);
        }
    }

    private void HandleSpecificMessages(TwitchChatMessage chatMessage)
    {
        CheckForSpotifyUri(chatMessage);
        CheckForForgottenPrefix(chatMessage);
    }

    public void CheckForPajaAlert(TwitchLib::ChatMessage chatMessage)
    {
        if (chatMessage.RoomId.ToInt() == _pajaChannelId && chatMessage.UserId.ToInt() == _pajaAlertUserId && _pajaAlertPattern.IsMatch(chatMessage.Message))
        {
            TwitchBot.TwitchClient.SendMessage(_pajaAlertChannel, _pajaAlertMessage);
            TwitchBot.TwitchClient.SendMessage(AppSettings.SecretOfflineChatChannel, $"{AppSettings.DefaultEmote} {Emoji.RotatingLight}");
        }
    }

    private void CheckForSpotifyUri(TwitchChatMessage chatMessage)
    {
        if (chatMessage.Channel.Name == AppSettings.SecretOfflineChatChannel)
        {
            string? uri = BotActions.SendDetectedSpotifyUri(chatMessage);
            if (!string.IsNullOrEmpty(uri))
            {
                TwitchBot.Send(AppSettings.SecretOfflineChatChannel, uri);
            }
        }
    }

    private void CheckForForgottenPrefix(TwitchChatMessage chatMessage)
    {
        if (_forgottenPrefixPattern.IsMatch(chatMessage.Message))
        {
            if (string.IsNullOrEmpty(chatMessage.Channel.Prefix))
            {
                TwitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, Suffix: {AppSettings.Suffix}");
            }
            else
            {
                TwitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, Prefix: {chatMessage.Channel.Prefix}");
            }
        }
    }
}
