namespace Spravy.ToDo.Domain.Models;

public readonly struct ParentItem
{
    public ParentItem(Guid id, ReadOnlyMemory<ToDoShortItem> parents)
    {
        Id = id;
        Parents = parents;
    }

    public readonly Guid Id;
    public readonly ReadOnlyMemory<ToDoShortItem> Parents;
}