namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase, IToDoItemsView
{
    public ToDoSubItemsViewModel(
        IToDoCache toDoCache,
        MultiToDoItemsViewModel list,
        IToDoUiService toDoUiService
    )
    {
        List = list;

        toDoCache.GetFavoriteItems()
           .IfSuccessAsync(
                items => this.InvokeUiBackgroundAsync(() => List.SetFavoriteItemsUi(items)),
                CancellationToken.None
            );

        toDoUiService.Response += response => response.FavoriteItems
           .Select(x => x.Item.Id)
           .IfSuccessForEach(toDoCache.GetToDoItem)
           .IfSuccessAsync(
                items => this.InvokeUiBackgroundAsync(() => List.SetFavoriteItemsUi(items)),
                CancellationToken.None
            );
    }

    public MultiToDoItemsViewModel List { get; }

    public Result SetItemsUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return List.SetItemsUi(items);
    }

    public Result AddOrUpdateUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return List.AddOrUpdateUi(items);
    }

    public Result RemoveUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return List.RemoveUi(items);
    }

    public Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetSelectedItems()
    {
        var selected = List.Items.ToDoItems.Where(x => x.IsSelected).ToArray().ToReadOnlyMemory();

        if (selected.IsEmpty)
        {
            return new(new NonItemSelectedError());
        }

        return new(selected);
    }
}