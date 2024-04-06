using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Ninject;
using Serilog;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Ui.Configurations;
using Spravy.Ui.Desktop.Configurations;
using Spravy.Ui.Extensions;

namespace Spravy.Ui.Desktop;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var r = Result.Success;
        var r2 = Result<object>.DefaultSuccess;
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");

            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        DiHelper.Kernel = new StandardKernel(new UiModule(true), DesktopModule.Default);

        return AppBuilder.Configure(() => DiHelper.Kernel.ThrowIfNull().Get<Application>())
            .UsePlatformDetect()
            .WithInterFont()
            .WithShantellSansFont()
            .UseReactiveUI();
    }
}