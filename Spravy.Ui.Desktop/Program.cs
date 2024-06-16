using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Serilog;
using Spravy.Core.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Desktop.Modules;
using Spravy.Ui.Extensions;

namespace Spravy.Ui.Desktop;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
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
        DiHelper.ServiceFactory = new DesktopServiceProvider();
        
        return AppBuilder.Configure(() => DiHelper.ServiceFactory.ThrowIfNull().CreateService<App>())
           .UsePlatformDetect()
           .WithInterFont()
           .WithShantellSansFont()
           .UseReactiveUI();
    }
}