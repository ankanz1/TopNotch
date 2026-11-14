using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Animation;
using System;
using System.Threading.Tasks;

namespace TopNotch.Views.Windows;

public partial class IslandWindow : Window
{
    private const double CompactHeight = 40.0;
    private const double ExpandedHeight = 200.0;

    private bool _isExpanded = false;
    private DateTime _lastInteraction = DateTime.Now;

    public IslandWindow()
    {
        InitializeComponent();
        InitializeTimer();
    }

    public string CurrentTime { get; set; } = DateTime.Now.ToString("h:mm tt");

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InitializeTimer()
    {
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1)
        };
        timer.Tick += (s, e) => CurrentTime = DateTime.Now.ToString("h:mm tt");
        timer.Start();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        PositionWindow();
    }

    private void PositionWindow()
    {
        // Position at top center of primary screen
        if (Screen.AllScreens.Count > 0)
        {
            var primaryScreen = Screen.AllScreens[0];
            var workArea = primaryScreen.WorkArea;

            // Center horizontally, position at top
            X = (workArea.Width - Width) / 2;
            Y = workArea.Top + 20;
        }
    }

    protected override void OnPointerEntered(Avalonia.Input.PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        _lastInteraction = DateTime.Now;
        _ = Task.Run(async () => await ExpandIslandAsync());
    }

    protected override void OnPointerExited(Avalonia.Input.PointerEventArgs e)
    {
        base.OnPointerExited(e);
        _lastInteraction = DateTime.Now;
    }

    protected override void OnPointerPressed(Avalonia.Input.PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _lastInteraction = DateTime.Now;
        _ = Task.Run(async () => await ToggleIslandAsync());
        e.Handled = true;
    }

    public async Task ExpandIslandAsync()
    {
        if (_isExpanded) return;

        _isExpanded = true;

        if (FindName("ExpandedContent") is Grid expandedContent)
        {
            expandedContent.IsVisible = true;
        }

        var animation = new DoubleAnimation
        {
            Duration = TimeSpan.FromMilliseconds(300),
            To = ExpandedHeight
        };

        this.ApplyAnimation(Window.HeightProperty, animation);
        Height = ExpandedHeight;
    }

    public async Task CollapseIslandAsync()
    {
        if (!_isExpanded) return;

        _isExpanded = false;

        if (FindName("ExpandedContent") is Grid expandedContent)
        {
            expandedContent.IsVisible = false;
        }

        var animation = new DoubleAnimation
        {
            Duration = TimeSpan.FromMilliseconds(300),
            To = CompactHeight
        };

        this.ApplyAnimation(Window.HeightProperty, animation);
        Height = CompactHeight;
    }

    public async Task ToggleIslandAsync()
    {
        if (_isExpanded)
            await CollapseIslandAsync();
        else
            await ExpandIslandAsync();
    }

    public void UpdatePosition() => PositionWindow();
    public DateTime LastInteraction => _lastInteraction;
    public bool IsExpanded => _isExpanded;
}