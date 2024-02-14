using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ninject;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByTypeViewModel : ViewModelBase
{
    private readonly ToDoItemsViewModel values;
    private readonly ToDoItemsViewModel groups;
    private readonly ToDoItemsViewModel planneds;
    private readonly ToDoItemsViewModel periodicitys;
    private readonly ToDoItemsViewModel periodicityOffsets;
    private readonly ToDoItemsViewModel circles;
    private readonly ToDoItemsViewModel steps;

    [Inject]
    public required ToDoItemsViewModel Values
    {
        get => values;
        [MemberNotNull(nameof(values))]
        init
        {
            values = value;
            values.Header = "Values";
        }
    }

    [Inject]
    public required ToDoItemsViewModel Groups
    {
        get => groups;
        [MemberNotNull(nameof(groups))]
        init
        {
            groups = value;
            groups.Header = "Groups";
        }
    }

    [Inject]
    public required ToDoItemsViewModel Planneds
    {
        get => planneds;
        [MemberNotNull(nameof(planneds))]
        init
        {
            planneds = value;
            planneds.Header = "Planneds";
        }
    }

    [Inject]
    public required ToDoItemsViewModel Periodicitys
    {
        get => periodicitys;
        [MemberNotNull(nameof(periodicitys))]
        init
        {
            periodicitys = value;
            periodicitys.Header = "Periodicitys";
        }
    }

    [Inject]
    public required ToDoItemsViewModel PeriodicityOffsets
    {
        get => periodicityOffsets;
        [MemberNotNull(nameof(periodicityOffsets))]
        init
        {
            periodicityOffsets = value;
            periodicityOffsets.Header = "PeriodicityOffsets";
        }
    }

    [Inject]
    public required ToDoItemsViewModel Circles
    {
        get => circles;
        [MemberNotNull(nameof(circles))]
        init
        {
            circles = value;
            circles.Header = "Circles";
        }
    }

    [Inject]
    public required ToDoItemsViewModel Steps
    {
        get => steps;
        [MemberNotNull(nameof(steps))]
        init
        {
            steps = value;
            steps.Header = "Steps";
        }
    }

    public void Clear()
    {
        Values.Clear();
        Groups.Clear();
        Planneds.Clear();
        Periodicitys.Clear();
        PeriodicityOffsets.Clear();
        Circles.Clear();
        Steps.Clear();
    }

    public void AddItems(IEnumerable<Selected<ToDoItemNotify>> items)
    {
        Values.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Value));
        Groups.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Group));
        Planneds.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Planned));
        Periodicitys.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Periodicity));
        PeriodicityOffsets.AddItems(items.Where(x => x.Value.Type == ToDoItemType.PeriodicityOffset));
        Circles.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Circle));
        Steps.AddItems(items.Where(x => x.Value.Type == ToDoItemType.Step));
    }

    public void ClearExcept(IEnumerable<Guid> ids)
    {
        Values.ClearExcept(ids);
        Groups.ClearExcept(ids);
        Planneds.ClearExcept(ids);
        Periodicitys.ClearExcept(ids);
        PeriodicityOffsets.ClearExcept(ids);
        Circles.ClearExcept(ids);
        Steps.ClearExcept(ids);
    }

    public void UpdateItem(ToDoItem item)
    {
        switch (item.Type)
        {
            case ToDoItemType.Value:
                Values.UpdateItem(item);
                Groups.RemoveItem(item.Id);
                Planneds.RemoveItem(item.Id);
                Periodicitys.RemoveItem(item.Id);
                PeriodicityOffsets.RemoveItem(item.Id);
                Circles.RemoveItem(item.Id);
                Steps.RemoveItem(item.Id);

                break;
            case ToDoItemType.Group:
                Values.RemoveItem(item.Id);
                Groups.UpdateItem(item);
                Planneds.RemoveItem(item.Id);
                Periodicitys.RemoveItem(item.Id);
                PeriodicityOffsets.RemoveItem(item.Id);
                Circles.RemoveItem(item.Id);
                Steps.RemoveItem(item.Id);

                break;
            case ToDoItemType.Planned:
                Values.RemoveItem(item.Id);
                Groups.RemoveItem(item.Id);
                Planneds.UpdateItem(item);
                Periodicitys.RemoveItem(item.Id);
                PeriodicityOffsets.RemoveItem(item.Id);
                Circles.RemoveItem(item.Id);
                Steps.RemoveItem(item.Id);

                break;
            case ToDoItemType.Periodicity:
                Values.RemoveItem(item.Id);
                Groups.RemoveItem(item.Id);
                Planneds.RemoveItem(item.Id);
                Periodicitys.UpdateItem(item);
                PeriodicityOffsets.RemoveItem(item.Id);
                Circles.RemoveItem(item.Id);
                Steps.RemoveItem(item.Id);

                break;
            case ToDoItemType.PeriodicityOffset:
                Values.RemoveItem(item.Id);
                Groups.RemoveItem(item.Id);
                Planneds.RemoveItem(item.Id);
                Periodicitys.RemoveItem(item.Id);
                PeriodicityOffsets.UpdateItem(item);
                Circles.RemoveItem(item.Id);
                Steps.RemoveItem(item.Id);

                break;
            case ToDoItemType.Circle:
                Values.RemoveItem(item.Id);
                Groups.RemoveItem(item.Id);
                Planneds.RemoveItem(item.Id);
                Periodicitys.RemoveItem(item.Id);
                PeriodicityOffsets.RemoveItem(item.Id);
                Circles.UpdateItem(item);
                Steps.RemoveItem(item.Id);

                break;
            case ToDoItemType.Step:
                Values.RemoveItem(item.Id);
                Groups.RemoveItem(item.Id);
                Planneds.RemoveItem(item.Id);
                Periodicitys.RemoveItem(item.Id);
                PeriodicityOffsets.RemoveItem(item.Id);
                Circles.RemoveItem(item.Id);
                Steps.UpdateItem(item);

                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}