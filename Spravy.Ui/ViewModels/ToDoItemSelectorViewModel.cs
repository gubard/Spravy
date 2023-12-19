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
    private string searchText;

    public ToDoItemSelectorViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        SearchCommand = CreateInitializedCommand(TaskWork.Create(Refresh).RunAsync);
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

        if (SearchText.IsNullOrWhiteSpace())
        {
            var items = await ToDoService.GetToDoSelectorItemsAsync(IgnoreIds.ToArray(), cancellationToken)
                .ConfigureAwait(false);
            await this.InvokeUIBackgroundAsync(() => Roots.AddRange(Mapper.Map<IEnumerable<ToDoSelectorItemNotify>>(items)));
        }
        else
        {
            var items = await ToDoService.SearchToDoItemIdsAsync(SearchText, cancellationToken).ConfigureAwait(false);

            await foreach (var i in ToDoService.GetToDoItemsAsync(items.ToArray(), 5, cancellationToken))
            {
                await this.InvokeUIBackgroundAsync(() => Roots.AddRange(Mapper.Map<IEnumerable<ToDoSelectorItemNotify>>(i)));
            }
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