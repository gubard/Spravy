namespace Spravy.Ui.Features.ToDo.Models;

public readonly struct ToDoItemEditId
{
    public ToDoItemEditId(Option<ToDoItemEntityNotify> item, ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        Item = item;
        Items = items;
        ResultItems = GetResultItems();
        ResultIds = ResultItems.Select(x => x.Id);
        ResultCurrentIds = ResultItems.Select(x => x.CurrentId);
    }

    public Option<ToDoItemEntityNotify> Item { get; }
    public ReadOnlyMemory<ToDoItemEntityNotify> Items { get; }
    public ReadOnlyMemory<Guid> ResultIds { get; }
    public ReadOnlyMemory<Guid> ResultCurrentIds { get; }
    public ReadOnlyMemory<ToDoItemEntityNotify> ResultItems { get; }

    private ReadOnlyMemory<ToDoItemEntityNotify> GetResultItems()
    {
        if (!Item.TryGetValue(out var item))
        {
            return Items;
        }

        if (Items.IsEmpty)
        {
            return new[]
            {
                item,
            };
        }

        return Items;
    }
}