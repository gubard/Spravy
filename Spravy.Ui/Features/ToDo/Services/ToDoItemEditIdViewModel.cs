namespace Spravy.Ui.Features.ToDo.Services;

public abstract class ToDoItemEditIdViewModel : DialogableViewModelBase
{
    protected ToDoItemEditIdViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems
    )
    {
        EditItem = editItem;
        EditItems = editItems;
        ResultItems = GetResultItems();
        ResultIds = ResultItems.Select(x => x.Id);
        ResultCurrentIds = ResultItems.Select(x => x.CurrentId);
    }

    protected Option<ToDoItemEntityNotify> EditItem { get; }
    protected ReadOnlyMemory<ToDoItemEntityNotify> EditItems { get; }
    protected ReadOnlyMemory<Guid> ResultIds { get; }
    protected ReadOnlyMemory<Guid> ResultCurrentIds { get; }
    protected ReadOnlyMemory<ToDoItemEntityNotify> ResultItems { get; }

    public string Name
    {
        get
        {
            if (!EditItem.TryGetValue(out var item))
            {
                return EditItems.Select(x => x.Name).JoinString(",");
            }

            if (EditItems.IsEmpty)
            {
                return item.Name;
            }

            return EditItems.Select(x => x.Name).JoinString(",");
        }
    }

    public object[] Path
    {
        get
        {
            if (!EditItem.TryGetValue(out var item))
            {
                return [];
            }

            return item.Path;
        }
    }

    private ReadOnlyMemory<ToDoItemEntityNotify> GetResultItems()
    {
        if (!EditItem.TryGetValue(out var item))
        {
            return EditItems;
        }

        if (EditItems.IsEmpty)
        {
            return new[] { item };
        }

        return EditItems;
    }
}
