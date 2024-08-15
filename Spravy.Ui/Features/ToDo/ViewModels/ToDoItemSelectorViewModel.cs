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
        ToDoItemEntityNotify? selectedItem,
        ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems,
        IToDoService toDoService,
        IToDoCache toDoCache,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        this.ignoreItems = ignoreItems;
        SelectedItem = selectedItem;

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
    public SpravyCommand SearchCommand { get; }

    private Cvtar InitializedAsync(CancellationToken ct)
    {
        return Refresh(ct);
    }

    private Cvtar Refresh(CancellationToken ct)
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

                            return SetupUi();
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
