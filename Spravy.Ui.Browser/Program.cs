﻿using System;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.ReactiveUI;
using Ninject;
using ReactiveUI;
using Splat;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Browser.Configurations;
using Spravy.Ui.Configurations;

[assembly: SupportedOSPlatform("browser")]

namespace Spravy.Ui.Browser;

internal partial class Program
{
    private static async Task Main()
    {
        var builder = new StringBuilder();
        builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
        builder.Append("<linker>");
        
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            builder.Append($"<assembly fullname=\"{assembly.FullName}\" preserve=\"all\" />");
        }

        builder.Append("</linker>");
        Console.WriteLine(builder.ToString());
        DiHelper.Kernel = new StandardKernel(BrowserModule.Default, new UiModule(false));

        await BuildAvaloniaApp()
            .WithInterFont()
            .UseReactiveUI()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure(() => DiHelper.Kernel.ThrowIfNull().Get<Application>())
            .UseReactiveUI()
            .AfterSetup(
                _ => Locator.CurrentMutable.RegisterLazySingleton(
                    () => DiHelper.Kernel.ThrowIfNull().Get<IViewLocator>(),
                    typeof(IViewLocator)
                )
            );
    }
}