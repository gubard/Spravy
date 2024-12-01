namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ResetToDoItemViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IObjectStorage objectStorage;
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private bool isCompleteChildrenTask;

    [ObservableProperty]
    private bool isCompleteCurrentTask;

    [ObservableProperty]
    private bool isMoveCircleOrderIndex = true;

    [ObservableProperty]
    private bool isOnlyCompletedTasks;

    public ResetToDoItemViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        IObjectStorage objectStorage,
        IToDoService toDoService
    ) : base(item, items)
    {
        this.objectStorage = objectStorage;
        this.toDoService = toDoService;
    }

    public override string ViewId =>
        EditItem.TryGetValue(out var editItem)
            ? $"{TypeCache<ResetToDoItemViewModel>.Type.Name}:{editItem.Id}"
            : $"{TypeCache<ResetToDoItemViewModel>.Type.Name}";

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return ResultCurrentIds.ToResult()
           .IfSuccessForEach(
                id => new ResetToDoItemOptions(
                    id,
                    IsCompleteChildrenTask,
                    IsMoveCircleOrderIndex,
                    IsOnlyCompletedTasks,
                    IsCompleteCurrentTask
                ).ToResult()
            )
           .IfSuccessAsync(options => toDoService.ResetToDoItemAsync(options, ct), ct);
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage.GetObjectOrDefaultAsync<ResetToDoItemViewModelSetting>(ViewId, ct)
           .IfSuccessAsync(
                s => this.PostUiBackground(
                    () =>
                    {
                        IsCompleteChildrenTask = s.IsCompleteChildrenTask;
                        IsMoveCircleOrderIndex = s.IsMoveCircleOrderIndex;
                        IsOnlyCompletedTasks = s.IsOnlyCompletedTasks;
                        IsCompleteCurrentTask = s.IsCompleteCurrentTask;

                        return Result.Success;
                    },
                    ct
                ),
                ct
            );
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new ResetToDoItemViewModelSetting(this), ct);
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}