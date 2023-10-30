using System;
using AutoMapper;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.ReactiveUI;
using Grpc.Net.Client;
using Ninject;
using Ninject.Modules;
using ReactiveUI;
using Spravy.Authentication.Domain.Client.Models;
using Spravy.Authentication.Domain.Client.Services;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Mapper.Profiles;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Domain.Services;
using Spravy.Authentication.Protos;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Di.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Client.Services;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Protos;
using Spravy.Schedule.Domain.Client.Models;
using Spravy.Schedule.Domain.Client.Services;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Mapper.Profiles;
using Spravy.Schedule.Protos;
using Spravy.ToDo.Domain.Client.Models;
using Spravy.ToDo.Domain.Client.Services;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Mapper.Profiles;
using Spravy.ToDo.Protos;
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
    private readonly bool useCache;

    public UiModule(bool useCache)
    {
        this.useCache = useCache;
    }

    public override void Load()
    {
        this.BindGrpcService2<GrpcAuthenticationService, AuthenticationService.AuthenticationServiceClient,
            GrpcAuthenticationServiceOptions>(useCache);

        this.BindGrpcService<GrpcScheduleService, ScheduleService.ScheduleServiceClient,
            GrpcScheduleServiceOptions>(useCache);

        this.BindGrpcService<GrpcToDoService, ToDoService.ToDoServiceClient,
            GrpcToDoServiceOptions>(useCache);

        this.BindGrpcService<GrpcEventBusService, EventBusService.EventBusServiceClient,
            GrpcEventBusServiceOptions>(useCache);

        Bind<ICacheValidator<Uri, GrpcChannel>>().To<GrpcChannelCacheValidator>();
        Bind<RoutingState>().ToConstructor(_ => new RoutingState(null)).InSingletonScope();
        Bind<RoutedViewHost>().ToSelf().OnActivation((c, x) => x.Router = c.Kernel.Get<RoutingState>());
        Bind<IViewLocator>().To<ModuleViewLocator>();
        Bind<INavigator>().To<Navigator>();
        Bind<PathControl>().ToSelf();
        Bind<IMapper>().ToConstructor(x => new Mapper(x.Context.Kernel.Get<MapperConfiguration>()));
        Bind<IToDoService>().ToMethod(x => x.Kernel.Get<GrpcToDoService>());
        Bind<IExceptionViewModel>().To<ExceptionViewModel>();
        Bind<Application>().To<App>();
        Bind<IResourceLoader>().To<FileResourceLoader>();
        Bind<AnnuallyPeriodicityViewModel>().ToSelf();
        Bind<DailyPeriodicityViewModel>().ToSelf();
        Bind<MonthlyPeriodicityViewModel>().ToSelf();
        Bind<WeeklyPeriodicityViewModel>().ToSelf();
        Bind<DayOfYearSelector>().ToSelf();
        Bind<DayOfWeekSelector>().ToSelf();
        Bind<DayOfMonthSelector>().ToSelf();
        Bind<ITokenService>().To<TokenService>().InSingletonScope();
        Bind<IAuthenticationService>().ToMethod(context => context.Kernel.Get<GrpcAuthenticationService>());
        Bind<IScheduleService>().ToMethod(context => context.Kernel.Get<GrpcScheduleService>());
        Bind<IKeeper<TokenResult>>().To<StaticKeeper<TokenResult>>();
        Bind<IKeeper<Guid>>().To<StaticKeeper<Guid>>();
        Bind<Control>().To<MainView>().OnActivation((c, x) => x.DataContext = c.Kernel.Get<MainViewModel>());
        RegisterViewModels(this);
        Bind<AppConfiguration>().ToConstructor(x => new AppConfiguration(typeof(LoginViewModel)));
        Bind<MapperConfiguration>().ToConstructor(x => new MapperConfiguration(SetupMapperConfiguration));
        Bind<IEventBusService>().ToMethod(context => context.Kernel.Get<GrpcEventBusService>());
        Bind<IDataTemplate>().To<ModuleDataTemplate>();
        Bind<IMetadataFactory>().To<MetadataFactory>();
        Bind<IHttpHeaderFactory>().To<TokenHttpHeaderFactory>();
        Bind<IDialogViewer>().To<DialogViewer>();

        Bind<IClipboard>()
        .ToMethod(
                _ => Application.Current.ThrowIfNull("Application")
                .GetTopLevel()
                .ThrowIfNull("TopLevel")
                .Clipboard.ThrowIfNull()
            );

        Bind<SplitView>()
        .ToMethod(
                _ =>
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
                    CornerRadius = new(24),
                    Padding = new(4),
                    Margin = new(4),
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
    }

    public static void SetupMapperConfiguration(IMapperConfigurationExpression expression)
    {
        expression.AddProfile<SpravyToDoProfile>();
        expression.AddProfile<SpravyUiProfile>();
        expression.AddProfile<SpravyAuthenticationProfile>();
        expression.AddProfile<SpravyScheduleProfile>();
    }

    private void RegisterViewModels(NinjectModule module)
    {
        var styledElementType = typeof(StyledElement);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach(var assembly in assemblies)
        {
            var types = assembly.GetTypes();

            foreach(var type in types)
            {
                if(type.Namespace.IsNullOrWhiteSpace())
                {
                    continue;
                }

                if(!styledElementType.IsAssignableFrom(type))
                {
                    continue;
                }

                var ns = type.Namespace
                .Replace(".Views.", ".ViewModels.")
                .Replace(".Views", ".ViewModels");

                var viewModelName = $"{ns}.{type.Name}Model";
                var viewModelType = assembly.GetType(viewModelName);

                if(viewModelType is null)
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