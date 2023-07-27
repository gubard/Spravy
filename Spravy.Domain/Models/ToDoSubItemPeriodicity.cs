using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct ToDoSubItemPeriodicity : IToDoSubItem
{
    public ToDoSubItemPeriodicity(
        Guid id,
        string name,
        uint orderIndex,
        ToDoItemStatus status,
        string description,
        bool isCurrent,
        DateTimeOffset dueDate,
        TypeOfPeriodicity typeOfPeriodicity,
        ActiveToDoItem? active,
        uint completedCount,
        uint skippedCount,
        uint failedCount
    )
    {
        Id = id;
        Name = name;
        OrderIndex = orderIndex;
        Status = status;
        Description = description;
        IsCurrent = isCurrent;
        DueDate = dueDate;
        TypeOfPeriodicity = typeOfPeriodicity;
        Active = active;
        CompletedCount = completedCount;
        SkippedCount = skippedCount;
        FailedCount = failedCount;
    }

    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    public bool IsCurrent { get; }
    public DateTimeOffset DueDate { get; }
    public TypeOfPeriodicity TypeOfPeriodicity { get; }
    public ActiveToDoItem? Active { get; }
    public uint CompletedCount { get; }
    public uint SkippedCount { get; }
    public uint FailedCount { get; }
}