namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ValueToDoItemSettingsViewModel : IconViewModel, IToDoItemSettings
{
    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    public ValueToDoItemSettingsViewModel(ToDoItemEntityNotify item, IObjectStorage objectStorage)
        : base(objectStorage)
    {
        Item = item;
        Icon = item.Icon;
        ChildrenType = item.ChildrenType;
    }

    public ToDoItemEntityNotify Item { get; }
    public override string ViewId => TypeCache<ValueToDoItemSettingsViewModel>.Type.Name;

    public EditToDoItems GetEditToDoItems()
    {
        return new EditToDoItems()
            .SetIds(new[] { Item.Id })
            .SetChildrenType(new(ChildrenType))
            .SetIcon(new(Icon));
    }

    public Result UpdateItemUi()
    {
        Item.ChildrenType = ChildrenType;
        Item.Icon = Icon;

        return Result.Success;
    }
}
