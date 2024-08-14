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

    public ConfiguredValueTaskAwaitable<Result> UpdateItemAsync(
        ToDoItemEntityNotify item,
        CancellationToken ct
    )
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

    public ConfiguredValueTaskAwaitable<Result> UpdateItemsAsync(
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        CancellationToken ct
    )
    {
        return taskProgressService.RunProgressAsync(
            (ushort)items.Length,
            item =>
                toDoService
                    .GetToDoItemsAsync(items.Select(x => x.Id), appOptions.ToDoItemsChunkSize, ct)
                    .IfSuccessForEachAsync(
                        x =>
                            x.IfSuccessForEach(i =>
                                this.PostUiBackground(
                                    () => toDoCache.UpdateUi(i).IfSuccess(_ => item.IncreaseUi()),
                                    ct
                                )
                            ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateSiblingsAsync(
        ToDoItemEntityNotify item,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => toDoItemsView.RefreshAsync(ct),
            () => UpdateItemAsync(item, ct),
            () =>
                toDoService
                    .GetSiblingsAsync(item.Id, ct)
                    .IfSuccessAsync(
                        ids =>
                            ids.IfSuccessForEach(i => toDoCache.UpdateUi(i))
                                .IfSuccess(x =>
                                    this.PostUiBackground(() => toDoItemsView.ClearExceptUi(x), ct)
                                ),
                        ct
                    )
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateLeafToDoItemsAsync(
        ToDoItemEntityNotify item,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => toDoItemsView.RefreshAsync(ct),
            () => UpdateItemAsync(item, ct),
            () =>
                toDoService
                    .GetLeafToDoItemIdsAsync(item.Id, ct)
                    .IfSuccessAsync(
                        ids =>
                            ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id))
                                .IfSuccess(x =>
                                    this.PostUiBackground(() => toDoItemsView.ClearExceptUi(x), ct)
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
                                        x.IfSuccessForEach(i =>
                                            this.PostUiBackground(
                                                () =>
                                                    toDoCache
                                                        .UpdateUi(i)
                                                        .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                                ct
                                            )
                                        ),
                                    ct
                                ),
                        ct
                    )
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateRootItemsAsync(
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => toDoItemsView.RefreshAsync(ct),
            () =>
                toDoService
                    .GetRootToDoItemIdsAsync(ct)
                    .IfSuccessAsync(
                        ids =>
                            ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id))
                                .IfSuccess(x =>
                                    this.PostUiBackground(() => toDoItemsView.ClearExceptUi(x), ct)
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
                                        x.IfSuccessForEach(i =>
                                            this.PostUiBackground(
                                                () =>
                                                    toDoCache
                                                        .UpdateUi(i)
                                                        .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                                ct
                                            )
                                        ),
                                    ct
                                ),
                        ct
                    )
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateSearchToDoItemsAsync(
        string searchText,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => toDoItemsView.RefreshAsync(ct),
            () =>
                toDoService
                    .SearchToDoItemIdsAsync(searchText, ct)
                    .IfSuccessAsync(
                        ids =>
                            ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id))
                                .IfSuccess(x =>
                                    this.PostUiBackground(() => toDoItemsView.ClearExceptUi(x), ct)
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
                                        x.IfSuccessForEach(i =>
                                            this.PostUiBackground(
                                                () =>
                                                    toDoCache
                                                        .UpdateUi(i)
                                                        .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                                ct
                                            )
                                        ),
                                    ct
                                ),
                        ct
                    )
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateItemChildrenAsync(
        ToDoItemEntityNotify item,
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => toDoItemsView.RefreshAsync(ct),
            () => UpdateItemAsync(item, ct),
            () =>
                toDoService
                    .GetChildrenToDoItemIdsAsync(item.Id, ct)
                    .IfSuccessAsync(
                        ids =>
                            ids.IfSuccessForEach(id => toDoCache.GetToDoItem(id))
                                .IfSuccess(x =>
                                    this.PostUiBackground(
                                        () =>
                                            toDoItemsView
                                                .ClearExceptUi(x)
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
                                        x.IfSuccessForEach(i =>
                                            this.PostUiBackground(
                                                () =>
                                                    toDoCache
                                                        .UpdateUi(i)
                                                        .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                                ct
                                            )
                                        ),
                                    ct
                                ),
                        ct
                    )
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateTodayItemsAsync(
        IToDoItemsView toDoItemsView,
        CancellationToken ct
    )
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
                                    this.PostUiBackground(() => toDoItemsView.ClearExceptUi(x), ct)
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
                                        x.IfSuccessForEach(i =>
                                            this.PostUiBackground(
                                                () =>
                                                    toDoCache
                                                        .UpdateUi(i)
                                                        .IfSuccess(toDoItemsView.AddOrUpdateUi),
                                                ct
                                            )
                                        ),
                                    ct
                                ),
                        ct
                    )
        );
    }
}