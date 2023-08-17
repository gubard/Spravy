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
        bool isCurrent
    )
    {
        Name = name;
        Items = items;
        Parents = parents;
        IsCompleted = isCompleted;
        Description = description;
        IsCurrent = isCurrent;
        Id = id;
    }

    public Guid Id { get; }
    public string Name { get; }
    public bool IsCompleted { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public bool IsCurrent { get; }
    public string Description { get; }
}