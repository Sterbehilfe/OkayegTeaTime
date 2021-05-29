﻿using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Whisper;
using System;
using System.Collections.Generic;
using System.Threading;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Time = System.Timers;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class TwitchBot
    {
        public TwitchClient TwitchClient { get; private set; }

        public ConnectionCredentials ConnectionCredentials { get; private set; }

        public ClientOptions ClientOptions { get; private set; }

        public WebSocketClient WebSocketClient { get; private set; } = new();

        public static List<Time::Timer> ListTimer { get; private set; } = new();

        public string Runtime => TimeHelper.ConvertMillisecondsToPassedTime(_runtime);

        private readonly long _runtime;

        public static readonly Dictionary<string, string> LastMessages = new();

        public static readonly List<Cooldown> ListCooldowns = new();

        public static readonly List<AfkCooldown> ListAfkCooldowns = new();

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

            #region Testing
            //TwitchClient.Initialize(ConnectionCredentials, "lbnshlfe");
            //LastMessages.Add("#lbnshlfe", "");
            #endregion Testing

            TwitchClient.OnLog += Client_OnLog;
            TwitchClient.OnConnected += Client_OnConnected;
            TwitchClient.OnJoinedChannel += Client_OnJoinedChannel;
            TwitchClient.OnMessageReceived += Client_OnMessageReceived;
            TwitchClient.OnWhisperReceived += Client_OnWhisperReceived;

            TwitchClient.Connect();

            _runtime = TimeHelper.Now();
            InitializeTimers();
        }

        public void SetBot()
        {
            _okayegTeaTime ??= this;
        }

        public void Send(string channel, string message)
        {
            message = LastMessages[$"#{channel.Replace("#", "")}"] == message ? $"{message} {Resources.ChatterinoChar}" : message;
            TwitchClient.SendMessage(channel.Replace("#", ""), $"Okayeg {message}");
            LastMessages[$"#{channel}"] = message;
        }

        public void JoinChannel(string channel)
        {
            DataBase.AddChannel(channel);
            try
            {
                TwitchClient.JoinChannel(channel.Replace("#", ""));
                Send(channel, "/ I'm online");
            }
            catch (Exception)
            {
                Send(Resources.Username, $"{Resources.Owner}, unable to join #{channel.Replace("#", "")}");
            }
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
            Thread thread = new(OnMessage);
            thread.Start(e.ChatMessage);

            Console.WriteLine($"#{e.ChatMessage.Channel}>{e.ChatMessage.Username}: {e.ChatMessage.GetMessage()}");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            WhisperHandler.Handle(_okayegTeaTime, e.WhisperMessage);

            Console.WriteLine($"WHISPER>{e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
        }

        #endregion Bot_On

        #region Threading

        private static void OnMessage(object chatMessage)
        {
            if (!MessageHelper.IsSpecialUser(((ChatMessage)chatMessage).Username))
            {
                MessageHandler.Handle(_okayegTeaTime, ((ChatMessage)chatMessage));
            }
        }

        #endregion Threading

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

        private static void OnTimer1000(object sender, Time::ElapsedEventArgs e)
        {
            TimerFunctions.CheckForTimedReminders(_okayegTeaTime);
        }

        private static void OnTimer30000(object sender, Time::ElapsedEventArgs e)
        {
            TimerFunctions.BanSecretChatUsers(_okayegTeaTime);
        }

        #endregion Timer
    }
}