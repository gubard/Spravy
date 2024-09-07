using EditDescriptionContentViewModel = Spravy.Ui.Features.ToDo.ViewModels.EditDescriptionContentViewModel;

namespace Spravy.Ui.Interfaces;

public interface IViewFactory
{
    AddTimerViewModel CreateAddTimerViewModel();
    ToDoItemCreateTimerViewModel CreateToDoItemCreateTimerViewModel(ToDoItemEntityNotify item);
    ToDoItemsViewModel CreateToDoItemsViewModel(SortBy sortBy, TextLocalization header);
    DeleteTimerViewModel CreateDeleteTimerViewModel(TimerItemNotify item);
    ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel();
    MultiToDoItemsViewModel CreateMultiToDoItemsViewModel(SortBy sortBy);
    TextViewModel CreateTextViewModel();
    ErrorViewModel CreateErrorViewModel(ReadOnlyMemory<Error> errors);
    ExceptionViewModel CreateExceptionViewModel(Exception exception);
    ResetToDoItemViewModel CreateResetToDoItemViewModel(ToDoItemEntityNotify item);
    ResetToDoItemViewModel CreateResetToDoItemViewModel(ReadOnlyMemory<ToDoItemEntityNotify> items);
    DeleteToDoItemViewModel CreateDeleteToDoItemViewModel(ToDoItemEntityNotify item);
    AddToDoItemViewModel CreateAddToDoItemViewModel(ToDoItemEntityNotify parent);
    AddToDoItemViewModel CreateAddToDoItemViewModel();
    ToDoItemContentViewModel CreateToDoItemContentViewModel();
    DeletePasswordItemViewModel CreateDeletePasswordItemViewModel(PasswordItemEntityNotify item);
    MultiToDoItemSettingViewModel CreateMultiToDoItemSettingViewModel();
    ToDoItemSettingsViewModel CreateToDoItemSettingsViewModel(ToDoItemEntityNotify item);
    ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(ToDoItemEntityNotify item);
    ToDoItemViewModel CreateToDoItemViewModel(ToDoItemEntityNotify item);
    AddPasswordItemViewModel CreateAddPasswordItemViewModel();
    ValueToDoItemSettingsViewModel CreateValueToDoItemSettingsViewModel(ToDoItemEntityNotify item);
    EditDescriptionContentViewModel CreateEditDescriptionContentViewModel();
    EditDescriptionViewModel CreateEditDescriptionViewModel(ToDoItemEntityNotify item);
    ToDoSubItemsViewModel CreateToDoSubItemsViewModel(SortBy sortBy);
    LoginViewModel CreateLoginViewModel();
    RootToDoItemsViewModel CreateRootToDoItemsViewModel();
    TodayToDoItemsViewModel CreateTodayToDoItemsViewModel();
    SearchToDoItemsViewModel CreateSearchToDoItemsViewModel();
    PasswordGeneratorViewModel CreatePasswordGeneratorViewModel();
    SettingViewModel CreateSettingViewModel();
    TimersViewModel CreateTimersViewModel();
    PolicyViewModel CreatePolicyViewModel();
    EmailOrLoginInputViewModel CreateEmailOrLoginInputViewModel();
    CreateUserViewModel CreateCreateUserViewModel();
    LeafToDoItemsViewModel CreateLeafToDoItemsViewModel(ReadOnlyMemory<ToDoItemEntityNotify> items);
    LeafToDoItemsViewModel CreateLeafToDoItemsViewModel(ToDoItemEntityNotify item);
    AddToDoItemToFavoriteEventViewModel CreateAddToDoItemToFavoriteEventViewModel();

    DeleteToDoItemViewModel CreateDeleteToDoItemViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    LeafToDoItemsViewModel CreateLeafToDoItemsViewModel(
        ToDoItemEntityNotify item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    DeleteAccountViewModel CreateDeleteAccountViewModel(
        string emailOrLogin,
        UserIdentifierType identifierType
    );

    VerificationCodeViewModel CreateVerificationCodeViewModel(
        string emailOrLogin,
        UserIdentifierType identifierType
    );

    ForgotPasswordViewModel CreateForgotPasswordViewModel(
        string emailOrLogin,
        UserIdentifierType identifierType
    );

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

    InfoViewModel CreateInfoViewModel(IDialogable content, Func<IDialogable, Cvtar> okTask);

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

    ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems
    );

    ConfirmViewModel CreateConfirmViewModel(
        IDialogable content,
        Func<IDialogable, Cvtar> confirmTask,
        Func<IDialogable, Cvtar> cancelTask
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

    ToDoItemToStringSettingsViewModel CreateToDoItemToStringSettingsViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    DeleteToDoItemViewModel CreateDeleteToDoItemViewModel(
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
