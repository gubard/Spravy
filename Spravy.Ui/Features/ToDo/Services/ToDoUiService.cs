namespace Spravy.Ui.Features.ToDo.Services;

public class ToDoUiService : IToDoUiService
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;

    public ToDoUiService(IToDoService toDoService, IToDoCache toDoCache)
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateItemsAsync(
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        CancellationToken ct
    )
    {
        return toDoService
            .GetToDoItemsAsync(items.Select(x => x.Id), UiHelper.ChunkSize, ct)
            .IfSuccessForEachAsync(
                x =>
                    x.IfSuccessForEach(i =>
                        this.PostUiBackground(() => toDoCache.UpdateUi(i).ToResultOnly(), ct)
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<ToDoItemEntityNotify>>
    > GetSiblingsAsync(ToDoItemEntityNotify item, CancellationToken ct)
    {
        return toDoService
            .GetToDoItemAsync(item.Id, ct)
            .IfSuccessAsync(
                x => this.PostUiBackground(() => toDoCache.UpdateUi(x).ToResultOnly(), ct),
                ct
            )
            .IfSuccessAsync(() => toDoService.GetSiblingsAsync(item.Id, ct), ct)
            .IfSuccessAsync(
                s =>
                    this.PostUiBackground(
                            () => s.IfSuccessForEach(x => toDoCache.UpdateUi(x)).ToResultOnly(),
                            ct
                        )
                        .IfSuccess(() => s.ToResult()),
                ct
            )
            .IfSuccessForEachAsync(x => toDoCache.GetToDoItem(x.Id), ct);
    }
}
