using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Ninject;
using ReactiveUI;
using Splat;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Configurations;
using Spravy.Ui.Desktop.Configurations;

namespace Spravy.Ui.Desktop;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        DiHelper.Kernel = new StandardKernel(UiModule.Default, DesktopModule.Default);

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure(() => DiHelper.Kernel.ThrowIfNull().Get<Application>())
            .UsePlatformDetect()
            .UseReactiveUI()
            .AfterSetup(
                _ => Locator.CurrentMutable.RegisterLazySingleton(
                    () => DiHelper.Kernel.ThrowIfNull().Get<IViewLocator>(),
                    typeof(IViewLocator)
                )
            );
    }
}