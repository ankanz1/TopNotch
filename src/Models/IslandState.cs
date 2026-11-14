using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace TopNotch.Models;

/// <summary>
/// Represents the current state of the island window
/// </summary>
public class IslandState : ObservableObject
{
    private bool _isExpanded = false;
    private bool _isVisible = true;
    private IslandViewState _currentView = IslandViewState.Compact;
    private DateTime _lastInteractionTime = DateTime.Now;
    private bool _isHovering = false;
    private bool _gamingMode = false;

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public IslandViewState CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public DateTime LastInteractionTime
    {
        get => _lastInteractionTime;
        set => SetProperty(ref _lastInteractionTime, value);
    }

    public bool IsHovering
    {
        get => _isHovering;
        set => SetProperty(ref _isHovering, value);
    }

    public bool GamingMode
    {
        get => _gamingMode;
        set => SetProperty(ref _gamingMode, value);
    }

    public TimeSpan TimeSinceLastInteraction => DateTime.Now - _lastInteractionTime;
}

public enum IslandViewState
{
    Compact,
    Expanded,
    Hidden
}

/// <summary>
/// Base model for island content
/// </summary>
public abstract class IslandContentModel : ObservableObject
{
    private bool _isVisible = true;
    private bool _isEnabled = true;

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }
}