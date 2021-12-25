﻿using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Files;
using OkayegTeaTimeCSharp.Twitch.Api;

namespace OkayegTeaTimeCSharp.Twitch.Bot;

public static class TimerFunctions
{
    public static void CheckForTimedReminders(TwitchBot twitchBot)
    {
        DatabaseController.CheckForTimedReminder(twitchBot);
    }

    public static void LoadJsonData()
    {
        JsonController.LoadJsonData();
    }

    public static void SetConsoleTitle(TwitchBot twitchBot)
    {
        Console.Title = $"OkayegTeaTime - {twitchBot.SystemInfo}";
    }

    public static void TwitchApiRefreshAccessToken()
    {
        TwitchApi.RefreshAccessToken();
    }
}
