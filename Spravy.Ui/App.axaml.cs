using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.Core.DependencyInjection.Extensions;
using ExtensionFramework.Core.DependencyInjection.Interfaces;
using ExtensionFramework.Core.DependencyInjection.Services;

namespace Spravy.Ui;

public partial class App : Application
{
    [Inject]
    public IResolver? Resolver { get; set; } = DependencyInjector.Default;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        DataTemplates.AddRange(Resolver.ThrowIfNull().Resolve<IEnumerable<IDataTemplate>>());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var resolver = Resolver.ThrowIfNull();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = resolver.Resolve<Window>();
            desktop.MainWindow = window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var control = resolver.Resolve<Control>();;
            singleViewPlatform.MainView = control;
        }

        base.OnFrameworkInitializationCompleted();
    }
}