using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct GetSearch
{
    public GetSearch(string searchText, ReadOnlyMemory<ToDoItemType> types)
    {
        SearchText = searchText;
        Types = types;
    }

    public readonly string SearchText;
    public readonly ReadOnlyMemory<ToDoItemType> Types;
}