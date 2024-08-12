namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class LeafToDoItemsViewModel
    : NavigatableViewModelBase,
        IRefresh,
        IToDoItemUpdater,
        IToDoSubItemsViewModelProperty,
        IObjectParameters
{
    private static readonly ReadOnlyMemory<char> headerParameterName = nameof(Header).AsMemory();

    private readonly TaskWork refreshWork;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoUiService toDoUiService;
    private readonly SpravyCommandNotifyService spravyCommandNotifyService;

    [ObservableProperty]
    private ToDoItemEntityNotify? item;

    public LeafToDoItemsViewModel(
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        ITaskProgressService taskProgressService,
        SpravyCommandNotifyService spravyCommandNotifyService,
        IToDoUiService toDoUiService
    )
        : base(true)
    {
        this.spravyCommandNotifyService = spravyCommandNotifyService;
        this.toDoUiService = toDoUiService;
        Commands = new();
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.objectStorage = objectStorage;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );

        ToDoSubItemsViewModel.List.PropertyChanged += OnPropertyChanged;
    }

    public SpravyCommand InitializedCommand { get; }
    public AvaloniaList<ToDoItemEntityNotify> Items { get; } = new();
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }

    public string Header
    {
        get => Item?.Name ?? Items.Select(x => x.Name).JoinString(", ");
    }

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
        if (Items.IsEmpty())
        {
            return Item.IfNotNull(nameof(Item))
                .IfSuccessAsync(
                    i => toDoUiService.UpdateLeafToDoItemsAsync(i, ToDoSubItemsViewModel, ct),
                    ct
                );
        }

        return Items
            .ToArray()
            .ToReadOnlyMemory()
            .ToResult()
            .IfSuccessForEachAllAsync(
                i => toDoUiService.UpdateLeafToDoItemsAsync(i, ToDoSubItemsViewModel, ct),
                ct
            );
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
            return ToDoSubItemsViewModel.List.AddOrUpdateUi(item);
        }

        return Result.Success;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToDoSubItemsViewModel.List.IsMulti))
        {
            if (ToDoSubItemsViewModel.List.IsMulti)
            {
                Commands.UpdateUi(spravyCommandNotifyService.LeafToDoItemsMulti);
            }
            else
            {
                Commands.Clear();
            }
        }
    }

    public Result<string> GetParameter(ReadOnlySpan<char> parameterName)
    {
        if (headerParameterName.Span.AreEquals(parameterName))
        {
            return Header.ToResult();
        }

        return new(new NotFoundNamedError(parameterName.ToString()));
    }

    public Result SetParameter(ReadOnlySpan<char> parameterName, ReadOnlySpan<char> parameterValue)
    {
        return new(new NotImplementedError(nameof(SetParameter)));
    }
}
