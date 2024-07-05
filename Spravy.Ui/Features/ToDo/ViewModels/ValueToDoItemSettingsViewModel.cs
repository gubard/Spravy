namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ValueToDoItemSettingsViewModel
    : ViewModelBase,
        IToDoChildrenTypeProperty,
        IApplySettings
{
    private readonly IToDoService toDoService;

    public ValueToDoItemSettingsViewModel(
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public ToDoItemChildrenType ChildrenType { get; set; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, ct);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return toDoService
            .GetValueToDoItemSettingsAsync(Id, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(() =>
                    {
                        ChildrenType = setting.ChildrenType;

                        return Result.Success;
                    }),
                ct
            );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }
}
