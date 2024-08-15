using Spravy.Core.Helpers;

namespace Spravy.Ui.Helpers;

public static class UiHelper
{
    public const bool True = true;
    public const bool False = false;

    public static bool IsDrag;
    public const string ToDoItemEntityNotifyDataFormat = "to-do-item-entity-notify";
    public static object? DragData;
    public static Control? DragControl;
    public static Panel? DragPanel;

    static UiHelper()
    {
        var commands = DiHelper.ServiceFactory.CreateService<SpravyCommandService>();
        var commandsNotify = DiHelper.ServiceFactory.CreateService<SpravyCommandNotifyService>();
        NavigateToToDoItem = commands.NavigateToToDoItem.Command;
        Complete = commands.Complete.Command;
        RemoveFromFavorite = commands.RemoveFromFavorite.Command;
        AddToFavorite = commands.AddToFavorite.Command;
        OpenLink = commands.OpenLink.Command;
        SendNewVerificationCode = commands.SendNewVerificationCode.Command;
        VerificationCodeViewModelInitialized = commands
            .VerificationCodeViewModelInitialized
            .Command;
        NavigateToCurrentToDoItem = commandsNotify.NavigateToCurrentToDoItem;
        Back = commands.Back.Command;
        SwitchPane = commands.SwitchPane.Command;
        AddRootToDoItem = commands.AddRootToDoItem.Command;
        Logout = commands.Logout.Command;
        RefreshCurrentView = commands.RefreshCurrentView.Command;
        SetToDoItemDescription = commands.SetToDoItemDescription.Command;
        AddPasswordItem = commands.AddPasswordItem.Command;
        ShowPasswordItemSetting = commands.ShowPasswordItemSetting.Command;
        NavigateToActiveToDoItem = commands.NavigateToActiveToDoItem.Command;
        ForgotPasswordViewInitialized = commands.ForgotPasswordViewInitialized.Command;
        ForgotPassword = commands.ForgotPassword.Command;
        CreateUserViewEnter = commands.CreateUserViewEnter.Command;
        CreateUser = commands.CreateUser.Command;
        LoginViewInitialized = commands.LoginViewInitialized.Command;
        LoginViewEnter = commands.LoginViewEnter.Command;
        Login = commands.Login.Command;
        PasswordGeneratorViewInitialized = commands.PasswordGeneratorViewInitialized.Command;
        GeneratePassword = commands.GeneratePassword.Command;
        DeletePasswordItem = commands.DeletePasswordItem.Command;
        NavigateToRootToDoItems = commands.NavigateToRootToDoItems.Command;
        NavigateToTodayToDoItems = commands.NavigateToTodayToDoItems.Command;
        NavigateToSearchToDoItems = commands.NavigateToSearchToDoItems.Command;
        NavigateToPasswordGenerator = commands.NavigateToPasswordGenerator.Command;
        NavigateToSetting = commands.NavigateToSetting.Command;
        NavigateToCreateUser = commands.NavigateToCreateUser.Command;
        NavigateToEmailOrLoginInput = commands.NavigateToEmailOrLoginInput.Command;
        MultiToDoItemsViewInitialized = commands.MultiToDoItemsViewInitialized.Command;
        CopyToClipboard = commands.CopyToClipboard.Command;
        UpdateEmail = commands.UpdateEmail.Command;
        VerificationEmail = commands.VerificationEmail.Command;
    }

    public static readonly ICommand UpdateEmail;
    public static readonly ICommand VerificationEmail;
    public static readonly ICommand VerificationCodeViewModelInitialized;
    public static readonly ICommand Back;
    public static readonly ICommand CopyToClipboard;
    public static readonly ICommand NavigateToToDoItem;
    public static readonly ICommand Complete;
    public static readonly ICommand RemoveFromFavorite;
    public static readonly ICommand AddToFavorite;
    public static readonly ICommand OpenLink;
    public static readonly ICommand SendNewVerificationCode;
    public static readonly ICommand NavigateToRootToDoItems;
    public static readonly ICommand NavigateToTodayToDoItems;
    public static readonly ICommand NavigateToSearchToDoItems;
    public static readonly ICommand NavigateToPasswordGenerator;
    public static readonly ICommand NavigateToEmailOrLoginInput;
    public static readonly ICommand NavigateToCreateUser;
    public static readonly ICommand NavigateToSetting;
    public static readonly ICommand SwitchPane;
    public static readonly ICommand AddRootToDoItem;
    public static readonly ICommand Logout;
    public static readonly ICommand RefreshCurrentView;
    public static readonly ICommand SetToDoItemDescription;
    public static readonly ICommand AddPasswordItem;
    public static readonly ICommand ShowPasswordItemSetting;
    public static readonly ICommand ForgotPasswordViewInitialized;
    public static readonly ICommand ForgotPassword;
    public static readonly ICommand NavigateToActiveToDoItem;
    public static readonly ICommand CreateUserViewEnter;
    public static readonly ICommand CreateUser;
    public static readonly ICommand LoginViewInitialized;
    public static readonly ICommand LoginViewEnter;
    public static readonly ICommand Login;
    public static readonly ICommand PasswordGeneratorViewInitialized;
    public static readonly ICommand GeneratePassword;
    public static readonly ICommand DeletePasswordItem;
    public static readonly ICommand MultiToDoItemsViewInitialized;
    public static readonly SpravyCommandNotify NavigateToCurrentToDoItem;

