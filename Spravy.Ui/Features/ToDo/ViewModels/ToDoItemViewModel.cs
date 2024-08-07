namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemViewModel : NavigatableViewModelBase, IRefresh, IToDoItemUpdater
{
    private readonly TaskWork refreshWork;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoCache toDoCache;
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private ToDoItemEntityNotify? item;

    public ToDoItemViewModel(
        IObjectStorage objectStorage,
        ToDoItemCommands commands,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IToDoService toDoService,
        IToDoCache toDoCache,
        IErrorHandler errorHandler,
        SpravyCommandService spravyCommandService
    )
        : base(true)
    {
        this.objectStorage = objectStorage;
        Commands = commands;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        SpravyCommandService = spravyCommandService;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        CommandItems = new();
        ToDoSubItemsViewModel.List.PropertyChanged += OnPropertyChanged;
    }

    public Guid Id { get; set; }
    public SpravyCommandService SpravyCommandService { get; }
    public AvaloniaList<SpravyCommandNotify> CommandItems { get; }
    public ToDoItemCommands Commands { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }

    public override string ViewId
    {
        get => $"{TypeCache<ToDoItemViewModel>.Type.Name}:{Id}";
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return RefreshCore().ConfigureAwait(false);
    }

    private async ValueTask<Result> RefreshCore()
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess.IfSuccessAllAsync(
            ct,
            () => RefreshToDoItemChildrenAsync(ct),
            () => RefreshToDoItemCore(ct),
            () => RefreshPathAsync(ct)
        );
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemCore(CancellationToken ct)
    {
        return toDoCache
            .GetToDoItem(Id)
            .IfSuccess(i =>
                this.PostUiBackground(
                    () =>
                    {
                        Item = i;

                        return Result.Success;
                    },
                    ct
                )
            )
            .IfSuccessAsync(() => toDoService.GetToDoItemAsync(Id, ct), ct)
            .IfSuccessAsync(
                i =>
                    this.PostUiBackground(
                        () => toDoCache.UpdateUi(i).IfSuccess(_ => UpdateCommandItemsUi()),
                        ct
                    ),
                ct
            );
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshPathAsync(CancellationToken ct)
    {
        return toDoService
            .GetParentsAsync(Id, ct)
            .IfSuccessAsync(
                parents => this.PostUiBackground(() => toDoCache.UpdateParentsUi(Id, parents), ct),
                ct
            );
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemChildrenAsync(CancellationToken ct)
    {
        return this.PostUiBackground(
                () =>
                {
                    if (Item is null)
                    {
                        ToDoSubItemsViewModel.ClearExceptUi(
                            ReadOnlyMemory<ToDoItemEntityNotify>.Empty
                        );

                        return Result.Success;
                    }

                    if (Item.Children.Count == 0)
                    {
                        ToDoSubItemsViewModel.ClearExceptUi(
                            ReadOnlyMemory<ToDoItemEntityNotify>.Empty
                        );

                        return Result.Success;
                    }

                    return ToDoSubItemsViewModel.ClearExceptUi(Item.Children.ToArray());
                },
                ct
            )
            .IfSuccessAsync(() => toDoService.GetChildrenToDoItemIdsAsync(Id, ct), ct)
            .IfSuccessAsync(
                ids => this.InvokeUiBackgroundAsync(() => toDoCache.UpdateChildrenItemsUi(Id, ids)),
                ct
            )
            .IfSuccessAsync(items => ToDoSubItemsViewModel.UpdateItemsAsync(items, false, ct), ct);
    }

    private Result UpdateCommandItemsUi()
    {
        CommandItems.Clear();

        if (Item is null)
        {
            return Result.Success;
        }

        CommandItems.AddRange(
            ToDoSubItemsViewModel.List.IsMulti ? Item.MultiCommands : Item.SingleCommands
        );

        return Result.Success;
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new ToDoItemViewModelSetting(this), ct);
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting
            .CastObject<ToDoItemViewModelSetting>()
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
            UpdateCommandItemsUi();
        }
    }
}
