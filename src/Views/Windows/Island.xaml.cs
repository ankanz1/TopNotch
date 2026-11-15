using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace TopNotch.Views.Windows;

/// <summary>
/// Interaction logic for Island.xaml
/// </summary>
public partial class Island : UserControl
{
    public Island()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static readonly StyledProperty<object?> CurrentViewProperty =
        AvaloniaProperty.Register<Island, object?>(nameof(CurrentView), null);

    public object? CurrentView
    {
        get => GetValue(CurrentViewProperty);
        set => SetValue(CurrentViewProperty, value);
    }

    public static readonly StyledProperty<DateTime> CurrentTimeProperty =
        AvaloniaProperty.Register<Island, DateTime>(nameof(CurrentTime), DateTime.Now);

    public DateTime CurrentTime
    {
        get => GetValue(CurrentTimeProperty);
        set => SetValue(CurrentTimeProperty, value);
    }
}