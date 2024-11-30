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

    public static IResourceProvider GetLang(this Application app, string cultureName)
    {
        foreach (var resourceProvider in app.Resources.MergedDictionaries)
        {
            if (!resourceProvider.TryGetResource("Lang.CultureName", null, out var value))
            {
                continue;
            }

            if (value?.ToString() == cultureName)
            {
                return resourceProvider;
            }
        }

        throw new($"Lang.CultureName not found for culture {cultureName}");
    }
}