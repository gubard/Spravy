namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PlannedToDoItemSettingsViewModel : ViewModelBase, IEditToDoItems
{
    [ObservableProperty]
    private bool isRequiredCompleteInDueDate;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private DateOnly dueDate;

    [ObservableProperty]
    private uint remindDaysBefore;

    public PlannedToDoItemSettingsViewModel(ToDoItemEntityNotify item)
    {
        Item = item;
        DueDate = item.DueDate;
        IsRequiredCompleteInDueDate = Item.IsRequiredCompleteInDueDate;
        ChildrenType = item.ChildrenType;
        RemindDaysBefore = item.RemindDaysBefore;
    }

    private ToDoItemEntityNotify Item { get; }

    public EditToDoItems GetEditToDoItems()
    {
        return new EditToDoItems()
            .SetIds(new[] { Item.Id })
            .SetChildrenType(new(ChildrenType))
            .SetDueDate(new(DueDate))
            .SetIsRequiredCompleteInDueDate(new(IsRequiredCompleteInDueDate))
            .SetRemindDaysBefore(new(RemindDaysBefore));
    }

    public Result UpdateItemUi()
    {
        Item.IsRequiredCompleteInDueDate = IsRequiredCompleteInDueDate;
        Item.ChildrenType = ChildrenType;
        Item.DueDate = DueDate;
        Item.RemindDaysBefore = RemindDaysBefore;

        return Result.Success;
    }
}
