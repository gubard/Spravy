namespace Spravy.Domain.Enums;

public enum ToDoItemStatus
{
    /// <summary>
    /// Overdue due date or sub to do item overdue due date.
    /// </summary>
    Miss,

    /// <summary>
    /// Due date today.
    /// </summary>
    Today,

    /// <summary>
    /// Don't have due date.
    /// </summary>
    Waiting,

    /// <summary>
    /// All sub to do items is complete.
    /// </summary>
    ReadyForComplete,

    /// <summary>
    /// To do item is complete.
    /// </summary>
    Complete,
}