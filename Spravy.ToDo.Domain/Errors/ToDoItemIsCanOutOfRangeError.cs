using Spravy.Domain.Errors;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Errors;

public class ToDoItemIsCanOutOfRangeError : ValueOutOfRangeError<ToDoItemIsCan>
{
    public static readonly Guid MainId = new("BBDA1D69-A789-43F2-ABBB-841D98EECFFF");

    protected ToDoItemIsCanOutOfRangeError() : base(ToDoItemIsCan.None, MainId)
    {
    }

    public ToDoItemIsCanOutOfRangeError(ToDoItemIsCan value) : base(value, MainId)
    {
    }
}