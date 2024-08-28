namespace Spravy.Ui.Features.ToDo.ViewModels;

public class TodayToDoItemsViewModel : NavigatableViewModelBase, IToDoItemsView
{
    private readonly IToDoUiService toDoUiService;
    private readonly SpravyCommandNotifyService spravyCommandNotifyService;

    public TodayToDoItemsViewModel(
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IErrorHandler errorHandler,
        SpravyCommandNotifyService spravyCommandNotifyService,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
    )
        : base(true)
    {
        Commands = new();
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.spravyCommandNotifyService = spravyCommandNotifyService;
        this.toDoUiService = toDoUiService;

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

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return toDoUiService.UpdateTodayItemsAsync(ToDoSubItemsViewModel, ct);
    }

    private Cvtar InitializedAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
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

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return ToDoSubItemsViewModel.ClearExceptUi(items);
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        return ToDoSubItemsViewModel.AddOrUpdateUi(item);
    }
}
