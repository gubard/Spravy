using Jab;
using Spravy.Authentication.Domain.Client.Modules;
using Spravy.Client.Modules;
using Spravy.PasswordGenerator.Domain.Client.Modules;
using Spravy.Picture.Domain.Client.Modules;
using Spravy.Schedule.Domain.Client.Modules;
using Spravy.ToDo.Domain.Client.Modules;
using EditDescriptionView = Spravy.Ui.Features.ToDo.Views.EditDescriptionView;

namespace Spravy.Ui.Modules;

[ServiceProviderModule]
[Import(typeof(IAuthenticationClientModule))]
[Import(typeof(IClientModule))]
[Import(typeof(IToDoClientModule))]
[Import(typeof(IPictureClientModule))]
[Import(typeof(IPasswordGeneratorClientModule))]
[Import(typeof(IScheduleClientModule))]
[Singleton(typeof(AccountNotify))]
[Singleton(typeof(SoundSettingsNotify))]
[Singleton(typeof(App))]
[Singleton(typeof(SpravyCommandService))]
[Singleton(typeof(SpravyCommandNotifyService))]
[Singleton(typeof(IToDoUiService), typeof(ToDoUiService))]
[Singleton(typeof(INavigator), typeof(Navigator))]
[Singleton(typeof(IRootViewFactory), typeof(RootViewFactory))]
[Singleton(typeof(ITaskProgressService), typeof(TaskProgressService))]
[Singleton(typeof(IUiApplicationService), typeof(UiApplicationService))]
[Singleton(typeof(ITokenService), typeof(TokenService))]
[Singleton(typeof(IToDoCache), typeof(ToDoCache))]
[Singleton(typeof(IDialogViewer), typeof(DialogViewer))]
[Singleton(typeof(IViewSelector), typeof(ViewSelector))]
[Singleton(typeof(IPasswordItemCache), typeof(PasswordItemCache))]
[Singleton(typeof(ISpravyNotificationManager), typeof(NotificationManager))]
[Singleton(typeof(IManagedNotificationManager), typeof(WindowNotificationManager))]
[Singleton(typeof(IViewFactory), typeof(ViewFactory))]
[Singleton(typeof(IRetryService), typeof(RetryService))]
[Singleton(typeof(IEventUpdater), typeof(EventUpdater))]
[Singleton(typeof(IAudioService), typeof(AudioService))]
[Singleton(typeof(IDesktopTopLevelControl), Factory = nameof(DesktopTopLevelControlFactory))]
[Singleton(typeof(ISingleViewTopLevelControl), Factory = nameof(SingleViewTopLevelControlFactory))]
[Singleton(typeof(IEnumerable<IDataTemplate>), Factory = nameof(DataTemplatesFactory))]
[Singleton(typeof(Application), Factory = nameof(ApplicationFactory))]
[Singleton(typeof(AppOptions), Factory = nameof(AppOptionsFactory))]
[Transient(typeof(TokenHttpHeaderFactory))]
[Transient(typeof(TimeZoneHttpHeaderFactory))]
[Transient(typeof(MainView))]
[Transient(typeof(PaneView))]
[Transient(typeof(MainSplitView))]
[Transient(typeof(EditDescriptionView))]
[Transient(typeof(ConfirmView))]
[Transient(typeof(DeleteAccountView))]
[Transient(typeof(EmailOrLoginInputView))]
[Transient(typeof(InfoView))]
[Transient(typeof(SettingView))]
[Transient(typeof(TextView))]
[Transient(typeof(MainProgressBarView))]
[Transient(typeof(ToDoItemsView))]
[Transient(typeof(ToDoItemSettingsView))]
[Transient(typeof(AddTimerView))]
[Transient(typeof(AddToDoItemView))]
[Transient(typeof(DeleteToDoItemView))]
[Transient(typeof(MultiToDoItemsView))]
[Transient(typeof(TimersView))]
[Transient(typeof(ToDoItemView))]
[Transient(typeof(ResetToDoItemView))]
[Transient(typeof(RootToDoItemsView))]
[Transient(typeof(ToDoItemToStringSettingsView))]
[Transient(typeof(ToDoItemSelectorView))]
[Transient(typeof(ToDoSubItemsView))]
[Transient(typeof(ChangeToDoItemOrderIndexView))]
[Transient(typeof(EditToDoItemView))]
[Transient(typeof(RandomizeChildrenOrderView))]
[Transient(typeof(SearchToDoItemsView))]
[Transient(typeof(LeafToDoItemsView))]
[Transient(typeof(TodayToDoItemsView))]
[Transient(typeof(EditPasswordItemView))]
[Transient(typeof(DeletePasswordItemView))]
[Transient(typeof(PasswordGeneratorView))]
[Transient(typeof(PasswordItemSettingsView))]
[Transient(typeof(ErrorView))]
[Transient(typeof(ExceptionView))]
[Transient(typeof(VerificationCodeView))]
[Transient(typeof(LoginView))]
[Transient(typeof(DeleteTimerView))]
[Transient(typeof(AddToDoItemToFavoriteEventView))]
[Transient(typeof(ForgotPasswordView))]
[Transient(typeof(ToDoItemCreateTimerView))]
[Transient(typeof(CreateUserView))]
[Transient(typeof(PolicyView))]
[Transient(typeof(ChangeParentView))]
[Transient(typeof(CloneView))]
[Transient(typeof(CreateReferenceView))]
[Transient(typeof(AddPasswordItemView))]
[Transient(typeof(IPropertyValidator), typeof(PropertyValidator))]
[Transient(typeof(JsonSerializerContext), typeof(SpravyJsonSerializerContext))]
[Transient(typeof(IErrorHandler), typeof(ErrorHandler))]
[Transient(typeof(ISerializer), typeof(SpravyJsonSerializer))]
[Transient(typeof(IResourceLoader), typeof(FileResourceLoader))]
[Transient(typeof(IKeeper<Guid>), typeof(StaticKeeper<Guid>))]
[Transient(typeof(IDataTemplate), typeof(ModuleDataTemplate))]
[Transient(typeof(IMetadataFactory), typeof(MetadataFactory))]
[Transient(typeof(IRpcExceptionHandler), typeof(RpcExceptionHandler))]
[Transient(typeof(IHttpHeaderFactory), Factory = nameof(HttpHeaderFactoryFactory))]
public interface IUiModule
{
    static AppOptions AppOptionsFactory(ISerializer serializer, IConfigurationLoader configurationLoader)
    {
        using var stream = configurationLoader.GetStream();
        var configuration = serializer.Deserialize<AppOptionsConfiguration>(stream);

        return configuration.ThrowIfError().AppOptions.ThrowIfNull();
    }

    static IDesktopTopLevelControl DesktopTopLevelControlFactory(IRootViewFactory rootViewFactory)
    {
        return new MainWindow
        {
            DataContext = rootViewFactory.CreateMainWindowModel(),
        };
    }

    static ISingleViewTopLevelControl SingleViewTopLevelControlFactory(IRootViewFactory rootViewFactory)
    {
        return new SingleView
        {
            DataContext = rootViewFactory.CreateSingleViewModel(),
        };
    }

    static IEnumerable<IDataTemplate> DataTemplatesFactory(
        IViewSelector viewSelector,
        IServiceFactory serviceFactory,
        IViewFactory viewFactory
    )
    {
        return new[]
        {
            new ModuleDataTemplate(viewSelector, serviceFactory, viewFactory),
        };
    }

    static Application ApplicationFactory()
    {
        return Application.Current.ThrowIfNull(nameof(Application));
    }

    static IHttpHeaderFactory HttpHeaderFactoryFactory(TokenHttpHeaderFactory token, TimeZoneHttpHeaderFactory timeZone)
    {
        return new CombineHttpHeaderFactory(token, timeZone);
    }
}