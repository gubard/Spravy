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
    private readonly IClipboardService clipboardService;

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
        ISerializer serializer,
        IClipboardService clipboardService
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
        this.clipboardService = clipboardService;
    }

    public EditToDoItemViewModel CreateEditToDoItemViewModel(
        bool isEditShow,
        bool isEditDescriptionShow
    )
    {
        return new(
            objectStorage,
            CreateToDoItemSelectorViewModel(),
            toDoCache,
            isEditShow,
            isEditDescriptionShow
        );
    }

    public ToDoItemSettingsViewModel CreateToDoItemSettingsViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        if (item.TryGetValue(out var value) && items.IsEmpty)
        {
            var result = new ToDoItemSettingsViewModel(
                item,
                items,
                toDoService,
                CreateEditToDoItemViewModel(false, false)
            );

            result.EditToDoItemViewModel.SetItem(value);
            result.EditToDoItemViewModel.UndoAllUi();

            return result;
        }

        return new(item, items, toDoService, CreateEditToDoItemViewModel(true, false));
    }

    public LeafToDoItemsViewModel CreateLeafToDoItemsViewModel(
        Option<ToDoItemEntityNotify> item,
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
            toDoUiService
        );
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

    public AddToDoItemToFavoriteEventViewModel CreateAddToDoItemToFavoriteEventViewModel()
    {
        return new(CreateToDoItemSelectorViewModel(), serializer, objectStorage, toDoCache);
    }

    public ToDoItemCreateTimerViewModel CreateToDoItemCreateTimerViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(
            item,
            items,
            objectStorage,
            serializer,
            scheduleService,
            errorHandler,
            taskProgressService
        );
    }

    public EditDescriptionViewModel CreateEditDescriptionViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(item, items, toDoService);
    }

    public CloneViewModel CreateCloneViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(
            item,
            items,
            CreateToDoItemSelectorViewModel(item, items),
            toDoService,
            toDoCache
        );
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
        if (items.IsEmpty)
        {
            if (item.TryGetValue(out var i))
            {
                return new(
                    item,
                    items,
                    CreateToDoItemSelectorViewModel(i.Parent.ToOption(), new[] { i }),
                    toDoService
                );
            }

            return new(item, items, CreateToDoItemSelectorViewModel(item, items), toDoService);
        }

        return new(item, items, CreateToDoItemSelectorViewModel(item, items), toDoService);
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
        if (items.IsEmpty)
        {
            if (item.TryGetValue(out var i))
            {
                return new(
                    item,
                    items,
                    toDoService,
                    CreateToDoItemSelectorViewModel(i.Parent.ToOption(), new[] { i })
                );
            }

            return new(item, items, toDoService, CreateToDoItemSelectorViewModel(item, items));
        }

        return new(item, items, toDoService, CreateToDoItemSelectorViewModel(item, items));
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
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByTypeView.References")),
            CreateToDoItemsViewModel(sortBy, new("ToDoItemsGroupByTypeView.ComingSoon"))
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
        return new(item, items, toDoService);
    }

    public ToDoItemToStringSettingsViewModel CreateToDoItemToStringSettingsViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(item, items, toDoService, clipboardService);
    }

    public ChangeToDoItemOrderIndexViewModel CreateChangeToDoItemOrderIndexViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(item, items, toDoService);
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
            toDoService,
            CreateEditToDoItemViewModel(false, true)
        );
    }

    public DeletePasswordItemViewModel CreateDeletePasswordItemViewModel(
        PasswordItemEntityNotify item
    )
    {
        return new(item);
    }
}
