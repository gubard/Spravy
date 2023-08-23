using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoItemPeriodicityOffset : IToDoItem
{
    public ToDoItemPeriodicityOffset(
        Guid id,
        string name,
        string description,
        IToDoSubItem[] items,
        ToDoItemParent[] parents,
        bool isCurrent,
        ushort daysOffset,
        ushort monthsOffset,
        ushort weeksOffset,
        ushort yearsOffset,
        DateTimeOffset dueDate
    )
    {
        Id = id;
        Name = name;
        Description = description;
        Items = items;
        Parents = parents;
        IsCurrent = isCurrent;
        DaysOffset = daysOffset;
        MonthsOffset = monthsOffset;
        WeeksOffset = weeksOffset;
        YearsOffset = yearsOffset;
        DueDate = dueDate;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public DateTimeOffset DueDate { get; }
    public bool IsCurrent { get; }
    public ushort DaysOffset { get; }
    public ushort MonthsOffset { get; }
    public ushort WeeksOffset { get; }
    public ushort YearsOffset { get; }
}