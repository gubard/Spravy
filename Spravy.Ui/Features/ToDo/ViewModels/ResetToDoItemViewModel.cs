namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ResetToDoItemViewModel : NavigatableViewModelBase
{
    private readonly IObjectStorage objectStorage;
    private readonly IToDoUiService toDoUiService;

    [ObservableProperty]
    private bool isCompleteCurrentTask;

    [ObservableProperty]
    private bool isCompleteChildrenTask;

    [ObservableProperty]
    private bool isMoveCircleOrderIndex = true;

    [ObservableProperty]
    private bool isOnlyCompletedTasks;

    public ResetToDoItemViewModel(
        ToDoItemEntityNotify item,
        IObjectStorage objectStorage,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
    )
        : base(true)
    {
        this.objectStorage = objectStorage;
        this.toDoUiService = toDoUiService;
        Item = item;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public ToDoItemEntityNotify Item { get; }
    public SpravyCommand InitializedCommand { get; }

    public override string ViewId
    {
        get => $"{TypeCache<ResetToDoItemViewModel>.Type.Name}:{Item.Id}";
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<ResetToDoItemViewModelSetting>(ViewId, ct)
            .IfSuccessAsync(obj => SetStateAsync(obj, ct), ct)
            .IfSuccessAsync(() => toDoUiService.UpdateItemAsync(Item, ct), ct);
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting
            .CastObject<ResetToDoItemViewModelSetting>()
            .IfSuccess(s =>
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
                )
            )
            .ToValueTaskResult()
            .ConfigureAwait(false);
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new ResetToDoItemViewModelSetting(this), ct);
    }
}
