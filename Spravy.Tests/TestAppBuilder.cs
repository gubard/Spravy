using Avalonia;
using Avalonia.Headless;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Configuration;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Tests;
using Spravy.Ui;
using Spravy.Ui.Configurations;
using Spravy.Ui.Desktop.Configurations;
using Spravy.Ui.Extensions;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace Spravy.Tests;

public class TestAppBuilder
{
    public static readonly IConfiguration Configuration;

    static TestAppBuilder()
    {
        Configuration = new ConfigurationBuilder().AddJsonFile("testsettings.json")
            .AddEnvironmentVariables("Spravy_")
            .AddCommandLine(Environment.GetCommandLineArgs())
            .Build();
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        DiHelper.Kernel = new StandardKernel(new UiModule(true), DesktopModule.Default);

        return AppBuilder.Configure<App>()
            .UseReactiveUI()
            .UseSkia()
            .UseHeadless(
                new AvaloniaHeadlessPlatformOptions
                {
                    UseHeadlessDrawing = false
                }
            )
            .WithShantellSansFont()
            .WithInterFont();
    }
}