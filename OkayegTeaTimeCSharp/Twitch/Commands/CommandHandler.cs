﻿using System.Reflection;
using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTimeCSharp.Twitch.Commands.Enums;
using OkayegTeaTimeCSharp.Twitch.Handlers;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands;

public class CommandHandler : Handler
{
    private const string _handleName = "Handle";

    public CommandHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
    }

    public override void Handle(ITwitchChatMessage chatMessage)
    {
        if (chatMessage.IsCommand())
        {
            foreach (CommandType type in (CommandType[])Enum.GetValues(typeof(CommandType)))
            {
                if (chatMessage.MatchesAnyAlias(type))
                {
                    if (!BotActions.IsOnCooldown(chatMessage.Username, type))
                    {
                        foreach (string alias in CommandHelper.GetCommand(type).Alias)
                        {
                            if (chatMessage.Message.IsMatch(PatternCreator.Create(alias, chatMessage.Channel.Prefix, @"(\s|$)")))
                            {
                                BotActions.AddUserToCooldownDictionary(chatMessage.Username, type);
                                InvokeCommandHandle(type, TwitchBot, chatMessage, alias);
                                BotActions.AddCooldown(chatMessage.Username, type);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            TwitchBot.CommandCount++;
        }
        else if (chatMessage.IsAfkCommand())
        {
            foreach (AfkCommandType type in (AfkCommandType[])Enum.GetValues(typeof(AfkCommandType)))
            {
                if (chatMessage.MatchesAnyAlias(type))
                {
                    if (!BotActions.IsOnAfkCooldown(chatMessage.Username))
                    {
                        foreach (string alias in CommandHelper.GetAfkCommand(type).Alias)
                        {
                            if (chatMessage.Message.IsMatch(PatternCreator.Create(alias, chatMessage.Channel.Prefix, @"(\s|$)")))
                            {
                                BotActions.AddUserToAfkCooldownDictionary(chatMessage.Username);
                                AfkCommandHandler.Handle(TwitchBot, chatMessage, type);
                                BotActions.AddAfkCooldown(chatMessage.Username);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            TwitchBot.CommandCount++;
        }
    }

    private void InvokeCommandHandle(CommandType type, TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
    {
        Type commandClass = Type.GetType(CommandHelper.GetCommandClassName(type));
        ConstructorInfo constructor = commandClass.GetConstructor(new Type[] { typeof(TwitchBot), typeof(ITwitchChatMessage), typeof(string) });
        commandClass.GetMethod(_handleName).Invoke(constructor.Invoke(new object[] { twitchBot, chatMessage, alias }), null);
    }
}
