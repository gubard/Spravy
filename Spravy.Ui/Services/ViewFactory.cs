using Spravy.Schedule.Domain.Interfaces;
using Spravy.Ui.Features.Schedule.ViewModels;
using EditDescriptionContentViewModel = Spravy.Ui.Features.ToDo.ViewModels.EditDescriptionContentViewModel;

namespace Spravy.Ui.Services;

public class ViewFactory : IViewFactory
{
    private readonly IToDoService toDoService;
    private readonly INavigator navigator;
    private readonly IErrorHandler errorHandler;
    private readonly ITaskProgressService taskProgressService;
    private readonly IToDoUiService toDoUiService;
    private readonly IToDoCache toDoCache;
    private readonly IObjectStorage objectStorage;
    private readonly ToDoItemCommands toDoItemCommands;
    private readonly AppOptions appOptions;
    private readonly IAuthenticationService authenticationService;
    private readonly IPropertyValidator propertyValidator;
    private readonly IServiceFactory serviceFactory;
    private readonly IPasswordService passwordService;
    private readonly IPasswordItemCache passwordItemCache;
    private readonly AccountNotify accountNotify;
    private readonly Application application;
    private readonly IScheduleService scheduleService;
    private readonly ISerializer serializer;

    public ViewFactory(
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService,
        IToDoCache toDoCache,
        IObjectStorage objectStorage,
        ToDoItemCommands toDoItemCommands,
        AppOptions appOptions,
        INavigator navigator,
        IAuthenticationService authenticationService,
        IPropertyValidator propertyValidator,
        IPasswordService passwordService,
        IPasswordItemCache passwordItemCache,
        AccountNotify accountNotify,
        Application application,
        IServiceFactory serviceFactory,
        IScheduleService scheduleService,
        ISerializer serializer
    )
    {
        this.toDoService = toDoService;
        this.errorHandler = errorHandler;
        this.taskProgressService = taskProgressService;
        this.toDoUiService = toDoUiService;
        this.toDoCache = toDoCache;
        this.objectStorage = objectStorage;
        this.toDoItemCommands = toDoItemCommands;
        this.appOptions = appOptions;
        this.navigator = navigator;
        this.authenticationService = authenticationService;
        this.propertyValidator = propertyValidator;
        this.passwordService = passwordService;
        this.passwordItemCache = passwordItemCache;
        this.accountNotify = accountNotify;
        this.application = application;
        this.serviceFactory = serviceFactory;
        this.scheduleService = scheduleService;
        this.serializer = serializer;
    }

    public ToDoItemSettingsViewModel CreateToDoItemSettingsViewModel(ToDoItemEntityNotify item)
    {
        return new(item, CreateToDoItemContentViewModel(), this);
    }

    public ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(ToDoItemEntityNotify item)
    {
        return CreateToDoItemSelectorViewModel(item, ReadOnlyMemory<ToDoItemEntityNotify>.Empty);
    }

    public ToDoItemViewModel CreateToDoItemViewModel(ToDoItemEntityNotify item)
    {
        return new(
            item,
            objectStorage,
            toDoItemCommands,
            CreateToDoSubItemsViewModel(),
            errorHandler,
            toDoUiService
        );
    }

    public AddRootToDoItemViewModel CreateAddRootToDoItemViewModel()
    {
        return new(
            objectStorage,
            CreateToDoItemContentViewModel(),
            CreateEditDescriptionContentViewModel()
        );
    }

    public AddPasswordItemViewModel CreateAddPasswordItemViewModel()
    {
        return new();
    }

