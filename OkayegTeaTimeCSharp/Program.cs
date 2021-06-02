﻿using OkayegTeaTimeCSharp.GitHub;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using System;

namespace OkayegTeaTimeCSharp
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "OkayegTeaTime";
            Console.WriteLine("args?");
            args = Console.ReadLine().Split(" ");

            JsonHelper.SetData();
            ReadMeGenerator.GenerateReadMe();
            PrefixHelper.FillDictionary();
            BotActions.FillLastMessagesDictionary();

            TwitchBot OkayegTeaTime = new(args);
            OkayegTeaTime.SetBot();

            //TwitchAPI twitchAPI = new();

            Console.ReadLine();
        }
    }
}

#warning needs a list of channels in which the bot is not allowed to send massages
#warning cmd type const missing in some cmd classes
#warning change prefixstring column to varbinary to support emojis
#warning fill prefix dictionary after set or unset
#warning poopeg, pisseg
#warning code needs doc
#warning move databasehelper methods
#warning afkmessage: add check message
#warning join channel cmd
#warning nukes wont be deleted
#warning "randeg strbhlfe" always sends my first message
#warning remove "(no message)" from afk messages