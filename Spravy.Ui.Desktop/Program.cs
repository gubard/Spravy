using Avalonia;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using Spravy.Core.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Desktop.Modules;
using Spravy.Ui.Extensions;

namespace Spravy.Ui.Desktop;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        DiHelper.ServiceFactory = new DesktopServiceProvider();
        IconProvider.Current.Register<MaterialDesignIconProvider>();

        return AppBuilder
            .Configure(() => DiHelper.ServiceFactory.ThrowIfNull().CreateService<App>())
            .UsePlatformDetect()
            .WithInterFont()
            .WithShantellSansFont();
    }
}
