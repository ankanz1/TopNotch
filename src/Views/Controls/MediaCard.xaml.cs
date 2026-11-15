using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TopNotch.Models;

namespace TopNotch.Views.Controls;

/// <summary>
/// Media card control for displaying now playing information
/// </summary>
public partial class MediaCard : UserControl
{
    public MediaCard()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}