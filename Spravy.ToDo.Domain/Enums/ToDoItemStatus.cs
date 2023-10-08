namespace Spravy.ToDo.Domain.Enums;

public enum ToDoItemStatus
{
    /// <summary>
    /// Overdue due date or sub to do item overdue due date.
    /// </summary>
    Miss,

    /// <summary>
    /// All sub to do items is complete.
    /// </summary>
    ReadyForComplete,

    Planned,

    /// <summary>
    /// To do item is complete.
    /// </summary>
    Completed,
}