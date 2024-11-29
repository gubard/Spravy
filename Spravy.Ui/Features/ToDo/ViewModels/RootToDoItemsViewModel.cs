namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class RootToDoItemsViewModel : NavigatableViewModelBase, IRemove, IToDoItemEditId
{
    private readonly TaskWork refreshWork;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoUiService toDoUiService;
    private readonly SpravyCommandNotifyService spravyCommandNotifyService;
    
    [ObservableProperty]
    private bool isMulti;

    public RootToDoItemsViewModel(
        SpravyCommandNotifyService spravyCommandNotifyService,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IObjectStorage objectStorage,
        IErrorHandler errorHandler,
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

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(IsMulti))
            {
                UpdateCommands();
                ToDoSubItemsViewModel.List.IsMulti = IsMulti;
            }
        };
    }

    public AvaloniaList<SpravyCommandNotify> Commands { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }

    public override string ViewId
    {
        get => TypeCache<RootToDoItemsViewModel>.Type.Name;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return RefreshCore().ConfigureAwait(false);
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
                            IsMulti = s.IsMulti;

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
    }
    
    private void UpdateCommands()
    {
        if (IsMulti)
        {
            Commands.UpdateUi(spravyCommandNotifyService.ToDoItemCommands);
        }
        else
        {
            Commands.Clear();
        }
    }

    public Result RemoveUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return ToDoSubItemsViewModel.RemoveUi(items);
    }

    public Result<ToDoItemEditId> GetToDoItemEditId()
    {
        if (!ToDoSubItemsViewModel.List.IsMulti)
        {
            return new(new NonItemSelectedError());
        }

        return ToDoSubItemsViewModel
            .GetSelectedItems()
            .IfSuccess(items => new ToDoItemEditId(new(), items).ToResult());
    }
}
