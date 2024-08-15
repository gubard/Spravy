namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase, IToDoItemsView
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;
    private readonly ITaskProgressService taskProgressService;
    private readonly AppOptions appOptions;

    public ToDoSubItemsViewModel(
        IToDoService toDoService,
        IToDoCache toDoCache,
        MultiToDoItemsViewModel list,
        ITaskProgressService taskProgressService,
        AppOptions appOptions
    )
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        List = list;
        this.taskProgressService = taskProgressService;
        this.appOptions = appOptions;
    }

    public MultiToDoItemsViewModel List { get; }

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
}
