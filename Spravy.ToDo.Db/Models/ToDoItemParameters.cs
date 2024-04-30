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

    public ToDoItemParameters With(ToDoItemStatus status)
    {
        return new(ActiveItem, status, IsCan);
    }

    public ToDoItemParameters With(ActiveToDoItem? activeToDoItem)
    {
        return new(activeToDoItem, Status, IsCan);
    }

    public ToDoItemParameters With(ToDoItemIsCan isCan)
    {
        return new(ActiveItem, Status, isCan);
    }
}