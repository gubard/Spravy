using Spravy.Domain.Enums;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IToDoSubItem
{
    public Guid Id { get; }
    public string Name { get; }
    public uint OrderIndex { get; }
    public ToDoItemStatus Status { get; }
    public string Description { get; }
    bool IsCurrent { get; }
    ActiveToDoItem? Active { get; }
    DateTimeOffset? LastCompleted { get; }
}
