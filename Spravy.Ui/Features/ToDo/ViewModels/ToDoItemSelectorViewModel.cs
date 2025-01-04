namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemSelectorViewModel : DialogableViewModelBase
{
    private readonly ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems;
    private readonly IToDoCache toDoCache;
    private readonly IToDoUiService toDoUiService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private ToDoItemEntityNotify? selectedItem;

    public ToDoItemSelectorViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> ignoreItems,
        IToDoCache toDoCache,
        IToDoUiService toDoUiService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Item = item;
        isBusy = true;
        this.toDoCache = toDoCache;
        this.ignoreItems = ignoreItems;
        this.toDoUiService = toDoUiService;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);

        SearchCommand = SpravyCommand.Create(
            ct => Search(ct).ToValueTaskResult().ConfigureAwait(false),
            errorHandler,
            taskProgressService
        );

        PropertyChanged += (s, e) =>
        {
            if (s is not ToDoItemSelectorViewModel vm)
            {
                return;
            }

            switch (e.PropertyName)
            {
                case nameof(SelectedItem):
                {
                    if (vm.SelectedItem is null)
                    {
                        return;
                    }

                    vm.SelectedItem.IsExpanded = true;

                    break;
                }
            }
        };
    }

    public Option<ToDoItemEntityNotify> Item { get; }
    public AvaloniaList<ToDoItemEntityNotify> Roots { get; } = new();
    public SpravyCommand InitializedCommand { get; }
    public SpravyCommand SearchCommand { get; }
    public override string ViewId => TypeCache<ToDoItemSelectorViewModel>.Name;

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
        return Result.AwaitableSuccess.IfSuccessTryFinallyAsync(
            () => toDoUiService.GetRequest(GetToDo.WithDefaultItems.SetIsSelectorItems(true), ct)
               .IfSuccessAsync(_ => this.PostUiBackground(() => toDoCache.SetIgnoreItemsUi(ignoreItems.Select(x => x.Id)), ct), ct)
               .IfSuccessAsync(
                    () =>
                    {
                        if (!Item.TryGetValue(out var i))
                        {
                            return Result.AwaitableSuccess;
                        }

                        return this.InvokeUiAsync(
                            () =>
                            {
                                SelectedItem = i;

                                return Result.Success;
                            }
                        );
                    },
                    ct
                )
               .IfSuccessAsync(() => toDoCache.GetRootItems(), ct)
               .IfSuccessAsync(items => this.InvokeUiAsync(() => Roots.UpdateUi(items).ToResultOnly()), ct),
            () => this.PostUiBackground(
                () =>
                {
                    IsBusy = false;

                     return Result.Success;
                },
                ct
            ),
            ct
        );
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
        return Result.Success
           .IfSuccess(
                () =>
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
                }
            )
           .IfSuccess(() => item.Children.ToArray().ToReadOnlyMemory().ToResult().IfSuccessForEach(x => Search(x, ct)));
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}