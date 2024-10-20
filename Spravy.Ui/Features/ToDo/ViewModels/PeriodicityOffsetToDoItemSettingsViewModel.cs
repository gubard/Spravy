namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityOffsetToDoItemSettingsViewModel : ViewModelBase, IEditToDoItems
{
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

    [ObservableProperty]
    private uint remindDaysBefore;

    public PeriodicityOffsetToDoItemSettingsViewModel(ToDoItemEntityNotify item)
    {
        Item = item;
        IsRequiredCompleteInDueDate = item.IsRequiredCompleteInDueDate;
        ChildrenType = item.ChildrenType;
        DaysOffset = item.DaysOffset;
        DueDate = item.DueDate;
        MonthsOffset = item.MonthsOffset;
        WeeksOffset = item.WeeksOffset;
        YearsOffset = item.YearsOffset;
        RemindDaysBefore = item.RemindDaysBefore;
    }

    public ToDoItemEntityNotify Item { get; }

    public EditToDoItems GetEditToDoItems()
    {
        return new EditToDoItems()
            .SetIds(new[] { Item.Id })
            .SetChildrenType(new(ChildrenType))
            .SetDueDate(new(DueDate))
            .SetDaysOffset(new(DaysOffset))
            .SetMonthsOffset(new(MonthsOffset))
            .SetWeeksOffset(new(WeeksOffset))
            .SetYearsOffset(new(YearsOffset))
            .SetIsRequiredCompleteInDueDate(new(IsRequiredCompleteInDueDate))
            .SetRemindDaysBefore(new(RemindDaysBefore));
    }

    public Result UpdateItemUi()
    {
        Item.IsRequiredCompleteInDueDate = IsRequiredCompleteInDueDate;
        Item.ChildrenType = ChildrenType;
        Item.DaysOffset = DaysOffset;
        Item.DueDate = DueDate;
        Item.MonthsOffset = MonthsOffset;
        Item.WeeksOffset = WeeksOffset;
        Item.YearsOffset = YearsOffset;
        Item.RemindDaysBefore = RemindDaysBefore;

        return Result.Success;
    }
}
