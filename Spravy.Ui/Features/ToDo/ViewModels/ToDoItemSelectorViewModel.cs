namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemSelectorViewModel : ViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private ToDoItemEntityNotify? selectedItem;

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

        SearchCommand = SpravyCommand.Create(
            ct => Search(ct).ToValueTaskResult().ConfigureAwait(false),
            errorHandler,
            taskProgressService
        );
    }

    public AvaloniaList<ToDoItemEntityNotify> Roots { get; } = new();
    public SpravyCommand InitializedCommand { get; }
    public ReadOnlyMemory<Guid> IgnoreIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    public Guid DefaultSelectedItemId { get; set; }
    public SpravyCommand SearchCommand { get; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return Refresh(ct);
    }

    private ConfiguredValueTaskAwaitable<Result> Refresh(CancellationToken ct)
    {
        return this.PostUiBackground(() => toDoCache.ResetItemsUi(), ct)
            .IfSuccess(() => toDoCache.GetRootItems())
            .IfSuccess(items =>
                this.PostUiBackground(
                    () =>
                    {
                        Roots.UpdateUi(items);

                        return SetupUi();
                    },
                    ct
                )
            )
            .IfSuccessAsync(() => toDoService.GetToDoSelectorItemsAsync(IgnoreIds, ct), ct)
            .IfSuccessAsync(
                items => this.InvokeUiBackgroundAsync(() => toDoCache.UpdateUi(items)),
                ct
            )
            .IfSuccessAsync(
                items =>
                    this.PostUiBackground(
                        () =>
                        {
                            Roots.UpdateUi(items);
                            Roots.BinarySort();

                            return SetupUi();
                        },
                        ct
                    ),
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
        return Roots.ToArray().ToReadOnlyMemory().ToResult().IfSuccessForEach(SetupUi);
    }

    private Result Search(CancellationToken ct)
    {
        return this.PostUiBackground(
                () =>
                {
                    Roots.Clear();

                    return Result.Success;
                },
                ct
            )
            .IfSuccess(() => toDoCache.GetRootItems())
            .IfSuccessForEach(x => Search(x, ct));
    }

    private Result Search(ToDoItemEntityNotify item, CancellationToken ct)
    {
        return Result
            .Success.IfSuccess(() =>
            {
                if (item.Name.Contains(SearchText))
                {
                    return this.PostUiBackground(
                        () =>
                        {
                            Roots.Add(item);

                            return Result.Success;
                        },
                        ct
                    );
                }

                return Result.Success;
            })
            .IfSuccess(
                () =>
                    item
                        .Children.ToArray()
                        .ToReadOnlyMemory()
                        .ToResult()
                        .IfSuccessForEach(x => Search(x, ct))
            );
    }
}
