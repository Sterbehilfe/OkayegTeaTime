using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Handlers;

public class CommandHandler : Handler
{
    private readonly AfkCommandHandler _afkCommandHandler;
    private readonly CooldownController _cooldownController;

    private readonly CommandType[] _commandTypes = Enum.GetValues<CommandType>();
    private readonly AfkType[] _afkTypes = Enum.GetValues<AfkType>();
    private readonly Dictionary<CommandType, CommandHandle> _commandHandles = new();

    public CommandHandler(TwitchBot twitchBot) : base(twitchBot)
    {
        _afkCommandHandler = new(twitchBot);
        _cooldownController = new(twitchBot);
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        bool handled = HandleCommand(chatMessage);
        if (!handled)
        {
            HandleAfkCommand(chatMessage);
        }
    }

    private bool HandleCommand(TwitchChatMessage chatMessage)
    {
        foreach (CommandType type in _commandTypes)
        {
            foreach (string alias in _twitchBot.CommandController[type].Aliases)
            {
                string? prefix = DbControl.Channels[chatMessage.ChannelId]?.Prefix;
                Regex pattern = PatternCreator.Create(alias, prefix);

                if (pattern.IsMatch(chatMessage.Message))
                {
                    if (_cooldownController.IsOnCooldown(chatMessage.UserId, type))
                    {
                        return false;
                    }

                    InvokeCommandHandle(type, _twitchBot, chatMessage, alias);
                    _cooldownController.AddCooldown(chatMessage.UserId, type);
                    _twitchBot.CommandCount++;
                    return true;
                }
            }
        }

        return false;
    }

    private void HandleAfkCommand(TwitchChatMessage chatMessage)
    {
        foreach (AfkType type in _afkTypes)
        {
            foreach (string alias in _twitchBot.CommandController[type].Aliases)
            {
                string? prefix = DbControl.Channels[chatMessage.ChannelId]?.Prefix;
                Regex pattern = PatternCreator.Create(alias, prefix);

                if (pattern.IsMatch(chatMessage.Message))
                {
                    if (_cooldownController.IsOnAfkCooldown(chatMessage.UserId))
                    {
                        return;
                    }

                    _afkCommandHandler.Handle(chatMessage, type);
                    _cooldownController.AddAfkCooldown(chatMessage.UserId);
                    _twitchBot.CommandCount++;
                    return;
                }
            }
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
    private void InvokeCommandHandle(CommandType type, TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
    {
        if (!_commandHandles.TryGetValue(type, out CommandHandle? handle))
        {
            string commandClassName = $"{AppSettings.AssemblyName}.Twitch.Commands.{type}Command";
            Type? commandClass = Type.GetType(commandClassName);
            if (commandClass is null)
            {
                throw new InvalidOperationException($"Could not get type of command class {commandClassName}");
            }

            ConstructorInfo? constructor = commandClass.GetConstructor(new[]
            {
                typeof(TwitchBot),
                typeof(TwitchChatMessage),
                typeof(string)
            });
            if (constructor is null)
            {
                throw new InvalidOperationException($"Could not get constructor for class {commandClass.FullName}");
            }

            MethodInfo? handleMethod = commandClass.GetMethod(nameof(Command.Handle));
            if (handleMethod is null)
            {
                throw new InvalidOperationException($"Could not get {nameof(Command.Handle)} method for command class {commandClass.FullName}");
            }

            MethodInfo? sendMethod = commandClass.GetMethod(nameof(Command.SendResponse));
            if (sendMethod is null)
            {
                throw new InvalidOperationException($"Could not get {nameof(Command.SendResponse)} method for command class {commandClass.FullName}");
            }

            handle = new(constructor, handleMethod, sendMethod);
            _commandHandles.Add(type, handle);
        }

        object handlerInstance = handle.Constructor.Invoke(new object[]
        {
            twitchBot,
            chatMessage,
            alias
        });
        handle.HandleMethod.Invoke(handlerInstance, null);
        handle.SendMethod.Invoke(handlerInstance, null);
    }
}
