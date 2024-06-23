using Spravy.Core.Helpers;

namespace Spravy.Ui.Helpers;

public static class UiHelper
{
    static UiHelper()
    {
        var commands = DiHelper.ServiceFactory.CreateService<SpravyCommandService>();
        NavigateToToDoItem = commands.NavigateToToDoItem.Command;
        Complete = commands.Complete.Command;
        RemoveFromFavorite = commands.RemoveFromFavorite.Command;
        AddToFavorite = commands.AddToFavorite.Command;
        OpenLink = commands.OpenLink.Command;
        SendNewVerificationCode = commands.SendNewVerificationCode.Command;
    }

    public static readonly ICommand NavigateToToDoItem;
    public static readonly ICommand Complete;
    public static readonly ICommand RemoveFromFavorite;
    public static readonly ICommand AddToFavorite;
    public static readonly ICommand OpenLink;
    public static readonly ICommand SendNewVerificationCode;
}