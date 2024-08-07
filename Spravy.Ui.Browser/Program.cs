using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Browser;
using Serilog;
using Spravy.Core.Helpers;
using Spravy.Ui.Browser.Modules;
using Spravy.Ui.Extensions;

[assembly: SupportedOSPlatform("browser")]

namespace Spravy.Ui.Browser;

internal class Program
{
    private static async Task Main()
    {
        DiHelper.ServiceFactory = new BrowserServiceProvider();
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        try
        {
            Log.Information("Starting web app");
            await JSHost.ImportAsync("localStorage.js", "./localStorage.js");
            await JSHost.ImportAsync("window.js", "./window.js");
            await BuildAvaloniaApp().WithInterFont().StartBrowserAppAsync("out");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder
            .Configure(() => DiHelper.ServiceFactory.CreateService<App>())
            .WithInterFont()
            .WithShantellSansFont();
    }
}
