﻿using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Twitch.Bot;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Database
{
    public static class DataBase
    {
        public static void LogMessage(ChatMessage chatMessage)
        {
            if (!JsonHelper.JsonToObject().UserLists.SpecialUsers.Contains(chatMessage.Username))
            {
                OkayegTeaTimeContext database = new();
                database.Messages.Add(new Message(chatMessage.Username, chatMessage.Message.MakeInsertable(), chatMessage.Channel));
                database.SaveChanges();
            }
        }

        public static void CheckIfAFK(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            User user = database.Users.Where(user => user.Username == chatMessage.Username).FirstOrDefault();
            if (user.IsAfk == "true")
            {
                twitchBot.SendComingBack(user);
                if (!MessageHelper.IsAfkCommand(chatMessage.Message))
                {
                    database.SetAfk(user, "false");
                }
            }
        }

        public static void CheckForReminder(TwitchBot twitchBot, string username)
        {
            OkayegTeaTimeContext database = new();
            if (database.Reminders.Any(reminder => reminder.ToTime == 0 && reminder.ToUser == username))
            {
                List<Reminder> listReminder = database.Reminders.Where(reminder => reminder.ToTime == 0 && reminder.ToUser == username).ToList();
                twitchBot.SendReminder(username, listReminder);
            }
        }

        public static void CheckForTimedReminder(TwitchBot twitchBot)
        {
            OkayegTeaTimeContext database = new();
            if (database.Reminders.Any(reminder => reminder.ToTime != 0))
            {
                List<Reminder> listReminder = database.Reminders.Where(reminder => reminder.ToTime != 0).ToList();
                listReminder.ForEach(reminder =>
                {
                    if (reminder.ToTime <= TimeHelper.Now())
                    {
                        twitchBot.SendTimedReminder(reminder);
                        database.Reminders.Remove(reminder);
                        database.SaveChanges();
                    }
                });
            }
        }

        public static void InsertNewUser(string username)
        {
            OkayegTeaTimeContext database = new();
            if (!database.Users.Any(user => user.Username == username))
            {
                database.AddUser(username);
            }
        }

        public static Dictionary<string, string> GetPrefixes()
        {
            OkayegTeaTimeContext database = new();
            return database.Prefixes.ToDictionary(prefix => prefix.Channel, prefix => prefix.PrefixString);
        }

        public static string GetPrefix(string channel)
        {
            OkayegTeaTimeContext database = new();
            return database.Prefixes.Where(prefix => prefix.Channel == $"#{channel}").FirstOrDefault().PrefixString;
        }
    }
}
