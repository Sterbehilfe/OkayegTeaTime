﻿using HLE.Collections;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files.Jsons.CommandData;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Models;
using User = OkayegTeaTime.Database.Models.User;

namespace OkayegTeaTime.Twitch.Handlers;

public class AfkCommandHandler
{
    private readonly TwitchBot _twitchBot;

    public AfkCommandHandler(TwitchBot twitchBot)
    {
        _twitchBot = twitchBot;
    }

    public void Handle(TwitchChatMessage chatMessage, AfkCommandType type)
    {
        User? user = DbControl.Users.GetUser(chatMessage.UserId, chatMessage.Username);
        if (user is null)
        {
            DbControl.Users.Add(chatMessage.UserId, chatMessage.Username, type);
        }

        user = DbControl.Users[chatMessage.UserId];
        if (user is null)
        {
            return;
        }

        string message = chatMessage.Split[1..].JoinToString(' ');
        user.AfkMessage = message;
        user.AfkType = type;
        user.AfkTime = TimeHelper.Now();
        user.IsAfk = true;

        AfkCommand cmd = _twitchBot.CommandController[type];
        AfkMessage afkMessage = new(chatMessage.UserId, cmd);
        if (afkMessage.GoingAway is null)
        {
            return;
        }

        _twitchBot.Send(chatMessage.Channel, afkMessage.GoingAway);
    }
}
