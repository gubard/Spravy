using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct PeriodicityToDoItemSettings
{
    public PeriodicityToDoItemSettings(
        ToDoItemChildrenType childrenType,
        DateOnly dueDate,
        TypeOfPeriodicity typeOfPeriodicity
    )
    {
        ChildrenType = childrenType;
        DueDate = dueDate;
        TypeOfPeriodicity = typeOfPeriodicity;
    }

    public ToDoItemChildrenType ChildrenType { get; }
    public DateOnly DueDate { get; }
    public TypeOfPeriodicity TypeOfPeriodicity { get; }
}