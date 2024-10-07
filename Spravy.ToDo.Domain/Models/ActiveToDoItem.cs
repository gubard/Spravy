using Spravy.Domain.Models;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ActiveToDoItem
{
    public ActiveToDoItem(Guid id, string name, OptionStruct<Guid> parentId, string icon)
    {
        Id = id;
        Name = name;
        ParentId = parentId;
        Icon = icon;
    }

    public Guid Id { get; }
    public OptionStruct<Guid> ParentId { get; }
    public string Name { get; }
    public string Icon { get; }
}
