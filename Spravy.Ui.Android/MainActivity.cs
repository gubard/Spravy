using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using Serilog;
using Serilog.Core;
using Spravy.Core.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Android.Modules;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Android;

[Activity(
    Label = "Spravy",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation
        | ConfigChanges.ScreenSize
        | ConfigChanges.UiMode
)]
public class MainActivity : AvaloniaMainActivity<App>
{
    private static MainActivity? _instance;
    private INavigator? navigator;

    public static MainActivity Instance => _instance.ThrowIfNull();

    private INavigator Navigator
    {
        get => navigator ??= DiHelper.ServiceFactory.CreateService<INavigator>();
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.AndroidLog()
            .Enrich.WithProperty(Constants.SourceContextPropertyName, "Spravy")
            .CreateLogger();

        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .WithShantellSansFont()
            .UseReactiveUI();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        _instance = this;
        DiHelper.ServiceFactory = new AndroidServiceProvider();
        base.OnCreate(savedInstanceState);
    }

    protected override void OnDestroy()
    {
        Log.CloseAndFlush();
        base.OnDestroy();
    }

    public override void OnBackPressed()
    {
        HandleBackPressedAsync();
    }

    private async void HandleBackPressedAsync()
    {
        var viewModel = await Navigator.NavigateBackAsync(CancellationToken.None);

        if (viewModel.IsHasError)
        {
            base.OnBackPressed();
        }
    }
}
