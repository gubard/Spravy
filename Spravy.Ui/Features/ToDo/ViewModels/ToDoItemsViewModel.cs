using System.Collections.Generic;
using Avalonia.Collections;
using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsViewModel : ViewModelBase
{
    private object? header;
    private bool isExpanded = true;
    public AvaloniaList<CommandItem> Commands { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Items { get; } = new();

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
}