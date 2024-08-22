namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemSelectorViewModel : ViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;
    private readonly ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private ToDoItemEntityNotify? selectedItem;

    public ToDoItemSelectorViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems,
        IToDoService toDoService,
        IToDoCache toDoCache,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Item = item;
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        this.ignoreItems = ignoreItems;

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

    public Option<ToDoItemEntityNotify> Item { get; }
    public AvaloniaList<ToDoItemEntityNotify> Roots { get; } = new();
    public SpravyCommand InitializedCommand { get; }
    public SpravyCommand SearchCommand { get; }

    public Result<ToDoItemEntityNotify> GetSelectedItem()
    {
        return SelectedItem.IfNotNull(nameof(SelectedItem));
    }

    private Cvtar InitializedAsync(CancellationToken ct)
    {
        return Refresh(ct);
    }

    private Cvtar Refresh(CancellationToken ct)
    {
        return this.InvokeUiAsync(() => toDoCache.ResetItemsUi())
            .IfSuccessAsync(() => toDoCache.GetRootItems(), ct)
            .IfSuccessAsync(
                items =>
                    this.PostUiBackground(
                        () =>
                        {
                            Roots.UpdateUi(items);

                            return SetupUi();
                        },
                        ct
                    ),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.GetToDoSelectorItemsAsync(ignoreItems.Select(x => x.Id), ct),
                ct
            )
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

                            return SetupUi()
                                .IfSuccess(() =>
                                {
                                    if (Item.TryGetValue(out var i))
                                    {
                                        SelectedItem = i;
                                    }

                                    return Result.Success;
                                });
                        },
                        ct
                    ),
                ct
            );
    }

    private Result SetupUi(ToDoItemEntityNotify item)
    {
        if (ignoreItems.Span.Contains(item))
        {
            item.IsIgnore = true;
        }

        if (SelectedItem is not null && SelectedItem.Id == item.Id)
        {
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
