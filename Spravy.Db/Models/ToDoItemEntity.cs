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
    public ulong OrderIndex { get; set; }

    [ForeignKey(nameof(Parent))]
    public Guid? ParentId { get; set; }

    public ToDoItemEntity? Parent { get; set; }
}