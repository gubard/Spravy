namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityOffsetToDoItemSettingsViewModel
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

    [ObservableProperty]
    private bool isRequiredCompleteInDueDate;

    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private ushort daysOffset;

    [ObservableProperty]
    private DateOnly dueDate;

    [ObservableProperty]
    private ushort monthsOffset;

    [ObservableProperty]
    private ushort weeksOffset;

    [ObservableProperty]
    private ushort yearsOffset;

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
                    this.PostUiBackground(
                        () =>
                        {
                            ChildrenType = setting.ChildrenType;
                            DueDate = setting.DueDate;
                            MonthsOffset = setting.MonthsOffset;
                            YearsOffset = setting.YearsOffset;
                            DaysOffset = setting.DaysOffset;
                            WeeksOffset = setting.WeeksOffset;
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
