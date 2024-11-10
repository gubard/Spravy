namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoSubItemsViewModel(
    IToDoService toDoService,
    IToDoCache toDoCache,
    MultiToDoItemsViewModel list,
    ITaskProgressService taskProgressService,
    AppOptions appOptions
) : ViewModelBase, IToDoItemsView
{
    public MultiToDoItemsViewModel List { get; } = list;

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
        return toDoService
            .GetFavoriteToDoItemIdsAsync(ct)
            .IfSuccessAsync(
                items =>
                    taskProgressService.RunProgressAsync(
                        (ushort)items.Length,
                        item =>
                            items
                                .ToResult()
                                .IfSuccessForEach(toDoCache.GetToDoItem)
                                .IfSuccessAsync(
                                    itemsNotify =>
                                        List.SetFavoriteItemsUi(itemsNotify)
                                            .IfSuccessAsync(
                                                () =>
                                                    toDoService
                                                        .GetToDoItemsAsync(
                                                            itemsNotify.Select(x => x.Id),
                                                            appOptions.ToDoItemsChunkSize,
                                                            ct
                                                        )
                                                        .IfSuccessForEachAsync(
                                                            fullItems =>
                                                                this.InvokeUiBackgroundAsync(
                                                                    () =>
                                                                        fullItems
                                                                            .IfSuccessForEach(
                                                                                updatedItem =>
                                                                                    toDoCache.UpdateUi(
                                                                                        updatedItem
                                                                                    )
                                                                            )
                                                                            .IfSuccess(i =>
                                                                                List.AddOrUpdateFavoriteUi(
                                                                                    i
                                                                                )
                                                                            )
                                                                ),
                                                            ct
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
