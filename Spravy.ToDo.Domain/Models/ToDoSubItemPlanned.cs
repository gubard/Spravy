using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoSubItemPlanned : IToDoSubItem
{
    public ToDoSubItemPlanned(
        Guid id,
        string name,
        uint orderIndex,
        ToDoItemStatus status,
        string description,
        bool isFavorite,
        ActiveToDoItem? active,
        DateTimeOffset dueDate,
        uint completedCount,
        uint skippedCount,
        uint failedCount,
        bool isCompleted,
        DateTimeOffset? lastCompleted,
        Uri? link
    )
    {
        Id = id;
        Name = name;
        OrderIndex = orderIndex;
        Status = status;
        Description = description;
        IsFavorite = isFavorite;
        Active = active;
        DueDate = dueDate;
        CompletedCount = completedCount;
        SkippedCount = skippedCount;
        FailedCount = failedCount;
        IsCompleted = isCompleted;
        LastCompleted = lastCompleted;
        Link = link;
    }

    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    public bool IsFavorite { get; }
    public ActiveToDoItem? Active { get; }
    public Uri? Link { get; }
    public DateTimeOffset? LastCompleted { get; }
    public DateTimeOffset DueDate { get; }
    public uint CompletedCount { get; }
    public uint SkippedCount { get; }
    public uint FailedCount { get; }
    public bool IsCompleted { get; }
}
