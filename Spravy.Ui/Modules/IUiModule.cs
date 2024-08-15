using System.Text.Json.Serialization;
using Jab;
using Spravy.Authentication.Domain.Client.Modules;
using Spravy.Client.Modules;
using Spravy.Core.Interfaces;
using Spravy.PasswordGenerator.Domain.Client.Modules;
using Spravy.ToDo.Domain.Client.Modules;
using Spravy.Ui.Features.PasswordGenerator.Views;
using Spravy.Ui.Features.ToDo.Views;
using EditDescriptionContentView = Spravy.Ui.Features.ToDo.Views.EditDescriptionContentView;
using EditDescriptionView = Spravy.Ui.Features.ToDo.Views.EditDescriptionView;

namespace Spravy.Ui.Modules;

[ServiceProviderModule]
[Import(typeof(IAuthenticationClientModule))]
[Import(typeof(IClientModule))]
[Import(typeof(IToDoClientModule))]
[Import(typeof(IPasswordGeneratorClientModule))]
[Singleton(typeof(AccountNotify))]
[Singleton(typeof(SingleViewModel))]
[Singleton(typeof(MainWindowModel))]
[Singleton(typeof(App))]
[Singleton(typeof(MainProgressBarViewModel))]
[Singleton(typeof(MainSplitViewModel))]
[Singleton(typeof(MainViewModel))]
[Singleton(typeof(MainView))]
[Singleton(typeof(SpravyCommandService))]
[Singleton(typeof(SpravyCommandNotifyService))]
[Singleton(typeof(INavigator), typeof(Navigator))]
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
[Singleton(typeof(IDesktopTopLevelControl), Factory = nameof(DesktopTopLevelControlFactory))]
[Singleton(typeof(ISingleViewTopLevelControl), Factory = nameof(SingleViewTopLevelControlFactory))]
[Singleton(typeof(IEnumerable<IDataTemplate>), Factory = nameof(DataTemplatesFactory))]
[Singleton(typeof(Application), Factory = nameof(ApplicationFactory))]
[Singleton(typeof(AppOptions), Factory = nameof(AppOptionsFactory))]
[Transient(typeof(TokenHttpHeaderFactory))]
[Transient(typeof(TimeZoneHttpHeaderFactory))]
[Transient(typeof(PaneViewModel))]
[Transient(typeof(PaneView))]
[Transient(typeof(MainSplitView))]
[Transient(typeof(EditDescriptionView))]
[Transient(typeof(EditDescriptionContentView))]
[Transient(typeof(ConfirmView))]
[Transient(typeof(DeleteAccountView))]
[Transient(typeof(EmailOrLoginInputViewModel))]
[Transient(typeof(EmailOrLoginInputView))]
[Transient(typeof(InfoView))]
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
[Transient(typeof(AddRootToDoItemView))]
[Transient(typeof(AddToDoItemView))]
[Transient(typeof(DeleteToDoItemView))]
[Transient(typeof(MultiToDoItemsViewModel))]
[Transient(typeof(MultiToDoItemsView))]
[Transient(typeof(ToDoItemView))]
[Transient(typeof(ToDoItemCommands))]
[Transient(typeof(ToDoItemsGroupByViewModel))]
[Transient(typeof(ToDoItemsGroupByView))]
[Transient(typeof(ResetToDoItemView))]
[Transient(typeof(RootToDoItemsViewModel))]
[Transient(typeof(RootToDoItemsView))]
[Transient(typeof(ToDoItemContentView))]
[Transient(typeof(PeriodicityToDoItemSettingsView))]
[Transient(typeof(PlannedToDoItemSettingsView))]
[Transient(typeof(PeriodicityOffsetToDoItemSettingsView))]
[Transient(typeof(ValueToDoItemSettingsView))]
[Transient(typeof(ToDoItemToStringSettingsView))]
[Transient(typeof(ToDoItemDayOfWeekSelectorView))]
[Transient(typeof(ToDoItemDayOfYearSelectorView))]
[Transient(typeof(ToDoItemSelectorView))]
[Transient(typeof(ToDoItemSettingsView))]
[Transient(typeof(ToDoSubItemsViewModel))]
[Transient(typeof(ToDoSubItemsView))]
[Transient(typeof(ReferenceToDoItemSettingsView))]
[Transient(typeof(ChangeToDoItemOrderIndexView))]
[Transient(typeof(MultiToDoItemSettingView))]
[Transient(typeof(RandomizeChildrenOrderView))]
[Transient(typeof(SearchToDoItemsViewModel))]
[Transient(typeof(SearchToDoItemsView))]
[Transient(typeof(LeafToDoItemsView))]
[Transient(typeof(TodayToDoItemsViewModel))]
[Transient(typeof(TodayToDoItemsView))]
[Transient(typeof(ToDoItemDayOfMonthSelectorView))]
[Transient(typeof(AddPasswordItemViewModel))]
[Transient(typeof(AddPasswordItemView))]
[Transient(typeof(DeletePasswordItemView))]
[Transient(typeof(PasswordGeneratorViewModel))]
[Transient(typeof(PasswordGeneratorView))]
[Transient(typeof(PasswordItemSettingsView))]
[Transient(typeof(ErrorViewModel))]
[Transient(typeof(ErrorView))]
[Transient(typeof(ExceptionView))]
[Transient(typeof(VerificationCodeView))]
[Transient(typeof(LoginViewModel))]
[Transient(typeof(LoginView))]
[Transient(typeof(ForgotPasswordView))]
[Transient(typeof(CreateUserViewModel))]
[Transient(typeof(CreateUserView))]
[Transient(typeof(PolicyViewModel))]
[Transient(typeof(PolicyView))]
[Transient(typeof(IToDoUiService), typeof(ToDoUiService))]
[Transient(typeof(IPropertyValidator), typeof(PropertyValidator))]
[Transient(typeof(JsonSerializerContext), typeof(SpravyJsonSerializerContext))]
[Transient(typeof(IErrorHandler), typeof(ErrorHandler))]
[Transient(typeof(ISerializer), typeof(SpravyJsonSerializer))]
[Transient(typeof(IResourceLoader), typeof(FileResourceLoader))]
[Transient(typeof(IKeeper<Guid>), typeof(StaticKeeper<Guid>))]
[Transient(typeof(IDataTemplate), typeof(ModuleDataTemplate))]
[Transient(typeof(IMetadataFactory), typeof(MetadataFactory))]
[Transient(typeof(IRpcExceptionHandler), typeof(RpcExceptionHandler))]
[Transient(typeof(IClipboard), Factory = nameof(ClipboardFactory))]
[Transient(typeof(IHttpHeaderFactory), Factory = nameof(HttpHeaderFactoryFactory))]
public interface IUiModule
{
    static AppOptions AppOptionsFactory(
        ISerializer serializer,
        IConfigurationLoader configurationLoader
    )
    {
        using var stream = configurationLoader.GetStream();

        var configuration = serializer.Deserialize<AppOptionsConfiguration>(stream);

        return configuration.ThrowIfError().AppOptions.ThrowIfNull();
    }

    static IDesktopTopLevelControl DesktopTopLevelControlFactory(MainWindowModel mainWindowModel)
    {
        return new MainWindow { DataContext = mainWindowModel, };
    }

    static ISingleViewTopLevelControl SingleViewTopLevelControlFactory(
        SingleViewModel singleViewModel
    )
    {
        return new SingleView { DataContext = singleViewModel, };
    }

    static IClipboard ClipboardFactory(TopLevel topLevel)
    {
        return topLevel.Clipboard.ThrowIfNull();
    }

    static IEnumerable<IDataTemplate> DataTemplatesFactory(
        IViewSelector viewSelector,
        IServiceFactory serviceFactory,
        IViewFactory viewFactory
    )
    {
        return new[] { new ModuleDataTemplate(viewSelector, serviceFactory, viewFactory), };
    }

    static Application ApplicationFactory()
    {
        return Application.Current.ThrowIfNull(nameof(Application));
    }

    static IHttpHeaderFactory HttpHeaderFactoryFactory(
        TokenHttpHeaderFactory token,
        TimeZoneHttpHeaderFactory timeZone
    )
    {
        return new CombineHttpHeaderFactory(token, timeZone);
    }
}
