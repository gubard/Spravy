namespace Spravy.Ui.Services;

public class ViewFactory : IViewFactory
{
    private readonly IToDoService toDoService;
    private readonly IErrorHandler errorHandler;
    private readonly ITaskProgressService taskProgressService;
    private readonly IToDoUiService toDoUiService;
    private readonly IToDoCache toDoCache;
    private readonly IObjectStorage objectStorage;

    public ViewFactory(
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService,
        IToDoCache toDoCache,
        IObjectStorage objectStorage
    )
    {
        this.toDoService = toDoService;
        this.errorHandler = errorHandler;
        this.taskProgressService = taskProgressService;
        this.toDoUiService = toDoUiService;
        this.toDoCache = toDoCache;
        this.objectStorage = objectStorage;
    }

    public ToDoItemSettingsViewModel CreateToDoItemSettingsViewModel(ToDoItemEntityNotify item)
    {
        return new(
            item,
            CreateToDoItemContentViewModel(),
            toDoService,
            errorHandler,
            taskProgressService,
            this
        );
    }

    public ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(ToDoItemEntityNotify item)
    {
        return CreateToDoItemSelectorViewModel(item, ReadOnlyMemory<ToDoItemEntityNotify>.Empty);
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

    public GroupToDoItemSettingsViewModel CreateGroupToDoItemSettingsViewModel()
    {
        return new();
    }

    public ToDoItemDayOfWeekSelectorViewModel CreateToDoItemDayOfWeekSelectorViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService, errorHandler, taskProgressService, toDoUiService);
    }

    public ToDoItemDayOfMonthSelectorViewModel CreateToDoItemDayOfMonthSelectorViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService, errorHandler, taskProgressService, toDoUiService);
    }

    public ToDoItemDayOfYearSelectorViewModel CreateToDoItemDayOfYearSelectorViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService, errorHandler, taskProgressService, toDoUiService);
    }

    public PeriodicityOffsetToDoItemSettingsViewModel CreatePeriodicityOffsetToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService, errorHandler, taskProgressService, toDoUiService);
    }

    public PeriodicityToDoItemSettingsViewModel CreatePeriodicityToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService, errorHandler, taskProgressService, toDoUiService, this);
    }

    public PlannedToDoItemSettingsViewModel CreatePlannedToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    )
    {
        return new(item, toDoService, errorHandler, taskProgressService, toDoUiService);
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
            toDoService,
            errorHandler,
            taskProgressService,
            toDoUiService
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
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        return new(items, errorHandler, taskProgressService, toDoUiService);
    }
}
