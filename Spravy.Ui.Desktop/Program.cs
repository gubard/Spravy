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
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        DiHelper.Kernel = new StandardKernel(new UiModule(true), DesktopModule.Default);

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