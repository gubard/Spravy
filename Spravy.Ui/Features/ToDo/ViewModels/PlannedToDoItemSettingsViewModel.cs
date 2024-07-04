namespace Spravy.Ui.Features.ToDo.ViewModels;

public class PlannedToDoItemSettingsViewModel
    : ViewModelBase,
        IToDoChildrenTypeProperty,
        IToDoDueDateProperty,
        IIsRequiredCompleteInDueDateProperty,
        IApplySettings
{
    private readonly IToDoService toDoService;

    public PlannedToDoItemSettingsViewModel(
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

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService
            .UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, ct)
            .IfSuccessAsync(() => toDoService.UpdateToDoItemDueDateAsync(Id, DueDate, ct), ct)
            .IfSuccessAsync(
                () =>
                    toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                        Id,
                        IsRequiredCompleteInDueDate,
                        ct
                    ),
                ct
            );
    }

    [Reactive]
    public bool IsRequiredCompleteInDueDate { get; set; }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public ToDoItemChildrenType ChildrenType { get; set; }

    [Reactive]
    public DateOnly DueDate { get; set; }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return toDoService
            .GetPlannedToDoItemSettingsAsync(Id, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(() =>
                    {
                        ChildrenType = setting.ChildrenType;
                        DueDate = setting.DueDate;
                        IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;

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
