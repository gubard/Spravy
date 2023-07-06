using Spravy.Core.Enums;

namespace Spravy.Core.Models;

public readonly struct ToDoItem
{
    public ToDoItem(
        Guid id,
        string name,
        TypeOfPeriodicity typeOfPeriodicity,
        DateTimeOffset? dueDate,
        ToDoSubItem[] items,
        ToDoItemParent[] parents,
        bool isComplete
    )
    {
        Name = name;
        Items = items;
        Parents = parents;
        IsComplete = isComplete;
        TypeOfPeriodicity = typeOfPeriodicity;
        DueDate = dueDate;
        Id = id;
    }

    public Guid Id { get; }
    public string Name { get; }
    public bool IsComplete { get; }
    public TypeOfPeriodicity TypeOfPeriodicity { get; }
    public DateTimeOffset? DueDate { get; }
    public ToDoSubItem[] Items { get; }
    public ToDoItemParent[] Parents { get; }
}