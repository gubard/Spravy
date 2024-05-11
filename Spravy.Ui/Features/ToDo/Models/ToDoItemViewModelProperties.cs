namespace Spravy.Ui.Features.ToDo.Models;

public class ToDoItemViewModelProperties : NotifyBase
{
    [Reactive]
    public object[] Path { get; set; } = [RootItem.Default,];
    
    [Reactive]
    public bool IsFavorite { get; set; }
    
    [Reactive]
    public ToDoItemStatus Status { get; set; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    [Reactive]
    public Guid? ReferenceId { get; set; }
    
    [Reactive]
    public ToDoItemIsCan IsCan { get; set; }
    
    [Reactive]
    public Guid? ParentId { get; set; }
    
    [Reactive]
    public string Name { get; set; } = string.Empty;
    
    [Reactive]
    public string Description { get; set; } = string.Empty;
    
    [Reactive]
    public DescriptionType DescriptionType { get; set; }
    
    [Reactive]
    public string Link { get; set; } = string.Empty;
    
    [Reactive]
    public ToDoItemType Type { get; set; }
}