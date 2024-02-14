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

    public void UpdateItem(ToDoItem item)
    {
        var selected = Items.SingleOrDefault(x => x.Value.Id == item.Id);

        if (selected is null)
        {
            AddSorted(new Selected<ToDoItemNotify>(SetupItem(Mapper.Map<ToDoItemNotify>(item))));
        }
        else
        {
            selected.Value.Type = item.Type;
            selected.Value.Status = item.Status;
            selected.Value.Active = Mapper.Map<ActiveToDoItemNotify?>(item.Active);
            selected.Value.Description = item.Description;
            selected.Value.Link = item.Link?.AbsoluteUri ?? string.Empty;
            selected.Value.Name = item.Name;
            selected.Value.IsCan = item.IsCan;
            selected.Value.IsFavorite = item.IsFavorite;
            selected.Value.ParentId = item.ParentId;

            if (selected.Value.OrderIndex != item.OrderIndex)
            {
                selected.Value.OrderIndex = item.OrderIndex;
                Items.Remove(selected);
                AddSorted(selected);
            }
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

    private ToDoItemNotify SetupItem(ToDoItemNotify item)
    {
        var toFavoriteCommand = CommandStorage.AddToDoItemToFavoriteItem.WithParam(item.Id);
        item.Commands.Add(CommandStorage.AddToDoItemChildItem.WithParam(item.Id));
        item.Commands.Add(CommandStorage.DeleteToDoItemItem.WithParam(item));

        if (item.IsCan != ToDoItemIsCan.None)
        {
            item.Commands.Add(CommandStorage.SwitchCompleteToDoItemItem.WithParam(item));
        }

        item.Commands.Add(CommandStorage.ShowToDoSettingItem.WithParam(item));

        if (item.IsFavorite)
        {
            toFavoriteCommand = CommandStorage.RemoveToDoItemFromFavoriteItem.WithParam(item.Id);
        }

        item.Commands.Add(toFavoriteCommand);
        item.Commands.Add(CommandStorage.NavigateToLeafItem.WithParam(item.Id));
        item.Commands.Add(CommandStorage.SetToDoParentItemItem.WithParam(item));
        item.Commands.Add(CommandStorage.MoveToDoItemToRootItem.WithParam(item));
        item.Commands.Add(CommandStorage.ToDoItemToStringItem.WithParam(item));
        item.Commands.Add(CommandStorage.ToDoItemRandomizeChildrenOrderIndexItem.WithParam(item));
        item.Commands.Add(CommandStorage.ChangeOrderIndexItem.WithParam(item));
        item.Commands.Add(CommandStorage.ResetToDoItemItem.WithParam(item));

        return item;
    }


    public void RemoveItem(Guid id)
    {
        var item = Items.SingleOrDefault(x => x.Value.Id == id);

        if (item is not null)
        {
            Items.Remove(item);
        }
    }
}