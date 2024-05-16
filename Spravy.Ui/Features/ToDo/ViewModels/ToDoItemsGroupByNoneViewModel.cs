namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByNoneViewModel : ViewModelBase
{
    private readonly ToDoItemsViewModel items;
    
    [Inject]
    public required ToDoItemsViewModel Items
    {
        get => items;
        [MemberNotNull(nameof(items))]
        init
        {
            items = value;
            items.Header = new("ToDoItemsGroupByNoneView.Header");
        }
    }
    
    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return Items.ClearExceptUi(ids);
    }
    
    public void UpdateItemUi(ToDoItemEntityNotify item)
    {
        Items.UpdateItemUi(item);
    }
}