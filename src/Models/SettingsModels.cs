using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace TopNotch.Models;

/// <summary>
/// Settings models
/// </summary>
public class SettingsModel : ObservableObject
{
    private IslandSettingsModel _island = new();
    private AppearanceSettingsModel _appearance = new();
    private HotkeySettingsModel _hotkeys = new();
    private GeneralSettingsModel _general = new();
    private PrivacySettingsModel _privacy = new();

    public IslandSettingsModel Island
    {
        get => _island;
        set => SetProperty(ref _island, value);
    }

    public AppearanceSettingsModel Appearance
    {
        get => _appearance;
        set => SetProperty(ref _appearance, value);
    }

    public HotkeySettingsModel Hotkeys
    {
        get => _hotkeys;
        set => SetProperty(ref _hotkeys, value);
    }

    public GeneralSettingsModel General
    {
        get => _general;
        set => SetProperty(ref _general, value);
    }

    public PrivacySettingsModel Privacy
    {
        get => _privacy;
        set => SetProperty(ref _privacy, value);
    }
}

public class IslandSettingsModel : ObservableObject
{
    private double _width = 200;
    private double _height = 40;
    private double _cornerRadius = 16;
    private double _opacity = 0.9;
    private double _blurRadius = 20;
    private bool _borderEnabled = true;
    private bool _showShadow = true;
    private IslandMode _mode = IslandMode.Floating;
    private IslandPosition _position = IslandPosition.Center;
    private MonitorSelection _monitor = MonitorSelection.Primary;
    private double _hoverSensitivity = 100;
    private TimeSpan _expansionDelay = TimeSpan.FromMilliseconds(200);
    private string _accentColor = "#00B7FF";

    public double Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }

    public double Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }

    public double CornerRadius
    {
        get => _cornerRadius;
        set => SetProperty(ref _cornerRadius, value);
    }

    public double Opacity
    {
        get => _opacity;
        set => SetProperty(ref _opacity, value);
    }

    public double BlurRadius
    {
        get => _blurRadius;
        set => SetProperty(ref _blurRadius, value);
    }

    public bool BorderEnabled
    {
        get => _borderEnabled;
        set => SetProperty(ref _borderEnabled, value);
    }

    public bool ShowShadow
    {
        get => _showShadow;
        set => SetProperty(ref _showShadow, value);
    }

    public IslandMode Mode
    {
        get => _mode;
        set => SetProperty(ref _mode, value);
    }

    public IslandPosition Position
    {
        get => _position;
        set => SetProperty(ref _position, value);
    }

    public MonitorSelection Monitor
    {
        get => _monitor;
        set => SetProperty(ref _monitor, value);
    }

    public double HoverSensitivity
    {
        get => _hoverSensitivity;
        set => SetProperty(ref _hoverSensitivity, value);
    }

    public TimeSpan ExpansionDelay
    {
        get => _expansionDelay;
        set => SetProperty(ref _expansionDelay, value);
    }

    public string AccentColor
    {
        get => _accentColor;
        set => SetProperty(ref _accentColor, value);
    }
}

public class AppearanceSettingsModel : ObservableObject
{
    private ThemeType _theme = ThemeType.Dark;
    private bool _reducedMotion = false;
    private double _animationSpeed = 1.0;

    public ThemeType Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value);
    }

    public bool ReducedMotion
    {
        get => _reducedMotion;
        set => SetProperty(ref _reducedMotion, value);
    }

    public double AnimationSpeed
    {
        get => _animationSpeed;
        set => SetProperty(ref _animationSpeed, value);
    }
}

public class HotkeySettingsModel : ObservableObject
{
    private string _openIsland = "Ctrl+J";
    private string _muteMicrophone = "Ctrl+M";
    private string _clipboardHistory = "Ctrl+V";
    private string _launcher = "Ctrl+Space";
    private string _gamingMode = "Ctrl+G";

    public string OpenIsland
    {
        get => _openIsland;
        set => SetProperty(ref _openIsland, value);
    }

    public string MuteMicrophone
    {
        get => _muteMicrophone;
        set => SetProperty(ref _muteMicrophone, value);
    }

    public string ClipboardHistory
    {
        get => _clipboardHistory;
        set => SetProperty(ref _clipboardHistory, value);
    }

    public string Launcher
    {
        get => _launcher;
        set => SetProperty(ref _launcher, value);
    }

    public string GamingMode
    {
        get => _gamingMode;
        set => SetProperty(ref _gamingMode, value);
    }
}

public class GeneralSettingsModel : ObservableObject
{
    private bool _startWithSystem = true;
    private bool _minimizeToTray = true;
    private bool _checkForUpdates = true;
    private string _language = "en-US";

    public bool StartWithSystem
    {
        get => _startWithSystem;
        set => SetProperty(ref _startWithSystem, value);
    }

    public bool MinimizeToTray
    {
        get => _minimizeToTray;
        set => SetProperty(ref _minimizeToTray, value);
    }

    public bool CheckForUpdates
    {
        get => _checkForUpdates;
        set => SetProperty(ref _checkForUpdates, value);
    }

    public string Language
    {
        get => _language;
        set => SetProperty(ref _language, value);
    }
}

public class PrivacySettingsModel : ObservableObject
{
    private bool _clipboardHistoryEnabled = true;
    private bool _telemetryEnabled = false;
    private int _clipboardHistorySize = 50;
    private bool _enableNotifications = true;
    private bool _enablePhoneConnections = true;

    public bool ClipboardHistoryEnabled
    {
        get => _clipboardHistoryEnabled;
        set => SetProperty(ref _clipboardHistoryEnabled, value);
    }

    public bool TelemetryEnabled
    {
        get => _telemetryEnabled;
        set => SetProperty(ref _telemetryEnabled, value);
    }

    public int ClipboardHistorySize
    {
        get => _clipboardHistorySize;
        set => SetProperty(ref _clipboardHistorySize, value);
    }

    public bool EnableNotifications
    {
        get => _enableNotifications;
        set => SetProperty(ref _enableNotifications, value);
    }

    public bool EnablePhoneConnections
    {
        get => _enablePhoneConnections;
        set => SetProperty(ref _enablePhoneConnections, value);
    }
}

public enum IslandMode { Floating, Notch }
public enum IslandPosition { Top, Center, Custom }
public enum MonitorSelection { Primary, Secondary, All }
public enum ThemeType { Light, Dark, System, Custom }