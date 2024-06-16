using Spravy.Domain.Enums;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct AddRootToDoItemOptions
{
    public AddRootToDoItemOptions(
        string name,
        ToDoItemType type,
        Option<Uri> link,
        string description,
        DescriptionType descriptionType
    )
    {
        Name = name;
        Type = type;
        Link = link;
        Description = description;
        DescriptionType = descriptionType;
    }

    public string Name { get; }
    public ToDoItemType Type { get; }
    public Option<Uri> Link { get; }
    public string Description { get; }
    public DescriptionType DescriptionType { get; }
}