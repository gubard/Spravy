using Spravy.Domain.Errors;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Errors;

public class ToDoItemStatusOutOfRangeError : ValueOutOfRangeError<ToDoItemStatus>
{
    public static readonly Guid MainId = new("7AA58F11-9755-4F49-9478-85EA1778EA9D");

    protected ToDoItemStatusOutOfRangeError() : base(ToDoItemStatus.Miss, MainId)
    {
    }

    public ToDoItemStatusOutOfRangeError(ToDoItemStatus value) : base(value, MainId)
    {
    }
}