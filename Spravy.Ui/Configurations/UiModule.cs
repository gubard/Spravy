using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Configuration;
using Ninject;
using Ninject.Modules;
using ReactiveUI;
using Spravy.Authentication.Domain.Core.Profiles;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.Schedule.Domain.Client.Models;
using Spravy.Schedule.Domain.Client.Services;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.ToDo.Domain.Client.Models;
using Spravy.ToDo.Domain.Client.Services;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Mapper.Profiles;
using Spravy.Ui.Controls;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Profiles;
using Spravy.Ui.Services;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;

namespace Spravy.Ui.Configurations;

public class UiModule : NinjectModule
{
    public static readonly UiModule Default = new();

    public override void Load()
    {
        Bind<RoutingState>().ToConstructor(_ => new RoutingState(null)).InSingletonScope();
        Bind<RoutedViewHost>().ToSelf().OnActivation((c, x) => x.Router = c.Kernel.Get<RoutingState>());
        Bind<IViewLocator>().To<ModuleViewLocator>();
        Bind<INavigator>().To<Navigator>();
        Bind<PathControl>().ToSelf();
        Bind<IMapper>().ToConstructor(x => new Mapper(x.Context.Kernel.Get<MapperConfiguration>()));
        Bind<IToDoService>().To<GrpcToDoService>();
        Bind<IExceptionViewModel>().To<ExceptionViewModel>();
        Bind<IEnumerable<IDataTemplate>>().ToMethod(_ => Enumerable.Empty<IDataTemplate>());
        Bind<Application>().To<App>();
        Bind<IResourceLoader>().To<FileResourceLoader>();
        Bind<AnnuallyPeriodicityViewModel>().ToSelf();
        Bind<DailyPeriodicityViewModel>().ToSelf();
        Bind<MonthlyPeriodicityViewModel>().ToSelf();
        Bind<WeeklyPeriodicityViewModel>().ToSelf();
        Bind<DayOfYearSelector>().ToSelf();
        Bind<DayOfWeekSelector>().ToSelf();
        Bind<DayOfMonthSelector>().ToSelf();
        Bind<IAuthenticationService>().To<GrpcAuthenticationService>();
        Bind<IScheduleService>().To<GrpcScheduleService>();
        Bind<IKeeper<TokenResult>>().To<StaticKeeper<TokenResult>>();
        Bind<Control>().To<MainView>().OnActivation((c, x) => x.DataContext = c.Kernel.Get<MainViewModel>());
        RegisterViewModels(this);

        Bind<IDialogViewer>()
            .ToMethod(
                x => new DialogViewer(DialogViewer.DefaultDialogIdentifier)
                {
                    Resolver = x.Kernel,
                }
            );

        Bind<IClipboard>()
            .ToMethod(
                _ => Application.Current.ThrowIfNull("Application")
                    .GetTopLevel()
                    .ThrowIfNull("TopLevel")
                    .Clipboard.ThrowIfNull()
            );

        Bind<SplitView>()
            .ToMethod(
                x =>
                {
                    var splitView = new SplitView
                    {
                        OpenPaneLength = 200,
                        PanePlacement = SplitViewPanePlacement.Left,
                    };

                    return splitView;
                }
            )
            .InSingletonScope();

        Bind<MapperConfiguration>()
            .ToConstructor(
                x => new MapperConfiguration(SetupMapperConfiguration)
            );

        Bind<Window>()
            .To<MainWindow>()
            .InSingletonScope()
            .OnActivation(
                (c, x) =>
                {
                    x.ViewModel = c.Kernel.Get<MainWindowModel>();
                    x.Content = c.Kernel.Get<Control>();
                }
            );

        Bind<IDialogProgressIndicator>()
            .ToMethod(
                _ => new DialogProgressIndicator
                {
                    CornerRadius = new CornerRadius(24),
                    Padding = new Thickness(4),
                    Margin = new Thickness(4),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Content = new ProgressBar
                    {
                        Classes =
                        {
                            "circular"
                        },
                        Height = 100,
                        Width = 100,
                        IsIndeterminate = true,
                    }
                }
            );

        Bind<GrpcToDoServiceOptions>()
            .ToMethod(
                x =>
                {
                    var options = x.Kernel.Get<IConfiguration>()
                        .GetRequiredSection(GrpcToDoServiceOptions.Section)
                        .Get<GrpcToDoServiceOptions>();

                    return options;
                }
            );

        Bind<GrpcScheduleServiceOptions>()
            .ToMethod(
                x =>
                {
                    var options = x.Kernel.Get<IConfiguration>()
                        .GetRequiredSection(GrpcScheduleServiceOptions.Section)
                        .Get<GrpcScheduleServiceOptions>();

                    return options;
                }
            );

        Bind<GrpcAuthenticationServiceOptions>()
            .ToMethod(
                x =>
                {
                    var options = x.Kernel.Get<IConfiguration>()
                        .GetRequiredSection("GrpcAuthenticationService")
                        .Get<GrpcAuthenticationServiceOptions>();

                    return options;
                }
            );


        Bind<AppConfiguration>().ToConstructor(x => new AppConfiguration(typeof(LoginViewModel)));
    }

    private static void SetupMapperConfiguration(IMapperConfigurationExpression expression)
    {
        expression.AddProfile<SpravyToDoProfile>();
        expression.AddProfile<SpravyUiProfile>();
        expression.AddProfile<SpravyAuthenticationProfile>();
    }

    private void RegisterViewModels(NinjectModule module)
    {
        var styledElementType = typeof(StyledElement);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (type.Namespace.IsNullOrWhiteSpace())
                {
                    continue;
                }

                if (!styledElementType.IsAssignableFrom(type))
                {
                    continue;
                }

                var ns = type.Namespace
                    .Replace(".Views.", ".ViewModels.")
                    .Replace(".Views", ".ViewModels");

                var viewModelName = $"{ns}.{type.Name}Model";
                var viewModelType = assembly.GetType(viewModelName);

                if (viewModelType is null)
                {
                    continue;
                }

                module.Bind(viewModelType).ToSelf();

                module.Bind(type)
                    .ToSelf()
                    .OnActivation((c, x) => ((StyledElement)x).DataContext = c.Kernel.Get(viewModelType));
            }
        }
    }
}