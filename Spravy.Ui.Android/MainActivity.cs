using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using Ninject;
using ReactiveUI;
using Splat;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Android.Configurations;
using Spravy.Ui.Configurations;

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
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseReactiveUI()
            .AfterSetup(
                _ => Locator.CurrentMutable.RegisterLazySingleton(
                    () => DiHelper.Kernel.ThrowIfNull().Get<IViewLocator>(),
                    typeof(IViewLocator)
                )
            );
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        DiHelper.Kernel = new StandardKernel(new UiModule(true), AndroidModule.Default);
        base.OnCreate(savedInstanceState);
    }
}