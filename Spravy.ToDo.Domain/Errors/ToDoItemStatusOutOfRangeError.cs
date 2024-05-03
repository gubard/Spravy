using Spravy.Domain.Errors;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Errors;

public class ToDoItemStatusOutOfRangeError : ValueOutOfRangeError<ToDoItemStatus>
{
    public static readonly Guid MainId = new("68A7FCB8-35CB-427F-8A89-A90591519F9B");

    protected ToDoItemStatusOutOfRangeError() : base(ToDoItemStatus.Miss, MainId)
    {
    }

    public ToDoItemStatusOutOfRangeError(ToDoItemStatus status) : base(status, MainId)
    {
    }
}