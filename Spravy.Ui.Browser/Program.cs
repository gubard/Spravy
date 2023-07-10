using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.ReactiveUI;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.Graph.Extensions;
using ExtensionFramework.Core.Graph.Services;
using ExtensionFramework.Core.ModularSystem.Extensions;
using ExtensionFramework.Core.ModularSystem.Interfaces;
using ExtensionFramework.Core.ModularSystem.Services;
using Spravy.Ui.Browser.Modules;
using Spravy.Ui.Modules;

[assembly: SupportedOSPlatform("browser")]

namespace Spravy.Ui.Browser;

internal partial class Program
{
    private static IModule? module;

    private static async Task Main(string[] args)
    {
        await BuildAvaloniaApp()
            .WithInterFont()
            .UseReactiveUI()
            .StartBrowserAppAsync("out");
    }

    private static void InitModules()
    {
        var builder = new TreeBuilder<Guid, IModule>().SetRoot(
            new TreeNodeBuilder<Guid, IModule>()
                .SetKey(SpravyBrowserModule.IdValue)
                .SetValue(new SpravyBrowserModule())
                .Add(SpravyModule.IdValue, new SpravyModule())
        );

        var moduleTree = new ModuleTree(builder.Build());
        moduleTree.SetupModule();
        module = moduleTree;
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        InitModules();

        return AppBuilder.Configure(() => module.ThrowIfNull().GetObject<Application>())
#if DEBUG
            .LogToTrace()
#endif
            .UseReactiveUI();
    }
}