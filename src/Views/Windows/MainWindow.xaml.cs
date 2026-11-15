using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TopNotch.Views.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}