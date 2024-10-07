using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using Spravy.Core.Helpers;

namespace Spravy.Integration.Tests;

public class TestAppBuilder
{
    public static readonly IConfiguration Configuration;

    static TestAppBuilder()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json")
            .AddEnvironmentVariables("Spravy_")
            .AddCommandLine(Environment.GetCommandLineArgs())
            .Build();
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        DiHelper.ServiceFactory = new TestServiceProvider();
        IconProvider.Current.Register<MaterialDesignIconProvider>();

        return AppBuilder
            .Configure<App>()
            .UseSkia()
            .UseHeadless(new() { UseHeadlessDrawing = false, })
            .WithShantellSansFont()
            .WithInterFont();
    }
}
