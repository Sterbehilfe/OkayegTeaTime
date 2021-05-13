﻿using OkayegTeaTimeCSharp.Commands.AfkCommandClasses;
using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Utils;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands
{
    public static class CommandHelper
    {
        public const string Suffix = "eg";

        public static bool MatchesAlias(this ChatMessage chatMessage, CommandType type)
        {
            return GetCommand(type).Alias.Any(alias => PrefixHelper.GetPrefix(chatMessage.Channel) + alias == chatMessage.GetLowerSplit()[0] || alias + Suffix == chatMessage.GetLowerSplit()[0]);
        }

        public static string GetCommandClassName(CommandType type)
        {
            return $"OkayegTeaTimeCSharp.Commands.CommandClasses.{type}Command";
        }

        public static Command GetCommand(CommandType type)
        {
            return JsonHelper.BotData.CommandLists.Commands.Where(cmd => cmd.CommandName == type.ToString().ToLower()).FirstOrDefault();
        }

        public static AfkCommand GetAfkCommand(AfkCommandType type)
        {
            return JsonHelper.BotData.CommandLists.AfkCommands.Where(cmd => cmd.CommandName == type.ToString().ToLower()).FirstOrDefault();
        }

        public static AfkCommand GetAfkCommand(string name)
        {
            return JsonHelper.BotData.CommandLists.AfkCommands.Where(cmd => cmd.CommandName == name).FirstOrDefault();
        }

        public static List<string> GetCommandAliases()
        {
            List<string> listAlias = new();
            JsonHelper.BotData.CommandLists.Commands.ForEach(cmd =>
            {
                cmd.Alias.ForEach(alias =>
                {
                    listAlias.Add(alias);
                });
            });
            return listAlias;
        }

        public static List<string> GetAfkCommandAliases()
        {
            List<string> listAlias = new();
            JsonHelper.BotData.CommandLists.AfkCommands.ForEach(cmd =>
            {
                cmd.Alias.ForEach(alias =>
                {
                    listAlias.Add(alias);
                });
            });
            return listAlias;
        }

        public static List<string> GetAllAliases()
        {
            return GetCommandAliases().Concat(GetAfkCommandAliases()).ToList();
        }

        public static AfkMessage ReplaceSpaceHolder(this AfkMessage afkMessage, User user)
        {
            afkMessage.ComingBack = afkMessage.ComingBack.Replace("{username}", user.Username)
                .Replace("{time}", TimeHelper.ConvertMillisecondsToPassedTime(user.Time, " ago"))
                .Replace("{message}", user.MessageText.Decode());

            afkMessage.GoingAway = afkMessage.ComingBack.Replace("{username}", user.Username);

            afkMessage.Resuming = afkMessage.Resuming.Replace("{username}", user.Username);

            return afkMessage;
        }

        public static long GetCoolDown(CommandType type)
        {
            return GetCommand(type).Cooldown;
        }
    }
}
