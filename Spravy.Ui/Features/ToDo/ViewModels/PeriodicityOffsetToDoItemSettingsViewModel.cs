namespace Spravy.Ui.Features.ToDo.ViewModels;

public class PeriodicityOffsetToDoItemSettingsViewModel
    : ViewModelBase,
        IToDoChildrenTypeProperty,
        IToDoDueDateProperty,
        IToDoDaysOffsetProperty,
        IToDoMonthsOffsetProperty,
        IToDoWeeksOffsetProperty,
        IToDoYearsOffsetProperty,
        IIsRequiredCompleteInDueDateProperty,
        IApplySettings
{
    private readonly IToDoService toDoService;

    public PeriodicityOffsetToDoItemSettingsViewModel(
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
    public bool IsRequiredCompleteInDueDate { get; set; }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public ToDoItemChildrenType ChildrenType { get; set; }

    [Reactive]
    public ushort DaysOffset { get; set; }

    [Reactive]
    public DateOnly DueDate { get; set; }

    [Reactive]
    public ushort MonthsOffset { get; set; }

    [Reactive]
    public ushort WeeksOffset { get; set; }

    [Reactive]
    public ushort YearsOffset { get; set; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService
            .UpdateToDoItemDaysOffsetAsync(Id, DaysOffset, ct)
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemWeeksOffsetAsync(Id, WeeksOffset, ct),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemYearsOffsetAsync(Id, YearsOffset, ct),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemMonthsOffsetAsync(Id, MonthsOffset, ct),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, ct),
                ct
            )
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
            .GetPeriodicityOffsetToDoItemSettingsAsync(Id, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(() =>
                    {
                        ChildrenType = setting.ChildrenType;
                        DueDate = setting.DueDate;
                        MonthsOffset = setting.MonthsOffset;
                        YearsOffset = setting.YearsOffset;
                        DaysOffset = setting.DaysOffset;
                        WeeksOffset = setting.WeeksOffset;
                        IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;

                        return Result.Success;
                    }, ct),
                ct
            );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }
}
