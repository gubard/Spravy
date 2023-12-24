using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemSelectorViewModel : ViewModelBase
{
    private ToDoSelectorItemNotify? selectedItem;
    private string searchText = string.Empty;
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

    public string SearchText
    {
        get => searchText;
        set => this.RaiseAndSetIfChanged(ref searchText, value);
    }

    public ToDoSelectorItemNotify? SelectedItem
    {
        get => selectedItem;
        set => this.RaiseAndSetIfChanged(ref selectedItem, value);
    }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        await Refresh(cancellationToken);
        SetItem(DefaultSelectedItemId);
    }

    private async Task Refresh(CancellationToken cancellationToken)
    {
        await this.InvokeUIBackgroundAsync(() => Roots.Clear());

        var items = await ToDoService.GetToDoSelectorItemsAsync(IgnoreIds.ToArray(), cancellationToken)
            .ConfigureAwait(false);

        itemsCache.Clear();
        itemsCache.AddRange(Mapper.Map<IEnumerable<ToDoSelectorItemNotify>>(items));
        await this.InvokeUIBackgroundAsync(() => Roots.AddRange(itemsCache));
    }

    private async Task SearchAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIBackgroundAsync(() => Roots.Clear());

        if (SearchText.IsNullOrWhiteSpace())
        {
            await this.InvokeUIBackgroundAsync(() => Roots.AddRange(itemsCache));

            return;
        }

        var result = new List<ToDoSelectorItemNotify>();

        foreach (var item in itemsCache)
        {
            Search(item, result);
        }

        await this.InvokeUIBackgroundAsync(() => Roots.AddRange(result));
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
                if (SetItem(id, root.Children))
                {
                    return;
                }

                continue;
            }

            SelectedItem = root;

            return;
        }
    }

    public bool SetItem(Guid id, IEnumerable<ToDoSelectorItemNotify> items)
    {
        foreach (var item in items)
        {
            if (item.Id != id)
            {
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