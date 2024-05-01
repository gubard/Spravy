namespace Spravy.Ui.Features.ToDo.Models;

public class ToDoItemNotify : NotifyBase,
    ICanCompleteProperty,
    IDeletable,
    IToDoSettingsProperty,
    ISetToDoParentItemParams,
    ILink
{
    private ActiveToDoItemNotify? active;
    private ICommand? completeCommand;
    private string description = string.Empty;
    private Guid id;
    private bool isBusy;
    private ToDoItemIsCan isCan;
    private bool isFavorite;
    private string link = string.Empty;
    private string name = string.Empty;
    private uint orderIndex;
    private Guid? parentId;
    private ToDoItemStatus status;
    private ToDoItemType type;

    public AvaloniaList<CommandItem> Commands { get; } = new();

    public ICommand? CompleteCommand
    {
        get => completeCommand;
        set => this.RaiseAndSetIfChanged(ref completeCommand, value);
    }

    public bool IsFavorite
    {
        get => isFavorite;
        set => this.RaiseAndSetIfChanged(ref isFavorite, value);
    }

    public string Description
    {
        get => description;
        set => this.RaiseAndSetIfChanged(ref description, value);
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

    public bool IsBusy
    {
        get => isBusy;
        set => this.RaiseAndSetIfChanged(ref isBusy, value);
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

    public Guid? ParentId
    {
        get => parentId;
        set => this.RaiseAndSetIfChanged(ref parentId, value);
    }

    public bool IsNavigateToParent
    {
        get => false;
    }

    public string Link
    {
        get => link;
        set => this.RaiseAndSetIfChanged(ref link, value);
    }

    public ToDoItemType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }
}