using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using System;

namespace TopNotch.Views.Controls;

/// <summary>
/// Settings card control for grouping settings
/// </summary>
public partial class SettingsCard : UserControl
{
    public SettingsCard()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<SettingsCard, string>(nameof(Header), string.Empty, OnHeaderChanged);

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    private static void OnHeaderChanged(Avalonia.AvaloniaObject d, AvaloniaProperty e, object? old, object? newvalue)
    {
        var control = (SettingsCard)d;
        var headerText = control.FindControl<TextBlock>("HeaderText");
        if (headerText != null)
        {
            headerText.Text = newvalue?.ToString() ?? string.Empty;
        }
    }
}