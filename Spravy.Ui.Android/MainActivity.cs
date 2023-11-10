using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Ui.Android.Configurations;
using Spravy.Ui.Configurations;

namespace Spravy.Ui.Android;

[Activity(
    Label = "Spravy",
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
            .UseReactiveUI();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        DiHelper.Kernel = new StandardKernel(new UiModule(true), new AndroidModule(this));
        base.OnCreate(savedInstanceState);
    }
}