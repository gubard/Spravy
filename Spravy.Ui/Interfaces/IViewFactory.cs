namespace Spravy.Ui.Interfaces;

public interface IViewFactory
{
    AddTimerViewModel CreateAddTimerViewModel();
    DeleteTimerViewModel CreateDeleteTimerViewModel(TimerItemNotify item);
    TextViewModel CreateTextViewModel();
    ErrorViewModel CreateErrorViewModel(ReadOnlyMemory<Error> errors);
    ExceptionViewModel CreateExceptionViewModel(Exception exception);
    DeletePasswordItemViewModel CreateDeletePasswordItemViewModel(PasswordItemEntityNotify item);
    ToDoItemViewModel CreateToDoItemViewModel(ToDoItemEntityNotify item);
    AddPasswordItemViewModel CreateAddPasswordItemViewModel();
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
    InfoViewModel CreateInfoViewModel(IDialogable content, Func<IDialogable, Cvtar> okTask);
    AddToDoItemToFavoriteEventViewModel CreateAddToDoItemToFavoriteEventViewModel();
    EditToDoItemViewModel CreateEditToDoItemViewModel(bool isEditShow, bool isEditDescriptionShow);

    ToDoItemSettingsViewModel CreateToDoItemSettingsViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    ToDoItemCreateTimerViewModel CreateToDoItemCreateTimerViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    EditDescriptionViewModel CreateEditDescriptionViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    CloneViewModel CreateCloneViewModel(Option<ToDoItemEntityNotify> item, ReadOnlyMemory<ToDoItemEntityNotify> items);

    ResetToDoItemViewModel CreateResetToDoItemViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    ToDoItemSelectorViewModel CreateToDoItemSelectorViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    ChangeParentViewModel CreateChangeParentViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    DeleteToDoItemViewModel CreateDeleteToDoItemViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    AddToDoItemViewModel CreateAddToDoItemViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    LeafToDoItemsViewModel CreateLeafToDoItemsViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    CreateReferenceViewModel CreateCreateReferenceViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    DeleteAccountViewModel CreateDeleteAccountViewModel(string emailOrLogin, UserIdentifierType identifierType);

    VerificationCodeViewModel CreateVerificationCodeViewModel(string emailOrLogin, UserIdentifierType identifierType);

    ForgotPasswordViewModel CreateForgotPasswordViewModel(string emailOrLogin, UserIdentifierType identifierType);

    PasswordItemSettingsViewModel CreatePasswordItemSettingsViewModel(PasswordItemEntityNotify item);

    ConfirmViewModel CreateConfirmViewModel(
        IDialogable content,
        Func<IDialogable, Cvtar> confirmTask,
        Func<IDialogable, Cvtar> cancelTask
    );

    RandomizeChildrenOrderViewModel CreateRandomizeChildrenOrderViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    ToDoItemToStringSettingsViewModel CreateToDoItemToStringSettingsViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );

    ChangeToDoItemOrderIndexViewModel CreateChangeToDoItemOrderIndexViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    );
}