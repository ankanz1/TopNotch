using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using System;

namespace TopNotch;

public partial class App : Application
{
    public override void Initialize()
    {
        // Load styles programmatically since XAML resources aren't precompiled
        this.Styles.Add(new FluentTheme());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Console.WriteLine("OnFrameworkInitializationCompleted called");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Console.WriteLine("Creating IslandWindow...");
            var islandWindow = new Views.Windows.IslandWindow();

            // Bind DataContext to self so bindings work in programmatic UI
            islandWindow.DataContext = islandWindow;
            islandWindow.Show();
            Console.WriteLine($"IslandWindow.Show() called");
        }

        base.OnFrameworkInitializationCompleted();
    }
}