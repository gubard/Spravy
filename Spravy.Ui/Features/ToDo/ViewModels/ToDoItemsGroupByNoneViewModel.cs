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
            items?.Dispose();
            items = value;
            items.Header = new("ToDoItemsGroupByNoneView.Header");
            Disposables.Add(items);
        }
    }

    public void Clear()
    {
        Items.Clear();
    }

    public void AddItems(IEnumerable<Selected<ToDoItemNotify>> items)
    {
        Items.AddItems(items);
    }

    public void ClearExcept(IEnumerable<Guid> ids)
    {
        Items.ClearExcept(ids);
    }

    public void UpdateItem(Selected<ToDoItemNotify> item, bool updateOrder)
    {
        Items.UpdateItem(item, updateOrder);
    }
}