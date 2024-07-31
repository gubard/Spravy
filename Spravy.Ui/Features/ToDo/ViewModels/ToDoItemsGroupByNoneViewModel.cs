namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByNoneViewModel : ViewModelBase
{
    public ToDoItemsGroupByNoneViewModel(ToDoItemsViewModel items)
    {
        items.Header = new("ToDoItemsGroupByNoneView.Header");
        Items = items;
    }

    public ToDoItemsViewModel Items { get; }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return Items.ClearExceptUi(ids);
    }

    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        return Items.UpdateItemUi(item);
    }
}
