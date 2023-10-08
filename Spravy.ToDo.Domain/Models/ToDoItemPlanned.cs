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
        bool isPinned,
        DateTimeOffset dueDate,
        bool isCompleted,
        ToDoItemChildrenType childrenType
    )
    {
        Id = id;
        Name = name;
        Description = description;
        Items = items;
        Parents = parents;
        IsPinned = isPinned;
        DueDate = dueDate;
        IsCompleted = isCompleted;
        ChildrenType = childrenType;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public bool IsPinned { get; }
    public ToDoItemChildrenType ChildrenType { get; }
    public DateTimeOffset DueDate { get; }
    public bool IsCompleted { get; }
}