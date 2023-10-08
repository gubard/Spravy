using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoItemGroup : IToDoItem
{
    public ToDoItemGroup(
        Guid id,
        string name,
        IToDoSubItem[] items,
        ToDoItemParent[] parents,
        string description,
        bool isPinned
    )
    {
        Id = id;
        Name = name;
        Items = items;
        Parents = parents;
        Description = description;
        IsPinned = isPinned;
    }

    public Guid Id { get; }
    public string Name { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public bool IsPinned { get; }
    public string Description { get; }
}