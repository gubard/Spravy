using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Interfaces;
using ExtensionFramework.Core.Graph.Extensions;
using ExtensionFramework.Core.Graph.Services;
using ExtensionFramework.Core.ModularSystem.Interfaces;
using ExtensionFramework.Core.ModularSystem.Services;
using ExtensionFramework.Core.ModularSystem.Extensions;
using Spravy.Android.Modules;
using Spravy.Modules;

namespace Spravy.Android;

[Activity(
    Label = "Spravy.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode
)]
public class MainActivity : AvaloniaMainActivity<App>
{
    private static IModule? module;
    
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
#if DEBUG
            .LogToTrace()
#endif
            .UseReactiveUI();
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        InitModules();
        Avalonia.Application.Current.As<App>().Resolver = module.GetObject<IResolver>();
    }
    
    private static void InitModules()
    {
        var builder = new TreeBuilder<Guid, IModule>().SetRoot(
            new TreeNodeBuilder<Guid, IModule>()
                .SetKey(AndroidModules.IdValue)
                .SetValue(new AndroidModules())
                .Add(SpravyModule.IdValue, new SpravyModule())
        );

        var moduleTree = new ModuleTree(builder.Build());
        module = moduleTree;
        moduleTree.SetupModule();
    }
}