    public static ReadOnlyMemory<ToDoItemStatus> ToDoItemStatuses =
        new(
            [
                ToDoItemStatus.Miss,
                ToDoItemStatus.ReadyForComplete,
                ToDoItemStatus.Planned,
                ToDoItemStatus.Completed
            ]
        );

    public static ReadOnlyMemory<ToDoItemType> ToDoItemTypes =
        new(
            [
                ToDoItemType.Value,
                ToDoItemType.Circle,
                ToDoItemType.Group,
                ToDoItemType.Periodicity,
                ToDoItemType.Reference,
                ToDoItemType.Planned,
                ToDoItemType.Step,
                ToDoItemType.PeriodicityOffset
            ]
        );

    public static ReadOnlyMemory<DayOfWeek> DayOfWeeks =
        new(
            [
                DayOfWeek.Monday,
                DayOfWeek.Thursday,
                DayOfWeek.Wednesday,
                DayOfWeek.Tuesday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            ]
        );

    public static ReadOnlyMemory<GroupBy> GroupBys =
        new([GroupBy.None, GroupBy.ByStatus, GroupBy.ByType,]);

    public static ReadOnlyMemory<ToDoItemViewType> ToDoItemViewTypes =
        new([ToDoItemViewType.List, ToDoItemViewType.Card,]);

    public static ReadOnlyMemory<DescriptionType> DescriptionTypes =
        new([DescriptionType.PlainText, DescriptionType.Markdown,]);

    public static ReadOnlyMemory<ToDoItemChildrenType> ToDoItemChildrenTypes =
        new([ToDoItemChildrenType.RequireCompletion, ToDoItemChildrenType.IgnoreCompletion,]);

    public static ReadOnlyMemory<TypeOfPeriodicity> TypeOfPeriodicitys =
        new(
            [
                TypeOfPeriodicity.Daily,
                TypeOfPeriodicity.Weekly,
                TypeOfPeriodicity.Monthly,
                TypeOfPeriodicity.Annually,
            ]
        );

    public static ReadOnlyMemory<ThemeType> ThemeTypes =
        new([ThemeType.Default, ThemeType.Light, ThemeType.Dark,]);

    public static ReadOnlyMemory<object> GetEnumValues(Type type)
    {
        if (typeof(ToDoItemStatus) == type)
        {
            return new(ToDoItemStatuses.ToArray().OfType<object>().ToArray());
        }

        if (typeof(ToDoItemType) == type)
        {
            return new(ToDoItemTypes.ToArray().OfType<object>().ToArray());
        }

        if (typeof(DayOfWeek) == type)
        {
            return new(DayOfWeeks.ToArray().OfType<object>().ToArray());
        }

        if (typeof(GroupBy) == type)
        {
            return new(GroupBys.ToArray().OfType<object>().ToArray());
        }

        if (typeof(ToDoItemViewType) == type)
        {
            return new(ToDoItemViewTypes.ToArray().OfType<object>().ToArray());
        }

        if (typeof(DescriptionType) == type)
        {
            return new(DescriptionTypes.ToArray().OfType<object>().ToArray());
        }

        if (typeof(ToDoItemChildrenType) == type)
        {
            return new(ToDoItemChildrenTypes.ToArray().OfType<object>().ToArray());
        }

        if (typeof(ThemeType) == type)
        {
            return new(ThemeTypes.ToArray().OfType<object>().ToArray());
        }

        if (typeof(TypeOfPeriodicity) == type)
        {
            return new(TypeOfPeriodicitys.ToArray().OfType<object>().ToArray());
        }

        throw new ArgumentOutOfRangeException(type.ToString());
    }
}
