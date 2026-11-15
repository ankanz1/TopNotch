using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            var islandWindow = new Views.Windows.IslandWindow();
            islandWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }
}