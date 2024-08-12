namespace Spravy.Ui.Features.ToDo.ViewModels;

public class TodayToDoItemsViewModel : NavigatableViewModelBase, IRefresh
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;
    private readonly SpravyCommandNotifyService spravyCommandNotifyService;

    public TodayToDoItemsViewModel(
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        IToDoCache toDoCache,
        SpravyCommandNotifyService spravyCommandNotifyService,
        ITaskProgressService taskProgressService
    )
        : base(true)
    {
        Commands = new();
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        this.spravyCommandNotifyService = spravyCommandNotifyService;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );

        ToDoSubItemsViewModel.List.PropertyChanged += OnPropertyChanged;
    }

    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }
    public SpravyCommand InitializedCommand { get; }

    public override string ViewId
    {
        get => TypeCache<TodayToDoItemsViewModel>.Type.Name;
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return toDoService
            .GetTodayToDoItemsAsync(ct)
            .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), ct)
            .IfSuccessAsync(ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids, false, ct), ct);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToDoSubItemsViewModel.List.IsMulti))
        {
            if (ToDoSubItemsViewModel.List.IsMulti)
            {
                Commands.UpdateUi(spravyCommandNotifyService.TodayToDoItemsMulti);
            }
            else
            {
                Commands.Clear();
            }
        }
    }
}
