using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoItemValue : IToDoItem
{
    public ToDoItemValue(
        Guid id,
        string name,
        IToDoSubItem[] items,
        ToDoItemParent[] parents,
        bool isCompleted,
        string description,
        bool isFavorite,
        ToDoItemChildrenType childrenType
    )
    {
        Name = name;
        Items = items;
        Parents = parents;
        IsCompleted = isCompleted;
        Description = description;
        IsFavorite = isFavorite;
        ChildrenType = childrenType;
        Id = id;
    }

    public Guid Id { get; }
    public string Name { get; }
    public bool IsCompleted { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public bool IsFavorite { get; }
    public ToDoItemChildrenType ChildrenType { get; }
    public string Description { get; }
}