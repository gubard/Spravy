using Spravy.Domain.Enums;
using Spravy.Domain.Models;
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
        Option<Uri> link,
        uint orderIndex,
        ToDoItemStatus status,
        OptionStruct<ActiveToDoItem> active,
        ToDoItemIsCan isCan,
        OptionStruct<Guid> parentId,
        DescriptionType descriptionType,
        OptionStruct<Guid> referenceId,
        bool isBookmark,
        string icon,
        string color
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
        ReferenceId = referenceId;
        IsBookmark = isBookmark;
        Icon = icon;
        Color = color;
    }

    public Guid Id { get; }
    public string Name { get; }
    public bool IsFavorite { get; }
    public ToDoItemType Type { get; }
    public string Description { get; }
    public Option<Uri> Link { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public OptionStruct<ActiveToDoItem> Active { get; }
    public ToDoItemIsCan IsCan { get; }
    public OptionStruct<Guid> ParentId { get; }
    public DescriptionType DescriptionType { get; }
    public OptionStruct<Guid> ReferenceId { get; }
    public bool IsBookmark { get; }
    public string Icon { get; }
    public string Color { get; }
}
