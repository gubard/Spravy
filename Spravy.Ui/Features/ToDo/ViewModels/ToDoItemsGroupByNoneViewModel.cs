using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Ninject;
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
            items = value;
            items.Header = "Items";
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
}