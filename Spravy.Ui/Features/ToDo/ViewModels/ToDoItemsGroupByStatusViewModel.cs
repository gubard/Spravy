using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ninject;
using Spravy.ToDo.Domain.Enums;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByStatusViewModel : ViewModelBase
{
    private readonly ToDoItemsViewModel missed;
    private readonly ToDoItemsViewModel readyForCompleted;
    private readonly ToDoItemsViewModel planned;
    private readonly ToDoItemsViewModel completed;

    [Inject]
    public required ToDoItemsViewModel Missed
    {
        get => missed;
        [MemberNotNull(nameof(missed))]
        init
        {
            missed = value;
            missed.Header = "Missed";
        }
    }

    [Inject]
    public required ToDoItemsViewModel ReadyForCompleted
    {
        get => readyForCompleted;
        [MemberNotNull(nameof(readyForCompleted))]
        init
        {
            readyForCompleted = value;
            readyForCompleted.Header = "ReadyForCompleted";
        }
    }

    [Inject]
    public required ToDoItemsViewModel Planned
    {
        get => planned;
        [MemberNotNull(nameof(planned))]
        init
        {
            planned = value;
            planned.Header = "Planned";
        }
    }

    [Inject]
    public required ToDoItemsViewModel Completed
    {
        get => completed;
        [MemberNotNull(nameof(completed))]
        init
        {
            completed = value;
            completed.Header = "Completed";
        }
    }

    public void Clear()
    {
        Missed.Clear();
        ReadyForCompleted.Clear();
        Planned.Clear();
        Completed.Clear();
    }

    public void AddItems(IEnumerable<Selected<ToDoItemNotify>> items)
    {
        Missed.AddItems(items.Where(x => x.Value.Status == ToDoItemStatus.Miss));
        ReadyForCompleted.AddItems(items.Where(x => x.Value.Status == ToDoItemStatus.ReadyForComplete));
        Planned.AddItems(items.Where(x => x.Value.Status == ToDoItemStatus.Planned));
        Completed.AddItems(items.Where(x => x.Value.Status == ToDoItemStatus.Completed));
    }

    public void ClearExcept(IEnumerable<Guid> ids)
    {
        Missed.ClearExcept(ids);
        ReadyForCompleted.ClearExcept(ids);
        Planned.ClearExcept(ids);
        Completed.ClearExcept(ids);
    }

    public void UpdateItem(Selected<ToDoItemNotify> item, bool updateOrder)
    {
        switch (item.Value.Status)
        {
            case ToDoItemStatus.Miss:
                Missed.UpdateItem(item, updateOrder);
                ReadyForCompleted.RemoveItem(item);
                Planned.RemoveItem(item);
                Completed.RemoveItem(item);
                break;
            case ToDoItemStatus.ReadyForComplete:
                Missed.RemoveItem(item);
                ReadyForCompleted.UpdateItem(item, updateOrder);
                Planned.RemoveItem(item);
                Completed.RemoveItem(item);
                break;
            case ToDoItemStatus.Planned:
                Missed.RemoveItem(item);
                ReadyForCompleted.RemoveItem(item);
                Planned.UpdateItem(item, updateOrder);
                Completed.RemoveItem(item);
                break;
            case ToDoItemStatus.Completed:
                Missed.RemoveItem(item);
                ReadyForCompleted.RemoveItem(item);
                Planned.RemoveItem(item);
                Completed.UpdateItem(item, updateOrder);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}