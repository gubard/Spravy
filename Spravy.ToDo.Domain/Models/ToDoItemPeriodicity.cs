using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoItemPeriodicity : IToDoItem
{
    public ToDoItemPeriodicity(
        Guid id,
        string name,
        string description,
        IToDoSubItem[] items,
        ToDoItemParent[] parents,
        bool isCurrent,
        DateTimeOffset dueDate,
        IPeriodicity periodicity
    )
    {
        Id = id;
        Name = name;
        Description = description;
        Items = items;
        Parents = parents;
        IsCurrent = isCurrent;
        DueDate = dueDate;
        Periodicity = periodicity;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public bool IsCurrent { get; }
    public DateTimeOffset DueDate { get; }
    public IPeriodicity Periodicity { get; }
}