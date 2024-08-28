namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class LeafToDoItemsViewModel
    : NavigatableViewModelBase,
        IObjectParameters,
        IToDoItemsView
{
    private static readonly ReadOnlyMemory<char> headerParameterName = nameof(Header).AsMemory();

    private readonly AvaloniaList<ToDoItemEntityNotify> items = new();

    private readonly TaskWork refreshWork;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoUiService toDoUiService;
    private readonly SpravyCommandNotifyService spravyCommandNotifyService;

    public LeafToDoItemsViewModel(
        ToDoItemEntityNotify? item,
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        ITaskProgressService taskProgressService,
        SpravyCommandNotifyService spravyCommandNotifyService,
        IToDoUiService toDoUiService
    )
        : base(true)
    {
        Item = item;
        this.items.AddRange(items.ToArray());
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
    public ToDoItemEntityNotify? Item { get; }
    public IAvaloniaReadOnlyList<ToDoItemEntityNotify> Items => items;
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

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return RefreshCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> RefreshCore(CancellationToken ct)
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private Cvtar RefreshCoreAsync(CancellationToken ct)
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

    private Cvtar InitializedAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<LeafToDoItemsViewModelSetting>(ViewId, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
                        () =>
                        {
                            ToDoSubItemsViewModel.List.IsMulti = setting.IsMulti;
                            ToDoSubItemsViewModel.List.GroupBy = setting.GroupBy;

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new LeafToDoItemsViewModelSetting(this), ct);
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

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return ToDoSubItemsViewModel.ClearExceptUi(items);
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        return ToDoSubItemsViewModel.AddOrUpdateUi(item);
    }
}
