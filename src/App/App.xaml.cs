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
        Console.WriteLine("OnFrameworkInitializationCompleted called");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Console.WriteLine("Creating IslandWindow...");
            var islandWindow = new Views.Windows.IslandWindow();
            Console.WriteLine($"IslandWindow created, starting position: {islandWindow.Position}");
            Console.WriteLine($"IslandWindow bounds: {islandWindow.Bounds}");
            islandWindow.Show();
            Console.WriteLine("IslandWindow.Show() called");
        }

        base.OnFrameworkInitializationCompleted();
    }
}