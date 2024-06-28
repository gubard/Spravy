namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByNoneViewModel : ViewModelBase
{
    public ToDoItemsGroupByNoneViewModel(ToDoItemsViewModel items)
    {
        items.Header = new("ToDoItemsGroupByNoneView.Header");
        Items = items;
        this.WhenAnyValue(x => x.IsMulti).Subscribe(x => Items.IsMulti = x);
    }

    public ToDoItemsViewModel Items { get; }

    [Reactive]
    public bool IsMulti { get; set; }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return Items.ClearExceptUi(ids);
    }

    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        return Items.UpdateItemUi(item);
    }
}
