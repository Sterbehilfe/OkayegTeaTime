using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
#if RELEASE
using System.Threading;
#endif
using System.Threading.Tasks;
using System.Timers;
using HLE.Time;
#if RELEASE
using OkayegTeaTime.Files;
#endif
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Spotify.Exceptions;
using OkayegTeaTime.Utils;
using SpotifyAPI.Web;
using Timer = System.Timers.Timer;

namespace OkayegTeaTime.Database.Models;

[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
public class SpotifyUser : CacheModel
{
    public long Id { get; }

    public string Username { get; }

    public string AccessToken
    {
        get => _accessToken;
        set
        {
            _accessToken = value;
            _mutex.WaitOne();
            EntityFrameworkModels.Spotify? user = DbContext.Spotify.FirstOrDefault(s => s.Id == Id);
            _mutex.ReleaseMutex();
            if (user is null)
            {
                return;
            }

            user.AccessToken = value;
            EditedProperty();
        }
    }

    public string RefreshToken
    {
        get => _refreshToken;
        set
        {
            _refreshToken = value;
            _mutex.WaitOne();
            EntityFrameworkModels.Spotify? user = DbContext.Spotify.FirstOrDefault(s => s.Id == Id);
            _mutex.ReleaseMutex();
            if (user is null)
            {
                return;
            }

            user.RefreshToken = value;
            EditedProperty();
        }
    }

    public long Time
    {
        get => _time;
        set
        {
            _time = value;
            _mutex.WaitOne();
            EntityFrameworkModels.Spotify? user = DbContext.Spotify.FirstOrDefault(s => s.Id == Id);
            _mutex.ReleaseMutex();
            if (user is null)
            {
                return;
            }

            user.Time = value;
            EditedProperty();
        }
    }

    public bool AreSongRequestsEnabled
    {
        get => _areSongRequestsEnabled;
        set
        {
            _areSongRequestsEnabled = value;
            _mutex.WaitOne();
            EntityFrameworkModels.Spotify? user = DbContext.Spotify.FirstOrDefault(s => s.Id == Id);
            _mutex.ReleaseMutex();
            if (user is null)
            {
                return;
            }

            user.SongRequestEnabled = value;
            EditedProperty();
        }
    }

    public List<SpotifyUser> ListeningUsers { get; } = new();

    private string _accessToken;
    private string _refreshToken;
    private long _time;
    private bool _areSongRequestsEnabled;

    private readonly Timer _timer = new();

    /// <summary>
    /// The length of "spotify:track:".
    /// </summary>
    private const byte _trackIdPrefixLength = 14;

    public SpotifyUser(EntityFrameworkModels.Spotify spotifyUser)
    {
        Id = spotifyUser.Id;
        Username = spotifyUser.Username;
        _accessToken = spotifyUser.AccessToken;
        _refreshToken = spotifyUser.RefreshToken;
        _time = spotifyUser.Time;
        _areSongRequestsEnabled = spotifyUser.SongRequestEnabled;

        _timer.Elapsed += Timer_OnElapsed;
    }

    public SpotifyUser(long id, string username, string accessToken, string refreshToken)
    {
        Id = id;
        Username = username;
        _accessToken = accessToken;
        _refreshToken = refreshToken;
        _time = TimeHelper.Now();

        _timer.Elapsed += Timer_OnElapsed;
    }

    private async void Timer_OnElapsed(object? sender, ElapsedEventArgs e)
    {
        SpotifyItem? song;
        try
        {
            song = await GetCurrentlyPlayingItem();
        }
        catch (SpotifyException)
        {
            ListeningUsers.Clear();
            _timer.Stop();
            return;
        }

        if (song is null)
        {
            ListeningUsers.Clear();
            _timer.Stop();
            return;
        }

        _timer.Interval = song.Duration;
        _timer.Start();

        foreach (SpotifyUser user in ListeningUsers)
        {
            try
            {
                await user.ListenTo(song);
            }
            catch (SpotifyException)
            {
                ListeningUsers.Remove(user);
            }
        }
    }

    private async Task<SpotifyClient?> GetClient()
    {
        if (!IsAccessTokenExpired())
        {
            return new(AccessToken);
        }

        string? accessToken = await SpotifyController.GetNewAccessToken(RefreshToken);
        if (accessToken is null)
        {
            return null;
        }

        AccessToken = accessToken;
        Time = TimeHelper.Now();
        return new(AccessToken);
    }

    private bool IsAccessTokenExpired()
    {
        return Time + TimeSpan.FromHours(1).TotalMilliseconds <= TimeHelper.Now() + TimeSpan.FromSeconds(30).TotalMilliseconds;
    }

#if RELEASE
    /// <summary>
    /// Adds the passed songs to the chat playlist (<see cref="AppSettings.Spotify.ChatPlaylistId"/>) in an own thread.
    /// </summary>
    /// <param name="songs">The songs that will be added to the playlist.</param>
    /// <exception cref="SpotifyException">Will be thrown if it was unable to add a song to the playlist.</exception>
    public void AddToChatPlaylist(params string[] songs)
    {
        async Task AddToChatPlaylistLocal()
        {
            string[] uris = songs.Select(s => SpotifyController.ParseSongToUri(s) ?? string.Empty)
                .Where(u => !string.IsNullOrEmpty(u)).ToArray();

            if (uris.Length == 0)
            {
                return;
            }

            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
            }

            try
            {
                uris = uris.Where(u => !DbControl.SpotifyUsers.ChatPlaylistUris.Contains(u)).ToArray();
                if (uris.Length == 0)
                {
                    return;
                }

                await client.Playlists.AddItems(AppSettings.Spotify.ChatPlaylistId, new(uris));
                DbControl.SpotifyUsers.ChatPlaylistUris.AddRange(uris);
            }
            catch (Exception ex)
            {
                DbController.LogException(ex);
                throw new SpotifyException("Something went wrong trying to add the song to the playlist");
            }
        }

        Thread thread = new(() => AddToChatPlaylistLocal().Wait());
        thread.Start();
    }

    public async Task<IEnumerable<SpotifyTrack>> GetPlaylistItems(string playlistUri)
    {
        SpotifyClient? client = await GetClient();
        if (client is null)
        {
            throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
        }

        List<SpotifyTrack> result = new();
        int offset = 0;
        while (true)
        {
            try
            {
                Paging<PlaylistTrack<IPlayableItem>> playlistItems = await client.Playlists.GetItems(playlistUri, new()
                {
                    Offset = offset
                });
                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                SpotifyTrack[]? items = playlistItems?.Items?.Select(i => new SpotifyTrack(i.Track)).ToArray();
                if (items is null)
                {
                    break;
                }

                result.AddRange(items);
                if (items.Length < 100)
                {
                    break;
                }

                offset += 100;
            }
            catch (Exception ex)
            {
                DbController.LogException(ex);
                break;
            }
        }

        return result;
    }
#endif

    public async Task<SpotifyTrack> AddToQueue(string song)
    {
        if (!AreSongRequestsEnabled)
        {
            throw new SpotifyException($"song requests are currently not enabled, {Username.Antiping()} or a moderator has to enable it first");
        }

        string? uri = SpotifyController.ParseSongToUri(song);
        if (uri is null)
        {
            SpotifyTrack? searchResult = await SearchTrack(song);
            uri = searchResult?.Uri;
        }

        if (uri is null)
        {
            throw new SpotifyException("no matching track could be found");
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.AddToQueue(new(uri));
            FullTrack item = await client.Tracks.Get(uri[_trackIdPrefixLength..], new());
            return new SpotifyTrack(item);
        }
        catch (APIException ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occurred, {Username.Antiping()} probably has to start their playback first");
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to add songs to {Username.Antiping()}'s queue");
        }
    }

    public async Task Skip()
    {
        if (!AreSongRequestsEnabled)
        {
            throw new SpotifyException($"song requests are currently not enabled, {Username.Antiping()} or a moderator has to enable it first");
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.SkipNext(new());
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to skip songs of {Username.Antiping()}'s queue");
        }
    }

    public async Task<SpotifyItem> ListenAlongWith(SpotifyUser target)
    {
        if (string.Equals(target.Username, Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new SpotifyException("you can't listen to yourself :)");
        }

        CurrentlyPlayingContext? playback = await target.GetCurrentlyPlayingContext();
        if (playback is null)
        {
            throw new SpotifyException($"{target.Username.Antiping()} isn't listening to anything at the moment");
        }

        SpotifyItem item;
        if (playback.Item is FullTrack track)
        {
            item = new SpotifyTrack(track);
        }
        else if (playback.Item is FullEpisode episode)
        {
            item = new SpotifyEpisode(episode);
        }
        else
        {
            item = new(playback.Item);
        }

        int seekTo = playback.ProgressMs > 500 ? playback.ProgressMs : 0;
        await ListenTo(item, seekTo);
        ListeningUsers.Clear();
        _timer.Stop();
        if (!target.ListeningUsers.Contains(this))
        {
            target.ListeningUsers.Add(this);
        }

        int interval = item.Duration - playback.ProgressMs;
        target.StartTimer(interval);
        return item;
    }

    private void StartTimer(int interval)
    {
        _timer.Stop();
        _timer.Interval = interval + 500;
        _timer.Start();
    }

    public SpotifyUser? GetListeningTo()
    {
        return DbControl.SpotifyUsers.FirstOrDefault(u => u.ListeningUsers.Contains(this) && u != this);
    }

    public async Task<SpotifyItem> ListenTo(SpotifyUser target, int seekToMs = default)
    {
        if (string.Equals(target.Username, Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new SpotifyException("you can't listen to your own songs :)");
        }

        CurrentlyPlayingContext? playback = await target.GetCurrentlyPlayingContext();
        if (playback is null)
        {
            throw new SpotifyException($"{target.Username.Antiping()} isn't listening to anything at the moment");
        }

        SpotifyItem song = new(playback.Item);
        return await ListenTo(song, seekToMs);
    }

    public async Task<SpotifyItem> ListenTo(SpotifyItem item, int seekToMs = default)
    {
        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.AddToQueue(new(item.Uri));
        }
        catch (APIException ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occurred, {Username.Antiping()} probably has to start their playback first");
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException("an error occurred, it might not be possible to listen to other people's songs");
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.SkipNext(new());
            if (seekToMs != default)
            {
                await client.Player.SeekTo(new(seekToMs));
            }
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occured while trying to play the song {Username.Antiping()} wanted to listen to");
        }

        return item;
    }

    public async Task<SpotifyItem?> GetCurrentlyPlayingItem()
    {
        CurrentlyPlaying? currentlyPlaying;
        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
            }

            currentlyPlaying = await client.Player.GetCurrentlyPlaying(new());

            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            if (currentlyPlaying?.IsPlaying == false)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to retrieve {Username.Antiping()}'s currently playing song");
        }

        SpotifyItem? item = null;
        if (currentlyPlaying?.Item is FullTrack track)
        {
            item = new SpotifyTrack(track);

#if RELEASE
            if (!AppSettings.Spotify.ChatPlaylistUsers.Contains(Id))
            {
                return item;
            }

            if (item.IsLocal)
            {
                return item;
            }

            string? username = DbControl.Users[AppSettings.UserLists.Owner]?.Username;
            if (username is null)
            {
                return item;
            }

            SpotifyUser? playlistUser = DbControl.SpotifyUsers[username];
            if (playlistUser is null)
            {
                return item;
            }

            try
            {
                playlistUser.AddToChatPlaylist(item.Uri);
            }
            catch (SpotifyException ex)
            {
                DbController.LogException(ex);
            }
#endif
        }
        else if (currentlyPlaying?.Item is FullEpisode episode)
        {
            item = new SpotifyEpisode(episode);
        }

        return item;
    }

    public async Task<SpotifyTrack?> SearchTrack(string query)
    {
        SpotifyClient? client = await GetClient();
        if (client is null)
        {
            throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
        }

        string? uri = SpotifyController.ParseSongToUri(query);
        if (uri is not null)
        {
            try
            {
                FullTrack track = await client.Tracks.Get(uri.Split(':')[^1], new());
                if (track is not null)
                {
                    return new(track);
                }
            }
            catch (Exception ex)
            {
                DbController.LogException(ex);
            }
        }

        try
        {
            SearchResponse searchResult = await client.Search.Item(new(SearchRequest.Types.Track, query));
            if (searchResult.Tracks.Items is null || searchResult.Tracks.Items.Count < 1)
            {
                return null;
            }

            FullTrack result = searchResult.Tracks.Items[0];
            return new(result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private async Task<CurrentlyPlayingContext?> GetCurrentlyPlayingContext()
    {
        SpotifyClient? client = await GetClient();
        if (client is null)
        {
            throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
        }

        CurrentlyPlayingContext playback = await client.Player.GetCurrentPlayback(new());
        if (playback is null || !playback.IsPlaying)
        {
            return null;
        }

        return playback;
    }
}
