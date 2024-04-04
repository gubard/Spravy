using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class ToDoItemSelectorViewModel : ViewModelBase
{
    private readonly List<ToDoSelectorItemNotify> itemsCache = new();

    public ToDoItemSelectorViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        SearchCommand = CreateInitializedCommand(TaskWork.Create(SearchAsync).RunAsync);
    }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    public AvaloniaList<ToDoSelectorItemNotify> Roots { get; } = new();
    public ICommand InitializedCommand { get; }
    public ICommand SearchCommand { get; }
    public List<Guid> IgnoreIds { get; } = new();

    public Guid DefaultSelectedItemId { get; set; }

    [Reactive]
    public string SearchText { get; set; } = string.Empty;

    [Reactive]
    public ToDoSelectorItemNotify? SelectedItem { get; set; }

    private ValueTask<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return Refresh(cancellationToken)
            .ConfigureAwait(false)
            .IfSuccessAsync(
                () =>
                {
                    SetItem(DefaultSelectedItemId);

                    return Result.AwaitableFalse;
                }
            );
    }

    private ValueTask<Result> Refresh(CancellationToken cancellationToken)
    {
        return this.InvokeUIBackgroundAsync(() => Roots.Clear())
            .ConfigureAwait(false)
            .IfSuccessAsync(
                () => ToDoService.GetToDoSelectorItemsAsync(IgnoreIds.ToArray(), cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        items =>
                        {
                            itemsCache.Clear();
                            itemsCache.AddRange(Mapper.Map<ToDoSelectorItemNotify[]>(items.ToArray()));

                            return this.InvokeUIBackgroundAsync(() => Roots.AddRange(itemsCache)).ConfigureAwait(false);
                        }
                    )
                    .ConfigureAwait(false)
            );
    }

    private ValueTask<Result> SearchAsync(CancellationToken cancellationToken)
    {
        return this.InvokeUIBackgroundAsync(() => Roots.Clear())
            .ConfigureAwait(false)
            .IfSuccessAsync(
                () =>
                {
                    if (SearchText.IsNullOrWhiteSpace())
                    {
                        return this.InvokeUIBackgroundAsync(() => Roots.AddRange(itemsCache)).ConfigureAwait(false);
                    }

                    var result = new List<ToDoSelectorItemNotify>();

                    foreach (var item in itemsCache)
                    {
                        Search(item, result);
                    }

                    return this.InvokeUIBackgroundAsync(() => Roots.AddRange(result)).ConfigureAwait(false);
                }
            );
    }

    private void Search(ToDoSelectorItemNotify item, List<ToDoSelectorItemNotify> result)
    {
        if (item.Name.ToUpperInvariant().Contains(SearchText.ToUpperInvariant()))
        {
            result.Add(item);
        }

        foreach (var child in item.Children)
        {
            Search(child, result);
        }
    }

    public void SetItem(Guid id)
    {
        foreach (var root in Roots)
        {
            if (root.Id != id)
            {
                foreach (var child in root.Children)
                {
                    child.Parent = root;
                }

                if (SetItem(id, root.Children))
                {
                    break;
                }

                continue;
            }

            SelectedItem = root;

            break;
        }

        if (SelectedItem is null)
        {
            return;
        }

        SelectedItem.IsExpanded = true;
        var current = SelectedItem;

        while (current.Parent is not null)
        {
            current.Parent.IsExpanded = true;
            current = current.Parent;
        }
    }

    public bool SetItem(Guid id, IEnumerable<ToDoSelectorItemNotify> items)
    {
        foreach (var item in items)
        {
            if (item.Id != id)
            {
                foreach (var child in item.Children)
                {
                    child.Parent = item;
                }

                if (SetItem(id, item.Children))
                {
                    return true;
                }

                continue;
            }

            SelectedItem = item;

            return true;
        }

        return false;
    }
}