using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct PeriodicityToDoItemSettings
{
    public PeriodicityToDoItemSettings(
        ToDoItemChildrenType childrenType,
        DateOnly dueDate,
        TypeOfPeriodicity typeOfPeriodicity,
        bool isRequiredCompleteInDueDate
    )
    {
        ChildrenType = childrenType;
        DueDate = dueDate;
        TypeOfPeriodicity = typeOfPeriodicity;
        IsRequiredCompleteInDueDate = isRequiredCompleteInDueDate;
    }

    public ToDoItemChildrenType ChildrenType { get; }
    public DateOnly DueDate { get; }
    public TypeOfPeriodicity TypeOfPeriodicity { get; }
    public bool IsRequiredCompleteInDueDate { get; }
}
