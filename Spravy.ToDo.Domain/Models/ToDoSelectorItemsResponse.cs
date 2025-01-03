namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoSelectorItemsResponse
{
    public ToDoSelectorItemsResponse(bool isResponse, ReadOnlyMemory<ToDoSelectorItem> items)
    {
        IsResponse = isResponse;
        Items = items;
    }

    public readonly bool IsResponse;
    public readonly ReadOnlyMemory<ToDoSelectorItem> Items;
}