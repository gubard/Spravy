using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoSubItemGroup : IToDoSubItem
{
    public ToDoSubItemGroup(
        Guid id,
        string name,
        uint orderIndex,
        ToDoItemStatus status,
        string description,
        bool isPinned,
        ActiveToDoItem? active,
        DateTimeOffset? lastCompleted
    )
    {
        Id = id;
        Name = name;
        OrderIndex = orderIndex;
        Status = status;
        Description = description;
        IsPinned = isPinned;
        Active = active;
        LastCompleted = lastCompleted;
    }

    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    public bool IsPinned { get; }
    public ActiveToDoItem? Active { get; }
    public DateTimeOffset? LastCompleted { get; }
}
