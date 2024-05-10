namespace Spravy.Ui.Features.ToDo.Models;

public class ToDoItemNotify : NotifyBase,
    ICanCompleteProperty,
    IDeletable,
    IToDoSettingsProperty,
    ISetToDoParentItemParams,
    ILink
{
    public AvaloniaList<CommandItem> Commands { get; } = new();
    
    [Reactive]
    public ICommand? CompleteCommand { get; set; }
    
    [Reactive]
    public bool IsFavorite { get; set; }
    
    [Reactive]
    public string Description { get; set; } = string.Empty;
    
    [Reactive]
    public uint OrderIndex { get; set; }
    
    [Reactive]
    public ToDoItemStatus Status { get; set; }
    
    [Reactive]
    public ActiveToDoItemNotify? Active { get; set; }
    
    [Reactive]
    public bool IsBusy { get; set; }
    
    [Reactive]
    public ToDoItemIsCan IsCan { get; set; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    [Reactive]
    public string Name { get; set; } = string.Empty;
    
    [Reactive]
    public Guid? ParentId { get; set; }
    
    [Reactive]
    public Guid? ReferenceId { get; set; }
    
    public bool IsNavigateToParent
    {
        get => false;
    }
    
    [Reactive]
    public string Link { get; set; } = string.Empty;
    
    [Reactive]
    public ToDoItemType Type { get; set; }
    
    public Guid CurrentId
    {
        get => ReferenceId ?? Id;
    }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }
}