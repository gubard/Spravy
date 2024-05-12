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
        DiHelper.Kernel = new StandardKernel(new UiModule(true), TestModule.Default);

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