namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ResetToDoItemViewModel : NavigatableViewModelBase
{
    public ResetToDoItemViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public bool IsCompleteCurrentTask { get; set; }

    [Reactive]
    public bool IsCompleteChildrenTask { get; set; }

    [Reactive]
    public bool IsMoveCircleOrderIndex { get; set; } = true;

    [Reactive]
    public bool IsOnlyCompletedTasks { get; set; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    public override string ViewId
    {
        get => $"{TypeCache<ResetToDoItemViewModel>.Type.Name}:{Id}";
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.GetObjectOrDefaultAsync<ResetToDoItemViewModelSetting>(ViewId, cancellationToken)
           .IfSuccessAsync(obj => SetStateAsync(obj, cancellationToken), cancellationToken);
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<ResetToDoItemViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUiBackgroundAsync(() =>
            {
                IsCompleteChildrenTask = s.IsCompleteChildrenTask;
                IsMoveCircleOrderIndex = s.IsMoveCircleOrderIndex;
                IsOnlyCompletedTasks = s.IsOnlyCompletedTasks;
                IsCompleteCurrentTask = s.IsCompleteCurrentTask;
            }), cancellationToken);
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new ResetToDoItemViewModelSetting(this));
    }

    [ProtoContract]
    private class ResetToDoItemViewModelSetting : IViewModelSetting<ResetToDoItemViewModelSetting>
    {
        public ResetToDoItemViewModelSetting(ResetToDoItemViewModel viewModel)
        {
            IsCompleteChildrenTask = viewModel.IsCompleteChildrenTask;
            IsMoveCircleOrderIndex = viewModel.IsMoveCircleOrderIndex;
            IsOnlyCompletedTasks = viewModel.IsOnlyCompletedTasks;
            IsCompleteCurrentTask = viewModel.IsCompleteCurrentTask;
        }

        public ResetToDoItemViewModelSetting()
        {
        }

        static ResetToDoItemViewModelSetting()
        {
            Default = new()
            {
                IsMoveCircleOrderIndex = true,
            };
        }

        [ProtoMember(1)]
        public bool IsCompleteChildrenTask { get; set; }

        [ProtoMember(2)]
        public bool IsMoveCircleOrderIndex { get; set; }

        [ProtoMember(3)]
        public bool IsOnlyCompletedTasks { get; set; }

        [ProtoMember(4)]
        public bool IsCompleteCurrentTask { get; set; }

        public static ResetToDoItemViewModelSetting Default { get; }
    }
}