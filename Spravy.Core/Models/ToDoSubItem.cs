using Spravy.Core.Enums;

namespace Spravy.Core.Models;

public readonly struct ToDoSubItem
{
    public ToDoSubItem(
        Guid id,
        string name,
        bool isComplete,
        DateTimeOffset? dueDate,
        ulong orderIndex,
        ToDoItemStatus status
    )
    {
        Name = name;
        DueDate = dueDate;
        OrderIndex = orderIndex;
        Status = status;
        IsComplete = isComplete;
        Id = id;
    }

    public Guid Id { get; }
    public bool IsComplete { get; }
    public string Name { get; }
    public DateTimeOffset? DueDate { get; }
    public ulong OrderIndex { get; }
    public ToDoItemStatus Status { get; }
}