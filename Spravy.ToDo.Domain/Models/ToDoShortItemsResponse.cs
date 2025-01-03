namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoShortItemsResponse
{
    public ToDoShortItemsResponse(bool isResponse, ReadOnlyMemory<ToDoShortItem> items)
    {
        IsResponse = isResponse;
        Items = items;
    }

    public readonly bool IsResponse;
    public readonly ReadOnlyMemory<ToDoShortItem> Items;
}