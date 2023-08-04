namespace Spravy.Domain.Models;

public readonly struct ToDoSelectorItem
{
    public ToDoSelectorItem(Guid id, string name, ToDoSelectorItem[] children)
    {
        Id = id;
        Name = name;
        Children = children;
    }

    public Guid Id { get; }
    public string Name { get; }
    public ToDoSelectorItem[] Children { get; }
}