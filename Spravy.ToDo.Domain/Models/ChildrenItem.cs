namespace Spravy.ToDo.Domain.Models;

public readonly struct ChildrenItem
{
    public ChildrenItem(Guid id, ReadOnlyMemory<FullToDoItem> children)
    {
        Id = id;
        Children = children;
    }

    public readonly Guid Id;
    public readonly ReadOnlyMemory<FullToDoItem> Children;
}