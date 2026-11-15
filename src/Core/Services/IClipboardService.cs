using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using TopNotch.Models;

namespace TopNotch.Core.Services;

/// <summary>
/// Service for clipboard history management - cross-platform
/// </summary>
public interface IClipboardService : IServiceBase
{
    IReadOnlyList<ClipboardItem> History { get; }
    ClipboardItem CurrentItem { get; }
    event EventHandler<ClipboardItem> ClipboardChanged;
    event EventHandler<ClipboardItem> HistoryChanged;
    bool IsHistoryEnabled { get; set; }
    Task CopyToClipboardAsync(ClipboardItem item);
    Task ClearHistoryAsync();
    Task PinItemAsync(ClipboardItem item);
    Task UnpinItemAsync(ClipboardItem item);
    Task DeleteItemAsync(ClipboardItem item);
    IEnumerable<ClipboardItem> SearchHistory(string query);
    Task LoadHistoryFromFileAsync();
}

/// <summary>
/// Clipboard service implementation - cross-platform using Avalonia clipboard
/// </summary>
public class ClipboardService : IClipboardService
{
    private readonly List<ClipboardItem> _history = new();
    private readonly ClipboardItem _currentItem = new();
    private readonly int _maxHistorySize;
    private string? _lastTextContent;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isMonitoring;
    private bool _isRunning;
    private IClipboard? _clipboard;
    private Window? _parentWindow;

    public bool IsRunning => _isRunning;
    public bool IsInitialized => true;

    public IReadOnlyList<ClipboardItem> History => _history.AsReadOnly();
    public ClipboardItem CurrentItem => _currentItem;
    public bool IsHistoryEnabled { get; set; } = true;

    public event EventHandler<ClipboardItem>? ClipboardChanged;
    public event EventHandler<ClipboardItem>? HistoryChanged;

    public ClipboardService(int maxHistorySize = 50, Window? parentWindow = null)
    {
        _maxHistorySize = maxHistorySize;
        _parentWindow = parentWindow;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning) return;
        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();

        // Get clipboard from Avalonia application - use DesktopApplicationLifetime
        if (Application.Current?.ApplicationLifetime is DesktopApplicationLifetime desktopLifetime &&
            desktopLifetime.MainWindow is Window mainWindow)
        {
            _clipboard = mainWindow.Clipboard;
        }

        await StartMonitoringAsync().ConfigureAwait(false);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_isRunning) return;
        _isRunning = false;

        await StopMonitoringAsync().ConfigureAwait(false);
        _cancellationTokenSource?.Cancel();
    }

    private async Task StartMonitoringAsync()
    {
        if (_isMonitoring) return;
        _isMonitoring = true;

        // Start clipboard monitoring in background
        _ = Task.Run(async () =>
        {
            while (_isMonitoring && !_cancellationTokenSource?.Token.IsCancellationRequested == true)
            {
                try
                {
                    await CheckClipboardChangesAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Clipboard monitoring error: {ex.Message}");
                }

                await Task.Delay(500).ConfigureAwait(false);
            }
        }, _cancellationTokenSource?.Token ?? default);
    }

    private async Task StopMonitoringAsync()
    {
        _isMonitoring = false;
        await Task.CompletedTask.ConfigureAwait(false);
    }

    private async Task CheckClipboardChangesAsync()
    {
        try
        {
            if (_clipboard == null) return;

            // Use Avalonia clipboard API
            var text = await _clipboard.GetTextAsync().ConfigureAwait(false);

            if (!string.IsNullOrEmpty(text) && text != _lastTextContent)
            {
                _lastTextContent = text;
                await AddToHistoryAsync(text).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Clipboard check error: {ex.Message}");
        }
    }

    private async Task AddToHistoryAsync(string text)
    {
        if (!IsHistoryEnabled) return;

        var item = new ClipboardItem
        {
            Content = text,
            ContentType = "text",
            Timestamp = DateTime.UtcNow
        };

        _history.Insert(0, item);

        if (_history.Count > _maxHistorySize)
        {
            _history.RemoveAt(_history.Count - 1);
        }

        HistoryChanged?.Invoke(this, item);
        ClipboardChanged?.Invoke(this, item);

        await SaveHistoryToFileAsync().ConfigureAwait(false);
    }

    public async Task CopyToClipboardAsync(ClipboardItem item)
    {
        try
        {
            if (_clipboard != null)
            {
                await _clipboard.SetTextAsync(item.Content).ConfigureAwait(false);
            }

            _currentItem.Content = item.Content;
            _currentItem.ContentType = item.ContentType;
            _currentItem.Timestamp = DateTime.UtcNow;
            _currentItem.IsPinned = item.IsPinned;

            ClipboardChanged?.Invoke(this, _currentItem);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Copy to clipboard error: {ex.Message}");
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task ClearHistoryAsync()
    {
        _history.Clear();
        HistoryChanged?.Invoke(this, _currentItem);
        await SaveHistoryToFileAsync().ConfigureAwait(false);
    }

    public async Task PinItemAsync(ClipboardItem item)
    {
        item.IsPinned = true;
        HistoryChanged?.Invoke(this, item);
        await SaveHistoryToFileAsync().ConfigureAwait(false);
    }

    public async Task UnpinItemAsync(ClipboardItem item)
    {
        item.IsPinned = false;
        HistoryChanged?.Invoke(this, item);
        await SaveHistoryToFileAsync().ConfigureAwait(false);
    }

    public async Task DeleteItemAsync(ClipboardItem item)
    {
        _history.Remove(item);
        HistoryChanged?.Invoke(this, _currentItem);
        await SaveHistoryToFileAsync().ConfigureAwait(false);
    }

    public IEnumerable<ClipboardItem> SearchHistory(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return _history;

        return _history.Where(item =>
            item.Content.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            item.Application.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    private async Task SaveHistoryToFileAsync()
    {
        try
        {
            var appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TopNotch", "Clipboard"
            );
            Directory.CreateDirectory(appDataDir);

            var historyPath = Path.Combine(appDataDir, "history.json");
            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
            var json = System.Text.Json.JsonSerializer.Serialize(_history, options);
            await File.WriteAllTextAsync(historyPath, json).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save clipboard history: {ex.Message}");
        }
    }

    public async Task LoadHistoryFromFileAsync()
    {
        try
        {
            var appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TopNotch", "Clipboard"
            );
            var historyPath = Path.Combine(appDataDir, "history.json");

            if (File.Exists(historyPath))
            {
                var json = await File.ReadAllTextAsync(historyPath).ConfigureAwait(false);
                var loaded = System.Text.Json.JsonSerializer.Deserialize<List<ClipboardItem>>(json);
                if (loaded != null)
                {
                    _history.Clear();
                    _history.AddRange(loaded.Take(_maxHistorySize));
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load clipboard history: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        await StopAsync().ConfigureAwait(false);
    }
}