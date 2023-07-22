using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct ToDoSubItemGroup : IToDoSubItem
{
    public ToDoSubItemGroup(Guid id, string name, ulong orderIndex, ToDoItemStatus status, string description, bool isCurrent, ActiveToDoItem? active)
    {
        Id = id;
        Name = name;
        OrderIndex = orderIndex;
        Status = status;
        Description = description;
        IsCurrent = isCurrent;
        Active = active;
    }

    public Guid Id { get; }
    public string Name { get; }
    public ulong OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    public bool IsCurrent { get; }
    public ActiveToDoItem? Active { get; }
}