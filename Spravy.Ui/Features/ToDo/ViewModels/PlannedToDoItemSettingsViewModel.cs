namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PlannedToDoItemSettingsViewModel : IconViewModel, IToDoItemSettings
{
    [ObservableProperty]
    private bool isRequiredCompleteInDueDate;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private DateOnly dueDate;

    public PlannedToDoItemSettingsViewModel(ToDoItemEntityNotify item, IObjectStorage objectStorage)
        : base(objectStorage)
    {
        Item = item;
        DueDate = item.DueDate;
        IsRequiredCompleteInDueDate = Item.IsRequiredCompleteInDueDate;
        ChildrenType = item.ChildrenType;
        Icon = item.Icon;
    }

    private ToDoItemEntityNotify Item { get; }
    public override string ViewId => TypeCache<PlannedToDoItemSettingsViewModel>.Type.Name;

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
