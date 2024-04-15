namespace Spravy.ToDo.Domain.Models;

public readonly struct ResetToDoItemOptions
{
    public ResetToDoItemOptions(Guid id, bool isCompleteTask, bool isMoveCircleOrderIndex)
    {
        Id = id;
        IsCompleteTask = isCompleteTask;
        IsMoveCircleOrderIndex = isMoveCircleOrderIndex;
    }

    public Guid Id { get; }
    public bool IsCompleteTask { get; }
    public bool IsMoveCircleOrderIndex { get; }
}