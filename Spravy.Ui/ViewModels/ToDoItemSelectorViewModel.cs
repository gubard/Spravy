using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemSelectorViewModel : ViewModelBase
{
    private ToDoSelectorItemNotify? selectedItem;

    public ToDoItemSelectorViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    public AvaloniaList<ToDoSelectorItemNotify> Roots { get; } = new();
    public ICommand InitializedCommand { get; }
    public List<Guid> IgnoreIds { get; } = new();

    public Guid DefaultSelectedItemId { get; set; }

    public ToDoSelectorItemNotify? SelectedItem
    {
        get => selectedItem;
        set => this.RaiseAndSetIfChanged(ref selectedItem, value);
    }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var items = await ToDoService.GetToDoSelectorItemsAsync(IgnoreIds.ToArray(), cancellationToken).ConfigureAwait(false);
        Roots.Clear();
        Roots.AddRange(Mapper.Map<IEnumerable<ToDoSelectorItemNotify>>(items));
        SetItem(DefaultSelectedItemId);
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