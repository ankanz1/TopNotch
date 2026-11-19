using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;

namespace TopNotch;

public partial class App : Application
{
    public override void Initialize()
    {
        try
        {
            AvaloniaXamlLoader.Load(this);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"XAML load failed: {ex.Message}");
            Console.WriteLine($"This is expected in Avalonia 12 without AvaloniaResource items.");
            Console.WriteLine($"The app will run with default styling.");
        }
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