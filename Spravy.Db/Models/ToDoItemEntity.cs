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

    public Guid? ParentId { get; set; }
    public ToDoItemEntity? Parent { get; set; }
    public Guid ValueId { get; set; }
    public ToDoItemValueEntity? Value { get; set; }
    public Guid GroupId { get; set; }
    public ToDoItemGroupEntity? Group { get; set; }
    public Guid StatisticalId { get; set; }
    public ToDoItemStatisticalEntity? Statistical { get; set; }
}