using Spravy.Core.Helpers;

namespace Spravy.Ui;

public class App : Application
{
    private readonly IServiceFactory serviceFactory = DiHelper.ServiceFactory;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        var dataTemplates = serviceFactory.CreateService<IEnumerable<IDataTemplate>>();
        DataTemplates.AddRange(dataTemplates);
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        var objectStorage = serviceFactory.CreateService<IObjectStorage>();
        var ct = CancellationToken.None;
        
        if (objectStorage.IsExistsAsync(TypeCache<SettingModel>.Type.Name, ct).GetAwaiter().GetResult().Value)
        {
            var model = objectStorage.GetObjectAsync<SettingModel>(TypeCache<SettingModel>.Type.Name, ct)
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
            var window = serviceFactory.CreateService<IDesktopTopLevelControl>().As<Window>().ThrowIfNull();
            desktop.MainWindow = window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var control = serviceFactory.CreateService<ISingleViewTopLevelControl>().As<Control>();
            
            singleViewPlatform.MainView = control;
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}