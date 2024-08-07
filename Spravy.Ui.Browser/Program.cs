using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Browser;
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
        await JSHost.ImportAsync("localStorage.js", "./localStorage.js");
        await JSHost.ImportAsync("window.js", "./window.js");
        await BuildAvaloniaApp().WithInterFont().StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder
            .Configure(() => DiHelper.ServiceFactory.CreateService<App>())
            .WithInterFont()
            .WithShantellSansFont();
    }
}
