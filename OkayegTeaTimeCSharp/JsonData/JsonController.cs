﻿using System.IO;
using System.Text.Json;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.Data;
using Path = OkayegTeaTimeCSharp.Properties.Path;

namespace OkayegTeaTimeCSharp.JsonData
{
    public class JsonController
    {
        public BotData BotData => _botData;

        public CommandLists CommandLists => _commandLists;

        private static BotData _botData;
        private static CommandLists _commandLists;

        public void LoadData()
        {
            _botData = JsonSerializer.Deserialize<BotData>(File.ReadAllText(Path.DataJson));
            _commandLists = JsonSerializer.Deserialize<CommandLists>(File.ReadAllText(Path.CommandsJson));
        }
    }
}
