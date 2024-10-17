namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoShortItem
{
    public ToDoShortItem(Guid id, string name, string icon, string color)
    {
        Id = id;
        Name = name;
        Icon = icon;
        Color = color;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Icon { get; }
    public string Color { get; }
}
