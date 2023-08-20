using System;
using Avalonia.Collections;
using ReactiveUI;

namespace Spravy.Ui.Models;

public class ToDoSelectorItemNotify : NotifyBase
{
    private Guid id;
    private string name = string.Empty;

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