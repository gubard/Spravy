using Avalonia;
using Avalonia.Headless;
using Avalonia.ReactiveUI;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Tests;
using Spravy.Ui;
using Spravy.Ui.Configurations;
using Spravy.Ui.Desktop.Configurations;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace Spravy.Tests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
    {
        DiHelper.Kernel = new StandardKernel(new UiModule(true), DesktopModule.Default);

        return AppBuilder.Configure<App>()
            .UseReactiveUI()
            .UseHeadless(
                new AvaloniaHeadlessPlatformOptions
                {
                    UseHeadlessDrawing = false
                }
            )
            .UseSkia();
    }
}