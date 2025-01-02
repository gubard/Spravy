namespace Spravy.ToDo.Domain.Models;

public readonly struct LeafItem
{
    public LeafItem(Guid id, ReadOnlyMemory<FullToDoItem> leafs)
    {
        Id = id;
        Leafs = leafs;
    }

    public readonly Guid Id;
    public readonly ReadOnlyMemory<FullToDoItem> Leafs;
}