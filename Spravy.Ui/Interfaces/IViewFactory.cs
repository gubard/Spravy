using EditDescriptionContentViewModel = Spravy.Ui.Features.ToDo.ViewModels.EditDescriptionContentViewModel;

namespace Spravy.Ui.Interfaces;

public interface IViewFactory
{
    ToDoItemsGroupByNoneViewModel CreateToDoItemsGroupByNoneViewModel();
    ToDoItemsGroupByStatusViewModel CreateToDoItemsGroupByStatusViewModel();
    ToDoItemsGroupByTypeViewModel CreateToDoItemsGroupByTypeViewModel();
    ToDoItemsViewModel CreateToDoItemsViewModel();
    ToDoItemsGroupByViewModel CreateToDoItemsGroupByViewModel();
    ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel();
    MultiToDoItemsViewModel CreateMultiToDoItemsViewModel();
    TextViewModel CreateTextViewModel();
    ErrorViewModel CreateErrorViewModel(ReadOnlyMemory<Error> errors);
    ExceptionViewModel CreateExceptionViewModel(Exception exception);
    ResetToDoItemViewModel CreateResetToDoItemViewModel(ToDoItemEntityNotify item);
    DeleteToDoItemViewModel CreateDeleteToDoItemViewModel(ToDoItemEntityNotify item);
    AddToDoItemViewModel CreateAddToDoItemViewModel(ToDoItemEntityNotify parent);
    ToDoItemContentViewModel CreateToDoItemContentViewModel();
    DeletePasswordItemViewModel CreateDeletePasswordItemViewModel(PasswordItemEntityNotify item);
    MultiToDoItemSettingViewModel CreateMultiToDoItemSettingViewModel(ToDoItemEntityNotify item);
    ToDoItemSettingsViewModel CreateToDoItemSettingsViewModel(ToDoItemEntityNotify item);
    ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(ToDoItemEntityNotify item);
    ToDoItemViewModel CreateToDoItemViewModel(ToDoItemEntityNotify item);
    AddRootToDoItemViewModel CreateAddRootToDoItemViewModel();
    AddPasswordItemViewModel CreateAddPasswordItemViewModel();
    ValueToDoItemSettingsViewModel CreateValueToDoItemSettingsViewModel(ToDoItemEntityNotify item);
    EditDescriptionContentViewModel CreateEditDescriptionContentViewModel();
    EditDescriptionViewModel CreateEditDescriptionViewModel(ToDoItemEntityNotify item);
    ToDoSubItemsViewModel CreateToDoSubItemsViewModel();

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
        ToDoItemEntityNotify item
    );

    ChangeToDoItemOrderIndexViewModel CreateChangeToDoItemOrderIndexViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    ChangeToDoItemOrderIndexViewModel CreateChangeToDoItemOrderIndexViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );
}
