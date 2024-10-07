namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoShortItem
{
    public ToDoShortItem(Guid id, string name, string icon)
    {
        Id = id;
        Name = name;
        Icon = icon;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Icon { get; }
}
