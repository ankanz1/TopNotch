using System;
using System.Threading;
using System.Threading.Tasks;
using TopNotch.Models;

namespace TopNotch.Core.Services;

/// <summary>
/// Service for media control integration (cross-platform)
/// </summary>
public interface IMediaService : IServiceBase
{
    /// <summary>
    /// Gets the current media information
    /// </summary>
    MediaInfo CurrentMedia { get; }

    /// <summary>
    /// Occurs when media information changes
    /// </summary>
    event EventHandler<MediaInfo> MediaChanged;

    /// <summary>
    /// Occurs when playback controls are requested
    /// </summary>
    event EventHandler<PlaybackCommand> PlaybackCommandRequested;

    /// <summary>
    /// Gets a value indicating whether media is available
    /// </summary>
    bool HasMedia { get; }

    /// <summary>
    /// Requests a playback control action
    /// </summary>
    Task RequestPlaybackControlAsync(PlaybackCommand command);

    /// <summary>
    /// Subscribes to system media transport controls
    /// </summary>
    Task SubscribeToMediaControlsAsync();

    /// <summary>
    /// Unsubscribes from system media transport controls
    /// </summary>
    Task UnsubscribeFromMediaControlsAsync();
}

public enum PlaybackCommand
{
    Play,
    Pause,
    PlayPause,
    Previous,
    Next,
    Stop,
    Seek
}

/// <summary>
/// Media service implementation - cross-platform stub
/// On Linux, this provides basic stub functionality
/// On Windows, this can be extended to use Windows Media APIs
/// </summary>
public class MediaService : IMediaService
{
    private readonly MediaInfo _currentMedia = new();
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isSubscribed;
    private bool _isRunning;

    public bool IsRunning => _isRunning;
    public bool IsInitialized => true; // Always initialized

    public MediaInfo CurrentMedia => _currentMedia;
    public bool HasMedia => _currentMedia.HasMedia;

    public event EventHandler<MediaInfo>? MediaChanged;
    public event EventHandler<PlaybackCommand>? PlaybackCommandRequested;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning) return;
        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();

        await SubscribeToMediaControlsAsync().ConfigureAwait(false);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_isRunning) return;
        _isRunning = false;

        await UnsubscribeFromMediaControlsAsync().ConfigureAwait(false);
        _cancellationTokenSource?.Cancel();
    }

    public async Task RequestPlaybackControlAsync(PlaybackCommand command)
    {
        // Cross-platform: event-based playback control
        // On Linux: could integrate with media players via IPC
        // On Windows: could use Windows Media Session API

        PlaybackCommandRequested?.Invoke(this, command);
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task SubscribeToMediaControlsAsync()
    {
        if (_isSubscribed) return;
        _isSubscribed = true;

        // Cross-platform media subscription
        // On Windows: subscribe to Windows Media Transport Controls
        // On Linux: subscribe to media player IPC (e.g., MPRIS)

        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task UnsubscribeFromMediaControlsAsync()
    {
        if (!_isSubscribed) return;
        _isSubscribed = false;
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public void UpdateMediaInfo(MediaInfo newInfo)
    {
        if (newInfo.GetHashCode() != _currentMedia.GetHashCode())
        {
            _currentMedia.Clear();
            _currentMedia.Title = newInfo.Title;
            _currentMedia.Artist = newInfo.Artist;
            _currentMedia.Album = newInfo.Album;
            _currentMedia.AppName = newInfo.AppName;
            _currentMedia.Position = newInfo.Position;
            _currentMedia.Duration = newInfo.Duration;
            _currentMedia.PlaybackState = newInfo.PlaybackState;
            _currentMedia.HasArtwork = newInfo.HasArtwork;
            _currentMedia.ArtworkPath = newInfo.ArtworkPath;

            MediaChanged?.Invoke(this, _currentMedia);
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        await StopAsync().ConfigureAwait(false);
    }
}