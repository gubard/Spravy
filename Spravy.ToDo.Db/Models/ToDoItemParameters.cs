using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Db.Models;

public readonly struct ToDoItemParameters
{
    public ToDoItemParameters(ActiveToDoItem? activeItem, ToDoItemStatus status, ToDoItemIsCan isCan)
    {
        ActiveItem = activeItem;
        Status = status;
        IsCan = isCan;
    }

    public ActiveToDoItem? ActiveItem { get; }
    public ToDoItemStatus Status { get; }
    public ToDoItemIsCan IsCan { get; }

    public ToDoItemParameters Set(ToDoItemStatus status)
    {
        return new ToDoItemParameters(ActiveItem, status, IsCan);
    }

    public ToDoItemParameters Set(ActiveToDoItem? activeToDoItem)
    {
        return new ToDoItemParameters(activeToDoItem, Status, IsCan);
    }

    public ToDoItemParameters Set(ToDoItemIsCan isCan)
    {
        return new ToDoItemParameters(ActiveItem, Status, isCan);
    }
}