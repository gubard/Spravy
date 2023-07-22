namespace Spravy.Db.Models;

public class ToDoItemGroupEntity
{
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }
    public ToDoItemEntity Item { get; set; }
}