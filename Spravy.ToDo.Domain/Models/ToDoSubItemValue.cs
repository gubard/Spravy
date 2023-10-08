using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoSubItemValue : IToDoSubItem
{
    public ToDoSubItemValue(
        Guid id,
        string name,
        bool isCompleted,
        uint orderIndex,
        ToDoItemStatus status,
        string description,
        uint completedCount,
        uint skippedCount,
        uint failedCount,
        bool isPinned,
        ActiveToDoItem? active,
        DateTimeOffset? lastCompleted
    )
    {
        Name = name;
        OrderIndex = orderIndex;
        Status = status;
        Description = description;
        CompletedCount = completedCount;
        SkippedCount = skippedCount;
        FailedCount = failedCount;
        IsPinned = isPinned;
        Active = active;
        LastCompleted = lastCompleted;
        IsCompleted = isCompleted;
        Id = id;
    }

    public Guid Id { get; }
    public bool IsCompleted { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    public bool IsPinned { get; }
    public ActiveToDoItem? Active { get; }
    public DateTimeOffset? LastCompleted { get; }
    public uint CompletedCount { get; }
    public uint SkippedCount { get; }
    public uint FailedCount { get; }
}
