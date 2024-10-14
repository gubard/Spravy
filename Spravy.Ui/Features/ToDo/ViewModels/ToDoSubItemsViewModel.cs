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

    private async ValueTask<Result> RefreshFavoriteToDoItemsCore(
        ReadOnlyMemory<ToDoItemEntityNotify> ids,
        TaskProgressItem progressItem,
        CancellationToken ct
    )
    {
        await foreach (
            var items in toDoService
                .GetToDoItemsAsync(ids.Select(x => x.Id), appOptions.ToDoItemsChunkSize, ct)
                .ConfigureAwait(false)
        )
        {
            if (!items.TryGetValue(out var value))
            {
                return new(items.Errors);
            }

            for (var index = 0; index < value.Length; index++)
            {
                var item = value.Span[index];
                var i = await this.InvokeUiBackgroundAsync(() => toDoCache.UpdateUi(item));

                if (!i.TryGetValue(out var t))
                {
                    return new(i.Errors);
                }

                var result = this.PostUiBackground(
                    () => List.UpdateFavoriteItemUi(t).IfSuccess(progressItem.IncreaseUi),
                    ct
                );

                if (result.IsHasError)
                {
                    return result;
                }
            }
        }

        return Result.Success;
    }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return List.ClearExceptUi(items);
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        return List.AddOrUpdateUi(item);
    }

    public Result RemoveUi(ToDoItemEntityNotify item)
    {
        return List.RemoveUi(item);
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
                                        List.ClearFavoriteExceptUi(itemsNotify)
                                            .IfSuccessAsync(
                                                () =>
                                                    RefreshFavoriteToDoItemsCore(
                                                            itemsNotify,
                                                            item,
                                                            ct
                                                        )
                                                        .ConfigureAwait(false),
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
        var selected = List.Items.Items.Where(x => x.IsSelected).ToArray().ToReadOnlyMemory();

        if (selected.IsEmpty)
        {
            return new(new NonItemSelectedError());
        }

        return new(selected);
    }
}
