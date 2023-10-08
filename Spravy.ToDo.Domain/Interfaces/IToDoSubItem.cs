using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Interfaces;

public interface IToDoSubItem
{
    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    bool IsPinned { get; }
    ActiveToDoItem? Active { get; }
    DateTimeOffset? LastCompleted { get; }
}
