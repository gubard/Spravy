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
        bool isFavorite
    )
    {
        Id = id;
        Name = name;
        Items = items;
        Parents = parents;
        Description = description;
        IsFavorite = isFavorite;
    }

    public Guid Id { get; }
    public string Name { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public bool IsFavorite { get; }
    public string Description { get; }
}