using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui;

public partial class App : Application
{
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