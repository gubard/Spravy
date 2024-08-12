namespace Spravy.Ui.Interfaces;

public interface IViewFactory
{
    ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel();
    TextViewModel CreateTextViewModel();
    ErrorViewModel CreateErrorViewModel(ReadOnlyMemory<Error> errors);
    ExceptionViewModel CreateExceptionViewModel(Exception exception);
    ResetToDoItemViewModel CreateResetToDoItemViewModel(ToDoItemEntityNotify item);
    DeleteToDoItemViewModel CreateDeleteToDoItemViewModel(ToDoItemEntityNotify item);
    AddToDoItemViewModel CreateAddToDoItemViewModel(ToDoItemEntityNotify parent);
    ToDoItemContentViewModel CreateToDoItemContentViewModel();
    EditDescriptionContentViewModel CreateEditDescriptionContentViewModel();
    DeletePasswordItemViewModel CreateDeletePasswordItemViewModel(PasswordItemEntityNotify item);
    MultiToDoItemSettingViewModel CreateMultiToDoItemSettingViewModel(ToDoItemEntityNotify item);
    ToDoItemSettingsViewModel CreateToDoItemSettingsViewModel(ToDoItemEntityNotify item);
    ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(ToDoItemEntityNotify item);
    AddRootToDoItemViewModel CreateAddRootToDoItemViewModel();
    AddPasswordItemViewModel CreateAddPasswordItemViewModel();
    ValueToDoItemSettingsViewModel CreateValueToDoItemSettingsViewModel(ToDoItemEntityNotify item);
    GroupToDoItemSettingsViewModel CreateGroupToDoItemSettingsViewModel();

    ToDoItemDayOfWeekSelectorViewModel CreateToDoItemDayOfWeekSelectorViewModel(
        ToDoItemEntityNotify item
    );

    ToDoItemDayOfMonthSelectorViewModel CreateToDoItemDayOfMonthSelectorViewModel(
        ToDoItemEntityNotify item
    );

    ToDoItemDayOfYearSelectorViewModel CreateToDoItemDayOfYearSelectorViewModel(
        ToDoItemEntityNotify item
    );

    PeriodicityOffsetToDoItemSettingsViewModel CreatePeriodicityOffsetToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    );

    PeriodicityToDoItemSettingsViewModel CreatePeriodicityToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    );

    PlannedToDoItemSettingsViewModel CreatePlannedToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    );

    InfoViewModel CreateInfoViewModel(
        object content,
        Func<object, ConfiguredValueTaskAwaitable<Result>> okTask
    );

    PasswordItemSettingsViewModel CreatePasswordItemSettingsViewModel(
        PasswordItemEntityNotify item
    );

    ReferenceToDoItemSettingsViewModel CreateReferenceToDoItemSettingsViewModel(
        ToDoItemEntityNotify item
    );

    ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems
    );

    ConfirmViewModel CreateConfirmViewModel(
        object content,
        Func<object, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<object, ConfiguredValueTaskAwaitable<Result>> cancelTask
    );

    RandomizeChildrenOrderViewModel CreateRandomizeChildrenOrderViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    RandomizeChildrenOrderViewModel CreateRandomizeChildrenOrderViewModel(
        ToDoItemEntityNotify item
    );

    ToDoItemToStringSettingsViewModel CreateToDoItemToStringSettingsViewModel(
        ToDoItemEntityNotify item
    );

    DeleteToDoItemViewModel CreateDeleteToDoItemViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    ChangeToDoItemOrderIndexViewModel CreateChangeToDoItemOrderIndexViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );
}
