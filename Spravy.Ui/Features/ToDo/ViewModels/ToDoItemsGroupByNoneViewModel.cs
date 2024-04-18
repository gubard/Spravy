using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Ninject;
using Spravy.Ui.Features.Localizations.Models;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByNoneViewModel : ViewModelBase
{
    private readonly ToDoItemsViewModel items;

    [Inject]
    public required ToDoItemsViewModel Items
    {
        get => items;
        [MemberNotNull(nameof(items))]
        init
        {
            items?.Dispose();
            items = value;
            items.Header = new TextView("ToDoItemsGroupByNoneView.Header");
            Disposables.Add(items);
        }
    }

    public void Clear()
    {
        Items.Clear();
    }
    
    public void AddItems(IEnumerable<Selected<ToDoItemNotify>> items)
    {
        Items.AddItems(items);
    }
    
    public void ClearExcept(IEnumerable<Guid> ids)
    {
        Items.ClearExcept(ids);
    }

    public void UpdateItem(Selected<ToDoItemNotify> item, bool updateOrder)
    {
        Items.UpdateItem(item, updateOrder);
    }
}