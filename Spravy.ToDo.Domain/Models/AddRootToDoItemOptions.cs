namespace Spravy.ToDo.Domain.Models;

public readonly struct AddRootToDoItemOptions
{
    public AddRootToDoItemOptions(string name)
    {
        Name = name;
    }

    public string Name { get; }
}