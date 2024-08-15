namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RootToDoItemsViewModel : NavigatableViewModelBase, IRefresh
{
    private readonly TaskWork refreshWork;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoUiService toDoUiService;
    private readonly SpravyCommandNotifyService spravyCommandNotifyService;

    public RootToDoItemsViewModel(
        SpravyCommandNotifyService spravyCommandNotifyService,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IObjectStorage objectStorage,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
    )
        : base(true)
    {
        Commands = new();
        this.spravyCommandNotifyService = spravyCommandNotifyService;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.objectStorage = objectStorage;
        this.toDoUiService = toDoUiService;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );

        ToDoSubItemsViewModel.List.PropertyChanged += OnPropertyChanged;
    }

    public AvaloniaList<SpravyCommandNotify> Commands { get; }
    public SpravyCommand InitializedCommand { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }

    public override string ViewId
    {
        get => TypeCache<RootToDoItemsViewModel>.Type.Name;
    }

    public Cvtar RefreshAsync(CancellationToken ct)
    {
        return RefreshCore().ConfigureAwait(false);
    }

    private Cvtar InitializedAsync(CancellationToken ct)
    {
        return refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false);
    }

    public async ValueTask<Result> RefreshCore()
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private Cvtar RefreshCoreAsync(CancellationToken ct)
    {
        return toDoUiService.UpdateRootItemsAsync(ToDoSubItemsViewModel, ct);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new RootToDoItemsViewModelSetting(this), ct);
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<RootToDoItemsViewModelSetting>(ViewId, ct)
            .IfSuccessAsync(
                s =>
                    this.PostUiBackground(
                        () =>
                        {
                            ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                            ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
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
                Commands.UpdateUi(spravyCommandNotifyService.RootToDoItemsMulti);
            }
            else
            {
                Commands.Clear();
            }
        }
    }
}
