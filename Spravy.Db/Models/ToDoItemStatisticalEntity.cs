namespace Spravy.Db.Models;

public class ToDoItemStatisticalEntity
{
    public Guid Id { get; set; }
    public uint CompletedCount { get; set; }
    public uint SkippedCount { get; set; }
    public uint FailedCount { get; set; }
    public Guid ItemId { get; set; }
    
    public ToDoItemEntity Item { get; set; }
}