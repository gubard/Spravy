namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase, IToDoItemsView
{
    private readonly IToDoCache toDoCache;
    private readonly IToDoUiService toDoUiService;
    
    public ToDoSubItemsViewModel(
        IToDoCache toDoCache,
        MultiToDoItemsViewModel list,
        IToDoUiService toDoUiService
    )
    {
        List = list;
        this.toDoCache = toDoCache;
        this.toDoUiService = toDoUiService;

        toDoCache.GetFavoriteItems()
           .IfSuccessAsync(
                items => this.InvokeUiBackgroundAsync(() => List.SetFavoriteItemsUi(items)),
                CancellationToken.None
            );

        toDoUiService.Requested += Requested;
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
        toDoUiService.Requested -= Requested;
        toDoUiService.Requested += Requested;

        return Result.AwaitableSuccess;
    }
    
    public Result Stop()
    {
        toDoUiService.Requested -= Requested;

        return Result.Success;
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

    private Cvtar Requested(ToDoResponse response)
    {
        return response.FavoriteItems
           .Select(x => x.Item.Id)
           .IfSuccessForEach(toDoCache.GetToDoItem)
           .IfSuccessAsync(
                items => this.InvokeUiBackgroundAsync(() => List.SetFavoriteItemsUi(items)),
                CancellationToken.None
            );
    }
}