using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ValueToDoItemSettings
{
    public ValueToDoItemSettings(ToDoItemChildrenType childrenType)
    {
        ChildrenType = childrenType;
    }

    public ToDoItemChildrenType ChildrenType { get; }
}