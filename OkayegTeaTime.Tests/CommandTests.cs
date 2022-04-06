﻿using System;
using System.Linq;
using HLE.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Files.Jsons.CommandData;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Controller;
using JCommand = OkayegTeaTime.Files.Jsons.CommandData.Command;

namespace OkayegTeaTime.Tests;

[TestClass]
public class CommandTests
{
    private readonly CommandType[] _commandTypes = Enum.GetValues<CommandType>();
    private readonly AfkCommandType[] _afkTypes = Enum.GetValues<AfkCommandType>();
    private readonly CommandController _commandController = new();

    [TestMethod]
    public void CommandCompletenessTestFromEnum()
    {
        foreach (CommandType type in _commandTypes)
        {
            JCommand command = _commandController[type];
            Assert.IsNotNull(command);
        }
    }

    [TestMethod]
    public void CommandCompletenessTestFromJson()
    {
        _commandController.Commands.ForEach(cmd =>
        {
            CommandType type = _commandTypes.SingleOrDefault(c => string.Equals(c.ToString(), cmd.Name, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(type);
        });
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromEnum()
    {
        foreach (AfkCommandType type in _afkTypes)
        {
            AfkCommand command = _commandController[type];
            Assert.IsNotNull(command);
        }
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromJson()
    {
        _commandController.AfkCommands.ForEach(cmd =>
        {
            AfkCommandType type = _afkTypes.SingleOrDefault(c => string.Equals(c.ToString(), cmd.Name, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(type);
        });
    }
}
