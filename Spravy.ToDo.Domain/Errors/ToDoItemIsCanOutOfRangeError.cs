using Spravy.Domain.Errors;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Errors;

public class ToDoItemIsCanOutOfRangeError : ValueOutOfRangeError<ToDoItemIsCan>
{
    public static readonly Guid MainId = new("7AA58F11-9755-4F49-9478-85EA1778EA9D");

    protected ToDoItemIsCanOutOfRangeError() : base(ToDoItemIsCan.None, MainId)
    {
    }

    public ToDoItemIsCanOutOfRangeError(ToDoItemIsCan value) : base(value, MainId)
    {
    }
}