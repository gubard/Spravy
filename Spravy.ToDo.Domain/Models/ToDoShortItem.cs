namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoShortItem
{
    public ToDoShortItem(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}