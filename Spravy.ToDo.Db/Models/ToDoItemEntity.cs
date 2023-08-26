using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Db.Models;

public class ToDoItemEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public uint OrderIndex { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedDateTime { get; set; } = DateTimeOffset.Now;
    public ToDoItemType Type { get; set; }
    public bool IsCurrent { get; set; }
    public DateTimeOffset DueDate { get; set; } = DateTimeOffset.Now.ToCurrentDay();
    public bool IsCompleted { get; set; }
    public TypeOfPeriodicity TypeOfPeriodicity { get; set; }
    public uint CompletedCount { get; set; }
    public uint SkippedCount { get; set; }
    public uint FailedCount { get; set; }
    public string DaysOfWeek { get; set; } = "Monday";
    public string DaysOfMonth { get; set; } = "1";
    public string DaysOfYear { get; set; } = "1.1";
    public DateTimeOffset? LastCompleted { get; set; }
    public ushort DaysOffset { get; set; } = 1;
    public ushort MonthsOffset { get; set; }
    public ushort WeeksOffset { get; set; }
    public ushort YearsOffset { get; set; }
    public ToDoItemChildrenType ChildrenType { get; set; }

    public Guid? ParentId { get; set; }
    public ToDoItemEntity? Parent { get; set; }
}