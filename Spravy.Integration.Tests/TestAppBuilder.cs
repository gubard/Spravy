using Avalonia;
using Avalonia.Headless;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Configuration;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Ui;
using Spravy.Ui.Configurations;
using Spravy.Ui.Desktop.Configurations;
using Spravy.Ui.Extensions;

namespace Spravy.Integration.Tests;

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
           .UseHeadless(new()
            {
                UseHeadlessDrawing = false,
            })
           .WithShantellSansFont()
           .WithInterFont();
    }
}