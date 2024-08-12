namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoUiService : IToDoUiService
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;
    private readonly ITaskProgressService taskProgressService;

    public ToDoUiService(
        IToDoService toDoService,
        IToDoCache toDoCache,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        this.taskProgressService = taskProgressService;
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
                                x =>
                                    this.PostUiBackground(
                                            () => toDoCache.UpdateUi(x).ToResultOnly(),
                                            ct
                                        )
                                        .IfSuccess(() => x.ToResult()),
                                ct
                            )
                            .IfSuccessAsync(
                                x =>
                                    x.Type switch
                                    {
                                        ToDoItemType.Value => Result.AwaitableSuccess,
                                        ToDoItemType.Group => Result.AwaitableSuccess,
                                        ToDoItemType.Planned
                                            => toDoService
                                                .GetPlannedToDoItemSettingsAsync(x.Id, ct)
                                                .IfSuccessAsync(
                                                    setting =>
                                                        this.PostUiBackground(
                                                            () =>
                                                                toDoCache
                                                                    .UpdateUi(x.Id, setting)
                                                                    .ToResultOnly(),
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                        ToDoItemType.Periodicity
                                            => toDoService
                                                .GetPeriodicityToDoItemSettingsAsync(x.Id, ct)
                                                .IfSuccessAsync(
                                                    setting =>
                                                        this.PostUiBackground(
                                                            () =>
                                                                toDoCache
                                                                    .UpdateUi(x.Id, setting)
                                                                    .ToResultOnly(),
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                        ToDoItemType.PeriodicityOffset
                                            => toDoService
                                                .GetPeriodicityOffsetToDoItemSettingsAsync(x.Id, ct)
                                                .IfSuccessAsync(
                                                    setting =>
                                                        this.PostUiBackground(
                                                            () =>
                                                                toDoCache
                                                                    .UpdateUi(x.Id, setting)
                                                                    .ToResultOnly(),
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                        ToDoItemType.Circle => Result.AwaitableSuccess,
                                        ToDoItemType.Step => Result.AwaitableSuccess,
                                        ToDoItemType.Reference => Result.AwaitableSuccess,
                                        _
                                            => new Result(new ToDoItemTypeOutOfRangeError(x.Type))
                                                .ToValueTaskResult()
                                                .ConfigureAwait(false)
                                    },
                                ct
                            ),
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
                    .GetToDoItemsAsync(items.Select(x => x.Id), UiHelper.ChunkSize, ct)
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

    public ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<ToDoItemEntityNotify>>
    > UpdateSiblingsAsync(ToDoItemEntityNotify item, CancellationToken ct)
    {
        Result<ReadOnlyMemory<ToDoShortItem>>? items = null;

        return Result
            .AwaitableSuccess.IfSuccessAllAsync(
                ct,
                () => UpdateItemAsync(item, ct),
                () =>
                    toDoService
                        .GetSiblingsAsync(item.Id, ct)
                        .IfSuccessAsync(i => items = i.ToResult(), ct)
                        .IfSuccessAsync(
                            s =>
                                this.PostUiBackground(
                                    () =>
                                        s.IfSuccessForEach(x => toDoCache.UpdateUi(x))
                                            .ToResultOnly(),
                                    ct
                                ),
                            ct
                        )
            )
            .IfSuccessAsync(
                () => items.ThrowIfNull().IfSuccessForEach(x => toDoCache.GetToDoItem(x.Id)),
                ct
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
                                .GetToDoItemsAsync(ids, UiHelper.ChunkSize, ct)
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
