﻿using System.Threading.Tasks;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;

namespace OkayegTeaTime.Database.Cache;

public class SpotifyUserCache : DbCache<SpotifyUser>
{
    public SpotifyUser? this[string username] => GetSpotifyUser(username);

    public List<string> ChatPlaylistUris
    {
        get
        {
            if (_chatPlaylistUris is not null)
            {
                return _chatPlaylistUris;
            }

            async Task GetPlaylistTracks()
            {
                SpotifyUser? user = this["strbhlfe"];
                if (user is null)
                {
                    return;
                }

                IEnumerable<SpotifyTrack> tracks = await user.GetPlaylistItems(AppSettings.Spotify.ChatPlaylistId);
                _chatPlaylistUris = tracks.Select(t => t.Uri).ToList();
            }

            GetPlaylistTracks().Wait();
            _chatPlaylistUris ??= new();
            return _chatPlaylistUris;
        }
    }

    private List<string>? _chatPlaylistUris;

    public void Add(string username, string accessToken, string refreshToken)
    {
        int? id = DbController.AddSpotifyUser(username, accessToken, refreshToken);
        if (!id.HasValue)
        {
            return;
        }

        SpotifyUser user = new(id.Value, username, accessToken, refreshToken);
        _items.Add(user);
    }

    private SpotifyUser? GetSpotifyUser(string username)
    {
        SpotifyUser? user = _items.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        if (user is not null)
        {
            return user;
        }

        EntityFrameworkModels.Spotify? efUser = DbController.GetSpotifyUser(username);
        if (efUser is null)
        {
            return null;
        }

        user = new(efUser);
        _items.Add(user);
        return user;
    }

    private protected override void GetAllFromDb()
    {
        List<EntityFrameworkModels.Spotify> users = DbController.GetSpotifyUsers();
        users.ForEach(uu =>
        {
            if (_items.All(u => u.Id != uu.Id))
            {
                _items.Add(new(uu));
            }
        });
        _containsAll = true;
    }

    public override IEnumerator<SpotifyUser> GetEnumerator()
    {
        if (_containsAll)
        {
            return _items.GetEnumerator();
        }

        GetAllFromDb();

        return _items.GetEnumerator();
    }
}
