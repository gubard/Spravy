using Spravy.Domain.Errors;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Errors;

public class ToDoItemTypeOutOfRangeError : ValueOutOfRangeError<ToDoItemType>
{
    public static readonly Guid MainId = new("68A7FCB8-35CB-427F-8A89-A90591519F9B");
    
    protected ToDoItemTypeOutOfRangeError() : base(ToDoItemType.Value, MainId)
    {
    }
    
    public ToDoItemTypeOutOfRangeError(ToDoItemType type) : base(type, MainId)
    {
    }
}