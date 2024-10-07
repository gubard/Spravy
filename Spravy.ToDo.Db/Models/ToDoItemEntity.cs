using Spravy.Core.Mappers;
using Spravy.Domain.Enums;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Db.Models;

public record ToDoItemEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public uint OrderIndex { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedDateTime { get; set; } = DateTimeOffset.Now;
    public ToDoItemType Type { get; set; }
    public bool IsBookmark { get; set; }
    public bool IsFavorite { get; set; }
    public DateOnly DueDate { get; set; } = DateTime.Now.ToDateOnly();
    public bool IsCompleted { get; set; }
    public TypeOfPeriodicity TypeOfPeriodicity { get; set; }
    public string DaysOfWeek { get; set; } = "Monday";
    public string DaysOfMonth { get; set; } = "1";
    public string DaysOfYear { get; set; } = "1.1";
    public DateTimeOffset? LastCompleted { get; set; }
    public ushort DaysOffset { get; set; } = 1;
    public ushort MonthsOffset { get; set; }
    public ushort WeeksOffset { get; set; }
    public ushort YearsOffset { get; set; }
    public ToDoItemChildrenType ChildrenType { get; set; }
    public uint CurrentCircleOrderIndex { get; set; }
    public string Link { get; set; } = string.Empty;
    public bool IsRequiredCompleteInDueDate { get; set; } = true;
    public DescriptionType DescriptionType { get; set; }
    public string Icon { get; set; } = string.Empty;

    public Guid? ReferenceId { get; set; }
    public ToDoItemEntity? Reference { get; set; }

    public Guid? ParentId { get; set; }
    public ToDoItemEntity? Parent { get; set; }
}
