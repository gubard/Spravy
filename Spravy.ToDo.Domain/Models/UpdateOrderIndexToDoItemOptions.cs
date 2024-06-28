namespace Spravy.ToDo.Domain.Models;

public readonly struct UpdateOrderIndexToDoItemOptions
{
    public UpdateOrderIndexToDoItemOptions(Guid id, Guid targetId, bool isAfter)
    {
        Id = id;
        TargetId = targetId;
        IsAfter = isAfter;
    }

    public Guid Id { get; }
    public Guid TargetId { get; }
    public bool IsAfter { get; }
}
