using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct PeriodicityOffsetToDoItemSettings
{
    public PeriodicityOffsetToDoItemSettings(
        ToDoItemChildrenType childrenType,
        DateOnly dueDate,
        ushort daysOffset,
        ushort monthsOffset,
        ushort weeksOffset,
        ushort yearsOffset,
        bool isRequiredCompleteInDueDate
    )
    {
        ChildrenType = childrenType;
        DueDate = dueDate;
        DaysOffset = daysOffset;
        MonthsOffset = monthsOffset;
        WeeksOffset = weeksOffset;
        YearsOffset = yearsOffset;
        IsRequiredCompleteInDueDate = isRequiredCompleteInDueDate;
    }

    public ToDoItemChildrenType ChildrenType { get; }
    public DateOnly DueDate { get; }
    public ushort DaysOffset { get; }
    public ushort MonthsOffset { get; }
    public ushort WeeksOffset { get; }
    public ushort YearsOffset { get; }
    public bool IsRequiredCompleteInDueDate { get; }
}