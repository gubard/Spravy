using System;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.ReactiveUI;
using Ninject;
using Serilog;
using Spravy.Client.Extensions;
using Spravy.Core.Services;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Ui.Browser.Configurations;
using Spravy.Ui.Configurations;
using Spravy.Ui.Extensions;

[assembly: SupportedOSPlatform("browser")]

namespace Spravy.Ui.Browser;

class Program
{
    private static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("Starting web app");
            await JSHost.ImportAsync("localStorage.js", "./localStorage.js");
            await JSHost.ImportAsync("window.js", "./window.js");
            DiHelper.Kernel = new StandardKernel(BrowserModule.Default, new UiModule(false));

            await BuildAvaloniaApp()
                .WithInterFont()
                .UseReactiveUI()
                .StartBrowserAppAsync("out");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure(() => DiHelper.Kernel.ThrowIfNull().Get<Application>())
            .UseReactiveUI()
            .WithInterFont()
            .WithShantellSansFont();
    }
}