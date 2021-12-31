﻿using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Handlers;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands;

public class CommandHandler : Handler
{
    private const string _handleName = "Handle";

    public CommandHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
    }

    public override void Handle(ITwitchChatMessage chatMessage)
    {
        if (chatMessage.IsCommand)
        {
            HandleCommand(chatMessage);
            TwitchBot.CommandCount++;
        }
        else if (chatMessage.IsAfkCommmand)
        {
            HandleAfkCommand(chatMessage);
            TwitchBot.CommandCount++;
        }
    }

    private void HandleCommand(ITwitchChatMessage chatMessage)
    {
        foreach (var type in (CommandType[])Enum.GetValues(typeof(CommandType)))
        {
            if (!AppSettings.CommandList.MatchesAnyAlias(chatMessage, type))
                continue;
            if (BotActions.IsOnCooldown(chatMessage.UserId, type))
                continue;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var alias in AppSettings.CommandList[type].Alias)
            {
                var pattern = PatternCreator.Create(alias, chatMessage.Channel.Prefix, @"(\s|$)");

                // ReSharper disable once InvertIf
                if (pattern.IsMatch(chatMessage.Message))
                {
                    BotActions.AddUserToCooldownDictionary(chatMessage.UserId, type);
                    InvokeCommandHandle(type, TwitchBot, chatMessage, alias);
                    BotActions.AddCooldown(chatMessage.UserId, type);
                    break;
                }
            }

            // Handled, we can jump out now
            break;
        }
    }

    private void HandleAfkCommand(ITwitchChatMessage chatMessage)
    {
        foreach (var type in (AfkCommandType[])Enum.GetValues(typeof(AfkCommandType)))
        {
            if (!AppSettings.CommandList.MatchesAnyAlias(chatMessage, type))
                continue;
            if (BotActions.IsOnAfkCooldown(chatMessage.UserId))
                continue;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var alias in AppSettings.CommandList[type].Alias)
            {
                var pattern = PatternCreator.Create(alias, chatMessage.Channel.Prefix, @"(\s|$)");

                // ReSharper disable once InvertIf
                if (pattern.IsMatch(chatMessage.Message))
                {
                    BotActions.AddUserToAfkCooldownDictionary(chatMessage.UserId);
                    AfkCommandHandler.Handle(TwitchBot, chatMessage, type);
                    BotActions.AddAfkCooldown(chatMessage.UserId);
                    break;
                }
            }

            break;
        }
    }

    /// <summary>
    /// Attempts to handle a command through a handler via reflection
    /// </summary>
    /// <param name="type">The command handler class type</param>
    /// <param name="twitchBot">The currently running bot that received this command</param>
    /// <param name="chatMessage">The chat message to handle</param>
    /// <param name="alias">A command alias</param>
    /// <exception cref="InvalidOperationException">The command handler doesn't conform</exception>
    private static void InvokeCommandHandle(CommandType type, TwitchBot twitchBot, ITwitchChatMessage chatMessage,
        string alias)
    {
        var commandClassName = AppSettings.CommandList.GetCommandClassName(type);

        var commandClass = Type.GetType(commandClassName);
        if (commandClass is null)
            throw new InvalidOperationException($"Could not get type of command class {commandClassName}");

        var constructor = commandClass.GetConstructor(new[] { typeof(TwitchBot), typeof(ITwitchChatMessage), typeof(string) });
        if (constructor is null)
            throw new InvalidOperationException($"Could not instantiate command class {commandClassName}");

        var handlerInstance = constructor.Invoke(new object[] { twitchBot, chatMessage, alias });

        var handleMethod = commandClass.GetMethod(_handleName);
        if (handleMethod is null)
            throw new InvalidOperationException($"Could not get handler method for command class {commandClassName}");

        handleMethod.Invoke(handlerInstance, null);
    }
}
