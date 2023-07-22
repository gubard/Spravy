using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public readonly struct ToDoSubItemGroup : IToDoSubItem
{
    public ToDoSubItemGroup(Guid id, string name, ulong orderIndex, ToDoItemStatus status, string description)
    {
        Id = id;
        Name = name;
        OrderIndex = orderIndex;
        Status = status;
        Description = description;
    }

    public Guid Id { get; }
    public string Name { get; }
    public ulong OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
}