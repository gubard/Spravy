using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct ToDoSubItemValue : IToDoSubItem
{
    public ToDoSubItemValue(
        Guid id,
        string name,
        bool isComplete,
        DateTimeOffset? dueDate,
        ulong orderIndex,
        ToDoItemStatus status,
        string description,
        uint completedCount,
        uint skippedCount,
        uint failedCount,
        bool isCurrent,
        ActiveToDoItem? active
    )
    {
        Name = name;
        DueDate = dueDate;
        OrderIndex = orderIndex;
        Status = status;
        Description = description;
        CompletedCount = completedCount;
        SkippedCount = skippedCount;
        FailedCount = failedCount;
        IsCurrent = isCurrent;
        Active = active;
        IsComplete = isComplete;
        Id = id;
    }

    public Guid Id { get; }
    public bool IsComplete { get; }
    public string Name { get; }
    public DateTimeOffset? DueDate { get; }
    public ulong OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    public bool IsCurrent { get; }
    public ActiveToDoItem? Active { get; }
    public uint CompletedCount { get; }
    public uint SkippedCount { get; }
    public uint FailedCount { get; }
}