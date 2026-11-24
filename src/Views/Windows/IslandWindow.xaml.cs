using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;

namespace TopNotch.Views.Windows;

public partial class IslandWindow : Window
{
    private const double CompactHeight = 40.0;
    private const double ExpandedHeight = 260.0;

    private bool _isExpanded = false;
    private TextBlock _timeTextBlock = null!;
    private TextBlock _dateTextBlock = null!;
    private TextBlock _songTitleTextBlock = null!;
    private TextBlock _artistTextBlock = null!;
    private TextBlock _lyricsTextBlock = null!;
    private Border _mainBorder = null!;
    private StackPanel _dashboardsContainer = null!;
    private ScrollViewer _scrollContainer = null!;
    private Border _compactHeader = null!;
    private StackPanel _musicControlsPanel = null!;
    private Border _dashboardTabsContainer = null!;
    private Border _playPauseButton = null!;

    private bool _showMusicControls = true;
    private bool _showCalendar = true;
    private bool _showWeather = true;
    private bool _showSystemMonitor = true;
    private bool _showLyrics = false;

    private List<Border> _dashboardTabs = new();
    private int _currentDashboardIndex = 0;

    private bool _hoveredOverWindow = false;

    // System monitoring state
    private double _cpuUsage = 24.0;
    private double _ramUsage = 8.2;
    private double _diskUsage = 45.0;

    // Media state
    private bool _isPlaying = true;
    private double _volume = 70.0;
    private string _currentSong = "Song Title";
    private string _currentArtist = "Artist Name";
    private string _currentLyrics = "";

    public IslandWindow()
    {
        InitializeComponent();
        BuildUI();

        this.WindowDecorations = WindowDecorations.None;
        this.Topmost = true;
        this.ShowInTaskbar = false;
        this.Background = new SolidColorBrush(Color.FromArgb(180, 12, 12, 15));
        this.Width = 220;
        this.Height = CompactHeight;

        InitializeTimer();
    }

    public string CurrentTime { get; set; } = DateTime.Now.ToString("h:mm tt");

    private void BuildUI()
    {
        // Main border - NO GLASS, CURVED BOTTOM EDGE, SOLID BACKGROUND
        // Modern dark theme with subtle gradient effect using solid colors
        _mainBorder = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(235, 15, 15, 20)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(120, 0, 183, 255)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(0, 0, 16, 16), // Curved bottom corners only
            Padding = new Thickness(6),
        };

        var mainGrid = new Grid();
        for (int i = 0; i < 10; i++)
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Time display
        _timeTextBlock = new TextBlock
        {
            FontSize = 11,
            FontWeight = FontWeight.SemiBold,
            Foreground = new SolidColorBrush(Colors.White),
            Text = CurrentTime,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            FontFamily = new Avalonia.Media.FontFamily("Segoe UI")
        };

        // Date display
        _dateTextBlock = new TextBlock
        {
            FontSize = 9,
            Foreground = new SolidColorBrush(Color.FromArgb(180, 200, 200, 220)),
            Text = DateTime.Now.ToString("MMM dd, yyyy"),
            IsVisible = false,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Thickness(0, 2, 0, 4)
        };

        // Song info - hidden initially
        _songTitleTextBlock = new TextBlock
        {
            FontSize = 12,
            FontWeight = FontWeight.SemiBold,
            Foreground = new SolidColorBrush(Colors.White),
            Text = _currentSong,
            IsVisible = false,
            MaxLines = 1,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = new Avalonia.Media.FontFamily("Segoe UI")
        };

        _artistTextBlock = new TextBlock
        {
            FontSize = 9,
            Foreground = new SolidColorBrush(Color.FromArgb(180, 180, 180, 200)),
            Text = _currentArtist,
            IsVisible = false
        };

        // Lyrics display
        _lyricsTextBlock = new TextBlock
        {
            FontSize = 9,
            Foreground = new SolidColorBrush(Color.FromArgb(200, 220, 220, 240)),
            Text = _currentLyrics,
            IsVisible = false,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Thickness(4, 0, 4, 4),
            TextWrapping = TextWrapping.Wrap,
            MaxLines = 2,
            FontStyle = FontStyle.Italic,
            FontFamily = new Avalonia.Media.FontFamily("Segoe UI")
        };

