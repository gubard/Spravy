using Spravy.Domain.Enums;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct AddToDoItemOptions
{
    public AddToDoItemOptions(
        Guid parentId,
        string name,
        ToDoItemType type,
        string description,
        DescriptionType descriptionType,
        Option<Uri> link
    )
    {
        ParentId = parentId;
        Name = name;
        Type = type;
        Description = description;
        DescriptionType = descriptionType;
        Link = link;
    }

    public Guid ParentId { get; }
    public string Name { get; }
    public ToDoItemType Type { get; }
    public Option<Uri> Link { get; }
    public string Description { get; }
    public DescriptionType DescriptionType { get; }
}
