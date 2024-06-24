using Spravy.Domain.Models;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ActiveToDoItem
{
    public ActiveToDoItem(Guid id, string name, OptionStruct<Guid> parentId)
    {
        Id = id;
        Name = name;
        ParentId = parentId;
    }

    public Guid Id { get; }
    public OptionStruct<Guid> ParentId { get; }
    public string Name { get; }
}