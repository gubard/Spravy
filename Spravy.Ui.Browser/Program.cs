using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.ReactiveUI;
using Ninject;
using ReactiveUI;
using Splat;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Browser.Configurations;
using Spravy.Ui.Configurations;

[assembly: SupportedOSPlatform("browser")]

namespace Spravy.Ui.Browser;

internal partial class Program
{
    private static async Task Main(string[] args)
    {
        DiHelper.Kernel = new StandardKernel(UiModule.Default, BrowserModule.Default);

        await BuildAvaloniaApp()
            .WithInterFont()
            .UseReactiveUI()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure(() => DiHelper.Kernel.ThrowIfNull().Get<Application>())
            .UseReactiveUI()
            .AfterSetup(
                _ => Locator.CurrentMutable.RegisterLazySingleton(
                    () => DiHelper.Kernel.ThrowIfNull().Get<IViewLocator>(),
                    typeof(IViewLocator)
                )
            );
        ;
    }
}