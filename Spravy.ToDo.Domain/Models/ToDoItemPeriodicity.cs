using Spravy.ToDo.Domain.Enums;
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
        IPeriodicity periodicity,
        ToDoItemChildrenType childrenType
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
        ChildrenType = childrenType;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public bool IsCurrent { get; }
    public ToDoItemChildrenType ChildrenType { get; }
    public DateTimeOffset DueDate { get; }
    public IPeriodicity Periodicity { get; }
}