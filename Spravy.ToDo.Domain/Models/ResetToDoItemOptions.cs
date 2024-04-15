namespace Spravy.ToDo.Domain.Models;

public readonly struct ResetToDoItemOptions
{
    public ResetToDoItemOptions(Guid id, bool isCompleteChildrenTask, bool isMoveCircleOrderIndex, bool isOnlyCompletedTasks, bool isCompleteCurrentTask)
    {
        Id = id;
        IsCompleteChildrenTask = isCompleteChildrenTask;
        IsMoveCircleOrderIndex = isMoveCircleOrderIndex;
        IsOnlyCompletedTasks = isOnlyCompletedTasks;
        IsCompleteCurrentTask = isCompleteCurrentTask;
    }

    public Guid Id { get; }
    public bool IsCompleteCurrentTask { get; }
    public bool IsCompleteChildrenTask { get; }
    public bool IsMoveCircleOrderIndex { get; }
    public bool IsOnlyCompletedTasks { get; }
}