    public ValueToDoItemSettingsViewModel CreateValueToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService, errorHandler, taskProgressService);
    }

    public EditDescriptionViewModel CreateEditDescriptionViewModel(ToDoItemEntityNotify item)
    {
        return new(item, CreateEditDescriptionContentViewModel());
    }

    public ToDoSubItemsViewModel CreateToDoSubItemsViewModel()
    {
        return new(
            toDoService,
            toDoCache,
            CreateMultiToDoItemsViewModel(),
            taskProgressService,
            appOptions
        );
    }

    public LoginViewModel CreateLoginViewModel()
    {
        return new(objectStorage, propertyValidator);
    }

    public RootToDoItemsViewModel CreateRootToDoItemsViewModel()
    {
        return new(
            serviceFactory.CreateService<SpravyCommandNotifyService>(),
            CreateToDoSubItemsViewModel(),
            objectStorage,
            errorHandler,
            taskProgressService,
            toDoUiService
        );
    }

    public TodayToDoItemsViewModel CreateTodayToDoItemsViewModel()
    {
        return new(
            CreateToDoSubItemsViewModel(),
            errorHandler,
            serviceFactory.CreateService<SpravyCommandNotifyService>(),
            taskProgressService,
            toDoUiService
        );
    }

    public SearchToDoItemsViewModel CreateSearchToDoItemsViewModel()
    {
        return new(
            CreateToDoSubItemsViewModel(),
            serviceFactory.CreateService<SpravyCommandNotifyService>(),
            errorHandler,
            objectStorage,
            toDoUiService
        );
    }

    public PasswordGeneratorViewModel CreatePasswordGeneratorViewModel()
    {
        return new(passwordService, passwordItemCache);
    }

    public SettingViewModel CreateSettingViewModel()
    {
        return new(
            errorHandler,
            navigator,
            accountNotify,
            taskProgressService,
            application,
            objectStorage,
            this
        );
    }

    public TimersViewModel CreateTimersViewModel()
    {
        return new(scheduleService, errorHandler, taskProgressService);
    }

    public EmailOrLoginInputViewModel CreateEmailOrLoginInputViewModel()
    {
        return new(
            errorHandler,
            navigator,
            objectStorage,
            authenticationService,
            taskProgressService,
            this
        );
    }

    public CreateUserViewModel CreateCreateUserViewModel()
    {
        return new(propertyValidator);
    }

    public LeafToDoItemsViewModel CreateLeafToDoItemsViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(
            null,
            items,
            CreateToDoSubItemsViewModel(),
            errorHandler,
            objectStorage,
            taskProgressService,
            serviceFactory.CreateService<SpravyCommandNotifyService>(),
            toDoUiService
        );
    }

    public LeafToDoItemsViewModel CreateLeafToDoItemsViewModel(ToDoItemEntityNotify item)
    {
        return new(
            item,
            ReadOnlyMemory<ToDoItemEntityNotify>.Empty,
            CreateToDoSubItemsViewModel(),
            errorHandler,
            objectStorage,
            taskProgressService,
            serviceFactory.CreateService<SpravyCommandNotifyService>(),
            toDoUiService
        );
    }

    public AddToDoItemToFavoriteEventViewModel CreateAddToDoItemToFavoriteEventViewModel()
    {
        return new(CreateToDoItemSelectorViewModel(), serializer);
    }

    public LeafToDoItemsViewModel CreateLeafToDoItemsViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(
            item,
            items,
            CreateToDoSubItemsViewModel(),
            errorHandler,
            objectStorage,
            taskProgressService,
            serviceFactory.CreateService<SpravyCommandNotifyService>(),
            toDoUiService
        );
    }

    public DeleteAccountViewModel CreateDeleteAccountViewModel(
        string emailOrLogin,
        UserIdentifierType identifierType
    )
    {
        return new(
            emailOrLogin,
            identifierType,
            errorHandler,
            navigator,
            authenticationService,
            taskProgressService,
            this
        );
    }

    public VerificationCodeViewModel CreateVerificationCodeViewModel(
        string emailOrLogin,
        UserIdentifierType identifierType
    )
    {
        return new(emailOrLogin, identifierType);
    }

    public ForgotPasswordViewModel CreateForgotPasswordViewModel(
        string emailOrLogin,
        UserIdentifierType identifierType
    )
    {
        return new(emailOrLogin, identifierType);
    }

    public ToDoItemDayOfWeekSelectorViewModel CreateToDoItemDayOfWeekSelectorViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService);
    }

    public ToDoItemDayOfMonthSelectorViewModel CreateToDoItemDayOfMonthSelectorViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService);
    }

    public ToDoItemDayOfYearSelectorViewModel CreateToDoItemDayOfYearSelectorViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService);
    }

    public PeriodicityOffsetToDoItemSettingsViewModel CreatePeriodicityOffsetToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService);
    }

    public PeriodicityToDoItemSettingsViewModel CreatePeriodicityToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService, this);
    }

    public PlannedToDoItemSettingsViewModel CreatePlannedToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService);
    }

    public InfoViewModel CreateInfoViewModel(
        object content,
        Func<object, ConfiguredValueTaskAwaitable<Result>> okTask
    )
    {
        return new(content, okTask, errorHandler, taskProgressService);
    }

    public PasswordItemSettingsViewModel CreatePasswordItemSettingsViewModel(
        PasswordItemEntityNotify item
    )
    {
        return new(item);
    }

    public ReferenceToDoItemSettingsViewModel CreateReferenceToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(
            item,
            CreateToDoItemSelectorViewModel(item, item.ToReadOnlyMemory()),
            toDoService
        );
    }

    public ToDoItemsGroupByNoneViewModel CreateToDoItemsGroupByNoneViewModel()
    {
        return new(CreateToDoItemsViewModel());
    }

    public ToDoItemsGroupByStatusViewModel CreateToDoItemsGroupByStatusViewModel()
    {
        return new(
            CreateToDoItemsViewModel(),
            CreateToDoItemsViewModel(),
            CreateToDoItemsViewModel(),
            CreateToDoItemsViewModel()
        );
    }

    public ToDoItemsGroupByTypeViewModel CreateToDoItemsGroupByTypeViewModel()
    {
        return new(
            CreateToDoItemsViewModel(),
            CreateToDoItemsViewModel(),
            CreateToDoItemsViewModel(),
            CreateToDoItemsViewModel(),
            CreateToDoItemsViewModel(),
            CreateToDoItemsViewModel(),
            CreateToDoItemsViewModel(),
            CreateToDoItemsViewModel()
        );
    }

    public ToDoItemsViewModel CreateToDoItemsViewModel()
    {
        return new(errorHandler, taskProgressService);
    }

    public AddTimerViewModel CreateAddTimerViewModel()
    {
        return new(this);
    }

    public ToDoItemsGroupByViewModel CreateToDoItemsGroupByViewModel()
    {
        return new(
            CreateToDoItemsGroupByNoneViewModel(),
            CreateToDoItemsGroupByStatusViewModel(),
            CreateToDoItemsGroupByTypeViewModel()
        );
    }

    public ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel()
    {
        return new(
            null,
            ReadOnlyMemory<ToDoItemEntityNotify>.Empty,
            toDoService,
            toDoCache,
            errorHandler,
            taskProgressService
        );
    }

    public MultiToDoItemsViewModel CreateMultiToDoItemsViewModel()
    {
        return new MultiToDoItemsViewModel(
            CreateToDoItemsViewModel(),
            CreateToDoItemsGroupByViewModel()
        );
    }

    public ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems
    )
    {
        return new(item, ignoreItems, toDoService, toDoCache, errorHandler, taskProgressService);
    }

    public ConfirmViewModel CreateConfirmViewModel(
        object content,
        Func<object, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<object, ConfiguredValueTaskAwaitable<Result>> cancelTask
    )
    {
        return new(content, errorHandler, taskProgressService, confirmTask, cancelTask);
    }

    public TextViewModel CreateTextViewModel()
    {
        return new();
    }

    public ErrorViewModel CreateErrorViewModel(ReadOnlyMemory<Error> errors)
    {
        return new(errors.ToArray());
    }

    public ExceptionViewModel CreateExceptionViewModel(Exception exception)
    {
        return new(exception);
    }

    public ResetToDoItemViewModel CreateResetToDoItemViewModel(ToDoItemEntityNotify item)
    {
        return new(item, objectStorage, errorHandler, taskProgressService, toDoUiService);
    }

    public RandomizeChildrenOrderViewModel CreateRandomizeChildrenOrderViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(items);
    }

    public RandomizeChildrenOrderViewModel CreateRandomizeChildrenOrderViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item);
    }

    public ToDoItemToStringSettingsViewModel CreateToDoItemToStringSettingsViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item);
    }

    public MultiToDoItemSettingViewModel CreateMultiToDoItemSettingViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item);
    }

    public DeleteToDoItemViewModel CreateDeleteToDoItemViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(item, items, toDoService, toDoUiService, errorHandler, taskProgressService);
    }

    public DeleteToDoItemViewModel CreateDeleteToDoItemViewModel(ToDoItemEntityNotify item)
    {
        return CreateDeleteToDoItemViewModel(item, ReadOnlyMemory<ToDoItemEntityNotify>.Empty);
    }

    public AddToDoItemViewModel CreateAddToDoItemViewModel(ToDoItemEntityNotify parent)
    {
        return new(
            parent,
            CreateToDoItemContentViewModel(),
            CreateEditDescriptionContentViewModel(),
            objectStorage
        );
    }

    public ToDoItemContentViewModel CreateToDoItemContentViewModel()
    {
        return new();
    }

    public EditDescriptionContentViewModel CreateEditDescriptionContentViewModel()
    {
        return new(errorHandler, taskProgressService);
    }

    public DeletePasswordItemViewModel CreateDeletePasswordItemViewModel(
        PasswordItemEntityNotify item
    )
    {
        return new(item);
    }

    public ChangeToDoItemOrderIndexViewModel CreateChangeToDoItemOrderIndexViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(
            item,
            ReadOnlyMemory<ToDoItemEntityNotify>.Empty,
            errorHandler,
            taskProgressService,
            toDoUiService
        );
    }

    public ChangeToDoItemOrderIndexViewModel CreateChangeToDoItemOrderIndexViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(item, items, errorHandler, taskProgressService, toDoUiService);
    }

    public ChangeToDoItemOrderIndexViewModel CreateChangeToDoItemOrderIndexViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(null, items, errorHandler, taskProgressService, toDoUiService);
    }
}
