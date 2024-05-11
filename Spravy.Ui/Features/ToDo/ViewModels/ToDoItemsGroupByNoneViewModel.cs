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

    public void Clear()
    {
        Items.Clear();
    }

    public void AddItems(IEnumerable<ToDoItemEntityNotify> items)
    {
        Items.AddItems(items);
    }

    public void ClearExcept(ReadOnlyMemory<Guid> ids)
    {
        Items.ClearExcept(ids);
    }

    public void UpdateItem(ToDoItemEntityNotify item)
    {
        Items.UpdateItem(item);
    }
}