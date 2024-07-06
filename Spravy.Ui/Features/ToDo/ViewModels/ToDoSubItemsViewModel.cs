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
                .GetToDoItemsAsync(
                    ids.ToArray().Select(x => x.Id).ToArray(),
                    UiHelper.ChunkSize,
                    ct
                )
                .ConfigureAwait(false)
        )
        {
            if (!items.TryGetValue(out var value))
            {
                return new(items.Errors);
            }

            foreach (var item in value.ToArray())
            {
                var i = await this.InvokeUiBackgroundAsync(() => toDoCache.UpdateUi(item));

                if (!i.TryGetValue(out var t))
                {
                    return new(i.Errors);
                }

                if (item.Type == ToDoItemType.Reference)
                {
                    var reference = await toDoService.GetReferenceToDoItemSettingsAsync(
                        item.Id,
                        ct
                    );

                    if (!reference.TryGetValue(out var v))
                    {
                        return new(reference.Errors);
                    }

                    t.ReferenceId = v.ReferenceId.GetValueOrNull();
                }
                else
                {
                    t.ReferenceId = null;
                }

                var result = this.PostUiBackground(() => List.UpdateFavoriteItemUi(t))
                    .IfSuccess(progressItem.Increase);

                if (result.IsHasError)
                {
                    return result;
                }
            }
        }

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemListsAsync(
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        bool autoOrder,
        TaskProgressItem progressItem,
        CancellationToken ct
    )
    {
        return this.PostUiBackground(() => ClearExceptUi(items))
            .IfSuccessAllAsync(
                ct,
                () => RefreshFavoriteToDoItemsAsync(ct),
                () =>
                    RefreshToDoItemListsCore(items, autoOrder, progressItem, ct)
                        .ConfigureAwait(false)
            );
    }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return List.ClearExceptUi(items);
    }

    private async ValueTask<Result> RefreshToDoItemListsCore(
        ReadOnlyMemory<ToDoItemEntityNotify> ids,
        bool autoOrder,
        TaskProgressItem progressItem,
        CancellationToken ct
    )
    {
        uint orderIndex = 1;

        await foreach (
            var items in toDoService
                .GetToDoItemsAsync(
                    ids.ToArray().Select(x => x.Id).ToArray(),
                    UiHelper.ChunkSize,
                    ct
                )
                .ConfigureAwait(false)
        )
        {
            await items
                .IfSuccessForEach(item =>
                    this.PostUiBackground(() => toDoCache.UpdateUi(item).ToResultOnly())
                )
                .IfSuccess(() => items.IfSuccessForEach(item => toDoCache.GetToDoItem(item.Id)))
                .IfSuccessForEachAsync(
                    item =>
                    {
                        return GetReferenceId(item, ct)
                            .IfSuccessAsync(
                                referenceId =>
                                    this.PostUiBackground(() =>
                                    {
                                        item.ReferenceId = referenceId.GetValueOrNull();

                                        return Result.Success;
                                    }),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                {
                                    if (autoOrder)
                                    {
                                        var oi = orderIndex;

                                        return this.PostUiBackground(() =>
                                        {
                                            item.OrderIndex = oi;

                                            return Result.Success;
                                        });
                                    }

                                    return Result.Success;
                                },
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                {
                                    orderIndex++;

                                    return progressItem.Increase();
                                },
                                ct
                            )
                            .IfSuccessAsync(item.ToResult, ct);
                    },
                    ct
                )
                .IfSuccessAsync(
                    itemsNotify =>
                        taskProgressService.RunProgress(
                            itemsNotify,
                            item => this.PostUiBackground(() => List.UpdateItemUi(item))
                        ),
                    ct
                );
        }

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result<OptionStruct<Guid>>> GetReferenceId(
        ToDoItemEntityNotify item,
        CancellationToken ct
    )
    {
        if (item.Type == ToDoItemType.Reference)
        {
            return toDoService
                .GetReferenceToDoItemSettingsAsync(item.Id, ct)
                .IfSuccessAsync(reference => reference.ReferenceId.ToResult(), ct);
        }

        return OptionStruct<Guid>.Default.ToResult().ToValueTaskResult().ConfigureAwait(false);
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
