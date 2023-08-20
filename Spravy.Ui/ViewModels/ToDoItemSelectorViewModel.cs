using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using ReactiveUI;
using Spravy.Domain.Attributes;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemSelectorViewModel : ViewModelBase
{
    private ToDoSelectorItemNotify? selectedItem;

    public ToDoItemSelectorViewModel()
    {
        InitializedCommand = CreateCommandFromTask(InitializedAsync);
    }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    public AvaloniaList<ToDoSelectorItemNotify> Roots { get; } = new();
    public ICommand InitializedCommand { get; }

    public ToDoSelectorItemNotify? SelectedItem
    {
        get => selectedItem;
        set => this.RaiseAndSetIfChanged(ref selectedItem, value);
    }

    private async Task InitializedAsync()
    {
        var items = await ToDoService.GetToDoSelectorItemsAsync();
        Roots.Clear();
        Roots.AddRange(Mapper.Map<IEnumerable<ToDoSelectorItemNotify>>(items));
    }
}