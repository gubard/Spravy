using Spravy.Domain.Models;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ActiveToDoItem
{
    public ActiveToDoItem(
        Guid id,
        string name,
        OptionStruct<Guid> parentId,
        string icon,
        string color
    )
    {
        Id = id;
        Name = name;
        ParentId = parentId;
        Icon = icon;
        Color = color;
    }

    public Guid Id { get; }
    public OptionStruct<Guid> ParentId { get; }
    public string Name { get; }
    public string Icon { get; }
    public string Color { get; }
}
