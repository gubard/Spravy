namespace Spravy.Ui;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        var dataTemplates = DiHelper.ServiceFactory.CreateService<IEnumerable<IDataTemplate>>();
        DataTemplates.AddRange(dataTemplates);
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        var objectStorage = DiHelper.ServiceFactory.CreateService<IObjectStorage>();
        
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
            var window = DiHelper.ServiceFactory.CreateService<IDesktopTopLevelControl>().As<Window>().ThrowIfNull();
            desktop.MainWindow = window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var control = DiHelper.ServiceFactory.CreateService<ISingleViewTopLevelControl>().As<Control>();
            
            singleViewPlatform.MainView = control;
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}