using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct ToDoItemValue : IToDoItem
{
    public ToDoItemValue(
        Guid id,
        string name,
        TypeOfPeriodicity typeOfPeriodicity,
        DateTimeOffset? dueDate,
        IToDoSubItem[] items,
        ToDoItemParent[] parents,
        bool isComplete,
        string description,
        bool isCurrent
    )
    {
        Name = name;
        Items = items;
        Parents = parents;
        IsComplete = isComplete;
        Description = description;
        IsCurrent = isCurrent;
        TypeOfPeriodicity = typeOfPeriodicity;
        DueDate = dueDate;
        Id = id;
    }

    public Guid Id { get; }
    public string Name { get; }
    public bool IsComplete { get; }
    public TypeOfPeriodicity TypeOfPeriodicity { get; }
    public DateTimeOffset? DueDate { get; }
    public IToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
    public bool IsCurrent { get; }
    public string Description { get; }
}