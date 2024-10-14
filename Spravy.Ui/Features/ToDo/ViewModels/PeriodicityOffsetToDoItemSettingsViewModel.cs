namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityOffsetToDoItemSettingsViewModel : ViewModelBase, IEditToDoItems
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

    [ObservableProperty]
    private string icon = string.Empty;

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
        Icon = item.Icon;
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
            .SetIcon(new(Icon));
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
        Item.Icon = Icon;

        return Result.Success;
    }
}
