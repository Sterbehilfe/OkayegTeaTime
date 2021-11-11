﻿using HLE.Time;
using OkayegTeaTimeCSharp.Twitch.Commands.Enums;

namespace OkayegTeaTimeCSharp.Twitch.Commands;

public class Cooldown
{
    public string Username { get; }

    public CommandType Type { get; }

    public long Time { get; private set; }

    public Cooldown(string username, CommandType type)
    {
        Username = username;
        Type = type;
        Time = TimeHelper.Now() + CommandList[type].Cooldown;
    }
}
