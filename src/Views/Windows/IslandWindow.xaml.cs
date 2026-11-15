using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Enums;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Linq;
using System.Numerics;
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
        // Position at top center of primary screen - Avalonia 12 API
        try
        {
            // Get primary screen using Avalonia.Screens in Avalonia 12
            var primaryScreen = Avalonia.Screens.Screen.AllScreens.FirstOrDefault();
            if (primaryScreen != null)
            {
                // Use WorkArea property for screen bounds
                var workArea = primaryScreen.WorkArea;

                // Center horizontally, position at top
                double x = Math.Max(0, (workArea.Width - Bounds.Width) / 2);
                double y = Math.Max(0, workArea.Y + 20);

                // Position using Position property (Vector) - Avalonia 12
                this.Position = new Vector((float)x, (float)y);
                return;
            }
        }
        catch
        {
            // Fall through to fallback
        }

        // Fallback: position near top-left
        this.Position = new Vector(0, 20);
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        _lastInteraction = DateTime.Now;
        _ = Task.Run(async () => await ExpandIslandAsync());
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        _lastInteraction = DateTime.Now;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
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

        if (GetTemplateChild("ExpandedContent") is Control expandedContent)
        {
            expandedContent.Visibility = Visibility.Visible;
        }

        // Simple height change
        Height = ExpandedHeight;
    }

    public async Task CollapseIslandAsync()
    {
        if (!_isExpanded) return;

        _isExpanded = false;

        if (GetTemplateChild("ExpandedContent") is Control expandedContent)
        {
            expandedContent.Visibility = Visibility.Collapsed;
        }

        // Simple height change
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