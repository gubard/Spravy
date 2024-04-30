using System;
using Avalonia.Collections;
using ReactiveUI;

namespace Spravy.Ui.Models;

public class ToDoSelectorItemNotify : NotifyBase
{
    private Guid id;
    private bool isExpanded;
    private string name = string.Empty;
    private ToDoSelectorItemNotify? parent;

    public bool IsExpanded
    {
        get => isExpanded;
        set => this.RaiseAndSetIfChanged(ref isExpanded, value);
    }

    public ToDoSelectorItemNotify? Parent
    {
        get => parent;
        set => this.RaiseAndSetIfChanged(ref parent, value);
    }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public AvaloniaList<ToDoSelectorItemNotify> Children { get; } = new();
}