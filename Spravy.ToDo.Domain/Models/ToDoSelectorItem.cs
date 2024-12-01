namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoSelectorItem
{
    public ToDoSelectorItem(ToDoShortItem item, ReadOnlyMemory<ToDoSelectorItem> children)
    {
        Children = children;
        Item = item;
    }

    public ToDoShortItem Item { get; }
    public ReadOnlyMemory<ToDoSelectorItem> Children { get; }
}