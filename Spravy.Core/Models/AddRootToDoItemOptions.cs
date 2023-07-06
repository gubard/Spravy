namespace Spravy.Core.Models;

public readonly struct AddRootToDoItemOptions
{
    public AddRootToDoItemOptions(string name)
    {
        Name = name;
    }

    public string Name { get; }
}