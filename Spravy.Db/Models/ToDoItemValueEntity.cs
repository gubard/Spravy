using Spravy.Domain.Enums;

namespace Spravy.Db.Models;

public class ToDoItemValueEntity
{
    public Guid Id { get; set; }
    public bool IsComplete { get; set; }
    public TypeOfPeriodicity TypeOfPeriodicity { get; set; }
    public DateTimeOffset? DueDate { get; set; }

    public Guid ItemId { get; set; }
    public ToDoItemEntity Item { get; set; }
}