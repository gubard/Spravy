using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct AddToDoItemOptions
{
    public AddToDoItemOptions(Guid parentId, string name, ToDoItemType type)
    {
        ParentId = parentId;
        Name = name;
        Type = type;
    }

    public Guid ParentId { get; }
    public string Name { get; }
    public ToDoItemType Type { get; }
}