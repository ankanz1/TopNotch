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

    private List<Border> _dashboardTabs = new();
    private int _currentDashboardIndex = 0;

    private bool _hoveredOverWindow = false;

    public IslandWindow()
    {
        InitializeComponent();
        BuildUI();

        this.WindowDecorations = WindowDecorations.None;
        this.Topmost = true;
        this.ShowInTaskbar = false;
        this.Background = new SolidColorBrush(Color.FromArgb(180, 20, 20, 20));
        this.Width = 220;
        this.Height = CompactHeight;

        InitializeTimer();
    }

    public string CurrentTime { get; set; } = DateTime.Now.ToString("h:mm tt");

    private void BuildUI()
    {
        // Main border - NO GLASS, NO CORNERS, SOLID BACKGROUND
        _mainBorder = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(235, 18, 18, 18)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(90, 255, 255, 255)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(0), // No rounded corners
            Padding = new Thickness(6),
        };

        var mainGrid = new Grid();
        for (int i = 0; i < 8; i++)
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Time display
        _timeTextBlock = new TextBlock
        {
            FontSize = 11,
            FontWeight = FontWeight.SemiBold,
            Foreground = new SolidColorBrush(Colors.White),
            Text = CurrentTime,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        // Date display
        _dateTextBlock = new TextBlock
        {
            FontSize = 9,
            Foreground = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)),
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
            Text = "Not Playing",
            IsVisible = false,
            MaxLines = 1,
            TextWrapping = TextWrapping.Wrap
        };

        _artistTextBlock = new TextBlock
        {
            FontSize = 9,
            Foreground = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)),
            Text = "",
            IsVisible = false
        };

        // Compact header (pill bar)
        _compactHeader = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(10, 10, 10)),
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
                    new Ellipse { Width = 6, Height = 6, Fill = new SolidColorBrush(Color.FromRgb(0, 183, 255)), VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center },
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

        // Wrap dashboard tabs in a container
        _dashboardTabsContainer = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(0), // No rounded corners
            Padding = new Thickness(8),
            IsVisible = false
        };
        _dashboardTabsContainer.Child = _dashboardsContainer;

        // Scroll container for dashboard tabs - HORIZONTAL SCROLL ENABLED
        _scrollContainer = new ScrollViewer
        {
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto, // Allow horizontal scroll
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Hidden,
            Content = _dashboardTabsContainer,
            Margin = new Thickness(0, 4, 0, 6),
            IsVisible = false
        };

        // Build dashboard tabs
        BuildDashboardTabs();

        // Play/Pause button as separate element for click handling
        var playPauseControl = BuildControlButton("⏸", "#00BCD4");
        _playPauseButton = playPauseControl as Border ?? new Border();
        // Add click handler for play/pause toggle
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

        Grid.SetRow(_musicControlsPanel, 5);
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
            // Use OS-native smooth dragging - buttery smooth at 60Hz+
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

        // Mouse wheel: scroll horizontally between dashboard tabs
        var direction = delta > 0 ? -1 : 1;
        _currentDashboardIndex = ((_currentDashboardIndex + direction) + _dashboardTabs.Count) % _dashboardTabs.Count;

        UpdateActiveTabHighlight();

        // Simple scroll to target tab
        ScrollToTargetTab();
        e.Handled = true;
    }

    private const double TabWidth = 118.0;

    private void ScrollToTargetTab()
    {
        if (_scrollContainer == null || _dashboardTabs.Count == 0 || _currentDashboardIndex < 0) return;

        // Simple scroll to tab index
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
                BuildWidgetStat("Event", "Meeting @ 2PM", "#8BC34A")
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
                BuildWidgetStat("CPU", "24%", "#F44336"),
                BuildWidgetStat("RAM", "8.2GB", "#2196F3"),
                BuildWidgetStat("Disk", "45%", "#9C27B0")
            });
            _dashboardTabs.Add(tab);
            _dashboardsContainer.Children.Add(tab);
        }

        if (_showMusicControls)
        {
            var tab = CreateTabCard("Media", new[] {
                BuildWidgetStat("Artist", "Artist Name", "#00BCD4"),
                BuildWidgetStat("Title", "Song Title", "#E91E63"),
                BuildWidgetStat("Volume", "70%", "#FF9800")
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
            Foreground = new SolidColorBrush(Colors.White)
        });

        foreach (var item in items)
            sp.Children.Add(item);

        return new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)),
            BorderBrush = new SolidColorBrush(Color.FromArgb(60, 255, 255, 255)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(0), // No rounded corners
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
            Spacing = 12,
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
            Background = new SolidColorBrush(Color.FromArgb(128, 40, 40, 40)),
            CornerRadius = new CornerRadius(0), // No rounded corners
            Width = 28,
            Height = 28,
            Child = new TextBlock
            {
                Text = icon,
                FontSize = 14,
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
                    Foreground = new SolidColorBrush(Color.FromArgb(150, 255, 255, 255)),
                    Width = 32
                },
                new TextBlock
                {
                    Text = value,
                    FontSize = 8,
                    FontWeight = FontWeight.SemiBold,
                    Foreground = new SolidColorBrush(Color.Parse(colorHex))
                }
            }
        };
    }

    private void PlayPause_Click(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.Child is TextBlock textBlock)
        {
            textBlock.Text = textBlock.Text == "⏸" ? "▶" : "⏸";
        }
        e.Handled = true;
    }

    private void UpdateActiveTabHighlight()
    {
        for (int i = 0; i < _dashboardTabs.Count; i++)
        {
            bool isActive = (i == _currentDashboardIndex);
            _dashboardTabs[i].Opacity = isActive ? 1.0 : 0.4;
            _dashboardTabs[i].BorderBrush = isActive
                ? new SolidColorBrush(Color.FromRgb(0, 183, 255))
                : new SolidColorBrush(Color.FromArgb(60, 255, 255, 255));
        }
    }

    // FIXED: Hover expansion logic - expand when mouse enters expanded area,
    // collapse only when mouse leaves entirely
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

        // Don't collapse if we're still inside the expanded area
        // Use a small delay to prevent flickering
        if (!_isExpanded) return;

        // Schedule collapse on next tick to ensure we're truly outside
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
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        timer.Tick += (s, e) =>
        {
            CurrentTime = DateTime.Now.ToString("h:mm:ss tt");
            _timeTextBlock.Text = CurrentTime;
        };
        timer.Start();
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

    public void SetMediaInfo(string title, string artist = "")
    {
        _songTitleTextBlock.Text = title ?? "Not Playing";
        _artistTextBlock.Text = artist ?? "";
        _songTitleTextBlock.IsVisible = !string.IsNullOrEmpty(title);
        _artistTextBlock.IsVisible = !string.IsNullOrEmpty(artist);
    }
}