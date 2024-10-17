namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ValueToDoItemSettingsViewModel : ViewModelBase, IEditToDoItems
{
    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    public ValueToDoItemSettingsViewModel(ToDoItemEntityNotify item)
    {
        Item = item;
        ChildrenType = item.ChildrenType;
    }

    public ToDoItemEntityNotify Item { get; }

    public EditToDoItems GetEditToDoItems()
    {
        return new EditToDoItems().SetIds(new[] { Item.Id }).SetChildrenType(new(ChildrenType));
    }

    public Result UpdateItemUi()
    {
        Item.ChildrenType = ChildrenType;

        return Result.Success;
    }
}
