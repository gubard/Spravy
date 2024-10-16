namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityOffsetToDoItemSettingsViewModel : IconViewModel, IToDoItemSettings
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

    public PeriodicityOffsetToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        IObjectStorage objectStorage
    )
        : base(objectStorage)
    {
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

    public override string ViewId =>
        TypeCache<PeriodicityOffsetToDoItemSettingsViewModel>.Type.Name;

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
