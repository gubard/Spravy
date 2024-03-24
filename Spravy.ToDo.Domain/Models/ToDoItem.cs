using Spravy.Domain.Enums;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoItem
{
    public ToDoItem(
        Guid id,
        string name,
        bool isFavorite,
        ToDoItemType type,
        string description,
        Uri? link,
        uint orderIndex,
        ToDoItemStatus status,
        ActiveToDoItem? active,
        ToDoItemIsCan isCan,
        Guid? parentId,
        DescriptionType descriptionType
    )
    {
        Id = id;
        Name = name;
        IsFavorite = isFavorite;
        Type = type;
        Description = description;
        Link = link;
        OrderIndex = orderIndex;
        Status = status;
        Active = active;
        IsCan = isCan;
        ParentId = parentId;
        DescriptionType = descriptionType;
    }

    public Guid Id { get; }
    public string Name { get; }
    public bool IsFavorite { get; }
    public ToDoItemType Type { get; }
    public string Description { get; }
    public Uri? Link { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public ActiveToDoItem? Active { get; }
    public ToDoItemIsCan IsCan { get; }
    public Guid? ParentId { get; }
    public DescriptionType DescriptionType { get; }

    public ToDoItem WithOrderIndex(uint orderIndex)
    {
        return new ToDoItem(
            Id,
            Name,
            IsFavorite,
            Type,
            Description,
            Link,
            orderIndex,
            Status,
            Active,
            IsCan,
            ParentId,
            DescriptionType
        );
    }
}