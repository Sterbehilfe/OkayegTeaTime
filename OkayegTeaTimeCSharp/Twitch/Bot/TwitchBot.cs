﻿using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Whisper;
using System;
using System.Collections.Generic;
using System.Timers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class TwitchBot
    {
        public TwitchClient TwitchClient { get; private set; }

        public ConnectionCredentials ConnectionCredentials { get; private set; }

        public ClientOptions ClientOptions { get; private set; }

        public WebSocketClient WebSocketClient { get; private set; } = new();

        public static List<Timer> ListTimer { get; private set; } = new();

        public string Runtime => TimeHelper.ConvertMillisecondsToPassedTime(_runtime);

        private readonly long _runtime = 0;

        private static TwitchBot _okayegTeaTime;

        public TwitchBot()
        {
            ConnectionCredentials = new(Resources.Username, Resources.OAuthToken);
            ClientOptions = new()
            {
                MessagesAllowedInPeriod = 10000,
                SendDelay = 250,
                ReconnectionPolicy = new(3000)
            };
            TwitchClient = new(WebSocketClient);
            TwitchClient.Initialize(ConnectionCredentials, Config.GetChannels());

            TwitchClient.OnLog += Client_OnLog;
            TwitchClient.OnConnected += Client_OnConnected;
            TwitchClient.OnJoinedChannel += Client_OnJoinedChannel;
            TwitchClient.OnMessageReceived += Client_OnMessageReceived;
            TwitchClient.OnWhisperReceived += Client_OnWhisperReceived;

            TwitchClient.Connect();

            _runtime = TimeHelper.Now();
            InitializeTimers();

            SetBot();
        }

        public void SetBot()
        {
            _okayegTeaTime ??= this;
        }

        #region Bot_On
        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //Console.WriteLine($"LOG: {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine("CONNECTED");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"JOINED CHANNEL>{e.Channel}");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (!MessageHelper.IsSpecialUser(e.ChatMessage.Username))
            {
                MessageHandler.Handle(_okayegTeaTime, e.ChatMessage);
            }

            Console.WriteLine($"#{e.ChatMessage.Channel}>{e.ChatMessage.Username}: {e.ChatMessage.GetMessage()}");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            WhisperHandler.Handle(_okayegTeaTime, e.WhisperMessage);

            Console.WriteLine($"WHISPER>{e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
        }
        #endregion

        #region Timer
        private static void InitializeTimers()
        {
            Timers.CreateTimers();
            AddTimerFunction();
            StartTimers();
        }

        private static void StartTimers()
        {
            ListTimer.ForEach(timer => timer.Enabled = true);
        }

        private static void StopTimers()
        {
            ListTimer.ForEach(timer => timer.Enabled = false);
        }

        private static void AddTimerFunction()
        {
            Timers.GetTimer(1000).Elapsed += OnTimer1000;
            Timers.GetTimer(30000).Elapsed += OnTimer30000;
        }

        private static void OnTimer1000(object sender, ElapsedEventArgs e)
        {
            TimerFunctions.CheckForTimedReminders(_okayegTeaTime);
        }

        private static void OnTimer30000(object sender, ElapsedEventArgs e)
        {
            TimerFunctions.BanSecretChatUsers(_okayegTeaTime);
        }
        #endregion
    }
}
