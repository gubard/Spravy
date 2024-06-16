using Jab;
using Spravy.Authentication.Domain.Client.Modules;
using Spravy.Client.Modules;
using Spravy.PasswordGenerator.Domain.Client.Modules;
using Spravy.ToDo.Domain.Client.Modules;
using Spravy.Ui.Features.PasswordGenerator.Views;
using Spravy.Ui.Features.ToDo.Views;

namespace Spravy.Ui.Modules;

[ServiceProviderModule]
[Import(typeof(IAuthenticationClientModule))]
[Import(typeof(IClientModule))]
[Import(typeof(IToDoClientModule))]
[Import(typeof(IPasswordGeneratorClientModule))]
[Singleton(typeof(AccountNotify))]
[Singleton(typeof(App))]
[Singleton(typeof(MainProgressBarViewModel))]
[Singleton(typeof(MainSplitViewModel))]
[Singleton(typeof(MainViewModel))]
[Singleton(typeof(INavigator), typeof(Navigator))]
[Singleton(typeof(ITaskProgressService), typeof(TaskProgressService))]
[Singleton(typeof(IUiApplicationService), typeof(UiApplicationService))]
[Singleton(typeof(ITokenService), typeof(TokenService))]
[Singleton(typeof(ISingleViewTopLevelControl), typeof(SingleView))]
[Singleton(typeof(IDesktopTopLevelControl), Factory = nameof(DesktopTopLevelControlFactory))]
[Singleton(typeof(IEnumerable<IDataTemplate>), Factory = nameof(DataTemplatesFactory))]
[Singleton(typeof(TopLevel), Factory = nameof(TopLevelFactory))]
[Singleton(typeof(IMapper), Factory = nameof(MapperFactory))]
[Singleton(typeof(IMapper), Factory = nameof(MapperFactory))]
[Singleton(typeof(Application), Factory = nameof(ApplicationFactory))]
[Transient(typeof(TokenHttpHeaderFactory))]
[Transient(typeof(TimeZoneHttpHeaderFactory))]
[Transient(typeof(MainView))]
[Transient(typeof(PaneViewModel))]
[Transient(typeof(PaneView))]
[Transient(typeof(PathViewModel))]
[Transient(typeof(PathView))]
[Transient(typeof(MainSplitView))]
[Transient(typeof(CalendarViewModel))]
[Transient(typeof(CalendarView))]
[Transient(typeof(SpravyDateTimeViewModel))]
[Transient(typeof(DateTimeView))]
[Transient(typeof(DialogProgressViewModel))]
[Transient(typeof(DialogProgressView))]
[Transient(typeof(ItemSelectorViewModel))]
[Transient(typeof(ItemSelectorView))]
[Transient(typeof(NumberViewModel))]
[Transient(typeof(NumberView))]
[Transient(typeof(EditDescriptionViewModel))]
[Transient(typeof(EditDescriptionView))]
[Transient(typeof(EditDescriptionContentViewModel))]
[Transient(typeof(EditDescriptionContentView))]
[Transient(typeof(ConfirmViewModel))]
[Transient(typeof(ConfirmView))]
[Transient(typeof(DeleteAccountViewModel))]
[Transient(typeof(DeleteAccountView))]
[Transient(typeof(EmailOrLoginInputViewModel))]
[Transient(typeof(EmailOrLoginInputView))]
[Transient(typeof(InfoViewModel))]
[Transient(typeof(InfoView))]
[Transient(typeof(PageHeaderViewModel))]
[Transient(typeof(PageHeaderView))]
[Transient(typeof(SettingViewModel))]
[Transient(typeof(SettingView))]
[Transient(typeof(TextViewModel))]
[Transient(typeof(TextView))]
[Transient(typeof(MainProgressBarView))]
[Transient(typeof(ToDoItemsViewModel))]
[Transient(typeof(ToDoItemsView))]
[Transient(typeof(ToDoItemsGroupByNoneViewModel))]
[Transient(typeof(ToDoItemsGroupByNoneView))]
[Transient(typeof(ToDoItemsGroupByTypeViewModel))]
[Transient(typeof(ToDoItemsGroupByTypeView))]
[Transient(typeof(ToDoItemsGroupByStatusViewModel))]
[Transient(typeof(ToDoItemsGroupByStatusView))]
[Transient(typeof(AddRootToDoItemViewModel))]
[Transient(typeof(AddRootToDoItemView))]
[Transient(typeof(AddToDoItemViewModel))]
[Transient(typeof(AddToDoItemView))]
[Transient(typeof(DeleteToDoItemViewModel))]
[Transient(typeof(DeleteToDoItemView))]
[Transient(typeof(MultiToDoItemsViewModel))]
[Transient(typeof(MultiToDoItemsView))]
[Transient(typeof(ToDoItemViewModel))]
[Transient(typeof(ToDoItemView))]
[Transient(typeof(ToDoItemCommands))]
[Transient(typeof(ToDoItemsGroupByViewModel))]
[Transient(typeof(ToDoItemsGroupByView))]
[Transient(typeof(ResetToDoItemViewModel))]
[Transient(typeof(ResetToDoItemView))]
[Transient(typeof(RootToDoItemsViewModel))]
[Transient(typeof(RootToDoItemsView))]
[Transient(typeof(RootToDoItemsCommands))]
[Transient(typeof(ToDoItemContentViewModel))]
[Transient(typeof(ToDoItemContentView))]
[Transient(typeof(PeriodicityToDoItemSettingsViewModel))]
[Transient(typeof(PeriodicityToDoItemSettingsView))]
[Transient(typeof(PlannedToDoItemSettingsViewModel))]
[Transient(typeof(PlannedToDoItemSettingsView))]
[Transient(typeof(PeriodicityOffsetToDoItemSettingsViewModel))]
[Transient(typeof(PeriodicityOffsetToDoItemSettingsView))]
[Transient(typeof(ValueToDoItemSettingsViewModel))]
[Transient(typeof(ValueToDoItemSettingsView))]
[Transient(typeof(GroupToDoItemSettingsViewModel))]
[Transient(typeof(GroupToDoItemSettingsView))]
[Transient(typeof(ToDoItemToStringSettingsViewModel))]
[Transient(typeof(ToDoItemToStringSettingsView))]
[Transient(typeof(ToDoItemDayOfWeekSelectorViewModel))]
[Transient(typeof(ToDoItemDayOfWeekSelectorView))]
[Transient(typeof(ToDoItemDayOfYearSelectorViewModel))]
[Transient(typeof(ToDoItemDayOfYearSelectorView))]
[Transient(typeof(ToDoItemSelectorViewModel))]
[Transient(typeof(ToDoItemSelectorView))]
[Transient(typeof(ToDoItemSettingsViewModel))]
[Transient(typeof(ToDoItemSettingsView))]
[Transient(typeof(ToDoSubItemsViewModel))]
[Transient(typeof(ToDoSubItemsView))]
[Transient(typeof(ReferenceToDoItemSettingsViewModel))]
[Transient(typeof(ReferenceToDoItemSettingsView))]
[Transient(typeof(FastAddToDoItemViewModel))]
[Transient(typeof(FastAddToDoItemView))]
[Transient(typeof(ChangeToDoItemOrderIndexViewModel))]
[Transient(typeof(ChangeToDoItemOrderIndexView))]
[Transient(typeof(MultiToDoItemSettingViewModel))]
[Transient(typeof(MultiToDoItemSettingView))]
[Transient(typeof(RandomizeChildrenOrderViewModel))]
[Transient(typeof(RandomizeChildrenOrderView))]
[Transient(typeof(SearchToDoItemsViewModel))]
[Transient(typeof(SearchToDoItemsView))]
[Transient(typeof(LeafToDoItemsViewModel))]
[Transient(typeof(LeafToDoItemsView))]
[Transient(typeof(TodayToDoItemsViewModel))]
[Transient(typeof(TodayToDoItemsView))]
[Transient(typeof(ToDoItemDayOfMonthSelectorViewModel))]
[Transient(typeof(ToDoItemDayOfMonthSelectorView))]
[Transient(typeof(AddPasswordItemViewModel))]
[Transient(typeof(AddPasswordItemView))]
[Transient(typeof(DeletePasswordItemViewModel))]
[Transient(typeof(DeletePasswordItemView))]
[Transient(typeof(PasswordGeneratorViewModel))]
[Transient(typeof(PasswordGeneratorView))]
[Transient(typeof(PasswordItemSettingsViewModel))]
[Transient(typeof(PasswordItemSettingsView))]
[Transient(typeof(ErrorViewModel))]
[Transient(typeof(ErrorView))]
[Transient(typeof(ExceptionViewModel))]
[Transient(typeof(ExceptionView))]
[Transient(typeof(VerificationCodeViewModel))]
[Transient(typeof(VerificationCodeView))]
[Transient(typeof(VerificationCodeCommands))]
[Transient(typeof(LoginViewModel))]
[Transient(typeof(LoginView))]
[Transient(typeof(LoginCommands))]
[Transient(typeof(ForgotPasswordViewModel))]
[Transient(typeof(ForgotPasswordView))]
[Transient(typeof(CreateUserViewModel))]
[Transient(typeof(CreateUserView))]
[Transient(typeof(CreateUserCommands))]
[Transient(typeof(IToDoCache), typeof(ToDoCache))]
[Transient(typeof(IPasswordItemCache), typeof(PasswordItemCache))]
[Transient(typeof(IPropertyValidator), typeof(PropertyValidator))]
[Transient(typeof(IManagedNotificationManager), typeof(WindowNotificationManager))]
[Transient(typeof(IErrorHandler), typeof(ErrorHandler))]
[Transient(typeof(ISerializer), typeof(ProtobufSerializer))]
[Transient(typeof(IConverter), typeof(AutoMapperConverter))]
[Transient(typeof(ISpravyNotificationManager), typeof(NotificationManager))]
[Transient(typeof(IResourceLoader), typeof(FileResourceLoader))]
[Transient(typeof(IKeeper<Guid>), typeof(StaticKeeper<Guid>))]
[Transient(typeof(IDataTemplate), typeof(ModuleDataTemplate))]
[Transient(typeof(IMetadataFactory), typeof(MetadataFactory))]
[Transient(typeof(IDialogViewer), typeof(DialogViewer))]
[Transient(typeof(IViewSelector), typeof(ViewSelector))]
[Transient(typeof(IClipboard), Factory = nameof(ClipboardFactory))]
[Transient(typeof(IHttpHeaderFactory), Factory = nameof(HttpHeaderFactoryFactory))]
[Transient(typeof(MapperConfiguration), Factory = nameof(MapperConfigurationFactory))]
public interface IUiModule
{
    static IClipboard ClipboardFactory(TopLevel topLevel)
    {
        return topLevel.Clipboard.ThrowIfNull();
    }

    static IDesktopTopLevelControl DesktopTopLevelControlFactory(MainViewModel viewModel)
    {
        return new MainWindow
        {
            Content = viewModel,
        };
    }

    static IEnumerable<IDataTemplate> DataTemplatesFactory(IViewSelector viewSelector, IServiceFactory serviceFactory)
    {
        return new[]
        {
            new ModuleDataTemplate(viewSelector, serviceFactory),
        };
    }

    static TopLevel TopLevelFactory(Application application)
    {
        return application.GetTopLevel().ThrowIfNull();
    }

    static Application ApplicationFactory()
    {
        return Application.Current.ThrowIfNull(nameof(Application));
    }

    static IHttpHeaderFactory HttpHeaderFactoryFactory(TokenHttpHeaderFactory token, TimeZoneHttpHeaderFactory timeZone)
    {
        return new CombineHttpHeaderFactory(token, timeZone);
    }

    static IMapper MapperFactory(MapperConfiguration configuration)
    {
        return new Mapper(configuration);
    }

    static MapperConfiguration MapperConfigurationFactory()
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