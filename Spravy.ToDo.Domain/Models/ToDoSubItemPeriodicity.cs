using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoSubItemPeriodicity : IToDoSubItem
{
    public ToDoSubItemPeriodicity(
        Guid id,
        string name,
        uint orderIndex,
        ToDoItemStatus status,
        string description,
        bool isFavorite,
        DateOnly dueDate,
        ActiveToDoItem? active,
        uint completedCount,
        uint skippedCount,
        uint failedCount,
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
        DueDate = dueDate;
        Active = active;
        CompletedCount = completedCount;
        SkippedCount = skippedCount;
        FailedCount = failedCount;
        LastCompleted = lastCompleted;
        Link = link;
    }

    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    public bool IsFavorite { get; }
    public DateOnly DueDate { get; }
    public ActiveToDoItem? Active { get; }
    public Uri? Link { get; }
    public DateTimeOffset? LastCompleted { get; }
    public uint CompletedCount { get; }
    public uint SkippedCount { get; }
    public uint FailedCount { get; }
}
