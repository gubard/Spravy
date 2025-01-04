namespace Spravy.Ui.ViewModels;

public class PaneViewModel : ViewModelBase, IBookmarksToDoItemsView
{
    public PaneViewModel(AccountNotify account, IToDoUiService toDoUiService, IToDoCache toDoCache)
    {
        Account = account;
        Bookmarks = new();
        Account.PropertyChanged += OnPropertyChanged;

        toDoUiService.Requested += response => response.BookmarkItems
           .Items
           .Select(x => x.Id)
           .IfSuccessForEach(toDoCache.GetToDoItem)
           .IfSuccess(items => this.PostUiBackground(() => SetBookmarksUi(items), CancellationToken.None))
           .GetAwaitable();
    }

    public AccountNotify Account { get; }
    public AvaloniaList<ToDoItemEntityNotify> Bookmarks { get; }

    public bool IsShowPasswordGenerator => Account.Login is "vafnir" or "admin";

    public Result SetBookmarksUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Bookmarks.UpdateUi(items).IfSuccess(x => x.BinarySortByLoadedIndex());
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Account.Login))
        {
            OnPropertyChanged(nameof(IsShowPasswordGenerator));
        }
    }
}