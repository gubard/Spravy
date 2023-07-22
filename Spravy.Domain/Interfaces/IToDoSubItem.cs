using Spravy.Domain.Enums;

namespace Spravy.Domain.Interfaces;

public interface IToDoSubItem
{
    public Guid Id { get; }
    public string Name { get; }
    public ulong OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
}