using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TopNotch.Views.Windows;

public partial class Settings : UserControl
{
    public Settings()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}