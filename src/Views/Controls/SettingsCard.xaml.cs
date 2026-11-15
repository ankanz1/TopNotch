using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
}