﻿using HLE.Collections;
using HLE.Enums;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.HttpRequests;
using OkayegTeaTimeCSharp.Models;
using OkayegTeaTimeCSharp.Twitch.Bot.EmoteManagementNotifications.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace OkayegTeaTimeCSharp.Twitch.Bot.EmoteManagementNotifications
{
    public class EmoteManagementNotificator
    {
        public TwitchBot TwitchBot { get; }

        private readonly List<NotificatorChannel> _channels = new();
        private readonly Timer _timer = new(new Minute().Milliseconds);

        public EmoteManagementNotificator(TwitchBot twitchBot)
        {
            TwitchBot = twitchBot;
            DataBase.GetEmoteManagementSubs().ForEach(c => _channels.Add(new(c)));
            InitChannels();
            _timer.Elapsed += Timer_OnElapsed;
            _timer.Start();
        }

        private void InitChannels()
        {
            _channels.Where(c => AreEmoteListsNull(c)).ForEach(c =>
            {
                try
                {
                    List<Emote> emotes7TV = HttpRequest.Get7TVEmotes(c.Name);
                    List<Emote> emotesBTTV = HttpRequest.GetBTTVEmotes(c.Name);
                    List<Emote> emotesFFZ = HttpRequest.GetFFZEmotes(c.Name);

                    c.New7TVEmotes = emotes7TV;
                    c.Old7TVEmotes = emotes7TV;

                    c.NewBTTVEmotes = emotesBTTV;
                    c.OldBTTVEmotes = emotesBTTV;

                    c.NewFFZEmotes = emotesFFZ;
                    c.NewFFZEmotes = emotesFFZ;
                }
                catch (Exception)
                {

                }
            });
        }

        private void LoadEmotes()
        {
            _channels.ForEach(c =>
            {
                try
                {
                    c.Old7TVEmotes = c.New7TVEmotes;
                    c.OldBTTVEmotes = c.NewBTTVEmotes;
                    c.OldFFZEmotes = c.OldFFZEmotes;

                    c.New7TVEmotes = HttpRequest.Get7TVEmotes(c.Name);
                    c.NewBTTVEmotes = HttpRequest.GetBTTVEmotes(c.Name);
                    c.NewFFZEmotes = HttpRequest.GetFFZEmotes(c.Name);
                }
                catch (Exception)
                {

                }
            });
        }

        private void DetectChange()
        {
            _channels.ForEach(c =>
            {
                List<Emote> new7TVEmotes = c.New7TVEmotes.Where(e => c.Old7TVEmotes?.Contains(e) == false).ToList();
                NotifyChannel(c.Name, new7TVEmotes, NotificationType.NewEmote);

                List<Emote> newBTTVEmotes = c.NewBTTVEmotes.Where(e => c.OldBTTVEmotes?.Contains(e) == false).ToList();
                NotifyChannel(c.Name, newBTTVEmotes, NotificationType.NewEmote);

                List<Emote> newFFZEmotes = c.NewFFZEmotes.Where(e => c.OldFFZEmotes?.Contains(e) == false).ToList();
                NotifyChannel(c.Name, newFFZEmotes, NotificationType.NewEmote);
            });
        }

        private void NotifyChannel(string channel, List<Emote> emotes, NotificationType type)
        {
            if (type == NotificationType.NewEmote && !emotes.IsNullOrEmpty())
            {
                TwitchBot.Send(channel, $"Newly added emote(s): {emotes.Select(e => e.Name).ToSequence()}");
            }
            else if (type == NotificationType.RemovedEmote)
            {

            }
        }

        private void Timer_OnElapsed(object sender, ElapsedEventArgs e)
        {
            LoadEmotes();
            DetectChange();
        }

        private bool AreEmoteListsNull(NotificatorChannel channel)
        {
            return channel.New7TVEmotes.IsNullOrEmpty() || channel.Old7TVEmotes.IsNullOrEmpty()
                || channel.NewBTTVEmotes.IsNullOrEmpty() || channel.OldBTTVEmotes.IsNullOrEmpty()
                || channel.NewFFZEmotes.IsNullOrEmpty() || channel.OldFFZEmotes.IsNullOrEmpty();
        }

        public void AddChannel(string channel)
        {
            _channels.Add(new(channel));
            InitChannels();
        }

        public void RemoveChannel(string channel)
        {
            _channels.Remove(new(channel));
        }
    }
}
