using Spravy.Ui.Features.PasswordGenerator.Interfaces;
using Spravy.Ui.Features.PasswordGenerator.Services;
using Spravy.Ui.Features.ToDo.Interfaces;
using Spravy.Ui.Features.ToDo.Services;

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
        Bind<IPropertyValidator>().To<PropertyValidator>();
        Bind<StorageDbContext>().ToMethod(c => new(c.Kernel.GetRequiredService<IDbContextSetup>()));

        this.BindGrpcService<GrpcAuthenticationService, AuthenticationService.AuthenticationServiceClient,
            GrpcAuthenticationServiceOptions>(useCache);

        this.BindGrpcServiceAuth<GrpcScheduleService, ScheduleService.ScheduleServiceClient,
            GrpcScheduleServiceOptions>(useCache);

        this.BindGrpcServiceAuth<GrpcToDoService, ToDoService.ToDoServiceClient, GrpcToDoServiceOptions>(useCache);

        this.BindGrpcServiceAuth<GrpcEventBusService, EventBusService.EventBusServiceClient,
            GrpcEventBusServiceOptions>(useCache);

        this.BindGrpcServiceAuth<GrpcPasswordService, PasswordService.PasswordServiceClient,
            GrpcPasswordServiceOptions>(useCache);

        Bind<IManagedNotificationManager>()
           .ToMethod(_ => new WindowNotificationManager(Application.Current.ThrowIfNull().GetTopLevel()));

        Bind<AccountNotify>().ToSelf().InSingletonScope();
        Bind<IErrorHandler>().To<ErrorHandler>();
        Bind<ISerializer>().To<ProtobufSerializer>();
        Bind<IConverter>().To<AutoMapperConverter>();
        Bind<ISpravyNotificationManager>().To<NotificationManager>();
        Bind<ICacheValidator<Uri, GrpcChannel>>().To<GrpcChannelCacheValidator>();
        Bind<IViewLocator>().To<ModuleViewLocator>();
        Bind<INavigator>().To<Navigator>().InSingletonScope();
        Bind<IMapper>().ToMethod(context => new Mapper(context.Kernel.Get<MapperConfiguration>()));
        Bind<IToDoService>().ToMethod(x => x.Kernel.Get<GrpcToDoService>());
        Bind<Application>().To<App>();
        Bind<IResourceLoader>().To<FileResourceLoader>();
        Bind<DailyPeriodicityViewModel>().ToSelf();
        Bind<ITokenService>().To<TokenService>().InSingletonScope();
        Bind<IAuthenticationService>().ToMethod(context => context.Kernel.Get<GrpcAuthenticationService>());
        Bind<IPasswordService>().ToMethod(context => context.Kernel.Get<GrpcPasswordService>());
        Bind<IScheduleService>().ToMethod(context => context.Kernel.Get<GrpcScheduleService>());
        //Bind<IKeeper<TokenResult>>().To<StaticKeeper<TokenResult>>();
        Bind<IKeeper<Guid>>().To<StaticKeeper<Guid>>();
        Bind<ToDoItemViewModel>().ToSelf();
        Bind<Control>().To<MainView>().OnActivation((c, x) => x.DataContext = c.Kernel.Get<MainViewModel>());
        //RegisterViewModels(this)
        Bind<MapperConfiguration>().ToMethod(_ => new(SetupMapperConfiguration));
        Bind<IEventBusService>().ToMethod(context => context.Kernel.Get<GrpcEventBusService>());
        Bind<IDataTemplate>().To<ModuleDataTemplate>();
        Bind<IMetadataFactory>().To<MetadataFactory>();
        Bind<TokenHttpHeaderFactory>().To<TokenHttpHeaderFactory>();
        Bind<TimeZoneHttpHeaderFactory>().To<TimeZoneHttpHeaderFactory>();
        Bind<IDialogViewer>().To<DialogViewer>();
        Bind<LeafToDoItemsViewModel>().ToSelf();
        Bind<LeafToDoItemsView>().ToSelf();
        Bind<IContent>().ToMethod(x => x.Kernel.Get<MainSplitViewModel>());
        Bind<MainProgressBarViewModel>().ToSelf().InSingletonScope();
        Bind<ITaskProgressService>().To<TaskProgressService>().InSingletonScope();
        Bind<IUiApplicationService>().To<UiApplicationService>().InSingletonScope();
        Bind<IPasswordItemCache>().To<PasswordItemCache>().InSingletonScope();
        Bind<IToDoCache>().To<ToDoCache>().InSingletonScope();
        
        Bind<AvaloniaList<DayOfYearSelectItem>>()
           .ToMethod(context => new(Enumerable.Range(1, 12)
               .Select(x =>
                {
                    var item = context.Kernel.Get<DayOfYearSelectItem>();
                    item.Month = (byte)x;

                    return item;
                })));

        Bind<IHttpHeaderFactory>()
           .ToMethod(context => new CombineHttpHeaderFactory(context.Kernel.Get<TokenHttpHeaderFactory>(),
                context.Kernel.Get<TimeZoneHttpHeaderFactory>()));

        Bind<IClipboardService>()
           .ToMethod(_ =>
            {
                var topLevel = Application.Current.ThrowIfNull("Application").GetTopLevel();

                if (topLevel is null)
                {
                    return new CodeClipboardService();
                }

                return new TopLevelClipboardService();
            });

        Bind<MainSplitViewModel>()
           .ToSelf()
           .InSingletonScope()
           .OnActivation((c, x) =>
            {
                var login = c.Kernel.Get<LoginViewModel>();
                login.TryAutoLogin = true;
                x.Pane = c.Kernel.Get<PaneViewModel>();
                x.Content = login;
            });

        Bind<IDesktopTopLevelControl>()
           .To<MainWindow>()
           .InSingletonScope()
           .OnActivation((c, x) => { x.Content = c.Kernel.Get<Control>(); });

        Bind<ISingleViewTopLevelControl>()
           .To<SingleView>()
           .InSingletonScope()
           .OnActivation((c, x) => { x.Content = c.Kernel.Get<Control>(); });
    }

    public static void SetupMapperConfiguration(IMapperConfigurationExpression expression)
    {
        expression.AddProfile<SpravyToDoProfile>();
        expression.AddProfile<SpravyUiProfile>();
        expression.AddProfile<SpravyAuthenticationProfile>();
        expression.AddProfile<SpravyScheduleProfile>();
        expression.AddProfile<SpravyEventBusProfile>();
        expression.AddProfile<SpravyPasswordGeneratorProfile>();
    }
}