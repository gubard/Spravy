namespace Spravy.ToDo.Domain.Models;

public readonly struct ResetToDoItemOptions
{
    public ResetToDoItemOptions(Guid id, bool isCompleteTask, bool isMoveCircleOrderIndex, bool isOnlyCompletedTasks)
    {
        Id = id;
        IsCompleteTask = isCompleteTask;
        IsMoveCircleOrderIndex = isMoveCircleOrderIndex;
        IsOnlyCompletedTasks = isOnlyCompletedTasks;
    }

    public Guid Id { get; }
    public bool IsCompleteTask { get; }
    public bool IsMoveCircleOrderIndex { get; }
    public bool IsOnlyCompletedTasks { get; }
}