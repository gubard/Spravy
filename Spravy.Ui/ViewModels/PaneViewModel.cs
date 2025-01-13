namespace Spravy.Ui.ViewModels;

public class PaneViewModel : ViewModelBase, IBookmarksToDoItemsView
{
    public PaneViewModel(AccountNotify account, IToDoUiService toDoUiService, IToDoCache toDoCache)
    {
        Account = account;
        Bookmarks = new();

        toDoUiService.Requested += response =>
        {
            if (!response.BookmarkItems.IsResponse)
            {
                return Result.AwaitableSuccess;
            }

            return response.BookmarkItems
               .Items
               .Select(x => x.Id)
               .IfSuccessForEach(toDoCache.GetToDoItem)
               .IfSuccess(items => this.PostUiBackground(() => SetBookmarksUi(items), CancellationToken.None))
               .GetAwaitable();
        };
    }

    public AccountNotify Account { get; }
    public AvaloniaList<ToDoItemEntityNotify> Bookmarks { get; }

    public Result SetBookmarksUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Bookmarks.UpdateUi(items).IfSuccess(() => Bookmarks.BinarySortByLoadedIndex());
    }
}