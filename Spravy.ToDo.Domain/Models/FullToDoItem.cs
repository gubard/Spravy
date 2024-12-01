using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct FullToDoItem
{
    public FullToDoItem(
        ToDoShortItem item,
        ToDoItemStatus status,
        OptionStruct<ToDoShortItem> active,
        ToDoItemIsCan isCan
    )
    {
        Item = item;
        Status = status;
        Active = active;
        IsCan = isCan;
    }

    public ToDoShortItem Item { get; }
    public ToDoItemStatus Status { get; }
    public OptionStruct<ToDoShortItem> Active { get; }
    public ToDoItemIsCan IsCan { get; }
}