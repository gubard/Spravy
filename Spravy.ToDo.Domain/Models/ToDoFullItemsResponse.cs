namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoFullItemsResponse
{
    public ToDoFullItemsResponse(bool isResponse, ReadOnlyMemory<FullToDoItem> items)
    {
        IsResponse = isResponse;
        Items = items;
    }

    public readonly bool IsResponse;
    public readonly ReadOnlyMemory<FullToDoItem> Items;
}