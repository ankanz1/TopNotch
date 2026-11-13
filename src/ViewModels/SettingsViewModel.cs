using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TopNotch.Models;

namespace TopNotch.ViewModels;

/// <summary>
/// Settings service for persisting settings
/// </summary>
public interface ISettingsService
{
    SettingsModel Settings { get; }
    Task LoadAsync();
    Task SaveAsync();
}

/// <summary>
/// Settings service implementation
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly string _settingsPath;
    public SettingsModel Settings { get; private set; } = new();

    public SettingsService()
    {
        var appDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TopNotch"
        );
        Directory.CreateDirectory(appDataDir);
        _settingsPath = Path.Combine(appDataDir, "settings.json");
    }

    public async Task LoadAsync()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = await File.ReadAllTextAsync(_settingsPath).ConfigureAwait(false);
                var loaded = JsonSerializer.Deserialize<SettingsModel>(json);
                if (loaded != null)
                {
                    Settings = loaded;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
        }
    }

    public async Task SaveAsync()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(Settings, options);
            await File.WriteAllTextAsync(_settingsPath, json).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }
}

/// <summary>
/// ViewModel for the settings page
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    public SettingsViewModel() : this(new SettingsService())
    {
    }

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        // Load settings
        GeneralSettings = _settingsService.Settings.General;
        IslandSettings = _settingsService.Settings.Island;
        AppearanceSettings = _settingsService.Settings.Appearance;
        HotkeySettings = _settingsService.Settings.Hotkeys;
        PrivacySettings = _settingsService.Settings.Privacy;

        // Initialize collections
        InitializeCollections();

        SaveCommand = new AsyncRelayCommand(SaveAsync);
        ResetCommand = new AsyncRelayCommand(ResetAsync);
    }

    public GeneralSettingsModel GeneralSettings { get; set; } = new();
    public IslandSettingsModel IslandSettings { get; set; } = new();
    public AppearanceSettingsModel AppearanceSettings { get; set; } = new();
    public HotkeySettingsModel HotkeySettings { get; set; } = new();
    public PrivacySettingsModel PrivacySettings { get; set; } = new();

    public ObservableCollection<WidgetType> AvailableWidgets { get; } = new()
    {
        new WidgetType { Name = "Clock", Icon = "" },
        new WidgetType { Name = "Weather", Icon = "" },
        new WidgetType { Name = "Battery", Icon = "" },
        new WidgetType { Name = "CPU", Icon = "" },
        new WidgetType { Name = "RAM", Icon = "" },
        new WidgetType { Name = "Network", Icon = "" }
    };

    public ObservableCollection<IslandMode> Modes { get; } = new()
    {
        IslandMode.Floating,
        IslandMode.Notch
    };

    public ObservableCollection<MonitorSelection> Monitors { get; } = new()
    {
        MonitorSelection.Primary,
        MonitorSelection.Secondary,
        MonitorSelection.All
    };

    public ObservableCollection<ThemeType> Themes { get; } = new()
    {
        ThemeType.Light,
        ThemeType.Dark,
        ThemeType.System
    };

    // Save command
    public IAsyncRelayCommand SaveCommand { get; }
    // Reset command
    public IAsyncRelayCommand ResetCommand { get; }

    private void InitializeCollections()
    {
        // Ensure collections are properly initialized
        if (Modes.Count == 0)
        {
            Modes.Add(IslandMode.Floating);
            Modes.Add(IslandMode.Notch);
        }

        if (Monitors.Count == 0)
        {
            Monitors.Add(MonitorSelection.Primary);
            Monitors.Add(MonitorSelection.Secondary);
            Monitors.Add(MonitorSelection.All);
        }

        if (Themes.Count == 0)
        {
            Themes.Add(ThemeType.Light);
            Themes.Add(ThemeType.Dark);
            Themes.Add(ThemeType.System);
        }
    }

    private async Task SaveAsync()
    {
        _settingsService.Settings.General = GeneralSettings;
        _settingsService.Settings.Island = IslandSettings;
        _settingsService.Settings.Appearance = AppearanceSettings;
        _settingsService.Settings.Hotkeys = HotkeySettings;
        _settingsService.Settings.Privacy = PrivacySettings;

        await _settingsService.SaveAsync().ConfigureAwait(false);
    }

    private async Task ResetAsync()
    {
        // Reset to defaults
        GeneralSettings = new GeneralSettingsModel();
        IslandSettings = new IslandSettingsModel();
        AppearanceSettings = new AppearanceSettingsModel();
        HotkeySettings = new HotkeySettingsModel();
        PrivacySettings = new PrivacySettingsModel();

        OnPropertyChanged(nameof(GeneralSettings));
        OnPropertyChanged(nameof(IslandSettings));
        OnPropertyChanged(nameof(AppearanceSettings));
        OnPropertyChanged(nameof(HotkeySettings));
        OnPropertyChanged(nameof(PrivacySettings));

        await SaveAsync().ConfigureAwait(false);
    }
}

public class WidgetType
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}