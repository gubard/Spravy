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
        bool isCurrent,
        DateTimeOffset dueDate,
        bool isCompleted
    )
    {
        Id = id;
        Name = name;
        Description = description;
        Items = items;
        Parents = parents;
        IsCurrent = isCurrent;
        DueDate = dueDate;
        IsCompleted = isCompleted;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public bool IsCurrent { get; }
    public DateTimeOffset DueDate { get; }
    public bool IsCompleted { get; }
}