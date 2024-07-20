namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;
    private readonly ITaskProgressService taskProgressService;

    public ToDoSubItemsViewModel(
        IToDoService toDoService,
        IToDoCache toDoCache,
        MultiToDoItemsViewModel list,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        List = list;
        this.taskProgressService = taskProgressService;
    }

    public MultiToDoItemsViewModel List { get; }

    private ConfiguredValueTaskAwaitable<Result> RefreshFavoriteToDoItemsAsync(CancellationToken ct)
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
                                .IfSuccessForEach(x => toDoCache.GetToDoItem(x))
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

    private async ValueTask<Result> RefreshFavoriteToDoItemsCore(
        ReadOnlyMemory<ToDoItemEntityNotify> ids,
        TaskProgressItem progressItem,
        CancellationToken ct
    )
    {
        await foreach (
            var items in toDoService
                .GetToDoItemsAsync(ids.Select(x => x.Id), UiHelper.ChunkSize, ct)
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
                    () => List.UpdateFavoriteItemUi(t).IfSuccess(progressItem.IncreaseUi), ct
                );

                if (result.IsHasError)
                {
                    return result;
                }
            }
        }

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemListsAsync(
        ReadOnlyMemory<ToDoItemEntityNotify> entities,
        bool autoOrder,
        TaskProgressItem progressItem,
        CancellationToken ct
    )
    {
        return this.PostUiBackground(() => ClearExceptUi(entities), ct)
            .IfSuccessAllAsync(
                ct,
                () => RefreshFavoriteToDoItemsAsync(ct),
                () => RefreshToDoItemListsCore(entities, autoOrder, progressItem, ct)
            );
    }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return List.ClearExceptUi(items);
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemListsCore(
        ReadOnlyMemory<ToDoItemEntityNotify> entities,
        bool autoOrder,
        TaskProgressItem progressItem,
        CancellationToken ct
    )
    {
        uint orderIndex = 1;

        return toDoService
            .GetToDoItemsAsync(entities.Select(x => x.Id), UiHelper.ChunkSize, ct)
            .IfSuccessForEachAsync(
                items =>
                    items
                        .ToResult()
                        .IfSuccessForEach(item =>
                            this.PostUiBackground(() => toDoCache.UpdateUi(item).ToResultOnly(), ct)
                        )
                        .IfSuccess(
                            () => items.IfSuccessForEach(item => toDoCache.GetToDoItem(item.Id))
                        )
                        .IfSuccessForEach(item =>
                            Result
                                .Success.IfSuccess(() =>
                                {
                                    if (autoOrder)
                                    {
                                        var oi = orderIndex;

                                        return this.PostUiBackground(() =>
                                        {
                                            item.OrderIndex = oi;

                                            return Result.Success;
                                        }, ct);
                                    }

                                    return Result.Success;
                                })
                                .IfSuccess(() =>
                                {
                                    orderIndex++;

                                    return this.PostUiBackground(progressItem.IncreaseUi, ct);
                                })
                                .IfSuccess(item.ToResult)
                        )
                        .IfSuccess(itemsNotify =>
                            taskProgressService.RunProgress(
                                (ushort)itemsNotify.Length,
                                item =>
                                    this.PostUiBackground(
                                        () =>
                                            itemsNotify
                                                .ToResult()
                                                .IfSuccessForEach(i =>
                                                    List.UpdateItemUi(i).IfSuccess(item.IncreaseUi)
                                                ), ct
                                    ), ct
                            )
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateItemsAsync(
        ReadOnlyMemory<ToDoItemEntityNotify> ids,
        bool autoOrder,
        CancellationToken ct
    )
    {
        return taskProgressService.RunProgressAsync(
            (ushort)ids.Length,
            item => RefreshToDoItemListsAsync(ids, autoOrder, item, ct),
            ct
        );
    }
}
