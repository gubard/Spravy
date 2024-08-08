namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PlannedToDoItemSettingsViewModel
    : ViewModelBase,
        IToDoChildrenTypeProperty,
        IToDoDueDateProperty,
        IIsRequiredCompleteInDueDateProperty,
        IApplySettings
{
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private bool isRequiredCompleteInDueDate;

    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private DateOnly dueDate;

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

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return toDoService
            .GetPlannedToDoItemSettingsAsync(Id, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
                        () =>
                        {
                            ChildrenType = setting.ChildrenType;
                            DueDate = setting.DueDate;
                            IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;

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
