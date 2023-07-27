using ExtensionFramework.Core.Common.Extensions;
using Spravy.Domain.Enums;

namespace Spravy.Db.Models;

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

    public Guid? ParentId { get; set; }
    public ToDoItemEntity? Parent { get; set; }
}