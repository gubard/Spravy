using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Models;

public class ToDoItemNotify : NotifyBase,
    ICanCompleteProperty,
    IDeletable,
    IToDoSettingsProperty,
    ISetToDoParentItemParams,
    ILink
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
    private Guid? parentId;
    private ICommand? completeCommand;
    private bool isBusy;

    public AvaloniaList<CommandItem> Commands { get; } = new();

    public bool IsBusy
    {
        get => isBusy;
        set => this.RaiseAndSetIfChanged(ref isBusy, value);
    }

    public ICommand? CompleteCommand
    {
        get => completeCommand;
        set => this.RaiseAndSetIfChanged(ref completeCommand, value);
    }

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

    public ValueTask<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return Result.Success.ToValueTaskResult();
    }

    public Guid? ParentId
    {
        get => parentId;
        set => this.RaiseAndSetIfChanged(ref parentId, value);
    }

    public bool IsNavigateToParent => false;
}