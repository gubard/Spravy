using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Errors;

public class ToDoItemChildrenTypeOutOfRangeError : ValueOutOfRangeError<ToDoItemChildrenType>
{
    public static readonly Guid MainId = new("24021BB0-7EA0-4DD2-BDCF-85C42F70C80E");

    protected ToDoItemChildrenTypeOutOfRangeError() : base(ToDoItemChildrenType.RequireCompletion, MainId)
    {
    }

    public ToDoItemChildrenTypeOutOfRangeError(ToDoItemChildrenType type) : base(type, MainId)
    {
    }
}