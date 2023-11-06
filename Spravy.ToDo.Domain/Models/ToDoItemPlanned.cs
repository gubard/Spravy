using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoItemPlanned : IToDoItem
{
    public ToDoItemPlanned(
        Guid id,
        string name,
        string description,
        IToDoSubItem[] items,
        ToDoItemParent[] parents,
        bool isFavorite,
        DateTimeOffset dueDate,
        bool isCompleted,
        ToDoItemChildrenType childrenType,
        Uri? link
    )
    {
        Id = id;
        Name = name;
        Description = description;
        Items = items;
        Parents = parents;
        IsFavorite = isFavorite;
        DueDate = dueDate;
        IsCompleted = isCompleted;
        ChildrenType = childrenType;
        Link = link;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public bool IsFavorite { get; }
    public Uri? Link { get; }
    public ToDoItemChildrenType ChildrenType { get; }
    public DateTimeOffset DueDate { get; }
    public bool IsCompleted { get; }
}