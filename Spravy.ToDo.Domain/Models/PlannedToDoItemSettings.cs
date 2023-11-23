using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct PlannedToDoItemSettings
{
    public PlannedToDoItemSettings(ToDoItemChildrenType childrenType, DateOnly dueDate)
    {
        ChildrenType = childrenType;
        DueDate = dueDate;
    }

    public ToDoItemChildrenType ChildrenType { get; }
    public DateOnly DueDate { get; }
}