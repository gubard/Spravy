using Spravy.Domain.Errors;

namespace Spravy.ToDo.Domain.Errors;

public class ToDoItemAlreadyCompleteError : Error
{
    public static readonly Guid MainId = new("21DC28BF-F54A-40FD-BECB-F712259DE20C");

    protected ToDoItemAlreadyCompleteError()
        : base(MainId)
    {
        ToDoItemName = string.Empty;
    }

    public ToDoItemAlreadyCompleteError(Guid toDoItemId, string toDoItemName)
        : base(MainId)
    {
        ToDoItemId = toDoItemId;
        ToDoItemName = toDoItemName;
    }

    public Guid ToDoItemId { get; protected set; }
    public string ToDoItemName { get; protected set; }

    public override string Message
    {
        get => $"To-do item \"{ToDoItemName}\"<{ToDoItemId}> already completed";
    }
}
