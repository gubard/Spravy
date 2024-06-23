namespace Spravy.Ui.Features.ToDo.ViewModels;

public class TodayToDoItemsViewModel : NavigatableViewModelBase, IRefresh, IToDoSubItemsViewModelProperty
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;

    public TodayToDoItemsViewModel(
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        IToDoCache toDoCache,
        SpravyCommandNotifyService spravyCommandNotifyService,
        ITaskProgressService taskProgressService
    ) : base(true)
    {
        Commands = new();
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);

        ToDoSubItemsViewModel.List
           .WhenAnyValue(x => x.IsMulti)
           .Subscribe(x =>
            {
                Commands.Clear();

                if (x)
                {
                    Commands.AddRange(spravyCommandNotifyService.TodayToDoItemsMulti.ToArray());
                }
            });
    }

    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }
    public SpravyCommand InitializedCommand { get; }

    public override string ViewId
    {
        get => TypeCache<TodayToDoItemsViewModel>.Type.Name;
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return toDoService.GetTodayToDoItemsAsync(cancellationToken)
           .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), cancellationToken)
           .IfSuccessAsync(ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), false, cancellationToken),
                cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableSuccess;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableSuccess;
    }
}