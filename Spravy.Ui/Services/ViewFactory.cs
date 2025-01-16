namespace Spravy.Ui.Services;

public class ViewFactory : IViewFactory
{
    private readonly AccountNotify accountNotify;
    private readonly Application application;
    private readonly AppOptions appOptions;
    private readonly IAuthenticationService authenticationService;
    private readonly IClipboardService clipboardService;
    private readonly IErrorHandler errorHandler;
    private readonly INavigator navigator;
    private readonly IObjectStorage objectStorage;
    private readonly IPasswordItemCache passwordItemCache;
    private readonly IPasswordService passwordService;
    private readonly IPropertyValidator propertyValidator;
    private readonly IScheduleService scheduleService;
    private readonly ISerializer serializer;
    private readonly IServiceFactory serviceFactory;
    private readonly ITaskProgressService taskProgressService;
    private readonly IToDoCache toDoCache;
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;
    private readonly SoundSettingsNotify soundSettingsNotify;

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
        IClipboardService clipboardService,
        SoundSettingsNotify soundSettingsNotify
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
        this.soundSettingsNotify = soundSettingsNotify;
    }

    public EditToDoItemViewModel CreateEditToDoItemViewModel(bool isEditShow, bool isEditDescriptionShow)
    {
        return new(
            objectStorage,
            CreateToDoItemSelectorViewModel(),
            toDoCache,
            errorHandler,
            taskProgressService,
            Application.Current.ThrowIfNull().GetTopLevel().ThrowIfNull(),
            isEditShow
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
            CreateToDoSubItemsViewModel(ViewModelSortBy.LoadedIndex),
            errorHandler,
            objectStorage,
            toDoUiService,
            toDoCache
        );
    }

    public ToDoItemViewModel CreateToDoItemViewModel(ToDoItemEntityNotify item)
    {
        return new(
            item,
            objectStorage,
            CreateToDoSubItemsViewModel(ViewModelSortBy.OrderIndex),
            errorHandler,
            toDoUiService
        );
    }

    public AddPasswordItemViewModel CreateAddPasswordItemViewModel()
    {
        return new(CreateEditPasswordItemViewModel(false));
    }

    public LoginViewModel CreateLoginViewModel()
    {
        return new(objectStorage, propertyValidator);
    }

    public RootToDoItemsViewModel CreateRootToDoItemsViewModel()
    {
        return new(
            serviceFactory.CreateService<SpravyCommandNotifyService>(),
            CreateToDoSubItemsViewModel(ViewModelSortBy.OrderIndex),
            objectStorage,
            errorHandler,
            toDoUiService,
            toDoCache
        );
    }

    public TodayToDoItemsViewModel CreateTodayToDoItemsViewModel()
    {
        return new(
            CreateToDoSubItemsViewModel(ViewModelSortBy.LoadedIndex),
            serviceFactory.CreateService<SpravyCommandNotifyService>(),
            toDoUiService,
            toDoCache
        );
    }

    public SearchToDoItemsViewModel CreateSearchToDoItemsViewModel()
    {
        return new(
            CreateToDoSubItemsViewModel(ViewModelSortBy.LoadedIndex),
            serviceFactory.CreateService<SpravyCommandNotifyService>(),
            errorHandler,
            objectStorage,
            toDoUiService,
            toDoCache
        );
    }

    public PasswordGeneratorViewModel CreatePasswordGeneratorViewModel()
    {
        return new(passwordService, passwordItemCache, appOptions);
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
            this,
            soundSettingsNotify
        );
    }

    public TimersViewModel CreateTimersViewModel()
    {
        return new(scheduleService);
    }

    public PolicyViewModel CreatePolicyViewModel()
    {
        return new(application, clipboardService, errorHandler, taskProgressService);
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
                    CreateToDoItemSelectorViewModel(
                        i.Parent.ToOption(),
                        new[]
                        {
                            i,
                        }
                    ),
                    toDoService
                );
            }

            return new(item, items, CreateToDoItemSelectorViewModel(item, items), toDoService);
        }

        return new(item, items, CreateToDoItemSelectorViewModel(item, items), toDoService);
    }

    public DeleteAccountViewModel CreateDeleteAccountViewModel(string emailOrLogin, UserIdentifierType identifierType)
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

    public ForgotPasswordViewModel CreateForgotPasswordViewModel(string emailOrLogin, UserIdentifierType identifierType)
    {
        return new(emailOrLogin, identifierType);
    }

    public InfoViewModel CreateInfoViewModel(IDialogable content, Func<IDialogable, Cvtar> okTask)
    {
        return new(content, okTask, errorHandler, taskProgressService);
    }

    public PasswordItemSettingsViewModel CreatePasswordItemSettingsViewModel(PasswordItemEntityNotify item)
    {
        return new(item, CreateEditPasswordItemViewModel(false), passwordService);
    }

    public DeleteTimerViewModel CreateDeleteTimerViewModel(TimerItemNotify item)
    {
        return new(item);
    }

    public EditPasswordItemViewModel CreateEditPasswordItemViewModel(bool isEditShow)
    {
        return new(isEditShow);
    }

    public AddTimerViewModel CreateAddTimerViewModel()
    {
        return new(
            this,
            objectStorage,
            errorHandler,
            taskProgressService,
            scheduleService
        );
    }

    public ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(
            item,
            items,
            toDoCache,
            toDoUiService,
            errorHandler,
            taskProgressService
        );
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
                    CreateToDoItemSelectorViewModel(
                        i.Parent.ToOption(),
                        new[]
                        {
                            i,
                        }
                    )
                );
            }

            return new(item, items, toDoService, CreateToDoItemSelectorViewModel(item, items));
        }

        return new(item, items, toDoService, CreateToDoItemSelectorViewModel(item, items));
    }

    public ConfirmViewModel CreateConfirmViewModel(
        IDialogable content,
        Func<IDialogable, Cvtar> confirmTask,
        Func<IDialogable, Cvtar> cancelTask
    )
    {
        return new(
            content,
            errorHandler,
            taskProgressService,
            confirmTask,
            cancelTask
        );
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
        return new(item, items, clipboardService, toDoUiService);
    }

    public ChangeToDoItemOrderIndexViewModel CreateChangeToDoItemOrderIndexViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(
            item,
            items,
            toDoService,
            toDoUiService,
            toDoCache
        );
    }

    public TextViewModel CreateTextViewModel()
    {
        return new();
    }

    public ErrorViewModel CreateErrorViewModel(ReadOnlyMemory<Error> errors)
    {
        return new(errors.ToArray(), clipboardService, errorHandler, taskProgressService);
    }

    public ExceptionViewModel CreateExceptionViewModel(Exception exception)
    {
        return new(exception, clipboardService, errorHandler, taskProgressService);
    }

    public DeleteToDoItemViewModel CreateDeleteToDoItemViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(item, items, toDoService, toDoUiService);
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

    public DeletePasswordItemViewModel CreateDeletePasswordItemViewModel(PasswordItemEntityNotify item)
    {
        return new(item);
    }

    public ToDoSubItemsViewModel CreateToDoSubItemsViewModel(ViewModelSortBy viewModelSortBy)
    {
        return new(toDoCache, CreateMultiToDoItemsViewModel(viewModelSortBy), toDoUiService);
    }

    public ToDoItemsViewModel CreateToDoItemsViewModel(ViewModelSortBy viewModelSortBy, TextLocalization header)
    {
        return new(viewModelSortBy, header, errorHandler, taskProgressService);
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

    public MultiToDoItemsViewModel CreateMultiToDoItemsViewModel(ViewModelSortBy viewModelSortBy)
    {
        return new(
            CreateToDoItemsViewModel(ViewModelSortBy.LoadedIndex, new("Lang.Favorite")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.Items")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemStatus.Missed")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemStatus.ReadyForCompleted")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemStatus.Planned")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemStatus.Completed")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemType.Value")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemType.Group")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemType.Planned")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemType.Periodicity")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemType.PeriodicityOffset")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemType.Circle")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemType.Step")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemType.Reference")),
            CreateToDoItemsViewModel(viewModelSortBy, new("Lang.ToDoItemStatus.ComingSoon"))
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
}