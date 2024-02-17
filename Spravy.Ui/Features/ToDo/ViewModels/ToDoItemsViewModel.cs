using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsViewModel : ViewModelBase
{
    private object? header;
    private bool isExpanded = true;
    public AvaloniaList<CommandItem> Commands { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Items { get; } = new();

    [Inject]
    public required IMapper Mapper { get; init; }

    public object? Header
    {
        get => header;
        set => this.RaiseAndSetIfChanged(ref header, value);
    }

    public bool IsExpanded
    {
        get => isExpanded;
        set => this.RaiseAndSetIfChanged(ref isExpanded, value);
    }

    public void Clear()
    {
        Items.Clear();
    }

    public void AddItems(IEnumerable<Selected<ToDoItemNotify>> items)
    {
        Items.AddRange(items);
    }

    public void ClearExcept(IEnumerable<Guid> ids)
    {
        Items.RemoveAll(Items.Where(x => !ids.Contains(x.Value.Id)));
    }

    public void UpdateItem(Selected<ToDoItemNotify> item, bool updateOrder)
    {
        if (!Items.Contains(item))
        {
            AddSorted(item);
        }
        else if (updateOrder)
        {
            Items.Remove(item);
            AddSorted(item);
        }
    }

    private void AddSorted(Selected<ToDoItemNotify> obj)
    {
        if (Items.Count == 0 || obj.Value.OrderIndex >= Items[^1].Value.OrderIndex)
        {
            Items.Add(obj);

            return;
        }

        for (var i = 0; i < Items.Count; i++)
        {
            if (obj.Value.OrderIndex >= Items[i].Value.OrderIndex)
            {
                continue;
            }

            Items.Insert(i, obj);

            break;
        }
    }

    public void RemoveItem(Selected<ToDoItemNotify> item)
    {
        Items.Remove(item);
    }
}