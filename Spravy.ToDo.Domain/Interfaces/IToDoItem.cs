using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Interfaces;

public interface IToDoItem
{
    Guid Id { get; }
    string Name { get; }
    string Description { get; }
    IToDoSubItem[] Items { get; }
    ToDoItemParent[] Parents { get; }
    bool IsCurrent { get; }
}