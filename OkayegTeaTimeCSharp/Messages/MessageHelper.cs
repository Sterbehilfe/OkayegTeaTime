﻿using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages
{
    public static class MessageHelper
    {
        public static string MakeUsable(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces();
        }

        public static byte[] MakeInsertable(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces().EscapeChars().ToByteArray();
        }

        public static string[] SplitToLowerCase(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces().ToLower().Split(" ");
        }

        public static string[] Split(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces().Split(" ");
        }

        public static bool IsSpecialUser(string username)
        {
            return JsonHelper.BotData.UserLists.SpecialUsers.Contains(username);
        }

        public static bool IsModOrBroadcaster(this ChatMessage chatMessage)
        {
            return chatMessage.IsModerator || chatMessage.IsBroadcaster;
        }

        public static bool IsNotLoggedChannel(string channel)
        {
            return Config.GetNotLoggedChannels().Contains(channel);
        }

        public static bool IsAnyCommand(string message)
        {
            bool result = false;
            CommandHelper.GetAllAliases().ForEach(alias =>
            {
                if (message.IsMatch(@"^\S{1,10}" + alias + @"(\s|$)") || message.IsMatch(@"^" + alias + @"eg(\s|$)"))
                {
                    result = true;
                }
            });
            return result;
        }

        public static bool IsCommand(string message)
        {
            bool result = false;
            CommandHelper.GetCommandAliases().ForEach(alias =>
            {
                if (message.IsMatch(@"^\S{1,10}" + alias + @"(\s|$)") || message.IsMatch(@"^" + alias + @"eg(\s|$)"))
                {
                    result = true;
                }
            });
            return result;
        }

        public static bool IsAfkCommand(string message)
        {
            bool result = false;
            CommandHelper.GetAfkCommandAliases().ForEach(alias =>
            {
                if (message.IsMatch(@"^\S{1,10}" + alias + @"(\s|$)") || message.IsMatch(@"^" + alias + @"eg(\s|$)"))
                {
                    result = true;
                }
            });
            return result;
        }
    }
}
