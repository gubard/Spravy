using Avalonia;
using Avalonia.Browser;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using Spravy.Ui.Browser.Modules;

[assembly: SupportedOSPlatform("browser")]

namespace Spravy.Ui.Browser;

internal class Program
{
    private static async Task Main()
    {
        DiHelper.ServiceFactory = new BrowserServiceProvider();
        await JSHost.ImportAsync("localStorage.js", "./../localStorage.js");
        await JSHost.ImportAsync("window.js", "./../window.js");
        await JSHost.ImportAsync("audio.js", "./../audio.js");
        await BuildAvaloniaApp().StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current.Register<MaterialDesignIconProvider>();

        return AppBuilder
            .Configure(() => DiHelper.ServiceFactory.CreateService<App>())
            .WithInterFont()
            .WithJetBrainsMonoFont();
    }
}
