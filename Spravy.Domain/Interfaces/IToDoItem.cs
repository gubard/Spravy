using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IToDoItem
{
    Guid Id { get; }
    string Name { get; }
    string Description { get; }
    IToDoSubItem[] Items { get; }
    ToDoItemParent[] Parents { get; }
}