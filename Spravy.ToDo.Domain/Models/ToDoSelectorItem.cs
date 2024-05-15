namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoSelectorItem
{
    public ToDoSelectorItem(Guid id, string name, ReadOnlyMemory<ToDoSelectorItem> children)
    {
        Id = id;
        Name = name;
        Children = children;
    }

    public Guid Id { get; }
    public string Name { get; }
    public ReadOnlyMemory<ToDoSelectorItem> Children { get; }
}