using Avalonia.Android;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using Spravy.Ui.Android.Modules;

namespace Spravy.Ui.Android;

[Activity(
    Label = "Spravy",
    Theme = "@style/AppTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode
)]
public class MainActivity : AvaloniaMainActivity<App>
{
    private static MainActivity? _instance;
    private INavigator? navigator;

    static MainActivity()
    {
        IconProvider.Current.Register<MaterialDesignIconProvider>();
    }

    public static MainActivity Instance => _instance.ThrowIfNull();

    private INavigator Navigator
    {
        get => navigator ??= DiHelper.ServiceFactory.CreateService<INavigator>();
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder).WithInterFont().WithShantellSansFont();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        _instance = this;
        DiHelper.ServiceFactory = new AndroidServiceProvider();
        base.OnCreate(savedInstanceState);
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
            OnBackPressedDispatcher.OnBackPressed();
        }
    }
}
