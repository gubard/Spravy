namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class LeafToDoItemsViewModel
    : NavigatableViewModelBase,
        IRefresh,
        IToDoItemUpdater,
        IToDoSubItemsViewModelProperty
{
    private readonly TaskWork refreshWork;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;
    private readonly SpravyCommandNotifyService spravyCommandNotifyService;

    [ObservableProperty]
    public ToDoItemEntityNotify? item;

    public LeafToDoItemsViewModel(
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        IToDoCache toDoCache,
        ITaskProgressService taskProgressService,
        SpravyCommandNotifyService spravyCommandNotifyService
    )
        : base(true)
    {
        this.spravyCommandNotifyService = spravyCommandNotifyService;
        Commands = new();
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoService = toDoService;
        this.objectStorage = objectStorage;
        this.toDoCache = toDoCache;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        ToDoSubItemsViewModel.List.PropertyChanged += OnPropertyChanged;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }
    public ReadOnlyMemory<Guid> LeafIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }

    public override string ViewId
    {
        get => $"{TypeCache<LeafToDoItemsViewModel>.Type.Name}:{Item?.Name}";
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return RefreshCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> RefreshCore(CancellationToken ct)
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken ct)
    {
        if (LeafIds.IsEmpty)
        {
            return Item.IfNotNull(nameof(Item))
                .IfSuccessAsync(
                    item =>
                        toDoService
                            .GetLeafToDoItemIdsAsync(item.Id, ct)
                            .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), ct)
                            .IfSuccessAsync(
                                items => ToDoSubItemsViewModel.UpdateItemsAsync(items, true, ct),
                                ct
                            ),
                    ct
                );
        }

        return LeafIds
            .ToResult()
            .IfSuccessForEachAsync(id => toDoService.GetLeafToDoItemIdsAsync(id, ct), ct)
            .IfSuccessAsync(items => items.SelectMany().ToResult(), ct)
            .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), ct)
            .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items, true, ct), ct);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<LeafToDoItemsViewModelSetting>(ViewId, ct)
            .IfSuccessAsync(obj => SetStateAsync(obj, ct), ct)
            .IfSuccessAsync(() => RefreshAsync(ct), ct);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new LeafToDoItemsViewModelSetting(this), ct);
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting
            .CastObject<LeafToDoItemsViewModelSetting>()
            .IfSuccess(s =>
                this.PostUiBackground(
                    () =>
                    {
                        ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                        ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;

                        return Result.Success;
                    },
                    ct
                )
            )
            .ToValueTaskResult()
            .ConfigureAwait(false);
    }

    public Result UpdateInListToDoItemUi(ToDoItemEntityNotify item)
    {
        if (ToDoSubItemsViewModel.List.ToDoItems.GroupByNone.Items.Items.Contains(item))
        {
            return ToDoSubItemsViewModel.List.UpdateItemUi(item);
        }

        return Result.Success;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToDoSubItemsViewModel.List.IsMulti))
        {
            if (ToDoSubItemsViewModel.List.IsMulti)
            {
                Commands.Update(spravyCommandNotifyService.LeafToDoItemsMulti);
            }
            else
            {
                Commands.Clear();
            }
        }
    }
}
