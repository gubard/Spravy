namespace Spravy.Core.Models;

public readonly struct AddToDoItemOptions
{
    public AddToDoItemOptions(Guid parentId, string name)
    {
        ParentId = parentId;
        Name = name;
    }

    public Guid ParentId { get; }
    public string Name { get; }
}