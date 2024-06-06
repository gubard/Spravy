namespace Spravy.Ui;

public class App : Application
{
    public const string NameTextBoxName = "name-text-box";
    [Inject]
    public IKernel? Resolver { get; init; } = DiHelper.Kernel;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        var dataTemplates = Resolver.ThrowIfNull().Get<IEnumerable<IDataTemplate>>();
        DataTemplates.AddRange(dataTemplates);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var resolver = Resolver.ThrowIfNull();
        var objectStorage = resolver.Get<IObjectStorage>();

        if (objectStorage.IsExistsAsync(TypeCache<SettingModel>.Type.Name).GetAwaiter().GetResult().Value)
        {
            var model = objectStorage.GetObjectAsync<SettingModel>(TypeCache<SettingModel>.Type.Name)
               .GetAwaiter()
               .GetResult()
               .Value;

            var theme = SukiTheme.GetInstance();
            theme.ChangeColorTheme(theme.ColorThemes.Single(x => x.DisplayName == model.ColorTheme));

            theme.ChangeBaseTheme(model.BaseTheme switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                _ => throw new ArgumentOutOfRangeException(),
            });
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = resolver.Get<IDesktopTopLevelControl>().As<Window>().ThrowIfNull();
            desktop.MainWindow = window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var control = resolver.Get<ISingleViewTopLevelControl>().As<Control>().ThrowIfNull();
            singleViewPlatform.MainView = control;
        }

        base.OnFrameworkInitializationCompleted();
    }
}