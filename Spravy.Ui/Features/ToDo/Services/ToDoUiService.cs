namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoUiService : IToDoUiService
{
    private readonly AppOptions appOptions;
    private readonly IToDoCache toDoCache;
    private readonly IToDoService toDoService;

    public ToDoUiService(IToDoService toDoService, IToDoCache toDoCache, AppOptions appOptions)
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        this.appOptions = appOptions;
    }

    public Cvtar UpdateItemAsync(ToDoItemEntityNotify item, CancellationToken ct)
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () =>
                toDoService
                    .GetToDoItemAsync(item.Id, ct)
                    .IfSuccessAsync(x => this.InvokeUiAsync(() => toDoCache.UpdateUi(x)), ct)
                    .ToResultOnlyAsync(),
            () =>
                toDoService
                    .GetParentsAsync(item.Id, ct)
                    .IfSuccessAsync(
                        x => this.PostUiBackground(() => toDoCache.UpdateParentsUi(item.Id, x), ct),
                        ct
                    )
        );
    }

    public Cvtar UpdateBookmarkItemsAsync(
        IBookmarksToDoItemsView bookmarksToDoItemsView,
        CancellationToken ct
    )
    {
        return toDoCache
            .GetBookmarkItems()
            .IfSuccessAsync(
                items =>
                    this.InvokeUiBackgroundAsync(
                        () => bookmarksToDoItemsView.ClearBookmarksExceptUi(items)
                    ),
                ct
            )
            .IfSuccessAsync(() => toDoService.GetBookmarkToDoItemIdsAsync(ct), ct)
            .IfSuccessAsync(
                ids =>
                    this.PostUiBackground(
                        () =>
                            ids.IfSuccessForEach(i => toDoCache.GetToDoItem(i))
                                .IfSuccess(bookmarksToDoItemsView.ClearBookmarksExceptUi),
                        ct
                    ),
                ct
            );
    }

    public Cvtar UpdateSelectorItemsAsync(
        Guid? selectedId,
        ReadOnlyMemory<Guid> ignoreIds,
        CancellationToken ct
    )
    {
        return this.InvokeUiAsync(() => toDoCache.ResetItemsUi())
            .IfSuccessAsync(
                () => toDoService.GetToDoSelectorItemsAsync(ReadOnlyMemory<Guid>.Empty, ct),
                ct
            )
            .IfSuccessAsync(
                items =>
                    this.InvokeUiAsync(() => toDoCache.IgnoreItemsUi(ignoreIds))
                        .IfSuccessAsync(() => items.ToResult(), ct),
                ct
            )
            .IfSuccessAsync(items => this.InvokeUiAsync(() => toDoCache.UpdateUi(items)), ct)
            .IfSuccessAsync(
                _ =>
                {
                    if (selectedId is null)
                    {
                        return Result.AwaitableSuccess;
                    }

                    return this.InvokeUiAsync(() => toDoCache.ExpandItemUi(selectedId.Value));
                },
                ct
            );
    }

    public Cvtar UpdateSiblingsAsync(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () =>
            {
                if (item.TryGetValue(out var i))
                {
                    return UpdateItemAsync(i, ct);
                }

                return Result.AwaitableSuccess;
            },
            () => toDoItemsView.RefreshAsync(ct),
            () =>
                toDoService
                    .GetChildrenToDoItemIdsAsync(
                        (item.GetNullable()?.Parent?.Id).ToOption(),
                        ignoreItems.IsEmpty && item.TryGetValue(out var i)
                            ? new[] { i.Id }
                            : ignoreItems.Select(x => x.Id),
                        ct
                    )
                    .IfSuccessAsync(ids => toDoService.GetShortToDoItemsAsync(ids, ct), ct)
                    .IfSuccessAsync(
                        ids =>
                            this.PostUiBackground(
                                () =>
                                    ids.IfSuccessForEach(id => toDoCache.UpdateUi(id))
                                        .IfSuccess(toDoItemsView.SetItemsUi),
                                ct
                            ),
                        ct
                    )
        );
    }

    public Cvtar UpdateLeafToDoItemsAsync(
        ToDoItemEntityNotify item,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => UpdateItemAsync(item, ct),
            () => toDoItemsView.RefreshAsync(ct),
            () =>
                toDoService
                    .GetLeafToDoItemIdsAsync(item.Id, ct)
                    .IfSuccessAsync(
                        ids =>
                            ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id))
                                .IfSuccess(x =>
                                    this.PostUiBackground(() => toDoItemsView.SetItemsUi(x), ct)
                                )
                                .IfSuccess(() => ids.ToResult()),
                        ct
                    )
                    .IfSuccessAsync(
                        ids =>
                        {
                            ushort loadedIndex = 0;

                            return toDoService
                                .GetToDoItemsAsync(ids, appOptions.ToDoItemsChunkSize, ct)
                                .IfSuccessForEachAsync(
                                    x =>
                                        this.PostUiBackground(
                                            () =>
                                                x.IfSuccessForEach(i =>
                                                        toDoCache
                                                            .UpdateUi(i)
                                                            .IfSuccess(notify =>
                                                            {
                                                                notify.LoadedIndex = loadedIndex;
                                                                loadedIndex++;

                                                                return notify.ToResult();
                                                            })
                                                    )
                                                    .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                            ct
                                        ),
                                    ct
                                );
                        },
                        ct
                    )
        );
    }

    public Cvtar UpdateRootItemsAsync(IToDoItemsView toDoItemsView, CancellationToken ct)
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => toDoItemsView.RefreshAsync(ct),
            () =>
                toDoCache
                    .GetRootItems()
                    .IfSuccess(items =>
                        this.PostUiBackground(() => toDoItemsView.SetItemsUi(items), ct)
                    )
                    .IfSuccessAsync(
                        () =>
                            toDoService.GetChildrenToDoItemIdsAsync(
                                OptionStruct<Guid>.Default,
                                ReadOnlyMemory<Guid>.Empty,
                                ct
                            ),
                        ct
                    )
                    .IfSuccessAsync(
                        ids =>
                            toDoCache
                                .UpdateRootItems(ids)
                                .IfSuccess(items =>
                                    this.PostUiBackground(() => toDoItemsView.SetItemsUi(items), ct)
                                )
                                .IfSuccess(() => ids.ToResult()),
                        ct
                    )
                    .IfSuccessAsync(
                        ids =>
                            toDoService
                                .GetToDoItemsAsync(ids, appOptions.ToDoItemsChunkSize, ct)
                                .IfSuccessForEachAsync(
                                    x =>
                                        this.PostUiBackground(
                                            () =>
                                                x.IfSuccessForEach(i => toDoCache.UpdateUi(i))
                                                    .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                            ct
                                        ),
                                    ct
                                ),
                        ct
                    )
        );
    }

    public Cvtar UpdateSearchToDoItemsAsync(
        string searchText,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => toDoItemsView.RefreshAsync(ct),
            () =>
                (
                    searchText.IsNullOrWhiteSpace()
                        ? ReadOnlyMemory<Guid>
                            .Empty.ToResult()
                            .ToValueTaskResult()
                            .ConfigureAwait(false)
                        : toDoService.SearchToDoItemIdsAsync(searchText, ct)
                )
                    .IfSuccessAsync(
                        ids =>
                            ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id))
                                .IfSuccess(x =>
                                    this.PostUiBackground(() => toDoItemsView.SetItemsUi(x), ct)
                                )
                                .IfSuccess(() => ids.ToResult()),
                        ct
                    )
                    .IfSuccessAsync(
                        ids =>
                        {
                            ushort loadedIndex = 0;

                            return toDoService
                                .GetToDoItemsAsync(ids, appOptions.ToDoItemsChunkSize, ct)
                                .IfSuccessForEachAsync(
                                    x =>
                                        this.PostUiBackground(
                                            () =>
                                                x.IfSuccessForEach(i =>
                                                        toDoCache
                                                            .UpdateUi(i)
                                                            .IfSuccess(notify =>
                                                            {
                                                                notify.LoadedIndex = loadedIndex;
                                                                loadedIndex++;

                                                                return notify.ToResult();
                                                            })
                                                    )
                                                    .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                            ct
                                        ),
                                    ct
                                );
                        },
                        ct
                    )
        );
    }

    public Cvtar UpdateItemChildrenAsync(
        ToDoItemEntityNotify item,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () =>
                this.PostUiBackground(() => toDoItemsView.SetItemsUi(item.Children.ToArray()), ct)
                    .IfSuccessAsync(
                        () =>
                            toDoService.GetChildrenToDoItemIdsAsync(
                                item.Id.ToOption(),
                                ReadOnlyMemory<Guid>.Empty,
                                ct
                            ),
                        ct
                    )
                    .IfSuccessAsync(
                        ids =>
                            ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id))
                                .IfSuccess(x =>
                                    this.PostUiBackground(
                                        () =>
                                            toDoItemsView
                                                .SetItemsUi(x)
                                                .IfSuccess(
                                                    () =>
                                                        toDoCache.UpdateChildrenItemsUi(
                                                            item.Id,
                                                            ids
                                                        )
                                                )
                                                .ToResultOnly(),
                                        ct
                                    )
                                )
                                .IfSuccess(() => ids.ToResult()),
                        ct
                    )
                    .IfSuccessAsync(
                        ids =>
                            toDoService
                                .GetToDoItemsAsync(ids, appOptions.ToDoItemsChunkSize, ct)
                                .IfSuccessForEachAsync(
                                    x =>
                                        this.PostUiBackground(
                                            () =>
                                                x.IfSuccessForEach(i => toDoCache.UpdateUi(i))
                                                    .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                            ct
                                        ),
                                    ct
                                ),
                        ct
                    ),
            () => UpdateItemAsync(item, ct),
            () => toDoItemsView.RefreshAsync(ct)
        );
    }

    public Cvtar UpdateTodayItemsAsync(IToDoItemsView toDoItemsView, CancellationToken ct)
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => toDoItemsView.RefreshAsync(ct),
            () =>
                toDoService
                    .GetTodayToDoItemsAsync(ct)
                    .IfSuccessAsync(
                        ids =>
                            ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id))
                                .IfSuccess(x =>
                                    this.PostUiBackground(() => toDoItemsView.SetItemsUi(x), ct)
                                )
                                .IfSuccess(() => ids.ToResult()),
                        ct
                    )
                    .IfSuccessAsync(
                        ids =>
                        {
                            ushort loadedIndex = 0;

                            return toDoService
                                .GetToDoItemsAsync(ids, appOptions.ToDoItemsChunkSize, ct)
                                .IfSuccessForEachAsync(
                                    x =>
                                        this.PostUiBackground(
                                            () =>
                                                x.IfSuccessForEach(i =>
                                                        toDoCache
                                                            .UpdateUi(i)
                                                            .IfSuccess(notify =>
                                                            {
                                                                notify.LoadedIndex = loadedIndex;
                                                                loadedIndex++;

                                                                return notify.ToResult();
                                                            })
                                                    )
                                                    .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                            ct
                                        ),
                                    ct
                                );
                        },
                        ct
                    )
        );
    }
}
