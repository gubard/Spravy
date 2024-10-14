namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemViewModel : NavigatableViewModelBase, IRemove, IToDoItemEditId
{
    private readonly TaskWork refreshWork;
    private readonly IObjectStorage objectStorage;
    private readonly IToDoUiService toDoUiService;

    public ToDoItemViewModel(
        ToDoItemEntityNotify item,
        IObjectStorage objectStorage,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IErrorHandler errorHandler,
        IToDoUiService toDoUiService,
        ITaskProgressService taskProgressService
    )
        : base(true)
    {
        this.objectStorage = objectStorage;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoUiService = toDoUiService;
        Item = item;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);

        InitializedCommand = SpravyCommand.Create<ToDoItemViewModel>(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }
    public ToDoItemEntityNotify Item { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }

    public override string ViewId
    {
        get => $"{TypeCache<ToDoItemViewModel>.Type.Name}:{Item.Id}";
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return RefreshCore().ConfigureAwait(false);
    }

    private Cvtar InitializedAsync(ToDoItemViewModel viewModel, CancellationToken ct)
    {
        return viewModel.RefreshAsync(ct);
    }

    private async ValueTask<Result> RefreshCore()
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private Cvtar RefreshCoreAsync(CancellationToken ct)
    {
        return toDoUiService.UpdateItemChildrenAsync(Item, ToDoSubItemsViewModel, ct);
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

    public Result RemoveUi(ToDoItemEntityNotify item)
    {
        return ToDoSubItemsViewModel.RemoveUi(item);
    }

    public Result<ToDoItemEditId> GetToDoItemEditId()
    {
        if (!ToDoSubItemsViewModel.List.IsMulti)
        {
            return new ToDoItemEditId(
                Item.ToOption(),
                ReadOnlyMemory<ToDoItemEntityNotify>.Empty
            ).ToResult();
        }

        return ToDoSubItemsViewModel
            .GetSelectedItems()
            .IfSuccess(selected => new ToDoItemEditId(Item.ToOption(), selected).ToResult());
    }
}
