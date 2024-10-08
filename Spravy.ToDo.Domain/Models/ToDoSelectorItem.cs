namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoSelectorItem
{
    public ToDoSelectorItem(
        Guid id,
        string name,
        uint orderIndex,
        ReadOnlyMemory<ToDoSelectorItem> children,
        string icon
    )
    {
        Id = id;
        Name = name;
        Children = children;
        Icon = icon;
        OrderIndex = orderIndex;
    }

    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public string Icon { get; }
    public ReadOnlyMemory<ToDoSelectorItem> Children { get; }
}
