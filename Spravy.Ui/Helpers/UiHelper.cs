using Spravy.Core.Helpers;

namespace Spravy.Ui.Helpers;

public static class UiHelper
{
    public static bool IsDrag;
    public const string ToDoItemEntityNotifyDataFormat = "to-do-item-entity-notify";
    public const uint ChunkSize = 10;
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
        AddToDoItemViewInitialized = commands.AddToDoItemViewInitialized.Command;
        AddRootToDoItemViewInitialized = commands.AddRootToDoItemViewInitialized.Command;
        PasswordItemSettingsViewInitialized = commands.PasswordItemSettingsViewInitialized.Command;
        PasswordGeneratorViewInitialized = commands.PasswordGeneratorViewInitialized.Command;
        DeletePasswordItemViewInitialized = commands.DeletePasswordItemViewInitialized.Command;
        GeneratePassword = commands.GeneratePassword.Command;
        DeletePasswordItem = commands.DeletePasswordItem.Command;
        NavigateToRootToDoItems = commands.GetNavigateTo<RootToDoItemsViewModel>().Command;
        NavigateToTodayToDoItems = commands.GetNavigateTo<TodayToDoItemsViewModel>().Command;
        NavigateToSearchToDoItems = commands.GetNavigateTo<SearchToDoItemsViewModel>().Command;
        NavigateToPasswordGenerator = commands.GetNavigateTo<PasswordGeneratorViewModel>().Command;
        NavigateToSetting = commands.GetNavigateTo<SettingViewModel>().Command;
        NavigateToCreateUser = commands.GetNavigateTo<CreateUserViewModel>().Command;
        NavigateToEmailOrLoginInput = commands.GetNavigateTo<EmailOrLoginInputViewModel>().Command;
        MultiToDoItemsViewInitialized = commands.MultiToDoItemsViewInitialized.Command;
    }

    public static readonly ICommand Back;
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
    public static readonly ICommand DeletePasswordItemViewInitialized;
    public static readonly ICommand PasswordGeneratorViewInitialized;
    public static readonly ICommand PasswordItemSettingsViewInitialized;
    public static readonly ICommand AddRootToDoItemViewInitialized;
    public static readonly ICommand AddToDoItemViewInitialized;
    public static readonly ICommand GeneratePassword;
    public static readonly ICommand DeletePasswordItem;
    public static readonly ICommand MultiToDoItemsViewInitialized;
    public static readonly SpravyCommandNotify NavigateToCurrentToDoItem;
}
