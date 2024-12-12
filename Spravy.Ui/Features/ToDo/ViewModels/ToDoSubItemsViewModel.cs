namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase, IToDoItemsView
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;
    private readonly AppOptions appOptions;

    public ToDoSubItemsViewModel(
        IToDoService toDoService,
        IToDoCache toDoCache,
        MultiToDoItemsViewModel list,
        AppOptions appOptions
    )
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        this.appOptions = appOptions;
        List = list;
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
        return toDoCache.GetFavoriteItems()
           .IfSuccessAsync(items => this.InvokeUiBackgroundAsync(() => List.SetFavoriteItemsUi(items)), ct)
           .IfSuccessAsync(() => toDoService.GetFavoriteToDoItemIdsAsync(ct), ct)
           .IfSuccessAsync(
                items => items.ToResult()
                   .IfSuccessForEach(toDoCache.GetToDoItem)
                   .IfSuccessAsync(
                        itemsNotify => List.SetFavoriteItemsUi(itemsNotify)
                           .IfSuccessAsync(
                                () => toDoService
                                   .GetToDoItemsAsync(itemsNotify.Select(x => x.Id), appOptions.ToDoItemsChunkSize, ct)
                                   .IfSuccessForEachAsync(
                                        fullItems => this.InvokeUiBackgroundAsync(
                                            () => fullItems
                                               .IfSuccessForEach(updatedItem => toDoCache.UpdateUi(updatedItem))
                                               .IfSuccess(i => List.AddOrUpdateFavoriteUi(i))
                                        ),
                                        ct
                                    ),
                                ct
                            ),
                        ct
                    ),
                ct
            );
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