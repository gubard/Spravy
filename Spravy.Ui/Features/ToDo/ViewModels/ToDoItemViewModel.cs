namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemViewModel : NavigatableViewModelBase, IToDoItemsView
{
    private readonly TaskWork refreshWork;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoUiService toDoUiService;

    public ToDoItemViewModel(
        ToDoItemEntityNotify item,
        IObjectStorage objectStorage,
        ToDoItemCommands commands,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IErrorHandler errorHandler,
        IToDoUiService toDoUiService
    )
        : base(true)
    {
        this.objectStorage = objectStorage;
        Commands = commands;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoUiService = toDoUiService;
        Item = item;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        CommandItems = new();
        ToDoSubItemsViewModel.List.PropertyChanged += OnPropertyChanged;
    }

    public ToDoItemEntityNotify Item { get; }
    public AvaloniaList<SpravyCommandNotify> CommandItems { get; }
    public ToDoItemCommands Commands { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }

    public override string ViewId
    {
        get => $"{TypeCache<ToDoItemViewModel>.Type.Name}:{Item.Id}";
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return RefreshCore().ConfigureAwait(false);
    }

    private async ValueTask<Result> RefreshCore()
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private Cvtar RefreshCoreAsync(CancellationToken ct)
    {
        return toDoUiService
            .UpdateItemChildrenAsync(Item, ToDoSubItemsViewModel, ct)
            .IfSuccessAsync(() => this.PostUiBackground(UpdateCommandItemsUi, ct), ct);
    }

    private Result UpdateCommandItemsUi()
    {
        CommandItems.Clear();

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

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new ToDoItemViewModelSetting(this), ct);
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<ToDoItemViewModelSetting>(ViewId, ct)
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

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToDoSubItemsViewModel.List.IsMulti))
        {
            UpdateCommandItemsUi();
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

    public Result RemoveUi(ToDoItemEntityNotify item)
    {
        return ToDoSubItemsViewModel.RemoveUi(item);
    }
}
