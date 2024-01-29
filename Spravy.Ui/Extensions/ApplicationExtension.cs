using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;

namespace Spravy.Ui.Extensions;

public static class ApplicationExtension
{
    public static TopLevel? GetTopLevel(this Application app)
    {
        if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        if (app.ApplicationLifetime is ISingleViewApplicationLifetime viewApp)
        {
            var visualRoot = viewApp.MainView?.GetVisualRoot();

            return visualRoot as TopLevel;
        }

        return null;
    }

    public static object? GetResource(this Application app, string key)
    {
        app.TryGetResource(key, out var value);

        return value;
    }
}