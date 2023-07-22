using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct ToDoItemGroup : IToDoItem
{
    public ToDoItemGroup(Guid id, string name, IToDoSubItem[] items, ToDoItemParent[] parents, string description)
    {
        Id = id;
        Name = name;
        Items = items;
        Parents = parents;
        Description = description;
    }

    public Guid Id { get; }
    public string Name { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public string Description { get; }
}