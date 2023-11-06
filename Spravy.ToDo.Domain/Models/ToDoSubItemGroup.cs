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
        bool isFavorite,
        ActiveToDoItem? active,
        Uri? link
    )
    {
        Id = id;
        Name = name;
        OrderIndex = orderIndex;
        Status = status;
        Description = description;
        IsFavorite = isFavorite;
        Active = active;
        Link = link;
    }

    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    public bool IsFavorite { get; }
    public ActiveToDoItem? Active { get; }
    public Uri? Link { get; }
}
