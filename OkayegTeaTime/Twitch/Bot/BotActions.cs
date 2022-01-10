﻿using System.Text;
using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Bot.Cooldowns;
using OkayegTeaTime.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Bot;

public static class BotActions
{
    private const string _channelEmotesError = "the channel doesn't have the specified amount of " +
        "emotes enabled or an error occurred";
    private const byte _defaultEmoteCount = 5;
    private const string _noModOrBroadcasterMessage = "you aren't a mod or the broadcaster";
    private const string _twitchUserDoesntExistMessage = "Twitch user doesn't exist";
    private const string _tooManyRemindersMessage = "that person has too many reminders set for them";
    private const string _userNotFoundMessage = "could not find any matching user";
    private const string _reminderNotFoundMessage = "could not find any matching reminder";

    public static void AddAfkCooldown(int userId)
    {
        AfkCooldown? cooldown = TwitchBot.AfkCooldowns.FirstOrDefault(c => c.UserId == userId);
        if (cooldown is not null)
        {
            TwitchBot.AfkCooldowns.Remove(cooldown);
        }
        AddUserToAfkCooldownDictionary(userId);
    }

    public static void AddCooldown(int userId, CommandType type)
    {
        Cooldown? cooldown = TwitchBot.Cooldowns.FirstOrDefault(c => c.UserId == userId && c.Type == type);
        if (cooldown is not null)
        {
            TwitchBot.Cooldowns.Remove(cooldown);
        }
        AddUserToCooldownDictionary(userId, type);
    }

    public static void AddUserToAfkCooldownDictionary(int userId)
    {
        if (!AppSettings.UserLists.Moderators.Contains(userId))
        {
            if (!TwitchBot.AfkCooldowns.Any(c => c.UserId == userId))
            {
                TwitchBot.AfkCooldowns.Add(new(userId));
            }
        }
    }

    public static void AddUserToCooldownDictionary(int userId, CommandType type)
    {
        if (!AppSettings.UserLists.Moderators.Contains(userId))
        {
            if (!TwitchBot.Cooldowns.Any(c => c.UserId == userId && c.Type == type))
            {
                TwitchBot.Cooldowns.Add(new(userId, type));
            }
        }
    }

    public static bool IsOnAfkCooldown(int userId)
    {
        return TwitchBot.AfkCooldowns.Any(c => c.UserId == userId && c.Time > TimeHelper.Now());
    }

    public static bool IsOnCooldown(int userId, CommandType type)
    {
        return TwitchBot.Cooldowns.Any(c => c.UserId == userId && c.Type == type && c.Time > TimeHelper.Now());
    }

    public static void SendComingBack(this TwitchBot twitchBot, User user, TwitchChatMessage chatMessage)
    {
        twitchBot.Send(chatMessage.Channel, new AfkMessage(user).ComingBack);
    }

    public static string? SendDetectedSpotifyUri(ChatMessage chatMessage)
    {
        if (new LinkRecognizer(chatMessage).TryFindSpotifyLink(out string uri))
        {
            return uri;
        }
        else
        {
            return null;
        }
    }

    public static void SendGoingAfk(this TwitchBot twitchBot, TwitchChatMessage chatMessage, AfkCommandType type)
    {
        DbController.SetAfk(chatMessage, type);
        User? user = DbController.GetUser(chatMessage.Username);
        if (user is null)
        {
            DbController.AddUser(chatMessage.Username);
            user = DbController.GetUser(chatMessage.Username);
        }

        twitchBot.Send(chatMessage.Channel, new AfkMessage(user!).GoingAway);
    }

    public static void SendReminder(this TwitchBot twitchBot, TwitchChatMessage chatMessage, List<Reminder> reminders)
    {
        string message = $"{chatMessage.Username}, reminder from {reminders[0].GetAuthor()} ({TimeHelper.ConvertUnixTimeToTimeStamp(reminders[0].Time, "ago")})";
        StringBuilder builder = new(message);
        if (reminders[0].Message.Length > 0)
        {
            builder.Append($": {reminders[0].Message.Decode()}");
        }

        if (reminders.Count > 1)
        {
            reminders.Skip(1).ForEach(r =>
            {
                builder.Append($" || {r.GetAuthor()} ({TimeHelper.ConvertUnixTimeToTimeStamp(r.Time, "ago")})");
                if (r.Message.Length > 0)
                {
                    builder.Append($": {r.Message.Decode()}");
                }
            });
        }
        twitchBot.Send(chatMessage.Channel, builder.ToString());
    }

    public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
    {
        string message = $"{reminder.ToUser}, reminder from {reminder.GetAuthor()} ({TimeHelper.ConvertUnixTimeToTimeStamp(reminder.Time, "ago")})";
        string reminderMessage = reminder.Message.Decode();
        if (!string.IsNullOrEmpty(reminderMessage))
        {
            message += $": {reminderMessage}";
        }
        twitchBot.Send(reminder.Channel, message);
    }
}
