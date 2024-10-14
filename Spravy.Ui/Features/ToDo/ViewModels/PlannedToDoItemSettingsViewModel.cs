namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PlannedToDoItemSettingsViewModel : ViewModelBase, IEditToDoItems
{
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private bool isRequiredCompleteInDueDate;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private DateOnly dueDate;

    [ObservableProperty]
    private string icon = string.Empty;

    public PlannedToDoItemSettingsViewModel(ToDoItemEntityNotify item, IToDoService toDoService)
    {
        Item = item;
        this.toDoService = toDoService;
        DueDate = item.DueDate;
        IsRequiredCompleteInDueDate = Item.IsRequiredCompleteInDueDate;
        ChildrenType = item.ChildrenType;
        Icon = item.Icon;
    }

    private ToDoItemEntityNotify Item { get; }

    public EditToDoItems GetEditToDoItems()
    {
        return new EditToDoItems()
            .SetIds(new[] { Item.Id })
            .SetChildrenType(new(ChildrenType))
            .SetDueDate(new(DueDate))
            .SetIcon(new(Icon))
            .SetIsRequiredCompleteInDueDate(new(IsRequiredCompleteInDueDate));
    }

    public Result UpdateItemUi()
    {
        Item.IsRequiredCompleteInDueDate = IsRequiredCompleteInDueDate;
        Item.ChildrenType = ChildrenType;
        Item.DueDate = DueDate;
        Item.Icon = Icon;

        return Result.Success;
    }
}
