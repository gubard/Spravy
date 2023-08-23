using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoSubItemPeriodicityOffset : IToDoSubItem
{
    public ToDoSubItemPeriodicityOffset(
        Guid id,
        string name,
        uint orderIndex,
        ToDoItemStatus status,
        string description,
        bool isCurrent,
        DateTimeOffset dueDate,
        ActiveToDoItem? active,
        uint completedCount,
        uint skippedCount,
        uint failedCount,
        DateTimeOffset? lastCompleted
    )
    {
        Id = id;
        Name = name;
        OrderIndex = orderIndex;
        Status = status;
        Description = description;
        IsCurrent = isCurrent;
        DueDate = dueDate;
        Active = active;
        CompletedCount = completedCount;
        SkippedCount = skippedCount;
        FailedCount = failedCount;
        LastCompleted = lastCompleted;
    }

    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    public bool IsCurrent { get; }
    public DateTimeOffset DueDate { get; }
    public ActiveToDoItem? Active { get; }
    public DateTimeOffset? LastCompleted { get; }
    public uint CompletedCount { get; }
    public uint SkippedCount { get; }
    public uint FailedCount { get; }
}