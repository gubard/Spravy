using Avalonia;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using Serilog;
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
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Logger.Fatal(e, "Application terminated unexpectedly");

            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        DiHelper.ServiceFactory = new DesktopServiceProvider();
        IconProvider.Current.Register<MaterialDesignIconProvider>();

        return AppBuilder.Configure(() => DiHelper.ServiceFactory.ThrowIfNull().CreateService<App>())
           .UsePlatformDetect()
           .WithInterFont()
           .WithJetBrainsMonoFont();
    }
}