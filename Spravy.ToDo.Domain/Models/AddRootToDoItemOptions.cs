using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct AddRootToDoItemOptions
{
    public AddRootToDoItemOptions(string name, ToDoItemType type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; }
    public ToDoItemType Type { get; }
}