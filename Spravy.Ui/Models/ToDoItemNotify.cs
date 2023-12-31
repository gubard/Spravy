using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using ReactiveUI;
using Spravy.ToDo.Domain.Enums;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public class ToDoItemNotify : NotifyBase, ICanComplete, IDeletable, IToDoSettingsProperty
{
    private Guid id;
    private string name = string.Empty;
    private bool isFavorite;
    private ToDoItemType type;
    private string description = string.Empty;
    private string link = string.Empty;
    private uint orderIndex;
    private ToDoItemStatus status;
    private ActiveToDoItemNotify? active;
    private ToDoItemIsCan isCan;

    public object Header => Name;
    public AvaloniaList<CommandItem> Commands { get; } = new();

    public ToDoItemIsCan IsCan
    {
        get => isCan;
        set => this.RaiseAndSetIfChanged(ref isCan, value);
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

    public bool IsFavorite
    {
        get => isFavorite;
        set => this.RaiseAndSetIfChanged(ref isFavorite, value);
    }

    public ToDoItemType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }

    public string Description
    {
        get => description;
        set => this.RaiseAndSetIfChanged(ref description, value);
    }

    public string Link
    {
        get => link;
        set => this.RaiseAndSetIfChanged(ref link, value);
    }

    public uint OrderIndex
    {
        get => orderIndex;
        set => this.RaiseAndSetIfChanged(ref orderIndex, value);
    }

    public ToDoItemStatus Status
    {
        get => status;
        set => this.RaiseAndSetIfChanged(ref status, value);
    }

    public ActiveToDoItemNotify? Active
    {
        get => active;
        set => this.RaiseAndSetIfChanged(ref active, value);
    }

    public Task RefreshAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}