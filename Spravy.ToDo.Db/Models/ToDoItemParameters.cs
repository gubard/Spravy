using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Db.Models;

public readonly struct ToDoItemParameters
{
    public ToDoItemParameters(
        IsSuccessValue<ActiveToDoItem?> activeItem,
        IsSuccessValue<ToDoItemStatus> status,
        IsSuccessValue<ToDoItemIsCan> isCan
    )
    {
        ActiveItem = activeItem;
        Status = status;
        IsCan = isCan;
    }

    public IsSuccessValue<ActiveToDoItem?> ActiveItem { get; }
    public IsSuccessValue<ToDoItemStatus> Status { get; }
    public IsSuccessValue<ToDoItemIsCan> IsCan { get; }

    public bool IsSuccess =>
        ActiveItem.IsSuccess && Status is { IsSuccess: true, Value: ToDoItemStatus.Miss } && IsCan.IsSuccess;

    public ToDoItemParameters WithIfNeed(ActiveToDoItem? activeToDoItem)
    {
        if (ActiveItem.IsSuccess)
        {
            return this;
        }

        return new ToDoItemParameters(activeToDoItem.ToSuccessValue(), Status, IsCan);
    }
    
    public ToDoItemParameters Set(ToDoItemStatus status)
    {
        return new ToDoItemParameters(ActiveItem, status.ToSuccessValue(), IsCan);
    }
    
    public ToDoItemParameters Set(ActiveToDoItem? activeToDoItem)
    {
        return new ToDoItemParameters(activeToDoItem.ToSuccessValue(), Status, IsCan);
    }

    public ToDoItemParameters WithIfNeed(ToDoItemStatus status, ActiveToDoItem? activeToDoItem)
    {
        if (Status.IsSuccess)
        {
            if (status < Status.Value)
            {
                return new ToDoItemParameters(activeToDoItem.ToSuccessValue(), status.ToSuccessValue(), IsCan);
            }
            
            return new ToDoItemParameters(ActiveItem, Status, IsCan);
        }

        return new ToDoItemParameters(ActiveItem, status.ToSuccessValue(), IsCan);
    }

    public ToDoItemParameters WithIfNeed(ToDoItemIsCan isCan)
    {
        if (IsCan.IsSuccess)
        {
            return this;
        }

        return new ToDoItemParameters(ActiveItem, Status, isCan.ToSuccessValue());
    }
    
    public ToDoItemParameters Set(ToDoItemIsCan isCan)
    {
        return new ToDoItemParameters(ActiveItem, Status, isCan.ToSuccessValue());
    }
}