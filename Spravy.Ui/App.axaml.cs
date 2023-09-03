using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;

namespace Spravy.Ui;

public partial class App : Application
{
    [Inject]
    public IKernel? Resolver { get; init; } = DiHelper.Kernel;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        DataTemplates.AddRange(Resolver.ThrowIfNull().Get<IEnumerable<IDataTemplate>>());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var resolver = Resolver.ThrowIfNull();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = resolver.Get<Window>();
            desktop.MainWindow = window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var control = resolver.Get<Control>();
            ;
            singleViewPlatform.MainView = control;
        }

        base.OnFrameworkInitializationCompleted();
    }
}