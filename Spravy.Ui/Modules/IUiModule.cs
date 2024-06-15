using Jab;
using Spravy.Authentication.Domain.Client.Modules;
using Spravy.Client.Modules;
using Spravy.ToDo.Domain.Client.Modules;

namespace Spravy.Ui.Modules;

[ServiceProviderModule]
[Import(typeof(IAuthenticationClientModule))]
[Import(typeof(IClientModule))]
[Import(typeof(IToDoClientModule))]
[Singleton(typeof(AccountNotify))]
[Singleton(typeof(INavigator), typeof(Navigator))]
[Singleton(typeof(ITaskProgressService), typeof(TaskProgressService))]
[Singleton(typeof(IUiApplicationService), typeof(UiApplicationService))]
[Singleton(typeof(ITokenService), typeof(TokenService))]
[Singleton(typeof(IDesktopTopLevelControl), typeof(MainWindow))]
[Singleton(typeof(ISingleViewTopLevelControl), typeof(SingleView))]
[Singleton(typeof(Application), Factory = nameof(ApplicationFactory))]
[Singleton(typeof(TopLevel), Factory = nameof(TopLevelFactory))]
[Singleton(typeof(IMapper), Factory = nameof(MapperFactory))]
[Singleton(typeof(IContent), typeof(MainSplitViewModel))]
[Singleton(typeof(MainSplitViewModel))]
[Singleton(typeof(MainProgressBarViewModel))]
[Transient(typeof(TokenHttpHeaderFactory))]
[Transient(typeof(TimeZoneHttpHeaderFactory))]
[Transient(typeof(IToDoCache), typeof(ToDoCache))]
[Transient(typeof(IPropertyValidator), typeof(PropertyValidator))]
[Transient(typeof(IManagedNotificationManager), typeof(WindowNotificationManager))]
[Transient(typeof(IErrorHandler), typeof(ErrorHandler))]
[Transient(typeof(ISerializer), typeof(ProtobufSerializer))]
[Transient(typeof(IConverter), typeof(AutoMapperConverter))]
[Transient(typeof(ISpravyNotificationManager), typeof(NotificationManager))]
[Transient(typeof(IResourceLoader), typeof(FileResourceLoader))]
[Transient(typeof(IKeeper<Guid>), typeof(StaticKeeper<Guid>))]
[Transient(typeof(Control), typeof(MainView))]
[Transient(typeof(IDataTemplate), typeof(ModuleDataTemplate))]
[Transient(typeof(IMetadataFactory), typeof(MetadataFactory))]
[Transient(typeof(IDialogViewer), typeof(DialogViewer))]
[Transient(typeof(IClipboardService), typeof(TopLevelClipboardService))]
[Transient(typeof(IHttpHeaderFactory), Factory = nameof(HttpHeaderFactoryFactory))]
[Transient(typeof(MapperConfiguration), Factory = nameof(MapperConfigurationFactory))]
[Transient(typeof(StorageDbContext), Factory = nameof(StorageDbContextFactory))]
public interface IUiModule : IAuthenticationClientModule, IClientModule, IToDoClientModule
{
    public TopLevel TopLevelFactory(Application application)
    {
        return application.GetTopLevel().ThrowIfNull();
    }
    
    public Application ApplicationFactory()
    {
        return Application.Current.ThrowIfNull(nameof(Application));
    }
    
    public IHttpHeaderFactory HttpHeaderFactoryFactory(TokenHttpHeaderFactory token, TimeZoneHttpHeaderFactory timeZone)
    {
        return new CombineHttpHeaderFactory(token, timeZone);
    }
    
    public IMapper MapperFactory(MapperConfiguration configuration)
    {
        return new Mapper(configuration);
    }
    
    public StorageDbContext StorageDbContextFactory(IDbContextSetup setup)
    {
        return new(setup);
    }
    
    public MapperConfiguration MapperConfigurationFactory()
    {
        return new(expression =>
        {
            expression.AddProfile<SpravyToDoProfile>();
            expression.AddProfile<SpravyUiProfile>();
            expression.AddProfile<SpravyAuthenticationProfile>();
            expression.AddProfile<SpravyScheduleProfile>();
            expression.AddProfile<SpravyEventBusProfile>();
            expression.AddProfile<SpravyPasswordGeneratorProfile>();
        });
    }
}