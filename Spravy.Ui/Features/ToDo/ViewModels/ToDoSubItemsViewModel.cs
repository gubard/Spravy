namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase
{
    private bool isSubscribe;
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
           .IfSuccess(
                items => this.PostUiBackground(() => List.SetFavoriteItemsUi(items), CancellationToken.None)
            );

        toDoUiService.Requested += Requested;
        isSubscribe = true;
    }

    public MultiToDoItemsViewModel List { get; }

    public Result SetItemsUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return List.SetItemsUi(items);
    }

    public Result RefreshUi()
    {
        if (!isSubscribe)
        {
            toDoUiService.Requested += Requested;
            isSubscribe = true;
        }

        return List.RefreshUi();
    }

    public Result Stop()
    {
        toDoUiService.Requested -= Requested;
        isSubscribe = false;

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
        if (!response.FavoriteItems.IsResponse)
        {
            return Result.AwaitableSuccess;
        }

        return response.FavoriteItems
           .Items
           .Select(x => x.Item.Id)
           .IfSuccessForEach(toDoCache.GetToDoItem)
           .IfSuccess(
                items => this.PostUiBackground(() => List.SetFavoriteItemsUi(items), CancellationToken.None)
            )
           .ToValueTaskResult()
           .ConfigureAwait(false);
    }
}