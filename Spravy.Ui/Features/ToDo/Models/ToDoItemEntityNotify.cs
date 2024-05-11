namespace Spravy.Ui.Features.ToDo.Models;

public class ToDoItemEntityNotify : NotifyBase
{
    public ToDoItemEntityNotify(Guid id)
    {
        Path = [RootItem.Default,];
        Id = id;
        Description = "Loading...";
        Name = "Loading...";
        Link = "Loading...";
        this.WhenAnyValue(x => x.ReferenceId).Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentId)));
        
        this.WhenAnyValue(x => x.DescriptionType)
           .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(IsDescriptionPlainText));
                this.RaisePropertyChanged(nameof(IsDescriptionMarkdownText));
            });
    }
    
    public Guid Id { get; }
    
    [Reactive]
    public object[] Path { get; set; }
    
    [Reactive]
    public ToDoItemEntityNotify? Active { get; set; }
    
    [Reactive]
    public bool IsSelected { get; set; }
    
    [Reactive]
    public bool IsFavorite { get; set; }
    
    [Reactive]
    public string Description { get; set; }
    
    [Reactive]
    public uint OrderIndex { get; set; }
    
    [Reactive]
    public ToDoItemStatus Status { get; set; }
    
    [Reactive]
    public ToDoItemIsCan IsCan { get; set; }
    
    [Reactive]
    public string Name { get; set; }
    
    [Reactive]
    public Guid? ParentId { get; set; }
    
    [Reactive]
    public Guid? ReferenceId { get; set; }
    
    [Reactive]
    public DescriptionType DescriptionType { get; set; }
    
    [Reactive]
    public string Link { get; set; }
    
    [Reactive]
    public ToDoItemType Type { get; set; }
    
    public bool IsDescriptionPlainText
    {
        get => DescriptionType == DescriptionType.PlainText;
    }
    
    public bool IsDescriptionMarkdownText
    {
        get => DescriptionType == DescriptionType.Markdown;
    }
    
    public Guid CurrentId
    {
        get => ReferenceId ?? Id;
    }
}