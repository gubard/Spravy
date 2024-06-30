namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoSelectorItem
{
    public ToDoSelectorItem(
        Guid id,
        string name,
        uint orderIndex,
        ReadOnlyMemory<ToDoSelectorItem> children
    )
    {
        Id = id;
        Name = name;
        Children = children;
        OrderIndex = orderIndex;
    }

    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public ReadOnlyMemory<ToDoSelectorItem> Children { get; }
}
