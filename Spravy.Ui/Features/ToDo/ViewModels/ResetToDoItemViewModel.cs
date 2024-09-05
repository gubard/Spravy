namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ResetToDoItemViewModel : DialogableViewModelBase
{
    private readonly IObjectStorage objectStorage;

    [ObservableProperty]
    private bool isCompleteCurrentTask;

    [ObservableProperty]
    private bool isCompleteChildrenTask;

    [ObservableProperty]
    private bool isMoveCircleOrderIndex = true;

    [ObservableProperty]
    private bool isOnlyCompletedTasks;

    public ResetToDoItemViewModel(Option<ToDoItemEntityNotify> item, IObjectStorage objectStorage)
    {
        this.objectStorage = objectStorage;
        Item = item.GetNullable();
    }

    public ToDoItemEntityNotify? Item { get; }

    public override string ViewId
    {
        get =>
            Item is null
                ? $"{TypeCache<ResetToDoItemViewModel>.Type.Name}"
                : $"{TypeCache<ResetToDoItemViewModel>.Type.Name}:{Item.CurrentId}";
    }

    public Result Stop()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<ResetToDoItemViewModelSetting>(ViewId, ct)
            .IfSuccessAsync(
                s =>
                    this.PostUiBackground(
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
}
