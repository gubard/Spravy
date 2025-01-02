using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct GetToStringItem
{
    public GetToStringItem(ReadOnlyMemory<Guid> ids, ReadOnlyMemory<ToDoItemStatus> statuses)
    {
        Ids = ids;
        Statuses = statuses;
    }

    public readonly ReadOnlyMemory<Guid> Ids;
    public readonly ReadOnlyMemory<ToDoItemStatus> Statuses;
}