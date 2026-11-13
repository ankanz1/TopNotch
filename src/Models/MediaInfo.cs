using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace TopNotch.Models;

/// <summary>
/// Represents currently playing media information
/// </summary>
public partial class MediaInfo : ObservableObject
{
    private string _title = string.Empty;
    private string _artist = string.Empty;
    private string _album = string.Empty;
    private string _appName = string.Empty;
    private TimeSpan _position = TimeSpan.Zero;
    private TimeSpan _duration = TimeSpan.Zero;
    private MediaPlaybackState _playbackState = MediaPlaybackState.Stopped;
    private bool _hasArtwork = false;
    private string _artworkPath = string.Empty;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Artist
    {
        get => _artist;
        set => SetProperty(ref _artist, value);
    }

    public string Album
    {
        get => _album;
        set => SetProperty(ref _album, value);
    }

    public string AppName
    {
        get => _appName;
        set => SetProperty(ref _appName, value);
    }

    public TimeSpan Position
    {
        get => _position;
        set => SetProperty(ref _position, value);
    }

    public TimeSpan Duration
    {
        get => _duration;
        set => SetProperty(ref _duration, value);
    }

    public MediaPlaybackState PlaybackState
    {
        get => _playbackState;
        set => SetProperty(ref _playbackState, value);
    }

    public bool HasArtwork
    {
        get => _hasArtwork;
        set => SetProperty(ref _hasArtwork, value);
    }

    public string ArtworkPath
    {
        get => _artworkPath;
        set => SetProperty(ref _artworkPath, value);
    }

    public bool IsPlaying => PlaybackState == MediaPlaybackState.Playing;
    public bool IsPaused => PlaybackState == MediaPlaybackState.Paused;
    public bool HasMedia => !string.IsNullOrEmpty(AppName);

    public void Clear()
    {
        Title = string.Empty;
        Artist = string.Empty;
        Album = string.Empty;
        AppName = string.Empty;
        Position = TimeSpan.Zero;
        Duration = TimeSpan.Zero;
        PlaybackState = MediaPlaybackState.Stopped;
        HasArtwork = false;
        ArtworkPath = string.Empty;
    }
}

public enum MediaPlaybackState
{
    Stopped,
    Playing,
    Paused,
    FastForwarding,
    Rewinding
}

/// <summary>
/// Represents a clipboard item
/// </summary>
public partial class ClipboardItem : ObservableObject
{
    private string _id = Guid.NewGuid().ToString();
    private string _content = string.Empty;
    private string _contentType = "text";
    private DateTime _timestamp = DateTime.UtcNow;
    private bool _isPinned = false;
    private string _application = string.Empty;

    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    public string ContentType
    {
        get => _contentType;
        set => SetProperty(ref _contentType, value);
    }

    public DateTime Timestamp
    {
        get => _timestamp;
        set => SetProperty(ref _timestamp, value);
    }

    public bool IsPinned
    {
        get => _isPinned;
        set => SetProperty(ref _isPinned, value);
    }

    public string Application
    {
        get => _application;
        set => SetProperty(ref _application, value);
    }

    public long AgeInSeconds => (long)(DateTime.UtcNow - Timestamp).TotalSeconds;
}