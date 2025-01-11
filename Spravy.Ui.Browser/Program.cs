using Avalonia;
using Avalonia.Browser;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using Serilog;
using Spravy.Ui.Browser.Modules;

[assembly: SupportedOSPlatform("browser")]

namespace Spravy.Ui.Browser;

internal class Program
{
    private static async Task Main()
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        try
        {
            DiHelper.ServiceFactory = new BrowserServiceProvider();
            await JSHost.ImportAsync("localStorage.js", "./../localStorage.js");
            await JSHost.ImportAsync("window.js", "./../window.js");
            await JSHost.ImportAsync("audio.js", "./../audio.js");
            await BuildAvaloniaApp().StartBrowserAppAsync("out");
        }
        catch (Exception e)
        {
            Log.Logger.Fatal(e, "Application terminated unexpectedly");

            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current.Register<MaterialDesignIconProvider>();

        return AppBuilder.Configure(() => DiHelper.ServiceFactory.CreateService<App>())
           .WithInterFont()
           .WithJetBrainsMonoFont();
    }
}