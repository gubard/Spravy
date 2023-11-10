﻿using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.ReactiveUI;
using Ninject;
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
        await JSHost.ImportAsync("localStorage.js", "./localStorage.js");
        await JSHost.ImportAsync("window.js", "./window.js");
        DiHelper.Kernel = new StandardKernel(BrowserModule.Default, new UiModule(false));

        await BuildAvaloniaApp()
            .WithInterFont()
            .UseReactiveUI()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure(() => DiHelper.Kernel.ThrowIfNull().Get<Application>())
            .UseReactiveUI();
    }
}