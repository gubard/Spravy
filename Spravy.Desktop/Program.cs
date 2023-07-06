using System;
using Avalonia;
using Avalonia.ReactiveUI;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.Graph.Extensions;
using ExtensionFramework.Core.Graph.Services;
using ExtensionFramework.Core.ModularSystem.Extensions;
using ExtensionFramework.Core.ModularSystem.Interfaces;
using ExtensionFramework.Core.ModularSystem.Services;
using Spravy.Desktop.Modules;
using Spravy.Modules;

namespace Spravy.Desktop;

class Program
{
    private static IModule? module;

    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static void InitModules()
    {
        var builder = new TreeBuilder<Guid, IModule>().SetRoot(
            new TreeNodeBuilder<Guid, IModule>()
                .SetKey(SpravyDesktopModule.IdValue)
                .SetValue(new SpravyDesktopModule())
                .Add(SpravyModule.IdValue, new SpravyModule())
        );

        var moduleTree = new ModuleTree(builder.Build());
        module = moduleTree;
        moduleTree.SetupModule();
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        InitModules();

        return AppBuilder.Configure(() => module.ThrowIfNull().GetObject<Application>())
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
    }
}