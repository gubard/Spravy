using Spravy.Domain.Models;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ActiveItem
{
    public ActiveItem(Guid id, OptionStruct<ToDoShortItem> item)
    {
        Id = id;
        Item = item;
    }

    public readonly Guid Id;
    public readonly OptionStruct<ToDoShortItem> Item;
}