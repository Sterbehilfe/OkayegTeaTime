﻿using OkayegTeaTime.Files.JsonClasses.HttpRequests.Bttv;
using OkayegTeaTime.Files.JsonClasses.HttpRequests.Ffz;
using OkayegTeaTime.Files.JsonClasses.HttpRequests.SevenTv;

namespace OkayegTeaTime.Twitch.Bot.EmoteNotifications;

public class ThirdPartyNotificatorChannel
{
    public string Name { get; }

    public List<SevenTvEmote>? New7TvEmotes { get; set; }

    public List<SevenTvEmote>? Old7TvEmotes { get; set; }

    public List<BttvSharedEmote>? NewBttvEmotes { get; set; }

    public List<BttvSharedEmote>? OldBttvEmotes { get; set; }

    public List<FfzEmote>? NewFfzEmotes { get; set; }

    public List<FfzEmote>? OldFfzEmotes { get; set; }

    public ThirdPartyNotificatorChannel(string name)
    {
        Name = name;
    }

    public override bool Equals(object? obj)
    {
        return obj is ThirdPartyNotificatorChannel notificatorChannel && notificatorChannel.Name == Name;
    }

    public override string ToString()
    {
        return Name;
    }
}