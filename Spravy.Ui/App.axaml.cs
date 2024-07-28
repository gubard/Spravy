using Avalonia.Input;
using Avalonia.Layout;
using Spravy.Core.Helpers;

namespace Spravy.Ui;

public class App : Application
{
    public const string RootToDoItemButtonName = "root-to-do-item-button";

    static App()
    {
        DialogHost.IsOpenProperty.Changed.AddClassHandler<DialogHost>(
            (control, _) => control.Classes.As<IPseudoClasses>()?.Set(":is-open", control.IsOpen)
        );
    }

    private readonly IServiceFactory serviceFactory = DiHelper.ServiceFactory;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        var dataTemplates = serviceFactory.CreateService<IEnumerable<IDataTemplate>>();
        DataTemplates.AddRange(dataTemplates);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = serviceFactory
                .CreateService<IDesktopTopLevelControl>()
                .As<Window>()
                .ThrowIfNull();

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
