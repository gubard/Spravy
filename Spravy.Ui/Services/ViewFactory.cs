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

    public MultiToDoItemSettingViewModel CreateMultiToDoItemSettingViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(items, toDoService);
    }

    public LeafToDoItemsViewModel CreateLeafToDoItemsViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        throw new NotImplementedException();
    }

    public ToDoItemSettingsViewModel CreateToDoItemSettingsViewModel(ToDoItemEntityNotify item)
    {
        return new(item, CreateToDoItemContentViewModel(), this, toDoService);
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
            CreateToDoSubItemsViewModel(SortBy.OrderIndex),
            errorHandler,
            toDoUiService,
            taskProgressService
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

    public ToDoSubItemsViewModel CreateToDoSubItemsViewModel(SortBy sortBy)
    {
        return new(
            toDoService,
            toDoCache,
            CreateMultiToDoItemsViewModel(sortBy),
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
            CreateToDoSubItemsViewModel(SortBy.OrderIndex),
            objectStorage,
            errorHandler,
            taskProgressService,
            toDoUiService
        );
    }

    public TodayToDoItemsViewModel CreateTodayToDoItemsViewModel()
    {
        return new(
            CreateToDoSubItemsViewModel(SortBy.LoadedIndex),
            errorHandler,
            serviceFactory.CreateService<SpravyCommandNotifyService>(),
            taskProgressService,
            toDoUiService
        );
    }

    public SearchToDoItemsViewModel CreateSearchToDoItemsViewModel()
    {
        return new(
            CreateToDoSubItemsViewModel(SortBy.LoadedIndex),
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

    public PolicyViewModel CreatePolicyViewModel()
    {
        return new();
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
            CreateToDoSubItemsViewModel(SortBy.LoadedIndex),
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
            CreateToDoSubItemsViewModel(SortBy.LoadedIndex),
            errorHandler,
            objectStorage,
            taskProgressService,
            serviceFactory.CreateService<SpravyCommandNotifyService>(),
            toDoUiService
        );
    }

    public AddToDoItemToFavoriteEventViewModel CreateAddToDoItemToFavoriteEventViewModel()
    {
        return new(CreateToDoItemSelectorViewModel(), serializer, objectStorage, toDoCache);
    }

    public ToDoItemCreateTimerViewModel CreateToDoItemCreateTimerViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        throw new NotImplementedException();
    }

    public EditDescriptionViewModel CreateEditDescriptionViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        throw new NotImplementedException();
    }

    public CloneViewModel CreateCloneViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        throw new NotImplementedException();
    }

    public ResetToDoItemViewModel CreateResetToDoItemViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(item, items, objectStorage, toDoService);
    }

    public CreateReferenceViewModel CreateCreateReferenceViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(item, items, CreateToDoItemSelectorViewModel(item, items), toDoService);
    }

    public LeafToDoItemsViewModel CreateLeafToDoItemsViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(
            item,
            items,
            CreateToDoSubItemsViewModel(SortBy.LoadedIndex),
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
        return new(item);
    }

    public ToDoItemDayOfMonthSelectorViewModel CreateToDoItemDayOfMonthSelectorViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item);
    }

    public ToDoItemDayOfYearSelectorViewModel CreateToDoItemDayOfYearSelectorViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item);
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

    public InfoViewModel CreateInfoViewModel(IDialogable content, Func<IDialogable, Cvtar> okTask)
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
            item.Reference is null
                ? CreateToDoItemSelectorViewModel()
                : CreateToDoItemSelectorViewModel(item.Reference),
            toDoService,
            toDoCache
        );
    }

    public ToDoItemsViewModel CreateToDoItemsViewModel(SortBy sortBy, TextLocalization header)
    {
        return new(sortBy, header, errorHandler, taskProgressService);
    }

    public DeleteTimerViewModel CreateDeleteTimerViewModel(TimerItemNotify item)
    {
        return new(item);
    }

    public AddTimerViewModel CreateAddTimerViewModel()
    {
        return new(this, objectStorage, errorHandler, taskProgressService, scheduleService);
    }

    public ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel()
    {
        return new(
            new(),
            ReadOnlyMemory<ToDoItemEntityNotify>.Empty,
            toDoCache,
            toDoUiService,
            errorHandler,
            taskProgressService
        );
    }

    public ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(item, items, toDoCache, toDoUiService, errorHandler, taskProgressService);
    }

    public ChangeParentViewModel CreateChangeParentViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(item, items, toDoService, CreateToDoItemSelectorViewModel());
    }

    public MultiToDoItemsViewModel CreateMultiToDoItemsViewModel(SortBy sortBy)
    {
        return new(
            CreateToDoItemsViewModel(SortBy.LoadedIndex, new("MultiToDoItemsView.Favorite")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByNoneView.Header")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByStatusView.Missed")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByStatusView.ReadyForCompleted")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByStatusView.Planned")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByStatusView.Completed")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByTypeView.Values")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByTypeView.Groups")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByTypeView.Planneds")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByTypeView.Periodicitys")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByTypeView.PeriodicityOffsets")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByTypeView.Circles")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByTypeView.Steps")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByTypeView.References"))
        );
    }

    public ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems
    )
    {
        return new(
            item.ToOption(),
            ignoreItems,
            toDoCache,
            toDoUiService,
            errorHandler,
            taskProgressService
        );
    }

    public ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems
    )
    {
        return new(
            Option<ToDoItemEntityNotify>.None,
            ignoreItems,
            toDoCache,
            toDoUiService,
            errorHandler,
            taskProgressService
        );
    }

    public ConfirmViewModel CreateConfirmViewModel(
        IDialogable content,
        Func<IDialogable, Cvtar> confirmTask,
        Func<IDialogable, Cvtar> cancelTask
    )
    {
        return new(content, errorHandler, taskProgressService, confirmTask, cancelTask);
    }

    public RandomizeChildrenOrderViewModel CreateRandomizeChildrenOrderViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        throw new NotImplementedException();
    }

    public ToDoItemToStringSettingsViewModel CreateToDoItemToStringSettingsViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        throw new NotImplementedException();
    }

    public ChangeToDoItemOrderIndexViewModel CreateChangeToDoItemOrderIndexViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        throw new NotImplementedException();
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

    public DeleteToDoItemViewModel CreateDeleteToDoItemViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(item, items, toDoService, errorHandler, taskProgressService);
    }

    public AddToDoItemViewModel CreateAddToDoItemViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(
            item,
            items,
            objectStorage,
            CreateToDoItemContentViewModel(),
            CreateEditDescriptionContentViewModel(),
            toDoService
        );
    }

    public ToDoItemContentViewModel CreateToDoItemContentViewModel()
    {
        return new();
    }

    public EditDescriptionContentViewModel CreateEditDescriptionContentViewModel()
    {
        return new();
    }

    public DeletePasswordItemViewModel CreateDeletePasswordItemViewModel(
        PasswordItemEntityNotify item
    )
    {
        return new(item);
    }
}
