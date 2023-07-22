namespace Spravy.Domain.Models;

public readonly struct ActiveToDoItem
{
    public ActiveToDoItem(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}