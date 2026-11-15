using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System;

namespace TopNotch;

class Program
{
    // Entry point - Avalonia application bootstrapping
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration - don't forget to call .UsePlatformDetect()
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}