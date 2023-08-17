namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoItemParent
{
    public ToDoItemParent(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}