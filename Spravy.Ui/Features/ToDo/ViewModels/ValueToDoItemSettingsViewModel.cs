namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ValueToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    public ValueToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        Item = item;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public ToDoItemEntityNotify Item { get; }
    public SpravyCommand InitializedCommand { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateToDoItemChildrenTypeAsync(Item.Id, ChildrenType, ct);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return toDoService
            .GetValueToDoItemSettingsAsync(Item.Id, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
                        () =>
                        {
                            ChildrenType = setting.ChildrenType;

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }
}
