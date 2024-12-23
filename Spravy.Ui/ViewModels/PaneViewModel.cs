namespace Spravy.Ui.ViewModels;

public class PaneViewModel : ViewModelBase, IBookmarksToDoItemsView
{
    public PaneViewModel(AccountNotify account)
    {
        Account = account;
        Bookmarks = new();
        Account.PropertyChanged += OnPropertyChanged;
    }

    public AccountNotify Account { get; }
    public AvaloniaList<ToDoItemEntityNotify> Bookmarks { get; }

    public bool IsShowPasswordGenerator => Account.Login is "vafnir" or "admin";

    public Result ClearBookmarksExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
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