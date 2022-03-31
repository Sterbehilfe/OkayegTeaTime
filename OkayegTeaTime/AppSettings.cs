﻿using System.Reflection;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Jsons.CommandData;
using OkayegTeaTime.Files.Jsons.Settings;

#nullable disable

namespace OkayegTeaTime;

public static class AppSettings
{
    public static string AssemblyName { get; } = Assembly.GetExecutingAssembly().GetName().Name;

    public static string ChatterinoChar => JsonController.Settings.ChatterinoChar;

    public static CommandList CommandList => JsonController.CommandList;

    public static DbConnection DbConnection => JsonController.Settings.DatabaseConnection;

    public static string DebugChannel => JsonController.Settings.DebugChannel;

    public static Files.Jsons.Settings.Discord Discord => JsonController.Settings.Discord;

    public static string RepositoryUrl => JsonController.Settings.RepositoryUrl;

    public static string SecretOfflineChatChannel => JsonController.Settings.SecretOfflineChatChannel;

    public static List<string> SecretOfflineChatEmotes => JsonController.Settings.SecretOfflineChatEmotes;

    public static Files.Jsons.Settings.Spotify Spotify => JsonController.Settings.Spotify;

    public static Files.Jsons.Settings.Twitch Twitch => JsonController.Settings.Twitch;

    public static UserLists UserLists => JsonController.Settings.UserLists;

    public const short AfkCooldown = 10000;
    public const string DefaultEmote = "Okayeg";
    public const byte MaxEmoteInFrontLength = 20;
    public const short MaxMessageLength = 500;
    public const byte MaxPrefixLength = 10;
    public const byte MaxReminders = 10;
    public const short DelayBetweenSentMessages = 1300;
    public const short DelayBetweenReceivedMessages = 500;
    public const string Suffix = "eg";
    public const string SettingsFileName = "Settings.json";
}
