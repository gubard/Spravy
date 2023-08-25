using System;

using Android.App;
using Android.Content.PM;
using Android.OS;

using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.Ui.Android.Modules;
using Spravy.Ui.Modules;

namespace Spravy.Ui.Android;

[Activity(
    Label = "Spravy.Ui.Android",
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
            .UseReactiveUI();
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        InitModules();
        DependencyInjector.Default = module.GetObject<IResolver>();
        base.OnCreate(savedInstanceState);
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