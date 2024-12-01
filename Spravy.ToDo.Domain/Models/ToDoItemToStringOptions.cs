using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoItemToStringOptions
{
    public ToDoItemToStringOptions(ReadOnlyMemory<ToDoItemStatus> statuses, Guid id)
    {
        Id = id;
        Statuses = statuses;
    }

    public Guid Id { get; }
    public ReadOnlyMemory<ToDoItemStatus> Statuses { get; }
}