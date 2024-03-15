using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using ReactiveUI.Fody.Helpers;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsViewModel : ViewModelBase
{
    public AvaloniaList<CommandItem> Commands { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Items { get; } = new();

    [Reactive]
    public string? Header { get; set; }

    [Reactive]
    public bool IsExpanded { get; set; } = true;

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