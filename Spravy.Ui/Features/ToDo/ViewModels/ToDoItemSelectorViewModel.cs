namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemSelectorViewModel : ViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;

    public ToDoItemSelectorViewModel(
        IToDoService toDoService,
        IToDoCache toDoCache,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
        SearchCommand = SpravyCommand.Create(SearchAsync, errorHandler, taskProgressService);
    }

    public AvaloniaList<ToDoItemEntityNotify> Roots { get; } = new();
    public SpravyCommand InitializedCommand { get; }
    public ReadOnlyMemory<Guid> IgnoreIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    public Guid DefaultSelectedItemId { get; set; }
    public SpravyCommand SearchCommand { get; }

    [Reactive]
    public string SearchText { get; set; } = string.Empty;

    [Reactive]
    public ToDoItemEntityNotify? SelectedItem { get; set; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return Refresh(ct);
    }

    private ConfiguredValueTaskAwaitable<Result> Refresh(CancellationToken ct)
    {
        return this.InvokeUiBackgroundAsync(() => toDoCache.ResetItemsUi())
            .IfSuccessAsync(() => toDoCache.GetRootItems(), ct)
            .IfSuccessAsync(
                items =>
                    this.InvokeUiBackgroundAsync(() =>
                    {
                        Roots.Clear();
                        Roots.AddRange(items.ToArray());

                        return SetupUi();
                    }),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.GetToDoSelectorItemsAsync(IgnoreIds.ToArray(), ct),
                ct
            )
            .IfSuccessAsync(
                items => this.InvokeUiBackgroundAsync(() => toDoCache.UpdateUi(items)),
                ct
            )
            .IfSuccessAsync(
                items =>
                    this.InvokeUiBackgroundAsync(() =>
                    {
                        Roots.Clear();
                        Roots.AddRange(items.ToArray());

                        return SetupUi();
                    }),
                ct
            );
    }

    private Result SetupUi(ToDoItemEntityNotify item)
    {
        if (IgnoreIds.Span.Contains(item.Id))
        {
            item.IsIgnore = true;
        }

        if (DefaultSelectedItemId == item.Id)
        {
            SelectedItem = item;
            var result = ExpandParentsUi(item);

            if (result.IsHasError)
            {
                return result;
            }
        }

        return item.Children.ToArray().ToReadOnlyMemory().ToResult().IfSuccessForEach(SetupUi);
    }

    private Result ExpandParentsUi(ToDoItemEntityNotify item)
    {
        item.IsExpanded = true;

        if (item.Parent == null)
        {
            return Result.Success;
        }

        return ExpandParentsUi(item.Parent);
    }

    private Result SetupUi()
    {
        return Roots
            .ToArray()
            .ToReadOnlyMemory()
            .ToResult()
            .IfSuccessForEach(item => SetupUi(item));
    }

    private ConfiguredValueTaskAwaitable<Result> SearchAsync(CancellationToken ct)
    {
        return this.InvokeUiBackgroundAsync(() =>
            {
                Roots.Clear();

                return Result.Success;
            })
            .IfSuccessAsync(() => toDoCache.GetRootItems(), ct)
            .IfSuccessForEachAsync(item => SearchAsync(item, ct), ct);
    }

    private ConfiguredValueTaskAwaitable<Result> SearchAsync(
        ToDoItemEntityNotify item,
        CancellationToken ct
    )
    {
        return Result
            .AwaitableSuccess.IfSuccessAsync(
                () =>
                {
                    if (item.Name.Contains(SearchText))
                    {
                        return this.InvokeUiBackgroundAsync(() =>
                        {
                            Roots.Add(item);

                            return Result.Success;
                        });
                    }

                    return Result.AwaitableSuccess;
                },
                ct
            )
            .IfSuccessAsync(
                () =>
                    item
                        .Children.ToArray()
                        .ToReadOnlyMemory()
                        .ToResult()
                        .IfSuccessForEachAsync(i => SearchAsync(i, ct), ct),
                ct
            );
    }
}
