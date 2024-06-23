using Spravy.Core.Helpers;

namespace Spravy.Ui.Helpers;

public static class UiHelper
{
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
        NavigateToRootToDoItems = commands.GetNavigateTo<RootToDoItemsViewModel>().Command;
        NavigateToTodayToDoItems = commands.GetNavigateTo<TodayToDoItemsViewModel>().Command;
        NavigateToSearchToDoItems = commands.GetNavigateTo<SearchToDoItemsViewModel>().Command;
        NavigateToPasswordGenerator = commands.GetNavigateTo<PasswordGeneratorViewModel>().Command;
        NavigateToSetting = commands.GetNavigateTo<SettingViewModel>().Command;
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
    public static readonly ICommand NavigateToSetting;
    public static readonly ICommand SwitchPane;
    public static readonly ICommand AddRootToDoItem;
    public static readonly SpravyCommandNotify NavigateToCurrentToDoItem;
}