        // Compact header (pill bar) - Futuristic style with glow
        _compactHeader = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(240, 25, 25, 30)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(100, 0, 183, 255)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10), // Rounded pill shape
            Padding = new Thickness(8, 4, 8, 4),
            Margin = new Thickness(0, 0, 0, 2),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Child = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Spacing = 6,
                Children =
                {
                    new Ellipse
                    {
                        Width = 6,
                        Height = 6,
                        Fill = new SolidColorBrush(Color.FromRgb(0, 183, 255))
                    },
                    _timeTextBlock
                }
            }
        };

        // Dashboard tabs container
        _dashboardsContainer = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 8,
            IsVisible = false
        };

        // Wrap dashboard tabs in a container - curved bottom edge to match main window
        _dashboardTabsContainer = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(40, 30, 30, 40)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(80, 100, 100, 120)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4, 4, 12, 12),
            Padding = new Thickness(8),
            IsVisible = false
        };
        _dashboardTabsContainer.Child = _dashboardsContainer;

        // Scroll container for dashboard tabs - HORIZONTAL SCROLL ENABLED
        _scrollContainer = new ScrollViewer
        {
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Hidden,
            Content = _dashboardTabsContainer,
            Margin = new Thickness(0, 4, 0, 6),
            IsVisible = false,
            Background = new SolidColorBrush(Color.Transparent)
        };

        // Build dashboard tabs
        BuildDashboardTabs();

        // Play/Pause button as separate element for click handling
        var playPauseControl = BuildControlButton("⏸", "#00BCD4");
        _playPauseButton = playPauseControl as Border ?? new Border();
        _playPauseButton.PointerPressed += PlayPause_Click;

        // Music controls panel
        _musicControlsPanel = BuildMusicControls(_playPauseButton);

        // Add to grid
        Grid.SetRow(_compactHeader, 0);
        mainGrid.Children.Add(_compactHeader);

        Grid.SetRow(_scrollContainer, 1);
        mainGrid.Children.Add(_scrollContainer);

        Grid.SetRow(_dateTextBlock, 2);
        mainGrid.Children.Add(_dateTextBlock);

        Grid.SetRow(_songTitleTextBlock, 3);
        _songTitleTextBlock.Margin = new Thickness(8, 0, 8, 2);
        mainGrid.Children.Add(_songTitleTextBlock);

        Grid.SetRow(_artistTextBlock, 4);
        _artistTextBlock.Margin = new Thickness(8, 0, 8, 4);
        mainGrid.Children.Add(_artistTextBlock);

        // Lyrics line
        Grid.SetRow(_lyricsTextBlock, 5);
        mainGrid.Children.Add(_lyricsTextBlock);

        Grid.SetRow(_musicControlsPanel, 6);
        mainGrid.Children.Add(_musicControlsPanel);

        _mainBorder.Child = mainGrid;
        this.Content = _mainBorder;

        // Window-level event handlers for hover/wheel
        this.PointerEntered += OnWindowPointerEntered;
        this.PointerExited += OnWindowPointerExited;

        // Handle wheel on the scroll container to switch tabs
        _scrollContainer.PointerWheelChanged += OnScrollViewerWheelChanged;

        // Header click toggles expand/collapse
        _compactHeader.PointerPressed += (s, e) =>
        {
            ToggleIsland();
            e.Handled = true;
        };

        // Main background drag - smooth OS-native dragging
        _mainBorder.PointerPressed += OnMainBorderPointerPressed;
    }

    private void OnMainBorderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

        if (_isExpanded)
        {
            this.BeginMoveDrag(e);
        }
        else
        {
            ToggleIsland();
        }
    }

    private void OnScrollViewerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (!_isExpanded || _dashboardTabs.Count == 0) return;

        var delta = e.Delta.Y;
        if (Math.Abs(delta) < 0.1) return;

        var direction = delta > 0 ? -1 : 1;
        _currentDashboardIndex = ((_currentDashboardIndex + direction) + _dashboardTabs.Count) % _dashboardTabs.Count;

        UpdateActiveTabHighlight();
        ScrollToTargetTab();
        e.Handled = true;
    }

    private const double TabWidth = 118.0;

    private void ScrollToTargetTab()
    {
        if (_scrollContainer == null || _dashboardTabs.Count == 0 || _currentDashboardIndex < 0) return;
        var targetX = -_currentDashboardIndex * TabWidth;
        _scrollContainer.Offset = new Vector(targetX, 0);
    }

    private void BuildDashboardTabs()
    {
        _dashboardTabs.Clear();
        _dashboardsContainer.Children.Clear();

        if (_showCalendar)
        {
            var tab = CreateTabCard("Calendar", new[] {
                BuildWidgetStat("Today", DateTime.Now.ToString("ddd, MMM dd"), "#4CAF50"),
                BuildWidgetStat("Event", "Meeting @ 2PM", "#8BC34A"),
                BuildWidgetStat("Next", "Fri", "#9C27B0")
            });
            _dashboardTabs.Add(tab);
            _dashboardsContainer.Children.Add(tab);
        }

        if (_showWeather)
        {
            var tab = CreateTabCard("Weather", new[] {
                BuildWidgetStat("Temp", "22°C", "#FFEB3B"),
                BuildWidgetStat("Sky", "Sunny", "#FFD700"),
                BuildWidgetStat("Humidity", "65%", "#2196F3")
            });
            _dashboardTabs.Add(tab);
            _dashboardsContainer.Children.Add(tab);
        }

        if (_showSystemMonitor)
        {
            var tab = CreateTabCard("System", new[] {
                BuildWidgetStat("CPU", $"{_cpuUsage:0}%", "#F44336"),
                BuildWidgetStat("RAM", $"{_ramUsage}GB / 16GB", "#2196F3"),
                BuildWidgetStat("Disk", $"{_diskUsage}%", "#9C27B0")
            });
            _dashboardTabs.Add(tab);
            _dashboardsContainer.Children.Add(tab);
        }

        if (_showMusicControls)
        {
            var tab = CreateTabCard("Media", new[] {
                BuildWidgetStat("Artist", _currentArtist, "#00BCD4"),
                BuildWidgetStat("Title", _currentSong, "#E91E63"),
                BuildWidgetStat("Vol", $"{_volume}%", "#FF9800")
            });
            _dashboardTabs.Add(tab);
            _dashboardsContainer.Children.Add(tab);
        }

        UpdateActiveTabHighlight();
    }

    private Border CreateTabCard(string title, IEnumerable<Control> items)
    {
        var sp = new StackPanel { Spacing = 4 };
        sp.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 10,
            FontWeight = FontWeight.Bold,
            Foreground = new SolidColorBrush(Colors.White),
            FontFamily = new Avalonia.Media.FontFamily("Segoe UI Semibold")
        });

        foreach (var item in items)
            sp.Children.Add(item);

        return new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(60, 255, 255, 255)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(80, 255, 255, 255)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4, 4, 0, 0),
            Padding = new Thickness(8),
            Width = 120,
            Child = sp
        };
    }

    private StackPanel BuildMusicControls(Border playPauseButton)
    {
        var panel = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 10,
            IsVisible = false,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Thickness(0, 6, 0, 4)
        };

        var prevButton = BuildControlButton("‹", "#FFFFFF");
        panel.Children.Add(prevButton);
        panel.Children.Add(playPauseButton);
        var nextButton = BuildControlButton("›", "#FFFFFF");
        panel.Children.Add(nextButton);

        return panel;
    }

    private Control BuildControlButton(string icon, string fgColor)
    {
        return new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(180, 45, 45, 55)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(60, 255, 255, 255)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Width = 30,
            Height = 30,
            Child = new TextBlock
            {
                Text = icon,
                FontSize = 14,
                FontWeight = FontWeight.SemiBold,
                Foreground = new SolidColorBrush(Color.Parse(fgColor)),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            }
        };
    }

    private Control BuildWidgetStat(string label, string value, string colorHex)
    {
        return new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 4,
            Children = {
                new TextBlock
                {
                    Text = $"{label}:",
                    FontSize = 8,
                    Foreground = new SolidColorBrush(Color.FromArgb(180, 180, 180, 200)),
                    Width = 32,
                    FontFamily = new Avalonia.Media.FontFamily("Segoe UI")
                },
                new TextBlock
                {
                    Text = value,
                    FontSize = 8,
                    FontWeight = FontWeight.SemiBold,
                    Foreground = new SolidColorBrush(Color.Parse(colorHex)),
                    FontFamily = new Avalonia.Media.FontFamily("Segoe UI Semibold")
                }
            }
        };
    }

    private void PlayPause_Click(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.Child is TextBlock textBlock)
        {
            _isPlaying = !_isPlaying;
            textBlock.Text = _isPlaying ? "⏸" : "▶";
        }
        e.Handled = true;
    }

    private void UpdateActiveTabHighlight()
    {
        for (int i = 0; i < _dashboardTabs.Count; i++)
        {
            bool isActive = (i == _currentDashboardIndex);
            _dashboardTabs[i].Opacity = isActive ? 1.0 : 0.5;
            _dashboardTabs[i].BorderBrush = isActive
                ? new SolidColorBrush(Color.FromRgb(0, 183, 255))
                : new SolidColorBrush(Color.FromArgb(60, 180, 180, 200));
        }
    }

    private void OnWindowPointerEntered(object? sender, PointerEventArgs e)
    {
        _hoveredOverWindow = true;
        if (!_isExpanded)
        {
            ExpandIsland();
        }
    }

    private void OnWindowPointerExited(object? sender, PointerEventArgs e)
    {
        _hoveredOverWindow = false;

        if (!_isExpanded) return;

        Dispatcher.UIThread.Post(() =>
        {
            if (!_hoveredOverWindow && _isExpanded)
            {
                CollapseIsland();
            }
        }, DispatcherPriority.Background);
    }

    public void ExpandIsland()
    {
        if (_isExpanded) return;
        _isExpanded = true;

        _dateTextBlock.IsVisible = true;
        _songTitleTextBlock.IsVisible = true;
        _artistTextBlock.IsVisible = true;
        _dashboardsContainer.IsVisible = true;
        _scrollContainer.IsVisible = true;
        _musicControlsPanel.IsVisible = _showMusicControls;
        _dashboardTabsContainer.IsVisible = true;
        _lyricsTextBlock.IsVisible = _showLyrics;

        this.Height = ExpandedHeight;
    }

    public void CollapseIsland()
    {
        if (!_isExpanded) return;
        _isExpanded = false;

        _dateTextBlock.IsVisible = false;
        _songTitleTextBlock.IsVisible = false;
        _artistTextBlock.IsVisible = false;
        _dashboardsContainer.IsVisible = false;
        _scrollContainer.IsVisible = false;
        _musicControlsPanel.IsVisible = false;
        _dashboardTabsContainer.IsVisible = false;
        _lyricsTextBlock.IsVisible = false;

        this.Height = CompactHeight;
    }

    public void ToggleIsland()
    {
        if (_isExpanded)
            CollapseIsland();
        else
            ExpandIsland();
    }

    private void InitializeComponent()
    {
        // No XAML - we build UI programmatically
    }

    private void InitializeTimer()
    {
        // Main update timer (1 second interval)
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        timer.Tick += (s, e) =>
        {
            CurrentTime = DateTime.Now.ToString("h:mm:ss tt");
            _timeTextBlock.Text = CurrentTime;

            // Simulate system stats updates
            _cpuUsage = (_cpuUsage + 0.5) % 100;
            _ramUsage = 8.0 + (_ramUsage * 0.01);
            if (_ramUsage > 16.0) _ramUsage = 8.0;
            _diskUsage = (_diskUsage + 0.3) % 100;
        };
        timer.Start();

        // Lyrics line update timer (simulated live lyrics)
        var lyricsTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(3000) };
        lyricsTimer.Tick += (s, e) =>
        {
            if (_isExpanded && _showLyrics)
            {
                UpdateLiveLyrics();
            }
        };
        lyricsTimer.Start();
    }

    private void UpdateLiveLyrics()
    {
        var lyricsLines = new[]
        {
            "I'm on my way, I'm on my way",
            "To the place where we both belong",
            "I'm on my way, I'm on my way",
            "With you, with you, all night long",
            "Oh-oh, oh-oh, oh-oh",
            "Oh-oh, oh-oh, oh-oh",
            "I'm on my way"
        };
        _currentLyrics = lyricsLines[new Random().Next(lyricsLines.Length)];
        _lyricsTextBlock.Text = _currentLyrics;
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        PositionWindow();
        this.IsVisible = true;
        this.Topmost = true;
    }

    private void PositionWindow()
    {
        try
        {
            var primaryScreen = this.Screens.Primary;
            if (primaryScreen != null)
            {
                var workArea = primaryScreen.WorkingArea;
                double x = Math.Max(0, (workArea.Width - this.Bounds.Width) / 2);
                this.Position = new PixelPoint((int)x, 20);
                return;
            }
        }
        catch { }
        this.Position = new PixelPoint(0, 20);
    }

    // Public API for external integrations
    public void SetMediaInfo(string title, string artist = "", double volume = 70.0)
    {
        _currentSong = title ?? "Not Playing";
        _currentArtist = artist ?? "";
        _volume = volume;
        _songTitleTextBlock.Text = _currentSong;
        _artistTextBlock.Text = _currentArtist;
        _songTitleTextBlock.IsVisible = !string.IsNullOrEmpty(title);
        _artistTextBlock.IsVisible = !string.IsNullOrEmpty(artist);
    }

    public void SetLyricsVisible(bool visible)
    {
        _showLyrics = visible;
        _lyricsTextBlock.IsVisible = visible;
    }

    public void SetCurrentLyrics(string lyrics)
    {
        _currentLyrics = lyrics ?? "";
        _lyricsTextBlock.Text = _currentLyrics;
    }

    public void UpdateSystemStats(double cpu, double ram, double disk)
    {
        _cpuUsage = cpu;
        _ramUsage = ram;
        _diskUsage = disk;
    }

    public void TogglePlayPause()
    {
        _isPlaying = !_isPlaying;
        if (_playPauseButton?.Child is TextBlock textBlock)
        {
            textBlock.Text = _isPlaying ? "⏸" : "▶";
        }
    }

    public void NextTrack()
    {
        // Simulate next track
        _currentSong = "Next Song";
        _currentArtist = "Next Artist";
        _songTitleTextBlock.Text = _currentSong;
    }

    public void PreviousTrack()
    {
        // Simulate previous track
        _currentSong = "Previous Song";
        _currentArtist = "Previous Artist";
        _songTitleTextBlock.Text = _currentSong;
    }
}