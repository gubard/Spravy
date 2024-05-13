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
    
    public ConfiguredValueTaskAwaitable<Result> ClearExceptAsync(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return Items.ClearExceptAsync(ids);
    }
    
    public void UpdateItem(ToDoItemEntityNotify item)
    {
        Items.UpdateItem(item);
    }
}