namespace Spravy.Domain.Enums;

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

    /// <summary>
    /// To do item is complete.
    /// </summary>
    Completed,
}