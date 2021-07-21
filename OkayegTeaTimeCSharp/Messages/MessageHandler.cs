﻿using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Discord;
using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages
{
    public static class MessageHandler
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            if (!chatMessage.Username.IsSpecialUser())
            {
                DataBase.InsertNewUser(chatMessage.Username);

                DataBase.LogMessage(chatMessage);

                DataBase.CheckIfAFK(twitchBot, chatMessage);

                DataBase.CheckForReminder(twitchBot, chatMessage);

                CommandHandler.Handle(twitchBot, chatMessage);

                DataBase.CheckForNukes(twitchBot, chatMessage);

                DiscordClient.SendDiscordMessageIfAFK(twitchBot, chatMessage);
            }
        }
    }
}