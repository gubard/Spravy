using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct PlannedToDoItemSettings
{
    public PlannedToDoItemSettings(
        ToDoItemChildrenType childrenType,
        DateOnly dueDate,
        bool isRequiredCompleteInDueDate
    )
    {
        ChildrenType = childrenType;
        DueDate = dueDate;
        IsRequiredCompleteInDueDate = isRequiredCompleteInDueDate;
    }

    public ToDoItemChildrenType ChildrenType { get; }
    public DateOnly DueDate { get; }
    public bool IsRequiredCompleteInDueDate { get; }
}