namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoUiService : IToDoUiService
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;
    private readonly ITaskProgressService taskProgressService;
    private readonly AppOptions appOptions;

    public ToDoUiService(
        IToDoService toDoService,
        IToDoCache toDoCache,
        ITaskProgressService taskProgressService,
        AppOptions appOptions
    )
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        this.taskProgressService = taskProgressService;
        this.appOptions = appOptions;
    }

    public Cvtar UpdateItemAsync(ToDoItemEntityNotify item, CancellationToken ct)
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () =>
                taskProgressService.RunProgressAsync(
                    _ =>
                        toDoService
                            .GetToDoItemAsync(item.Id, ct)
                            .IfSuccessAsync(
                                x => this.InvokeUiAsync(() => toDoCache.UpdateUi(x)),
                                ct
                            )
                            .ToResultOnlyAsync(),
                    ct
                ),
            () =>
                taskProgressService.RunProgressAsync(
                    _ =>
                        toDoService
                            .GetParentsAsync(item.Id, ct)
                            .IfSuccessAsync(
                                x =>
                                    this.PostUiBackground(
                                        () => toDoCache.UpdateParentsUi(item.Id, x),
                                        ct
                                    ),
                                ct
                            ),
                    ct
                )
        );
    }

    public Cvtar UpdateItemsAsync(ReadOnlyMemory<ToDoItemEntityNotify> items, CancellationToken ct)
    {
        return taskProgressService.RunProgressAsync(
            (ushort)items.Length,
            item =>
                toDoService
                    .GetToDoItemsAsync(items.Select(x => x.Id), appOptions.ToDoItemsChunkSize, ct)
                    .IfSuccessForEachAsync(
                        x =>
                            x.IfSuccessForEach(
                                i =>
                                    this.PostUiBackground(
                                        () =>
                                            toDoCache.UpdateUi(i).IfSuccess(_ => item.IncreaseUi()),
                                        ct
                                    ),
                                ct
                            ),
                        ct
                    ),
            ct
        );
    }

    public Cvtar UpdateBookmarkItemsAsync(
        IBookmarksToDoItemsView bookmarksToDoItemsView,
        CancellationToken ct
    )
    {
        return toDoService
            .GetBookmarkToDoItemIdsAsync(ct)
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
            .IfSuccessAsync(items => toDoCache.UpdateUi(items), ct)
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
            () =>
                toDoItemsView
                    .RefreshAsync(ct)
                    .IfSuccessAsync(
                        () =>
                            toDoService.GetChildrenToDoItemIdsAsync(
                                (item.GetNullable()?.Parent?.Id).ToOption(),
                                ignoreItems.IsEmpty && item.TryGetValue(out var i)
                                    ? new[] { i.Id }
                                    : ignoreItems.Select(x => x.Id),
                                ct
                            ),
                        ct
                    )
                    .IfSuccessAsync(ids => toDoService.GetShortToDoItemsAsync(ids, ct), ct)
                    .IfSuccessAsync(
                        ids =>
                            this.PostUiBackground(
                                () =>
                                    ids.IfSuccessForEach(i => toDoCache.UpdateUi(i))
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
            () =>
                toDoItemsView
                    .RefreshAsync(ct)
                    .IfSuccessAsync(() => toDoService.GetLeafToDoItemIdsAsync(item.Id, ct), ct)
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
        return toDoItemsView
            .RefreshAsync(ct)
            .IfSuccessAsync(
                () =>
                    toDoCache
                        .GetRootItems()
                        .IfSuccess(items =>
                            this.PostUiBackground(() => toDoItemsView.SetItemsUi(items), ct)
                        ),
                ct
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
            );
    }

    public Cvtar UpdateSearchToDoItemsAsync(
        string searchText,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    )
    {
        return toDoItemsView
            .RefreshAsync(ct)
            .IfSuccessAsync(() => searchText.IsNullOrWhiteSpace() ? ReadOnlyMemory<Guid>.Empty.ToResult().ToValueTaskResult().ConfigureAwait(false) : toDoService.SearchToDoItemIdsAsync(searchText, ct), ct)
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
            () => UpdateItemAsync(item, ct),
            () =>
                toDoItemsView
                    .RefreshAsync(ct)
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
                    )
        );
    }

    public Cvtar UpdateTodayItemsAsync(IToDoItemsView toDoItemsView, CancellationToken ct)
    {
        return toDoItemsView
            .RefreshAsync(ct)
            .IfSuccessAsync(() => toDoService.GetTodayToDoItemsAsync(ct), ct)
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
            );
    }
}
