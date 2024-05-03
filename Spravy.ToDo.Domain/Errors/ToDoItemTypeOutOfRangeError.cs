using Spravy.Domain.Errors;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Errors;

public class ToDoItemTypeOutOfRangeError : ValueOutOfRangeError<ToDoItemType>
{
    public static readonly Guid MainId = new("24021BB0-7EA0-4DD2-BDCF-85C42F70C80E");
    
    protected ToDoItemTypeOutOfRangeError() : base(ToDoItemType.Value, MainId)
    {
    }
    
    public ToDoItemTypeOutOfRangeError(ToDoItemType type) : base(type, MainId)
    {
    }
}