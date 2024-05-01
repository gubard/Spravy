namespace Spravy.ToDo.Db.Models;

public class DependencyToDoItemEntity
{
    public Guid Id { get; set; }
    public Guid ToDoItemId { get; set; }
    public Guid DependencyToDoItemId { get; set; }
    public ToDoItemEntity? ToDoItem { get; set; }
    public ToDoItemEntity? DependencyToDoItem { get; set; }
}