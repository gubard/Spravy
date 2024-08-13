namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityOffsetToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private bool isRequiredCompleteInDueDate;

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
        ToDoItemEntityNotify item,
        IToDoService toDoService
    )
    {
        this.toDoService = toDoService;
        Item = item;
        IsRequiredCompleteInDueDate = item.IsRequiredCompleteInDueDate;
        ChildrenType = item.ChildrenType;
        DaysOffset = item.DaysOffset;
        DueDate = item.DueDate;
        MonthsOffset = item.MonthsOffset;
        WeeksOffset = item.WeeksOffset;
        YearsOffset = item.YearsOffset;
    }

    public ToDoItemEntityNotify Item { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService
            .UpdateToDoItemDaysOffsetAsync(Item.Id, DaysOffset, ct)
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemWeeksOffsetAsync(Item.Id, WeeksOffset, ct),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemYearsOffsetAsync(Item.Id, YearsOffset, ct),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemMonthsOffsetAsync(Item.Id, MonthsOffset, ct),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemChildrenTypeAsync(Item.Id, ChildrenType, ct),
                ct
            )
            .IfSuccessAsync(() => toDoService.UpdateToDoItemDueDateAsync(Item.Id, DueDate, ct), ct)
            .IfSuccessAsync(
                () =>
                    toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                        Item.Id,
                        IsRequiredCompleteInDueDate,
                        ct
                    ),
                ct
            );
    }
}
