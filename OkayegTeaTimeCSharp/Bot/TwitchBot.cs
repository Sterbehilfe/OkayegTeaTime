﻿using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.Time;
using System;
using System.Collections.Generic;
using System.Timers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace OkayegTeaTimeCSharp.Bot
{
    public class TwitchBot
    {
        public TwitchClient TwitchClient { get; private set; }

        public ConnectionCredentials ConnectionCredentials { get; private set; }

        public ClientOptions ClientOptions { get; private set; }

        public WebSocketClient WebSocketClient { get; private set; } = new();

        public List<string> Channels { get; private set; } = new()
        {
            "okayegteatime",
            "strbhlfe",
            "xxdirkthecrafterxx",
            "derpalt",
            "moondye7",
            "ronic76",
            "odin_eu",
            "winnie_po",
            "benastro",
            "jonas5477",
            "enno_of",
            "jann_amh_",
            "timix2g",
            "jonasenbluten",
            "w201diesel"
        };

        public List<Timer> ListTimer { get; private set; } = new();

        public const string Username = "okayegteatime";

        private const string token = "oauth:h9kaxuxtjj9r58vcmz1kaerf1zp6kd";

        public string Runtime => TimeHelper.ConvertMillisecondsToPassedTime(_runtime);

        private readonly long _runtime = 0;

        public TwitchBot()
        {
            ConnectionCredentials = new(Username, token);
            ClientOptions = new()
            {
                MessagesAllowedInPeriod = 10000,
                SendDelay = 250,
                ReconnectionPolicy = new(3000)
            };
            TwitchClient = new(WebSocketClient);
            TwitchClient.Initialize(ConnectionCredentials, Channels);

            TwitchClient.OnLog += Client_OnLog;
            TwitchClient.OnConnected += Client_OnConnected;
            TwitchClient.OnJoinedChannel += Client_OnJoinedChannel;
            TwitchClient.OnMessageReceived += Client_OnMessageReceived;
            TwitchClient.OnWhisperReceived += Client_OnWhisperReceived;

            TwitchClient.Connect();

            _runtime = TimeHelper.Now();
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //Console.WriteLine("LOG: " + e.Data);
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine("CONNECTED");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("JOINED CHANNEL>" + e.Channel);
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Console.WriteLine("#" + e.ChatMessage.Channel + "> " + e.ChatMessage.Username + ": " + e.ChatMessage.Message);
            MessageHandler.Handle(e.ChatMessage);
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            Console.WriteLine("WHISPER>" + e.WhisperMessage.Username + ": " + e.WhisperMessage.Message);
        }
    }
}
