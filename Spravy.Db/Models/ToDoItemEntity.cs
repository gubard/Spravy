using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Spravy.Core.Enums;

namespace Spravy.Db.Models;

[Table("ToDoItems")]
public class ToDoItemEntity
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }
    public TypeOfPeriodicity TypeOfPeriodicity { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public bool IsComplete { get; set; }
    public uint OrderIndex { get; set; }
    public string Description { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; } = DateTimeOffset.Now;
    public uint CompletedCount { get; set; }

    [ForeignKey(nameof(Parent))]
    public Guid? ParentId { get; set; }

    public ToDoItemEntity? Parent { get; set; }
}