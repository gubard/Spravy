using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct ToDoSubItemPlanned : IToDoSubItem
{
    public ToDoSubItemPlanned(
        Guid id,
        string name,
        uint orderIndex,
        ToDoItemStatus status,
        string description,
        bool isCurrent,
        ActiveToDoItem? active,
        DateTimeOffset dueDate,
        uint completedCount,
        uint skippedCount,
        uint failedCount,
        bool isCompleted,
        DateTimeOffset? lastCompleted
    )
    {
        Id = id;
        Name = name;
        OrderIndex = orderIndex;
        Status = status;
        Description = description;
        IsCurrent = isCurrent;
        Active = active;
        DueDate = dueDate;
        CompletedCount = completedCount;
        SkippedCount = skippedCount;
        FailedCount = failedCount;
        IsCompleted = isCompleted;
        LastCompleted = lastCompleted;
    }

    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    public bool IsCurrent { get; }
    public ActiveToDoItem? Active { get; }
    public DateTimeOffset? LastCompleted { get; }
    public DateTimeOffset DueDate { get; }
    public uint CompletedCount { get; }
    public uint SkippedCount { get; }
    public uint FailedCount { get; }
    public bool IsCompleted { get; }
}
