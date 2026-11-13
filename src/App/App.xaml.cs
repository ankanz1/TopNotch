using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;

namespace TopNotch;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Create and show the island window
            var islandWindow = new Views.Windows.IslandWindow();
            islandWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}